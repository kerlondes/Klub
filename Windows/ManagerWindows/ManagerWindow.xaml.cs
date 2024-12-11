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

namespace Klub.Windows.ManagerWindows
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow n = new MainWindow();
            n.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EditOrderWindow editOrder = new EditOrderWindow();
            editOrder.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ViewSupplyWindow supplyWindow = new ViewSupplyWindow();
            supplyWindow.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AddBookWindow addBook = new AddBookWindow();
            addBook.Show();
        }
    }
}
