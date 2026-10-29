using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace splitbill.View
{
    /// <summary>
    /// Logique d'interaction pour NewExpenseUC.xaml
    /// </summary>
    public partial class NewExpenseUC : UserControl
    {
        public NewExpenseUC()
        {
            InitializeComponent();
        }

        private static readonly Regex _moneyRegex =
            new Regex(@"^\d*(\.|,)?\d{0,2}$");

        private void MoneyOnly(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            string newText = tb.Text.Insert(tb.SelectionStart, e.Text);

            e.Handled = !_moneyRegex.IsMatch(newText);
            BtnSave.IsEnabled = _moneyRegex.IsMatch(newText);
        }

        private void Montant_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            BtnSave.IsEnabled = _moneyRegex.IsMatch(tb.Text);
        }
    }
}
