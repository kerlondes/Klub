using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Klub.Windows
{
    /// <summary>
    /// Логика взаимодействия для ViewSupplyWindow.xaml
    /// </summary>
    public partial class ViewSupplyWindow : Window
    {
        private BDEntities bd = new BDEntities();
        public ObservableCollection<Book> TovarList { get; set; }
        public ViewSupplyWindow()
        {
            InitializeComponent();
            LoadTovarData();
        }
        private void LoadTovarData()
        {
            // Загрузка данных о товарах с остатками, включая статус
            var tovarData = bd.Books
                .Include(t => t.Status_Book) // Загружаем статус товара
                .Where(t => t.Remains.HasValue) // Только товары с остатками
                .ToList();

            // Преобразуем данные в ObservableCollection для привязки
            TovarList = new ObservableCollection<Book>(tovarData);
            // Устанавливаем привязку для DataGrid
            ZakazDataGrid.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SupplyWindow supply = new SupplyWindow();
            supply.Show();
            this.Close();
        }
    }
}
