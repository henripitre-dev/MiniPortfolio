using splitbill.Commands;
using splitbill.Model;
using splitbill.Service;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace splitbill.ViewModels
{
   public class NewExpenseVM : BaseVM
    {

        #region Properties

        private readonly INavigationService _navigation;
        private readonly Participant _owner;
        private readonly ActivityVM _parent;

        //TODO: renommer Activité Parent en dequoi de plus clair
        public ActivityVM Parent => _parent;


        #region Expense Fields

        private string _expenseName;
        public string ExpenseName
        {
            get => _expenseName;
            set
            {
                _expenseName = value;
                OnPropertyChanged(nameof(ExpenseName));
            }
        }

        private decimal _expenseAmount;
        public decimal ExpenseAmount
        {
            get => _expenseAmount;
            set
            {
                _expenseAmount = value;
                OnPropertyChanged(nameof(ExpenseAmount));
            }
        }

        private string _expenseDescription;
        public string ExpenseDescription
        {
            get => _expenseDescription;
            set
            {
                _expenseDescription = value;
                OnPropertyChanged(nameof(ExpenseDescription));
            }
        }

        #endregion

        #region Participant Lists

        public ObservableCollection<Participant> AvailableParticipants { get; }
        public ObservableCollection<Participant> AssignedParticipants { get; }

        #endregion

        #region Commands

        public ICommand AssignSelectedCommand { get; }
        public ICommand RemoveSelectedCommand { get; }
        public ICommand AssignAllCommand { get; }
        public ICommand RemoveAllCommand { get; }

        public ICommand ReturnToConsultCommand { get; }
        public ICommand CreateExpenseCommand { get; }

        #endregion

        #endregion

        public NewExpenseVM(INavigationService navigation, ActivityVM parent, Participant user)
        {
            _navigation = navigation;
            _owner = user;
            _parent = parent;

            //Base names
            ExpenseName = "Dépense";
            ExpenseDescription = "Description";

            AvailableParticipants = new ObservableCollection<Participant>(Parent.Participants);
            AssignedParticipants = new ObservableCollection<Participant>();

            // owner automatically included
            AvailableParticipants.Remove(_owner);
            AssignedParticipants.Add(_owner);

            AssignedParticipants.CollectionChanged += (s, e) => (CreateExpenseCommand as RelaisCommande)?.RaiseCanExecuteChanged();

            #region Link commands to methods

            //List management commands
            AssignSelectedCommand = new RelaisCommandeParametre(AssignSelected);
            RemoveSelectedCommand = new RelaisCommandeParametre(RemoveSelected);
            AssignAllCommand = new RelaisCommandeParametre(_ => AssignAll());
            RemoveAllCommand = new RelaisCommandeParametre(_ => RemoveAll());

            ReturnToConsultCommand = new RelaisCommande(ReturnToConsultActivity);
            CreateExpenseCommand = new RelaisCommande(CreateExpense, CanCreateExpense);
            
            #endregion
        
        }

        #region Methods

        #region List Management

        private void AssignSelected(object parameter)
        {
            IList selected = parameter as IList;
            if (selected == null || selected.Count == 0) return;

            List<Participant> toMove = selected.Cast<Participant>().ToList();

            foreach (Participant p in toMove)
            {
                AvailableParticipants.Remove(p);
                AssignedParticipants.Add(p);
            }
        }

        private void RemoveSelected(object parameter)
        {
            IList selected = parameter as IList;
            if (selected == null) return;

            List<Participant> toMove = selected
                .Cast<Participant>()
                .Where(p => p != _owner)
                .ToList();

            if (!toMove.Any()) return;

            foreach (Participant p in toMove)
            {
                AssignedParticipants.Remove(p);
                AvailableParticipants.Add(p);
            }
        }

        private void AssignAll()
        {
            List<Participant> toMove = AvailableParticipants.ToList();

            foreach (Participant p in toMove)
            {
                AvailableParticipants.Remove(p);
                AssignedParticipants.Add(p);
            }
        }

        private void RemoveAll()
        {
            List<Participant> toMove = AssignedParticipants
                .Where(p => p != _owner)
                .ToList();

            if (!toMove.Any()) return;

            foreach (Participant p in toMove)
            {
                AssignedParticipants.Remove(p);
                AvailableParticipants.Add(p);
            }
        }

        #endregion

        #region Expense Creation

        private bool CanCreateExpense() => AssignedParticipants.Count > 0 && AssignedParticipants.Contains(_owner);
        
        private void CreateExpense()
        {
            Parent.Activity.AddExpense(new Expense(
                Parent.Activity,
                ExpenseName,
                ExpenseAmount,
                _owner,
                AssignedParticipants.ToList(),
                ExpenseDescription));

            //Parent.RecalculateBalances(); TODO: Recalculer les balances en considérant le initial amount paid

            _navigation.NavigateToConsultActivity(Parent);
        }

        #endregion

        private void ReturnToConsultActivity()
        {
            MessageBoxResult r = MessageBox.Show(
                "Quitter sans créer la dépense?",
                "Modifications non enregistrées.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (r == MessageBoxResult.No)
                return;

            _navigation.NavigateToConsultActivity(Parent);
        }

        #endregion

    }
}