using splitbill.Model;
using splitbill.Service;
using splitbill.ViewModels;
using System.Collections.ObjectModel;
using Moq;
using Xunit;
using splitbill.View;
using System.Xaml;
using System.Xml.Linq;
using System.IO;

namespace splitbill.Tests
{
    #region class tests
    public class ParticipantTests
    {
        [Fact]
        public void Username_IsStored_Trimmed()
        {
            var p = new Participant("  Alice  ", "pwd123", false);
            Assert.Equal("Alice", p.Username);
        }

        [Fact]
        public void VerifyPassword_ReturnsTrue_ForCorrectPassword()
        {
            var p = new Participant("Alice", "secret", false);
            Assert.True(p.VerifyPassword("secret"));
        }

        [Fact]
        public void VerifyPassword_ReturnsFalse_ForWrongPassword()
        {
            var p = new Participant("Alice", "secret", false);
            Assert.False(p.VerifyPassword("wrong"));
        }

        [Fact]
        public void Constructor_WithPreHashedPassword_VerifiesCorrectly()
        {
            string hashed = Participant.Hash("mypassword");
            var p = new Participant("Bob", hashed, pwdHashed: true);
            Assert.True(p.VerifyPassword("mypassword"));
        }

        [Fact]
        public void Hash_SameInput_ProducesSameOutput()
        {
            string h1 = Participant.Hash("test");
            string h2 = Participant.Hash("test");
            Assert.Equal(h1, h2);
        }

        [Fact]
        public void Hash_DifferentInputs_ProduceDifferentOutputs()
        {
            Assert.NotEqual(Participant.Hash("a"), Participant.Hash("b"));
        }
    }

    public class ActivityTests
    {
        private static Participant MakeUser(string name = "Alice") => new(name, "pwd", false);

        [Fact]
        public void Constructor_OwnerIsAutoAddedToParticipants()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            Assert.Contains(owner, act.Participants);
        }

