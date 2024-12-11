using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System;
using System.Data.Entity;

namespace Klub.Windows.ManagerWindows
{
    /// <summary>
    /// Логика взаимодействия для EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        private BDEntities bd = new BDEntities();
        public ObservableCollection<Order> ZakazList { get; set; }
        public EditOrderWindow()
        {
            InitializeComponent();
            LoadZakazData();
        }

        private void LoadZakazData()
        {
            // Используем Include для загрузки связанных объектов (Polz и StatusZakaz)
            var zakazData = bd.Orders
                .Include(z => z.Basket)  // Загрузка связанной корзины
                .Include(z => z.Book)    // Загрузка связанного товара
                .Include(z => z.Basket.User)        // Загрузка пользователя
                .Include(z => z.Basket.Status_Basket) // Загрузка статуса заказа
                .ToList();

            ZakazList = new ObservableCollection<Order>(zakazData);
            ZakazDataGrid.DataContext = this;
        }

        private void SaveChanges()
        {
            // Проверяем каждую запись в коллекции ZakazList на изменения
            foreach (var item in ZakazList)
            {
                bd.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            // Сохраняем изменения в базу данных
            try
            {
                bd.SaveChanges();
                MessageBox.Show("Изменения сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveChanges();
        }
    }
}
