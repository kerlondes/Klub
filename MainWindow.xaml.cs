using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Klub
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BDEntities bd = new BDEntities();
        public MainWindow()
        {
            InitializeComponent();
            LoadTovars();
            UpdateCartButtonVisibility();
        }

        private void BusketButton_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = bd.Users.FirstOrDefault(u => u.Id == CurrentUser.UserId);
            if (currentUser == null)
            {
                MessageBox.Show("Для начала авторизуйтесь!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем корзину текущего пользователя
            var korzina = currentUser.Baskets.FirstOrDefault(k => k.Id_status == 1); // Корзина в статусе "в процессе"
            if (korzina == null)
            {
                MessageBox.Show("Ваша корзина пуста!", "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Открываем окно с корзиной и передаем туда данные
            Windows.BasketWindow korzinaWindow = new Windows.BasketWindow(korzina);
            korzinaWindow.Show();
            this.Close();
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Очищаем текущие товары на UI
            TovarsContainer.Children.Clear();

            // Получаем текст для поиска
            string searchText = Naim.Text.ToLower();

            // Загружаем товары, фильтруем их по введенному тексту в поисковом поле
            var filteredTovars = bd.Books
                .Where(t => (t.Name.ToLower().Contains(searchText)) && (t.Id_Status == 2 || t.Id == 3)) // Фильтруем по названию
                .OrderBy(t => t.Name) // Сортируем по алфавиту
                .ToList();

            // Добавляем товары на UI
            foreach (var tovar in filteredTovars)
            {
                AddTovarToUI(tovar);
            }
        }
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.AuthWindow auth = new Windows.AuthWindow();
            auth.Show();
            this.Close();
        }
        private void UpdateCartButtonVisibility()
        {
            // Проверяем, есть ли хотя бы один товар в корзине
            var userCart = bd.Baskets.FirstOrDefault(k => k.Id_User == CurrentUser.UserId && k.Id_status == 1);

            if (userCart != null && userCart.Orders.Any())
            {
                // Показываем кнопку "Корзина", если она еще не видна
                if (BusketButton.Visibility != Visibility.Visible)
                {
                    BusketButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Скрываем кнопку "Корзина", если товаров нет
                if (BusketButton.Visibility != Visibility.Hidden)
                {
                    BusketButton.Visibility = Visibility.Hidden;
                }
            }
        }
        private void LoadTovars()
        {
            try
            {
                var tovars = bd.Books
                    .Where(t => t.Id_Status == 2 || t.Id_Status == 3)
                    .OrderBy(t => t.Name) // Сортируем товары по алфавиту
                    .ToList();

                // Создаем Canvas для каждого товара
                foreach (var tovar in tovars)
                {
                    AddTovarToUI(tovar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
        private string GetRelativeImagePath(string imageFileName)
        {
            // Путь к папке с изображениями (относительно корня проекта)
            string imagesFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            // Проверка существования папки
            if (!System.IO.Directory.Exists(imagesFolderPath))
            {
                // Если папка не существует, создаем её
                System.IO.Directory.CreateDirectory(imagesFolderPath);
            }

            // Строим полный путь к файлу
            string fullImagePath = System.IO.Path.Combine(imagesFolderPath, imageFileName);

            // Проверка существования изображения
            if (System.IO.File.Exists(fullImagePath))
            {
                return fullImagePath;
            }
            else
            {
                // Если изображения нет, возвращаем путь к изображению по умолчанию
                return System.IO.Path.Combine(imagesFolderPath, "default_image.png");
            }
        }

        private void AddTovarToUI(Book tovar)
        {
            // Создаем Canvas для товара
            Canvas tovarCanvas = new Canvas
            {
                Margin = new Thickness(0, 10, 0, 0),
                Width = 497,
                Height = 79,
                Background = Brushes.White
            };
            tovarCanvas.SetValue(BorderBrushProperty, Brushes.DarkGreen);

            // Получаем относительный путь к изображению
            string imagePath = GetRelativeImagePath(tovar.Image);

            // Создаем объект Image
            Image image = new Image
            {
                Width = 62,
                Height = 53
            };

            // Создаем BitmapImage с использованием относительного пути
            image.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            Canvas.SetLeft(image, 10);
            Canvas.SetTop(image, 8);
            tovarCanvas.Children.Add(image);

            // Добавляем название товара
            Label nameLabel = new Label
            {
                Content = tovar.Name,
                Width = 225
            };
            Canvas.SetLeft(nameLabel, 82);
            Canvas.SetTop(nameLabel, 8);
            tovarCanvas.Children.Add(nameLabel);

            // Добавляем автора товара
            Label authorLabel = new Label
            {
                Content = tovar.Supplier?.Name ?? "Не указан",
                Width = 225
            };
            Canvas.SetLeft(authorLabel, 82);
            Canvas.SetTop(authorLabel, 40);
            tovarCanvas.Children.Add(authorLabel);

            // Добавляем метку для "Цена"
            Label priceLabel = new Label
            {
                Content = "Цена",
                Width = 50,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Canvas.SetLeft(priceLabel, 400);
            Canvas.SetTop(priceLabel, 8);
            tovarCanvas.Children.Add(priceLabel);

            // Добавляем цену товара с учетом скидки
            Label priceValueLabel = new Label
            {
                Content = CalculateDiscountedPrice(tovar.Prise, tovar.Discount),
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Canvas.SetLeft(priceValueLabel, 400);
            Canvas.SetTop(priceValueLabel, 40);
            tovarCanvas.Children.Add(priceValueLabel);

            // Обработчик левого клика (открытие подробной информации о товаре)
            tovarCanvas.MouseLeftButtonUp += (sender, e) =>
            {
                Windows.DescriptionForBooks tovarMnogoWindow = new Windows.DescriptionForBooks(tovar);
                tovarMnogoWindow.Show();
            };

            // Обработчик ПКМ (добавление товара в корзину)
            tovarCanvas.MouseRightButtonUp += (sender, e) =>
            {
                AddToCart(tovar); // Вызываем метод добавления в корзину
            };

            // Добавление выделения при наведении
            tovarCanvas.MouseEnter += (sender, e) =>
            {
                tovarCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
                tovarCanvas.Cursor = Cursors.Hand;
            };

            tovarCanvas.MouseLeave += (sender, e) =>
            {
                tovarCanvas.Background = Brushes.White;
                tovarCanvas.Cursor = Cursors.Arrow;
            };

            // Добавляем Canvas в контейнер на экране
            TovarsContainer.Children.Add(tovarCanvas);
        }
        private void AddToCart(Book tovar)
        {
            if (tovar == null)
            {
                MessageBox.Show("Ошибка: товар не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем текущего пользователя
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
                    SumOrder = 0, // Начальная сумма
                    Descount = 0, // Начальная скидка
                    Delivery_time = 0, // Срок не определен
                    GenericCode = GenerateRandomCode(), // Генерируем случайный код
                    Orders = new List<Order>() // Инициализация коллекции Zakazs
                };
                bd.Baskets.Add(korzina);
                bd.SaveChanges(); // Сохраняем корзину в базе данных
            }

            // Проверяем, есть ли уже этот товар в корзине
            var existingZakaz = korzina.Orders.FirstOrDefault(z => z.Id_book == tovar.Id);

            // Получаем цену с учетом скидки
            decimal discountedPrice = CalculateDiscountedPrice(tovar.Prise, tovar.Discount);

            // Приводим цену к целому числу
            int discountedPriceAsInt = Convert.ToInt32(Math.Round(discountedPrice));

            if (existingZakaz != null)
            {
                // Если товар уже есть в корзине, увеличиваем его количество и пересчитываем сумму
                existingZakaz.Quantity += 1;
                existingZakaz.SumOrder = existingZakaz.Quantity * discountedPriceAsInt;
            }
            else
            {
                // Если товара нет в корзине, добавляем его как новый заказ
                var zakaz = new Order
                {
                    Id_book = tovar.Id, // ID товара
                    Id_Busket = korzina.Id, // ID корзины
                    Quantity = 1, // Количество товара
                    SumOrder = discountedPriceAsInt // Сумма с учетом скидки
                };

                // Добавляем заказ в корзину
                korzina.Orders.Add(zakaz);
            }

            // Сохраняем изменения в базе данных
            bd.SaveChanges();

            // Обновляем видимость кнопки "Корзина"
            UpdateCartButtonVisibility();

            MessageBox.Show("Товар добавлен в корзину!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private int GenerateRandomCode()
        {
            Random random = new Random();
            // Генерируем случайное трехзначное число (от 100 до 999)
            return random.Next(100, 1000);  // Возвращаем как int
        }
        public decimal CalculateDiscountedPrice(decimal originalPrice, decimal? discount)
        {
            if (discount.HasValue && discount.Value > 0)
            {
                // Возвращаем цену с учетом скидки как decimal
                return originalPrice * (1 - discount.Value / 100);
            }
            else
            {
                // Если скидки нет, возвращаем исходную цену
                return originalPrice;
            }
        }

    }
}
