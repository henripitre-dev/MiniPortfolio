using splitbill.Commands;
using splitbill.Model;
using splitbill.Service;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace splitbill.ViewModels
{
    public class NewActivityVM : BaseVM
    {
        #region Properties

        private readonly INavigationService _navigation;
        private readonly Participant _activityOwner;
        private readonly Participant _user;
        private readonly ObservableCollection<ActivityVM> _activities;

        #region Activity Fields

        private string _activityName;
        public string ActivityName
        {
            get => _activityName;
            set
            {
                _activityName = value;
                OnPropertyChanged(nameof(ActivityName));
            }
        }

        private string _activityDescription;
        public string ActivityDescription
        {
            get => _activityDescription;
            set
            {
                _activityDescription = value;
                OnPropertyChanged(nameof(ActivityDescription));
            }
        }

        public Participant ActivityOwner => _activityOwner;

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

        public ICommand CreateActivityCommand { get; }
        public ICommand ReturnToOverviewCommand { get; }

        #endregion

        #endregion

        public NewActivityVM(INavigationService navigationService, List<Participant> allParticipants, Participant user, ObservableCollection<ActivityVM> activities)
        {
            _navigation = navigationService;
            _user = user;
            _activityOwner = _user;
            _activities = activities;

            ActivityName = "Activité";
            ActivityDescription = "Description";

            AvailableParticipants = new(allParticipants);
            AssignedParticipants = [];

            // owner automatically part of activity
            AvailableParticipants.Remove(_activityOwner);
            AssignedParticipants.Add(_activityOwner);


            AssignedParticipants.CollectionChanged += (s, e) => (CreateActivityCommand as RelaisCommande)?.RaiseCanExecuteChanged();

            #region Link commands to methods

            //List moving commands
            AssignSelectedCommand = new RelaisCommandeParametre(AssignSelected);
            RemoveSelectedCommand = new RelaisCommandeParametre(RemoveSelected);
            AssignAllCommand = new RelaisCommandeParametre(_ => AssignAll());
            RemoveAllCommand = new RelaisCommandeParametre(_ => RemoveAll());

            CreateActivityCommand = new RelaisCommande(CreateActivity, CanCreateActivity);
            ReturnToOverviewCommand = new RelaisCommande(ReturnToOverview);

            #endregion
        }

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

            List<Participant> toMove = selected.Cast<Participant>().Where(p => p != _activityOwner).ToList();

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
                .Where(p => p != _activityOwner)
                .ToList();

            if (!toMove.Any()) return;

            foreach (Participant p in toMove)
            {
                AssignedParticipants.Remove(p);
                AvailableParticipants.Add(p);
            }
        }

        #endregion

        #region Navigation Methods

        private void ReturnToOverview()
        {
            MessageBoxResult r = MessageBox.Show(
                "Quitter sans créer l'activité?",
                "Modifications non enregistrées.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (r == MessageBoxResult.No)
                return;

            _navigation.NavigateToOverview();
        }

        #endregion

        #region Activity Creation

        private bool CanCreateActivity() => AssignedParticipants.Contains(_activityOwner) && AssignedParticipants.Count > 0;

        private void CreateActivity()
        {
            _activities.Add(new ActivityVM(new Activity(
                ActivityName,
                ActivityOwner,
                AssignedParticipants.ToList(),
                ActivityDescription), _user));

            _navigation.NavigateToOverview();
        }

        #endregion
    }
}