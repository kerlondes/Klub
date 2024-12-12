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
        public ObservableCollection<Order> orderList { get; set; }
        public EditOrderWindow()
        {
            InitializeComponent();
            LoadorderData();
        }

        private void LoadorderData()
        {
            // Используем Include для загрузки связанных объектов (Polz и Statusorder)
            var orderData = bd.Orders
                .Include(z => z.Basket)  // Загрузка связанной корзины
                .Include(z => z.Book)    // Загрузка связанного товара
                .Include(z => z.Basket.User)        // Загрузка пользователя
                .Include(z => z.Basket.Status_Basket) // Загрузка статуса заказа
                .ToList();

            orderList = new ObservableCollection<Order>(orderData);
            orderDataGrid.DataContext = this;
        }

        private void SaveChanges()
        {
            // Проверяем каждую запись в коллекции orderList на изменения
            foreach (var item in orderList)
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
