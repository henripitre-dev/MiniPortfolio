using splitbill.Commands;
using splitbill.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using splitbill.Model;
using System.Diagnostics;

namespace splitbill.ViewModels
{
    class ModifyParticipantVM : BaseVM
    {

        #region Properties

        private readonly INavigationService _navigation;
        private readonly Participant _activityOwner;
        private readonly ActivityVM _activity;
        private readonly ActivityVM _parent;

        //TODO: renommer Activité Parent en dequoi de plus clair
        public ActivityVM Parent => _parent;


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

        public ICommand UpdateActivityCommand { get; }
        public ICommand ReturnToOverviewCommand { get; }

        public ICommand ReturnToConsultActivityCommand { get; }

        #endregion

        #endregion

        public ModifyParticipantVM(INavigationService navigationService, List<Participant> allParticipants, ActivityVM activity)
        {
            _navigation = navigationService;
            _activityOwner = activity.Owner;
            _activity = activity;
            _parent = activity;

            ActivityName = activity.Name;
            ActivityDescription = activity.Description;

            AssignedParticipants = new(activity.Participants.ToList());
            AvailableParticipants = new(allParticipants.Where(p => !AssignedParticipants.Contains(p)).ToList());

            // owner automatically part of activity
            AvailableParticipants.Remove(_activityOwner);
            //AssignedParticipants.Add(_activityOwner);


            AssignedParticipants.CollectionChanged += (s, e) => (CreateActivityCommand as RelaisCommande)?.RaiseCanExecuteChanged();

            #region Link commands to methods

            //List moving commands
            AssignSelectedCommand = new RelaisCommandeParametre(AssignSelected);
            RemoveSelectedCommand = new RelaisCommandeParametre(RemoveSelected);
            AssignAllCommand = new RelaisCommandeParametre(_ => AssignAll());
            RemoveAllCommand = new RelaisCommandeParametre(_ => RemoveAll());

            UpdateActivityCommand = new RelaisCommande(UpdateActivity, CanUpdateActivity);
            ReturnToConsultActivityCommand = new RelaisCommande(ReturnToConsultActivity);
            //ReturnToOverviewCommand = new RelaisCommande(ReturnToOverview);

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
            List<Participant> toMove = AssignedParticipants.Where(p => p != _activityOwner).ToList();

            if (!toMove.Any()) return;

            foreach (Participant p in toMove)
            {
                AssignedParticipants.Remove(p);
                AvailableParticipants.Add(p);
            }
        }

        #endregion

        #region Navigation Methods

        private void ReturnToConsultActivity()
        {
            MessageBoxResult r = MessageBox.Show(
                "Quitter sans modifier les participants?",
                "Modifications non enregistrées.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (r == MessageBoxResult.No)
                return;

            _navigation.NavigateToConsultActivity(Parent);
        }

        #endregion

        #region Activity Participant Update

        private bool CanUpdateActivity() => AssignedParticipants.Contains(_activityOwner) && AssignedParticipants.Count > 0;

        private void UpdateActivity()
        {
            _activity.Participants.Clear();
            foreach (Participant p in AssignedParticipants) 
                _activity.Participants.Add(p);

            _navigation.NavigateToConsultActivity(_activity);  // return to the activity
        }

        #endregion



    }
}
