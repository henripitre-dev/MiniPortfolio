using splitbill.Commands;
using splitbill.Model;
using splitbill.Service;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace splitbill.ViewModels
{
   public class ConsultActivityVM : BaseVM
    {
        #region Properties

        #region Private/Readonly/Get Properties
        private readonly INavigationService _navigation;

        private readonly Participant _user;

        private bool _participantsModified;

        public ActivityVM thisActivityVM { get; }

        public ObservableCollection<Expense> Expenses => thisActivityVM.Expenses;

        public ObservableCollection<Participant> AssignedParticipants { get; }
        public ObservableCollection<Participant> AvailableParticipants { get; }

        public bool IsOwner => thisActivityVM.Owner == _user;

        public bool HasUnsavedChanges =>
                        EditName != thisActivityVM.Name ||
                        EditDescription != thisActivityVM.Description ||
                        EditIsActive != thisActivityVM.IsActive ||
                        _participantsModified;

        #endregion

        #region ConsultActivityVM Commands

        public RelaisCommande ReturnToOverviewCommand { get; }
        public RelaisCommande SaveActivityCommand { get; }
        public RelaisCommande NewExpenseCommand { get; }
        public RelaisCommande GoToModifyParticipantCommand { get; }

        #endregion

        #region Get/Set Properties for discardable/edit fields
        private string _editName;
        public string EditName
        {
            get => _editName;
            set
            {
                if (_editName != value)
                {
                    _editName = value;
                    OnPropertyChanged(nameof(EditName));
                    OnPropertyChanged(nameof(HasUnsavedChanges));

                    SaveActivityCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _editDescription;
        public string EditDescription
        {
            get => _editDescription;
            set
            {
                if (_editDescription != value)
                {
                    _editDescription = value;
                    OnPropertyChanged(nameof(EditDescription));
                    OnPropertyChanged(nameof(HasUnsavedChanges));

                    SaveActivityCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _editIsActive;
        public bool EditIsActive
        {
            get => _editIsActive;
            set
            {
                if (_editIsActive != value)
                {
                    _editIsActive = value;
                    OnPropertyChanged(nameof(EditIsActive));
                    OnPropertyChanged(nameof(HasUnsavedChanges));

                    SaveActivityCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Animation Properties
        private readonly string[][] _eyeAnimations =
        [
            ["👁️", "👁️", "👁️", "◡", "👁️", "👁️", "◡", "◠"],
            ["👁️", "👁️", "👁️", "👁️", "◡", "👁️", "◡", "👁️", "👁️"],
            ["👁️", "👁️", "◡", "◡", "◡", "◠", "◡", "👁️", "👁️"],
        ];

        public Dictionary<Expense, string> EyeIcons { get; } = new();


        private readonly Random _rng = new();

        #endregion

        #endregion

        #region ListView management and Related Commands (assign/remove participants to/from the ActivityVM)

        public ICommand AssignSelectedCommand { get; }
        public ICommand RemoveSelectedCommand { get; }
        public ICommand AssignAllCommand { get; }
        public ICommand RemoveAllCommand { get; }

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

            _participantsModified = true;
            OnPropertyChanged(nameof(HasUnsavedChanges));
            SaveActivityCommand.RaiseCanExecuteChanged();
        }

        private void RemoveSelected(object parameter)
        {
            IList selected = parameter as IList;
            if (selected == null) return;

            List<Participant> toMove = selected
                .Cast<Participant>()
                .Where(p => p != _user)   // owner cannot be removed
                .ToList();

            if (!toMove.Any()) return;

            foreach (Participant p in toMove)
            {
                AssignedParticipants.Remove(p);
                AvailableParticipants.Add(p);
            }

            _participantsModified = true;
            OnPropertyChanged(nameof(HasUnsavedChanges));
            SaveActivityCommand.RaiseCanExecuteChanged();
        }

        private void AssignAll()
        {
            List<Participant> toMove = AvailableParticipants.ToList();

            foreach (Participant p in toMove)
            {
                AvailableParticipants.Remove(p);
                AssignedParticipants.Add(p);
            }

            _participantsModified = true;
            OnPropertyChanged(nameof(HasUnsavedChanges));
            SaveActivityCommand.RaiseCanExecuteChanged();
        }

        private void RemoveAll()
        {
            List<Participant> toMove = AssignedParticipants
                .Where(p => p != _user)
                .ToList();

            if (!toMove.Any()) return;

            foreach (Participant p in toMove)
            {
                AssignedParticipants.Remove(p);
                AvailableParticipants.Add(p);
            }

            _participantsModified = true;
            OnPropertyChanged(nameof(HasUnsavedChanges));
            SaveActivityCommand.RaiseCanExecuteChanged();
        }
        #endregion

        public ConsultActivityVM(INavigationService navigation, ActivityVM selected, Participant user, List<Participant> AllParticipants)
        {
            _navigation = navigation;
            _user = user;
            thisActivityVM = selected;


            AssignedParticipants = new ObservableCollection<Participant>(selected.Participants.ToList());

            AvailableParticipants = new ObservableCollection<Participant>(
                AllParticipants.Where(p => !AssignedParticipants.Contains(p)).ToList()
);

            // _ = dump task
            AssignSelectedCommand = new RelaisCommandeParametre(AssignSelected, _ => IsOwner);

            RemoveSelectedCommand = new RelaisCommandeParametre(RemoveSelected, _ => IsOwner);

            AssignAllCommand = new RelaisCommandeParametre(_ => AssignAll(), _ => IsOwner);

            RemoveAllCommand = new RelaisCommandeParametre(_ => RemoveAll(), _ => IsOwner);

            //Temp values for editable values
            _editName = thisActivityVM.Name;
            _editDescription = thisActivityVM.Description;
            _editIsActive = thisActivityVM.IsActive;

            //Link commands to methods
            NewExpenseCommand = new RelaisCommande(() => _navigation.NavigateToNewExpense(thisActivityVM));
            SaveActivityCommand = new RelaisCommande(SaveActivityVM, () => HasUnsavedChanges && IsOwner);
            ReturnToOverviewCommand = new RelaisCommande(ReturnToOverview);
            GoToModifyParticipantCommand = new RelaisCommande(GoToModifyParticipant, () => IsOwner);

            StartEyeAnimation();
        }

        #region Methods

        #region Navigation Methods

        private void ReturnToOverview()
        {
            if (HasUnsavedChanges)
            {
                MessageBoxResult r = MessageBox.Show(
                    "Quitter sans enregistrer les modifications ?",
                    "Modifications non enregistrées",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (r == MessageBoxResult.No)
                    return;

                //if quit without saving = yes:
                DiscardChanges();
            }

            _navigation.NavigateToOverview();
        }

        private void GoToModifyParticipant()
        {
            if (HasUnsavedChanges)
            {
                MessageBoxResult r = MessageBox.Show(
                    "Quitter sans enregistrer les modifications ?",
                    "Modifications non enregistrées",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (r == MessageBoxResult.No)
                    return;

                //if quit without saving = yes:
                DiscardChanges();
            }

            _navigation.NavigateToModifyParticipant(thisActivityVM);
        }


        #endregion

        #region Data Handling Methods

        private void SaveActivityVM()
        {
            if (thisActivityVM.Name != EditName)
                    thisActivityVM.Activity.Rename(EditName);

            if (thisActivityVM.Description != EditDescription)
                    thisActivityVM.Activity.ReDescribe(EditDescription);

            if (thisActivityVM.IsActive != EditIsActive)
                    thisActivityVM.Activity.SwitchState();

            thisActivityVM.Participants.Clear();
            thisActivityVM.Participants.Add(_user); // owner always present
            foreach (Participant p in AssignedParticipants)
            {
                if (p != _user && !thisActivityVM.Participants.Contains(p))
                    thisActivityVM.Participants.Add(p);
            }
            _participantsModified = false;

            OnPropertyChanged(nameof(HasUnsavedChanges));

            SaveActivityCommand.RaiseCanExecuteChanged();
        }

        public void DiscardChanges()
        {
            EditName = thisActivityVM.Name;
            EditDescription = thisActivityVM.Description;
            EditIsActive = thisActivityVM.IsActive;
        }

        #endregion

        #region Animation Methods

        private void StartEyeAnimation()
        {
            int frame = 0;
            string[]? currentAnimation = null;
            Expense? blinkingExpense = null;
            bool isAnimating = false;

            DispatcherTimer animTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };

            DispatcherTimer idleTimer = new()
            {
                Interval = TimeSpan.FromSeconds(_rng.Next(3, 7))
            };

            animTimer.Tick += (s, e) =>
            {
                if (!isAnimating || currentAnimation == null || blinkingExpense == null)
                    return;

                if (frame < currentAnimation.Length)
                {
                    EyeIcons[blinkingExpense] = currentAnimation[frame];
                    frame++;
                    OnPropertyChanged(nameof(EyeIcons));
                }
                else
                {
                    EyeIcons[blinkingExpense] = "👁️";
                    OnPropertyChanged(nameof(EyeIcons));

                    isAnimating = false;
                    frame = 0;

                    idleTimer.Interval = TimeSpan.FromSeconds(_rng.Next(3, 7));
                    idleTimer.Start();
                }
            };

            idleTimer.Tick += (s, e) =>
            {
                idleTimer.Stop();

                List<Expense> list = Expenses.ToList();
                if (list.Count == 0)
                    return;

                blinkingExpense = list[_rng.Next(list.Count)];
                currentAnimation = _eyeAnimations[_rng.Next(_eyeAnimations.Length)];

                frame = 0;
                isAnimating = true;
            };

            idleTimer.Start();
            animTimer.Start();
        }

        #endregion

        #endregion
    }
}