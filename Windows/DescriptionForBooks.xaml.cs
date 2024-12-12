using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Klub.Windows
{
    public partial class DescriptionForBooks : Window
    {
        private readonly BDEntities bd = new BDEntities();
        private readonly Book tovar; // Поле для хранения товара

        public DescriptionForBooks(Book tovar)
        {
            InitializeComponent();
            this.tovar = tovar;

            // Заполняем данные о товаре
            image.Source = LoadImage(tovar.Image); // Обновленный метод для загрузки изображения
            nameTextBox.Text = tovar.Name;
            authorTextBox.Text = tovar.Supplier?.Name ?? "Не указан";
            descriptionTextBlock.Text = tovar.Description;
            priceTextBox.Text = tovar.Prise.ToString("C");
            saleTextBox.Text = tovar.Discount.HasValue ? (tovar.Discount.Value / 100.0).ToString("0%") : "Нет скидки";

            // Блокируем возможность изменения
            nameTextBox.IsReadOnly = true;
            authorTextBox.IsReadOnly = true;
            priceTextBox.IsReadOnly = true;
            saleTextBox.IsReadOnly = true;
            descriptionTextBlock.IsHitTestVisible = false; // Запрещаем взаимодействие с TextBlock
        }

        private BitmapImage LoadImage(string imagePath)
        {
            try
            {
                // Если путь указан, создаем изображение
                if (!string.IsNullOrWhiteSpace(imagePath) && System.IO.File.Exists(GetFullImagePath(imagePath)))
                {
                    string fullPath = GetFullImagePath(imagePath);
                    return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Если что-то пошло не так, возвращаем изображение по умолчанию
            return new BitmapImage(new Uri("pack://application:,,,/Resources/default_image.png", UriKind.Absolute));
        }

        private string GetFullImagePath(string relativePath)
        {
            // Формируем полный путь на основе пути к папке приложения
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", relativePath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = bd.Users.FirstOrDefault(u => u.Id == CurrentUser.UserId);
            if (currentUser == null)
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем корзину текущего пользователя
            var korzina = currentUser.Baskets.FirstOrDefault(k => k.Id_status == 1); // Корзина в статусе "в процессе"
            if (korzina == null)
            {
                // Если корзина не найдена, создаем новую корзину
                korzina = new Basket
                {
                    Id_User = currentUser.Id,
                    Date = DateTime.Now,
                    Id_status = 1, // Статус "в процессе"
                    SumOrder = 0,
                    Descount = 0,
                    Delivery_time = 0,
                    GenericCode = GenerateRandomCode()
                };
                bd.Baskets.Add(korzina);
                bd.SaveChanges(); // Сохраняем корзину в базе данных
            }

            var existingZakaz = korzina.Orders.FirstOrDefault(z => z.Id_book == tovar.Id);

            if (existingZakaz != null)
            {
                existingZakaz.Quantity += 1;
                decimal discountedPrice = CalculateDiscountedPrice(tovar.Prise, tovar.Discount);
                existingZakaz.SumOrder = (int)(discountedPrice * existingZakaz.Quantity);
                bd.SaveChanges();

                MessageBox.Show("Количество товара в корзине увеличено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Если товара нет в корзине, добавляем его
                decimal discountedPrice = CalculateDiscountedPrice(tovar.Prise, tovar.Discount);
                int sum = (int)discountedPrice;

                var zakaz = new Order
                {
                    Id_book = tovar.Id,
                    Id_Busket = korzina.Id,
                    Quantity = 1, // Начинаем с 1
                    SumOrder = sum
                };

                // Добавляем заказ в корзину
                korzina.Orders.Add(zakaz);
                bd.SaveChanges(); // Сохраняем изменения в базе данных

                MessageBox.Show("Товар добавлен в корзину!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private int GenerateRandomCode()
        {
            Random random = new Random();
            // Генерируем случайное трехзначное число (от 100 до 999)
            return random.Next(100, 1000);
        }

        private decimal CalculateDiscountedPrice(decimal originalPrice, decimal? discount)
        {
            if (discount.HasValue && discount.Value > 0)
            {
                // Скидка в процентах, например, скидка 50% означает, что discount = 50
                decimal discountedPrice = originalPrice * (1 - discount.Value / 100);
                return discountedPrice; // Возвращаем цену с учетом скидки в виде decimal
            }
            else
            {
                // Если скидки нет, возвращаем исходную цену
                return originalPrice;
            }
        }

    }
}
