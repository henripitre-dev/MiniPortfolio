using GoGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GoGame.Model;

namespace GoGame.Service
{
    #region Interface de Navigation 

    //Une interface définit un contrat.
    //Tout class, record ou struct qui implémente ce contrat doit fournir une implémentation des membres définis dans l’interface.
    public interface INavigationService
    {
        BaseVM CurrentView { get; }

        void NavigateToCreateGame();
        void NavigateToNull();
        void NavigateToGame(Game game);
    }

    #endregion

    //Classe qui découle de l'interface ci-haut, et qui implémente l'action de créer/naviguer entre les différentes instances de l'application.
    //TODO:  Vérifier si cette forme de navigation entraine une création en boucle de nouvelles instances. Si oui, est-ce que GarbageCollector le ramasse ?????
    public class NavigationService(MainVM main) : INavigationService
    {
        private BaseVM? _currentView;

        public BaseVM? CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                CurrentViewChanged?.Invoke(); //Un peu comme NotifyPropertyChanged, notifie que changement dans la CurrentView.
            }
        }

        public event Action? CurrentViewChanged;

        private readonly MainVM _main = main;

        //Toujours ajouter le lien VM --> View dans App.xaml.cs après la création d'une méthode de navigation 
        #region Méthodes distinctes pour chaque nouvelle navigation liée à un VM et à une vue

        public void NavigateToCreateGame() =>
            CurrentView = new CreateGameVM(this);

        public void NavigateToGame(Game game) =>
            CurrentView = new GameVM(this, game);
        public void NavigateToNull() =>
            CurrentView = null;
        #endregion
    }
}
