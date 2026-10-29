using splitbill.Commands;
using splitbill.Model;
using splitbill.Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace splitbill.ViewModels
{
    public class LoginVM : BaseVM
    {
        #region Fields

        private readonly INavigationService _navigation;
        private readonly ObservableCollection<Participant> _participants;

        private readonly MainVM _main;

        #region Inputs

        private string _nameInput;
        public string NameInput
        {
            get => _nameInput;
            set
            {
                if (_nameInput != value)
                {
                    _nameInput = value;
                    OnPropertyChanged(nameof(NameInput));
                    (LoginCommand as RelaisCommande)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _pwdInput;
        public string PwdInput
        {
            get => _pwdInput;
            set
            {
                if (_pwdInput != value)
                {
                    _pwdInput = value;
                    OnPropertyChanged(nameof(PwdInput));
                    (LoginCommand as RelaisCommande)?.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Commands
        
        public ICommand GoToCreateAccount { get; }
        public ICommand LoginCommand { get; }

        #endregion

        #region Error Handling

        private string _errorMessage;
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

        #endregion

        public LoginVM(INavigationService navigation, ObservableCollection<Participant> participants, MainVM main)
        {
            _navigation = navigation;
            _participants = participants;

            _main = main;
            LoginCommand = new RelaisCommande(Login, CanLogin);
            GoToCreateAccount= new RelaisCommande(() =>{_navigation.NavigateToCreateAccount();});

        }

        #region Login Logic

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(NameInput)
                && !string.IsNullOrWhiteSpace(PwdInput);
        }

        private void Login() => ErrorMessage =  _main.Login(NameInput, PwdInput, _participants);
        

        #endregion
    }
}