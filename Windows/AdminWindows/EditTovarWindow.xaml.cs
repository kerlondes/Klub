using Klub.Windows.ManagerWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Klub.Windows.AdminWindows
{
    /// <summary>
    /// Логика взаимодействия для EditTovarWindow.xaml
    /// </summary>
    public partial class EditTovarWindow : Window
    {

        private BDEntities bd = new BDEntities();
        public ObservableCollection<Book> TovarList { get; set; }
        public EditTovarWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            TovarList = new ObservableCollection<Book>(bd.Books.Where(t => t.Id_Status == 2 || t.Id_Status == 3).ToList());
            TovarDataGrid.ItemsSource = TovarList;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var tovar = (Book)((Button)sender).DataContext;
            // Изменяем статус товара на удалено
            tovar.Id_Status = 1;
            bd.SaveChanges();
            LoadData();
            MessageBox.Show("Товар был удален.");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddBookWindow dobavTovar = new AddBookWindow();
            dobavTovar.Show();
            this.Close();
        }
    }
}
