using splitbill.Commands;
using splitbill.Model;
using splitbill.Service;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace splitbill.ViewModels
{
    public class CreateAccountVM : BaseVM
    {

        #region Properties

        #region Private/Readonly methods
        
        private readonly ObservableCollection<Participant> _participants;
        private readonly MainVM _main;

        #endregion

        #region Error Handling
        
        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasError));
            }
        }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        #endregion

        #region Inputs
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                (CreateCommand as RelaisCommande)?.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                (CreateCommand as RelaisCommande)?.RaiseCanExecuteChanged();
            }
        }

        private readonly INavigationService _navigation;
        public ICommand CreateCommand { get; }

        #endregion

        /// <summary>
        /// Property linked to CreateAccount.xaml.cs
        /// </summary>
        public Action? CloseAction { get; set; }

        public RelaisCommande ReturnToLoginCommand { get; }

        #endregion

        public CreateAccountVM(INavigationService navigationService, ObservableCollection<Participant> participants, MainVM main)
        {
            _navigation = navigationService;
            _participants = participants;
            _main = main;
            CreateCommand = new RelaisCommande(CreateAccount, CanCreate);
            ReturnToLoginCommand = new RelaisCommande(ReturnToLogin);
        }

        #region Account creation methods

        private bool CanCreate() =>
          !string.IsNullOrWhiteSpace(Username) &&
          !string.IsNullOrWhiteSpace(Password);


        private void CreateAccount()
        {
            // Username must be unique
            if (_participants.Any(p => p.Username == Username))
            {
                ErrorMessage = "Ce nom d'utilisateur est déjà pris.";
                return;
            }

            // Add participant and persist
            _participants.Add(new Participant(Username, Password, false));
            _main.Save();

            ReturnToLogin();
        }

        #endregion


        private void ReturnToLogin() => _navigation.NavigateToLogin();
        
    }
}