using splitbill.Commands;
using splitbill.Model;
using splitbill.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace splitbill.ViewModels
{
    public class OverviewVM : BaseVM
    {

        #region Properties

        #region Functional Properties

        private readonly INavigationService _navigation;

        #region Animation Properties
        private readonly string[][] _eyeAnimations =
        [
            ["👁️", "👁️", "👁️", "◡", "👁️", "👁️", "◡", "◠"],
            ["👁️", "👁️", "👁️", "👁️", "◡", "👁️", "◡", "👁️", "👁️"],
            ["👁️", "👁️", "◡", "◡", "◡", "◠", "◡", "👁️", "👁️"],
        ];

        public Dictionary<ActivityVM, string> EyeIcons { get; } = new();


        private readonly Random _rng = new();

        #endregion

        #region Commands
        public ICommand NewRefundCommand { get; }
        public ICommand Logout_Command { get; }
        public ICommand NewActivityCommand { get; }
        public ICommand OpenActivityCommand { get; }

        #endregion  

        #endregion

        #region Project Properties

        private readonly ObservableCollection<Participant> _participants;

        public Participant User { get; }



        public decimal DebtAmount
        {
            get
            {
                decimal r = 0;
                foreach (ActivityVM a in UserActivities)
                    if (a.CurrentBalance < 0)
                    r += a.CurrentBalance;
                return Math.Abs(r);
            }
        }
        public decimal RefundAmount
        {
            get
            {
                decimal z = 0;
                foreach (ActivityVM a in UserActivities)
                    foreach (Refund r in a.Refunds)
                        if (r.Debitor == User)
                            z += r.Amount;
                return z;
            }
        }

        private readonly ObservableCollection<ActivityVM> _activities;

        private ObservableCollection<ActivityVM> _userActivities = new();
        public ObservableCollection<ActivityVM> UserActivities => _userActivities;

        public ObservableCollection<Participant> Participants => _participants;

        #endregion

        #endregion

        public OverviewVM(ObservableCollection<ActivityVM> activities, ObservableCollection<Participant> participants, Participant user, INavigationService nav, MainVM main)
        {
            _navigation = nav;

            _activities = activities;
            _participants = participants;
            User = user;

            //Refresh the list of activities and the eye icons to display on those refreshed activities
            RefreshActivities();

            NewActivityCommand = new RelaisCommande(() => _navigation.NavigateToNewActivity());

            NewRefundCommand = new RelaisCommande(() =>_navigation.NavigateToNewRefund());
            
            Logout_Command= new RelaisCommande(() => main.Logout());


            OpenActivityCommand = new RelaisCommandeParametre(param =>
            {
                if (param is ActivityVM a)
                    _navigation.NavigateToConsultActivity(a);
            });

            foreach (ActivityVM a in UserActivities)
                EyeIcons[a] = "👁️";

            StartEyeAnimation();
        }

        #region Methods

        public bool IsUserOwner(ActivityVM a) => a.Owner == User;

        /// <summary>
        /// Refresh the list of activities and the eye icons to display on those refreshed activities
        /// </summary>
        private void RefreshActivities()
        {
            _userActivities.Clear();

            foreach (ActivityVM a in _activities)
            {
                if (a.Owner == User || a.Participants.Contains(User))
                {
                    _userActivities.Add(a);

                    if (!EyeIcons.ContainsKey(a))
                        EyeIcons[a] = "👁️";
                }
            }
        }

        #region Animation Methods

        private void StartEyeAnimation()
        {
            int frame = 0;
            string[]? currentAnimation = null;
            ActivityVM? blinkingActivityVM = null;
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
                if (!isAnimating || currentAnimation == null || blinkingActivityVM == null)
                    return;

                if (frame < currentAnimation.Length)
                {
                    EyeIcons[blinkingActivityVM] = currentAnimation[frame];
                    frame++;
                    OnPropertyChanged(nameof(EyeIcons));
                }
                else
                {
                    EyeIcons[blinkingActivityVM] = "👁️";
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

                List<ActivityVM> list = UserActivities.ToList();
                if (list.Count == 0)
                    return;

                blinkingActivityVM = list[_rng.Next(list.Count)];
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
