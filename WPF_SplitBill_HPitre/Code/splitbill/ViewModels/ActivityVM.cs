using splitbill.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace splitbill.ViewModels
{
    public class ActivityVM : BaseVM
    {
        #region Properties

        private Participant _user;

        #region Activity object property extensions (easier for datacontext and to call)
        public string Name => Activity.Name;
        public string Description => Activity.Description;
        public Participant Owner => Activity.Owner;
        public Activity Activity { get; }
        public bool IsActive => Activity.IsActive;
        public ObservableCollection<Expense> Expenses => Activity.Expenses;
        public ObservableCollection<Participant> Participants => Activity.Participants;
        public ObservableCollection<Refund> Refunds => Activity.Refunds;
        public decimal TotalAmount => Activity.TotalAmount;
        public decimal CurrentBalance => GetUserBalance();

        #endregion

        #endregion

        public ActivityVM(Activity activity, Participant user)
        {
            Activity = activity;
            _user = user;

            // Update balance when expenses change
            Expenses.CollectionChanged += ExpensesChanged;
            Refunds.CollectionChanged += RefundsChanged;
        }

        #region Collection change handling methods
        private void ExpensesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CurrentBalance));
            OnPropertyChanged(nameof(TotalAmount));
        }

        private void RefundsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CurrentBalance));
        }

        #endregion


        /// <summary>
        /// Add an expense to this activity's expenses list by defining each property, then adding it as object inside Activity.cs (shortcut to bypass ActivityVM)
        /// </summary>
        /// <param name="User">User logged in</param>
        /// <param name="participants">List of participants</param>
        /// <param name="description">Description of the expense. Nullable</param>
        public void AddExpense(string name, decimal amount, Participant User, List<Participant> participants, string? description = null)
            => Activity.AddExpense(new Expense(this.Activity, name, amount, User, participants, description));

        /// <summary>
        /// Non-static way to get user's balance in ActivityVM (this.X)
        /// </summary>
        /// <returns>User's balance (can be negative or positive, or 0)</returns>
        private decimal GetUserBalance() => Activity.GetUserBalance(_user, Activity);
    }
}