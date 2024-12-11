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

namespace Klub.Windows
{
    /// <summary>
    /// Логика взаимодействия для SupplyWindow.xaml
    /// </summary>
    public partial class SupplyWindow : Window
    {
        private BDEntities bd = new BDEntities();
        public SupplyWindow()
        {
            InitializeComponent();
            LoadTovarData();
        }

        private void LoadTovarData()
        {
            var tovarList = bd.Books.ToList();
            TovarComboBox.ItemsSource = tovarList;
            TovarComboBox.DisplayMemberPath = "Name"; // Отображаем название товара
            TovarComboBox.SelectedValuePath = "Id"; // Используем ID товара как выбранное значение
        }

        private void PostavkaButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный товар и количество из поля ввода
            var selectedTovarId = (int)TovarComboBox.SelectedValue;
            var quantity = int.Parse(QuantityTextBox.Text);

            // Находим товар в базе данных
            var tovar = bd.Books.SingleOrDefault(t => t.Id == selectedTovarId);

            if (tovar != null)
            {
                // Обновляем остаток товара
                tovar.Remains = (tovar.Remains ?? 0) + quantity;
                if (tovar.Remains > 0)
                {
                    tovar.Id_Status = 2; // статус "На складе" 
                }
                try
                {
                    bd.SaveChanges();
                    MessageBox.Show("Поставка товара успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ViewSupplyWindow supplyWindow = new ViewSupplyWindow();
            supplyWindow.Show();
            this.Close();
        }
    }
}
