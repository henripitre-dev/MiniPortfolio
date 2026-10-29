using GoGame.Commands;
using GoGame.Model;
using GoGame.Service;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
namespace GoGame.ViewModels
{
    public class MainVM : BaseVM
    {
        #region Properties

        #region Functional Properties

        /// <summary>
        /// Save FilePath in bin
        /// </summary>
        private static readonly string DataFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "data.xml"));

        private NavigationService _navigation;
        public BaseVM CurrentView => _navigation.CurrentView;

        #endregion

        #region Project Properties
        public ICommand GoToCreateGameCommand { get; }

        #endregion

        #endregion

        public MainVM()
        {
            _navigation = new NavigationService(this);

            _navigation.CurrentViewChanged += () =>
                    OnPropertyChanged(nameof(CurrentView));

            GoToCreateGameCommand = new RelaisCommande(() =>
            {
                _navigation.NavigateToCreateGame();
            });
        }
    }
}