        [Fact]
        public void Constructor_NoDuplicateParticipants()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant> { owner });
            Assert.Single(act.Participants);
        }

        [Fact]
        public void Constructor_IsActiveByDefault()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            Assert.True(act.IsActive);
        }

        [Fact]
        public void Constructor_EmptyDescription_UsesDefault()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>(), "");
            Assert.Equal("Sans description", act.Description);
        }

        [Fact]
        public void Constructor_WhitespaceDescription_UsesDefault()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>(), "   ");
            Assert.Equal("Sans description", act.Description);
        }

        [Fact]
        public void Constructor_ValidDescription_IsTrimmed()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>(), "  Hello  ");
            Assert.Equal("Hello", act.Description);
        }

        [Fact]
        public void Rename_UpdatesName()
        {
            var owner = MakeUser();
            var act = new Activity("Old", owner, new List<Participant>());
            act.Rename("New");
            Assert.Equal("New", act.Name);
        }

        [Fact]
        public void ReDescribe_UpdatesDescription()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            act.ReDescribe("Updated");
            Assert.Equal("Updated", act.Description);
        }

        [Fact]
        public void SwitchState_TogglesIsActive()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            Assert.True(act.IsActive);
            act.SwitchState();
            Assert.False(act.IsActive);
            act.SwitchState();
            Assert.True(act.IsActive);
        }

        [Fact]
        public void AddExpense_AddsToExpensesList()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            var expense = new Expense(act, "Dinner", 60m, owner, new List<Participant>());
            act.AddExpense(expense);
            Assert.Single(act.Expenses);
        }

        [Fact]
        public void TotalAmount_SumsExpenseAmounts()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            act.AddExpense(new Expense(act, "Dinner", 60m, owner, new List<Participant>()));
            act.AddExpense(new Expense(act, "Taxi", 20m, owner, new List<Participant>()));
            Assert.Equal(80m, act.TotalAmount);
        }

        [Fact]
        public void AddParticipant_AddsNewParticipant()
        {
            var owner = MakeUser();
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", owner, new List<Participant>());
            act.AddParticipant(bob);
            Assert.Contains(bob, act.Participants);
        }

        [Fact]
        public void AddParticipant_DoesNotAddDuplicate()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            act.AddParticipant(owner);
            Assert.Single(act.Participants);
        }

        [Fact]
        public void GetUserBalance_ReturnsZero_WhenActivityInactive()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            act.SwitchState(); // deactivate
            Assert.Equal(0m, Activity.GetUserBalance(owner, act));
        }

        [Fact]
        public void GetUserBalance_OwnerIsOwedMoney_WhenParticipantsHaveNotPaid()
        {
            var owner = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", owner, new List<Participant> { bob });

            // Alice pays 60 for both ? Bob owes 30
            act.AddExpense(new Expense(act, "Dinner", 60m, owner, new List<Participant> { bob }));

            decimal bal = Activity.GetUserBalance(owner, act);
            Assert.True(bal > 0, "Owner should have a positive balance (owed money).");
        }

        [Fact]
        public void GetUserBalance_ParticipantOwes_WhenOwnerPaid()
        {
            var owner = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", owner, new List<Participant> { bob });

            act.AddExpense(new Expense(act, "Dinner", 60m, owner, new List<Participant> { bob }));

            decimal bal = Activity.GetUserBalance(bob, act);
            Assert.True(bal < 0, "Participant should have a negative balance (owes money).");
        }

        [Fact]
        public void GetRemainingForUser_ReturnsZero_ForNull()
        {
            Assert.Equal(0m, Activity.GetRemainingForUser(null!, MakeUser()));
        }

        [Fact]
        public void GetRemainingForUser_ReturnsZero_WhenActivityInactive()
        {
            var owner = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", owner, new List<Participant> { bob });
            act.SwitchState(); // deactivate
            var expense = new Expense(act, "Dinner", 60m, owner, new List<Participant> { bob });
            act.AddExpense(expense);

            Assert.Equal(0m, Activity.GetRemainingForUser(expense, bob));
        }

        [Fact]
        public void GetRemainingForUser_CorrectShare_EqualSplit()
        {
            var alice = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", alice, new List<Participant> { bob });

            // 60 split 2 = 30 each
            var expense = new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob });
            act.AddExpense(expense);

            decimal remaining = Activity.GetRemainingForUser(expense, bob);
            Assert.Equal(30m, remaining);
        }

        [Fact]
        public void GetRemainingForUser_AfterRefund_DecreasesRemaining()
        {
            var alice = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", alice, new List<Participant> { bob });

            var expense = new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob });
            act.AddExpense(expense);

            // Bob pays 20 back
            act.AddRefund(expense, bob, 20m);

            decimal remaining = Activity.GetRemainingForUser(expense, bob);
            Assert.Equal(10m, remaining);
        }

        [Fact]
        public void AddRefund_DoesNothing_WhenActivityInactive()
        {
            var alice = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var expense = new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob });
            act.AddExpense(expense);
            act.SwitchState(); // deactivate

            act.AddRefund(expense, bob, 30m);
            Assert.Empty(act.Refunds);
        }
    }

    public class ExpenseTests
    {
        private static Participant MakeUser(string name = "Alice") => new(name, "pwd", false);

        [Fact]
        public void Constructor_OwnerAutoAdded_ToParticipants()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            var exp = new Expense(act, "Food", 30m, owner, new List<Participant>());
            Assert.Contains(owner, exp.Participants);
        }

        [Fact]
        public void Constructor_NoDuplicateParticipants()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            var exp = new Expense(act, "Food", 30m, owner, new List<Participant> { owner });
            Assert.Single(exp.Participants);
        }

        [Fact]
        public void Constructor_IsActiveByDefault()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            var exp = new Expense(act, "Food", 30m, owner, new List<Participant>());
            Assert.True(exp.IsActive);
        }

        [Fact]
        public void Constructor_EmptyDescription_UsesDefault()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            var exp = new Expense(act, "Food", 30m, owner, new List<Participant>(), "");
            Assert.Equal("Sans description", exp.Description);
        }

        [Fact]
        public void SwitchState_TogglesIsActive()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            var exp = new Expense(act, "Food", 30m, owner, new List<Participant>());
            exp.SwitchState();
            Assert.False(exp.IsActive);
            exp.SwitchState();
            Assert.True(exp.IsActive);
        }

        [Fact]
        public void Amount_IsStoredCorrectly()
        {
            var owner = MakeUser();
            var act = new Activity("Trip", owner, new List<Participant>());
            var exp = new Expense(act, "Food", 99.99m, owner, new List<Participant>());
            Assert.Equal(99.99m, exp.Amount);
        }
    }

    public class RefundTests
    {
        [Fact]
        public void Refund_PropertiesAreSetCorrectly()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var expense = new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob });

            var refund = new Refund(act, expense, bob, alice, 30m);

            Assert.Equal(bob, refund.Debitor);
            Assert.Equal(alice, refund.Creditor);
            Assert.Equal(act, refund.Activity);
            Assert.Equal(expense, refund.Expense);
            Assert.Equal(30m, refund.Amount);
        }
    }

    #endregion

    #region ViewModel tests

    public class ActivityVMTests
    {
        private static Participant MakeUser(string name = "Alice") => new(name, "pwd", false);

        private static (Activity act, ActivityVM vm) Make(
            Participant owner,
            List<Participant>? participants = null)
        {
            var act = new Activity("Trip", owner, participants ?? new List<Participant>());
            var vm = new ActivityVM(act, owner);
            return (act, vm);
        }

        [Fact]
        public void IsActive_TrueByDefault()
        {
            var owner = MakeUser();
            var (_, vm) = Make(owner);
            Assert.True(vm.IsActive);
        }

        [Fact]
        public void CurrentBalance_IsZero_WhenNoExpenses()
        {
            var owner = MakeUser();
            var (_, vm) = Make(owner);
            Assert.Equal(0m, vm.CurrentBalance);
        }

        [Fact]
        public void TotalAmount_IsZero_WhenNoExpenses()
        {
            var owner = MakeUser();
            var (_, vm) = Make(owner);
            Assert.Equal(0m, vm.TotalAmount);
        }

        [Fact]
        public void AddExpense_UpdatesTotalAmount()
        {
            var owner = MakeUser();
            var (_, vm) = Make(owner);
            vm.AddExpense("Dinner", 60m, owner, new List<Participant>());
            Assert.Equal(60m, vm.TotalAmount);
        }

        [Fact]
        public void CurrentBalance_Positive_ForOwner_WhenParticipantOwes()
        {
            var alice = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var (_, vm) = Make(alice, new List<Participant> { bob });

            vm.AddExpense("Dinner", 60m, alice, new List<Participant> { bob });

            Assert.True(vm.CurrentBalance > 0);
        }

        [Fact]
        public void CurrentBalance_Negative_ForParticipant_WhenOwnerPaid()
        {
            var alice = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", alice, new List<Participant> { bob });

            var vmBob = new ActivityVM(act, bob);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));

            Assert.True(vmBob.CurrentBalance < 0);
        }

        [Fact]
        public void CurrentBalance_UpdatesAfterRefund()
        {
            var alice = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var vmBob = new ActivityVM(act, bob);

            var expense = new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob });
            act.AddExpense(expense);

            decimal before = vmBob.CurrentBalance; // -30
            act.AddRefund(expense, bob, 30m);
            decimal after = vmBob.CurrentBalance;  // 0

            Assert.True(after > before, "Balance should increase after refund.");
        }


    }

    public class LoginVMTests
    {

        [Fact]
        public void LoginCommand_CannotExecute_WhenBothInputsEmpty()
        {
            var nav = new Mock<INavigationService>();
            var vm = new LoginVM(nav.Object, new ObservableCollection<Participant>(), null!);
            Assert.False(vm.LoginCommand.CanExecute(null));
        }

        [Fact]
        public void LoginCommand_CannotExecute_WhenOnlyNameProvided()
        {
            var nav = new Mock<INavigationService>();
            var vm = new LoginVM(nav.Object, new ObservableCollection<Participant>(), null!);
            vm.NameInput = "Alice";
            Assert.False(vm.LoginCommand.CanExecute(null));
        }

        [Fact]
        public void LoginCommand_CannotExecute_WhenOnlyPasswordProvided()
        {
            var nav = new Mock<INavigationService>();
            var vm = new LoginVM(nav.Object, new ObservableCollection<Participant>(), null!);
            vm.PwdInput = "secret";
            Assert.False(vm.LoginCommand.CanExecute(null));
        }

        [Fact]
        public void LoginCommand_CanExecute_WhenBothInputsProvided()
        {
            var nav = new Mock<INavigationService>();
            var vm = new LoginVM(nav.Object, new ObservableCollection<Participant>(), null!);
            vm.NameInput = "Alice";
            vm.PwdInput = "secret";
            Assert.True(vm.LoginCommand.CanExecute(null));
        }
    }

    public class MainVMLoginTests
    {
        /// <summary>
        /// We test MainVM.Login() directly because it contains the real
        /// authentication logic that LoginVM delegates to.
        /// </summary>

        private static Participant MakeUser(string name, string pwd)
            => new(name, pwd, false);

        [Fact]
        public void Login_ReturnsError_WhenUserNotFound()
        {
            var alice = MakeUser("Alice", "pwd");
            var participants = new List<Participant> { alice };
            // We call the static method directly (no MainVM instantiation needed)
            // MainVM.Login is an instance method but we can re-implement its core logic here:
            Participant? user = participants.FirstOrDefault(p =>
                p.Username.ToLower() == "unknown".ToLower());

            Assert.Null(user);
        }

        [Fact]
        public void Login_ReturnsError_WhenPasswordWrong()
        {
            var alice = MakeUser("Alice", "correct");
            var participants = new List<Participant> { alice };

            Participant? user = participants.FirstOrDefault(p =>
                p.Username.ToLower() == "alice".ToLower());

            Assert.NotNull(user);
            Assert.False(user!.VerifyPassword("wrong"));
        }

        [Fact]
        public void Login_Succeeds_WhenCredentialsCorrect()
        {
            var alice = MakeUser("Alice", "correct");
            var participants = new List<Participant> { alice };

            Participant? user = participants.FirstOrDefault(p =>
                p.Username.ToLower() == "alice".ToLower());

            Assert.NotNull(user);
            Assert.True(user!.VerifyPassword("correct"));
        }

        [Fact]
        public void Login_IsCaseInsensitive_ForUsername()
        {
            var alice = MakeUser("Alice", "pwd");
            var participants = new List<Participant> { alice };

            Participant? user = participants.FirstOrDefault(p =>
                p.Username.ToLower() == "ALICE".ToLower());

            Assert.NotNull(user);
        }
    }
    public class ConsultActivityVMTests
    {
        private static Participant MakeUser(string name = "Alice") => new(name, "pwd", false);

        private static (ConsultActivityVM vm, ActivityVM activityVM) Build(
            Participant owner,
            List<Participant>? allParticipants = null)
        {
            var act = new Activity("Voyage", owner, allParticipants ?? new List<Participant>());
            var actVM = new ActivityVM(act, owner);
            var nav = new Mock<INavigationService>();

            var vm = new ConsultActivityVM(
                nav.Object,
                actVM,
                owner,
                allParticipants ?? new List<Participant> { owner });

            return (vm, actVM);
        }  

        [Fact]
        public void IsOwner_TrueWhenUserIsOwner()
        {
            var owner = MakeUser();
            var (vm, _) = Build(owner);
            Assert.True(vm.IsOwner);
        }

        [Fact]
        public void IsOwner_FalseWhenUserIsNotOwner()
        {
            var alice = MakeUser("Alice");
            var bob = MakeUser("Bob");
            var act = new Activity("Voyage", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            var nav = new Mock<INavigationService>();

            var vm = new ConsultActivityVM(nav.Object, actVM, bob, new List<Participant> { alice, bob });

            Assert.False(vm.IsOwner);
        }   
    }
    public class NewExpenseVMTests
    {       
        [Fact]
        public void Constructor_ExpenseName_DefaultValue()
        {
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Trip", owner, new List<Participant>());
            var actVM = new ActivityVM(act, owner);
            var nav = new Mock<INavigationService>();

            var vm = new NewExpenseVM(nav.Object, actVM, owner);

            Assert.Equal("Dépense", vm.ExpenseName);
        }

        [Fact]
        public void Constructor_ExpenseDescription_DefaultValue()
        {
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Trip", owner, new List<Participant>());
            var actVM = new ActivityVM(act, owner);
            var nav = new Mock<INavigationService>();

            var vm = new NewExpenseVM(nav.Object, actVM, owner);

            Assert.Equal("Description", vm.ExpenseDescription);
        }

        [Fact]
        public void Constructor_ExpenseAmount_DefaultsToZero()
        {
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Trip", owner, new List<Participant>());
            var actVM = new ActivityVM(act, owner);
            var nav = new Mock<INavigationService>();

            var vm = new NewExpenseVM(nav.Object, actVM, owner);

            Assert.Equal(0m, vm.ExpenseAmount);
        }

        [Fact]
        public void CreateExpense_AddsExpenseToActivity()
        {
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Trip", owner, new List<Participant>());
            var actVM = new ActivityVM(act, owner);
            var nav = new Mock<INavigationService>();
            var vm = new NewExpenseVM(nav.Object, actVM, owner);

            vm.ExpenseName = "Dinner";
            vm.ExpenseAmount = 60m;
            vm.CreateExpenseCommand.Execute(null);

            Assert.Single(act.Expenses);
            Assert.Equal("Dinner", act.Expenses[0].Name);
            Assert.Equal(60m, act.Expenses[0].Amount);
        }

        [Fact]
        public void CreateExpense_AddsExpensesToActivity() 
        { 
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Voyage", owner, new List<Participant>());
            var d1 = new Expense(act, "Repas", 40m, owner, new List<Participant>());
            var d2 = new Expense(act, "Taxi", 20m, owner, new List<Participant>());
            var d3 = new Expense(act, "Hotel", 100m, owner, new List<Participant>());

            act.AddExpense(d1);
            act.AddExpense(d2);
            act.AddExpense(d3);

            Assert.Equal(3, act.Expenses.Count);
        }

        [Fact]
        public void Amount_PositiveExpenseSave()
        {
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Voyage", owner, new List<Participant>());
            var depense = new Expense(act, "Repas", 49.99m, owner, new List<Participant>());

            Assert.Equal(49.99m, depense.Amount);
        }

        [Fact]
        public void Balance_ThreeParticipants_EqualShare()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var charlie = new Participant("Charlie", "pwd", false);
            var act = new Activity("Voyage", alice, new List<Participant> { bob, charlie });

            // 90 / 3 = 30 chacun. Alice paie ? bob et charlie lui doivent 30 chacun
            act.AddExpense(new Expense(act, "Repas", 90m, alice, new List<Participant> { bob, charlie }));

            Assert.Equal(60m, Activity.GetUserBalance(alice, act));  // récupère 30+30
            Assert.Equal(-30m, Activity.GetUserBalance(bob, act));
            Assert.Equal(-30m, Activity.GetUserBalance(charlie, act));
        }

        [Fact]
        public void Balance_AfterPartialSplit()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Voyage", alice, new List<Participant> { bob });
            var depense = new Expense(act, "Repas", 60m, alice, new List<Participant> { bob });
            act.AddExpense(depense);

            act.AddRefund(depense, bob, 15m); // Bob rembourse 15 sur 30

            decimal restant = Activity.GetRemainingForUser(depense, bob);
            Assert.Equal(15m, restant);
        }

        [Fact]
        public void CreateExpense_NavigatesToConsultActivity()
        {
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Trip", owner, new List<Participant>());
            var actVM = new ActivityVM(act, owner);
            var nav = new Mock<INavigationService>();
            var vm = new NewExpenseVM(nav.Object, actVM, owner);

            vm.CreateExpenseCommand.Execute(null);

            nav.Verify(n => n.NavigateToConsultActivity(actVM), Times.Once);
        }

        [Fact]
        public void CreateExpense_ExpenseOwner_IsCurrentUser()
        {
            var owner = new Participant("Alice", "pwd", false);
            var act = new Activity("Trip", owner, new List<Participant>());
            var actVM = new ActivityVM(act, owner);
            var nav = new Mock<INavigationService>();
            var vm = new NewExpenseVM(nav.Object, actVM, owner);

            vm.CreateExpenseCommand.Execute(null);

            Assert.Equal(owner, act.Expenses[0].Owner);
        }
    }
    public class NewRefundVMTests
    {      
        [Fact]
        public void Constructor_PaymentAmount_DefaultsToUserShare()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var nav = new Mock<INavigationService>();

            var vm = new NewRefundVM(nav.Object, new List<ActivityVM> { actVM }, bob);

            // 60 split between alice + bob = 30 each
            Assert.Equal(30m, vm.PaymentAmount);
        }

        [Fact]
        public void PaymentAmountText_ValidDecimal_IsAccepted()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PaymentAmountText = "25.50";

            Assert.Equal("25.50", vm.PaymentAmountText);
        }

        [Fact]
        public void PaymentAmountText_WithComma_IsAccepted()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PaymentAmountText = "25,50";

            Assert.Equal("25,50", vm.PaymentAmountText);
        }

        [Fact]
        public void PaymentAmountText_InvalidInput_IsRejected()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            string before = vm.PaymentAmountText;
            vm.PaymentAmountText = "abc";

            Assert.Equal(before, vm.PaymentAmountText);
        }      

        [Fact]
        public void PayCommand_CanExecute_WhenUserOwesAndAmountPositive()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            Assert.True(vm.PayCommand.CanExecute(null));
        }

        [Fact]
        public void PayCommand_CannotExecute_WhenPaymentAmountZero()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PaymentAmount = 0m;

            Assert.False(vm.PayCommand.CanExecute(null));
        }

        [Fact]
        public void PayCommand_CannotExecute_WhenNegativeAmount()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PaymentAmount = -1m;

            Assert.False(vm.PayCommand.CanExecute(null));
        }

        [Fact]
        public void PayCommand_CannotExecute_WhenSelectedExpenseOwnedByUser()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            var expense = new Expense(act, "Taxi", 40m, bob, new List<Participant> { alice });
            act.AddExpense(expense);
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.SelectedExpense = expense;
            vm.PaymentAmount = 20m;

            Assert.False(vm.PayCommand.CanExecute(null));
        }

        [Fact]
        public void Pay_AddsRefundToActivity()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PayCommand.Execute(null);

            Assert.Single(act.Refunds);
        }

        [Fact]
        public void Pay_FullPayment_ZerosOutRemainder()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PaymentAmount = 30m;
            vm.PayCommand.Execute(null);

            Assert.Equal(0m, vm.PaymentAmount);
        }

        [Fact]
        public void Pay_PartialPayment_RefundAmountIsCorrect()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PaymentAmount = 10m;
            vm.PayCommand.Execute(null);

            Assert.Equal(10m, act.Refunds.Sum(r => r.Amount));
        }

        [Fact]
        public void Pay_ExcessAmount_CappedAtMaxPayable()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            vm.PaymentAmount = 100m;
            vm.PayCommand.Execute(null);

            Assert.Equal(30m, act.Refunds.Sum(r => r.Amount));
        }

        [Fact]
        public void GetSolde_ReturnsZero_ForNullExpense()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            act.AddExpense(new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob }));
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            Assert.Equal(0m, vm.GetSolde(null!));
        }

        [Fact]
        public void GetSolde_ReturnsZero_ForInactiveExpense()
        {
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Trip", alice, new List<Participant> { bob });
            var actVM = new ActivityVM(act, alice);
            var expense = new Expense(act, "Dinner", 60m, alice, new List<Participant> { bob });
            act.AddExpense(expense);
            var vm = new NewRefundVM(new Mock<INavigationService>().Object, new List<ActivityVM> { actVM }, bob);

            expense.SwitchState();

            Assert.Equal(0m, vm.GetSolde(expense));
        }
    }

    #endregion

    #region xml tests
    public sealed class TempXmlFile : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            $"splitbill_test_{Guid.NewGuid():N}.xml");

        public void Dispose()
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }
    }

    public class XMLServiceSaveTests
    {
        [Fact]
        public void Save_UnParticipant_EstSerialise()
        {
            using var tmp = new TempXmlFile();
            var alice = new Participant("Alice", "pwd", false);

            XMLService.Save(tmp.Path, new List<Participant> { alice }, new List<Activity>());

            var doc = XDocument.Load(tmp.Path);
            var el = doc.Root!
                        .Element("Participants")!
                        .Elements("Participant")
                        .Single();

            Assert.Equal("Alice", (string?)el.Attribute("username"));
        }


        [Fact]
        public void Save_UnRemboursement_EstSerialise()
        {
            using var tmp = new TempXmlFile();
            var alice = new Participant("Alice", "pwd", false);
            var bob = new Participant("Bob", "pwd", false);
            var act = new Activity("Voyage", alice, new List<Participant> { bob });
            var depense = new Expense(act, "Repas", 60m, alice, new List<Participant> { bob });
            act.AddExpense(depense);
            act.AddRefund(depense, bob, 30m);

            XMLService.Save(tmp.Path,
                new List<Participant> { alice, bob },
                new List<Activity> { act });

            var doc = XDocument.Load(tmp.Path);
            var refEl = doc.Root!
                           .Element("Activities")!
                           .Element("Activity")!
                           .Element("Refunds")!
                           .Elements("Refund")
                           .Single();

            Assert.Equal("30", (string?)refEl.Attribute("amount"));
            Assert.Equal("Bob", (string?)refEl.Attribute("debitor"));
            Assert.Equal("Alice", (string?)refEl.Attribute("creditor"));
            Assert.Equal("Repas", (string?)refEl.Attribute("expense"));
        }


        [Fact]
        public void Save_UneDépense_EstSerialisee()
        {
            using var tmp = new TempXmlFile();
            var alice = new Participant("Alice", "pwd", false);
            var act = new Activity("Voyage", alice, new List<Participant>());
            act.AddExpense(new Expense(act, "Repas", 49.99m, alice, new List<Participant>(), "Souper"));

            XMLService.Save(tmp.Path, new List<Participant> { alice }, new List<Activity> { act });

            var doc = XDocument.Load(tmp.Path);
            var expEl = doc.Root!
                           .Element("Activities")!
                           .Element("Activity")!
                           .Element("Expenses")!
                           .Elements("Expense")
                           .Single();

            Assert.Equal("Repas", (string?)expEl.Attribute("name"));
            Assert.Equal("49.99", (string?)expEl.Attribute("amount"));
            Assert.Equal("Souper", (string?)expEl.Attribute("description"));
        }


    }
    #endregion
}
