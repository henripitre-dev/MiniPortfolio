using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace splitbill.View
{
    /// <summary>
    /// Logique d'interaction pour CreateAccountWindow.xaml
    /// </summary>
    public partial class CreateAccountWindow : UserControl
    {
        public CreateAccountWindow()
        {
            InitializeComponent();

            // Once DataContext is set, wire up CloseAction
            DataContextChanged += (s, e) =>
            {
                if (DataContext is ViewModels.CreateAccountVM vm)
                    vm.CloseAction = () =>
                        Window.GetWindow(this)?.Close();
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.CreateAccountVM vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}