using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZXing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing;


namespace Klub.Windows
{
    /// <summary>
    /// Логика взаимодействия для BasketWindow.xaml
    /// </summary>
    public partial class BasketWindow : Window
    {
        BDEntities bd = new BDEntities();
        Basket basket;

        public BasketWindow(Basket basket)
        {
            InitializeComponent();
            this.basket = basket;

            RefreshItemsList();
        }
        public class OrderViewModel
        {
            public string DisplayText => $"{Order.Book.Name} - {Order.Quantity} шт. - {Order.SumOrder.ToString("C")}";
            public Order Order { get; }

            public OrderViewModel(Order order)
            {
                Order = order;
            }
        }
        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is OrderViewModel orderViewModel)
            {
                UpdateItemQuantity(orderViewModel.Order, 1);
                RefreshItemsList();
            }
        }

        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is OrderViewModel orderViewModel)
            {
                UpdateItemQuantity(orderViewModel.Order, -1);
                RefreshItemsList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is OrderViewModel orderViewModel)
            {
                RemoveItemFromCart(orderViewModel.Order);
                RefreshItemsList();
            }
        }

        private void UpdateItemQuantity(Order order, int delta)
        {
            // Обновляем количество товара
            order.Quantity += delta;

            if (order.Quantity <= 0)
            {
                RemoveItemFromCart(order);
            }
            else
            {
                order.SumOrder = Convert.ToInt32(CalculateDiscountedPrice(order.Book.Prise, order.Book.Discount) * order.Quantity);

                bd.SaveChanges();
                RefreshItemsList();
            }
        }

        private void RemoveItemFromCart(Order order)
        {
            var trackedorder = bd.Orders.Local.FirstOrDefault(z => z.Id == order.Id);

            if (trackedorder == null)
            {
                trackedorder = bd.Orders.FirstOrDefault(z => z.Id == order.Id);
            }

            if (trackedorder != null)
            {
                // Удаляем товар из базы данных
                bd.Orders.Remove(trackedorder);
                bd.SaveChanges();  // Сохраняем изменения в базе данных
            }
            basket.Orders.Remove(order);
            RefreshItemsList();
        }

        private void RefreshItemsList()
        {
            var items = basket.Orders.Select(order => new OrderViewModel(order)).ToList();
            ItemsListView.ItemsSource = items; // Привязка данных
        }
        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            // Рассчитываем итоговую цену без скидки (цена товара без учета скидки)
            decimal totalPriceWithoutDiscount = basket.Orders.Sum(z => z.Quantity * z.Book.Prise);
            // Рассчитываем итоговую цену со скидкой (цена товара с учетом скидки)
            decimal totalPriceWithDiscount = basket.Orders.Sum(z => z.Quantity * CalculateDiscountedPrice(z.Book.Prise, z.Book.Discount));
            // Сумма скидки
            decimal totalDiscount = (totalPriceWithoutDiscount - totalPriceWithDiscount);

            // Проверяем наличие товара на складе
            bool allAvailable = basket.Orders.All(z => z.Book.Remains.HasValue && z.Book.Remains.Value >= z.Quantity);
            int deliveryDays = basket.Orders.Count >= 3 && allAvailable ? 3 : 6;
            DateTime deliveryDate = DateTime.Now.AddDays(deliveryDays);

            foreach (var order in basket.Orders)
            {
                var tovar = order.Book;

                if (tovar.Remains.HasValue && tovar.Remains.Value >= order.Quantity)
                {
                    // Уменьшаем остаток товара
                    tovar.Remains -= order.Quantity;
                }
                else
                {
                    MessageBox.Show($"Недостаточно товара {tovar.Name} на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Если недостаточно товара, прерываем оформление
                }
            }

            basket.SumOrder = totalPriceWithDiscount;
            basket.Descount = totalDiscount;
            basket.Delivery_time = deliveryDays;
            basket.Id_status = 2; // Статус заказа (предполагаем, что 2 — это статус, который означает подтверждение заказа)

            // Перезагружаем объект корзины из базы данных для обновления всех данных
            var reloadedbasket = bd.Baskets.FirstOrDefault(k => k.Id == basket.Id);
            if (reloadedbasket != null)
            {
                reloadedbasket.SumOrder = basket.SumOrder;
                reloadedbasket.Descount = basket.Descount;
                reloadedbasket.Delivery_time = basket.Delivery_time;
                reloadedbasket.Id_status = basket.Id_status;
                // Сохраняем изменения в базе данных
                bd.SaveChanges();
            }

            // Сохраняем изменения в базе данных для каждого товара (обновленные остатки)
            bd.SaveChanges();

            MessageBox.Show($"Итоговая сумма: {totalPriceWithDiscount.ToString("C")}\nСумма скидки: {totalDiscount.ToString("C")}\nДата доставки: {deliveryDate.ToShortDateString()}", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Information);

            // Генерация PDF с QR-кодом
            GenerateOrderPdf(basket);

            // Переходим в главное окно
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
            decimal finalSum = basket.Orders.Sum(z => z.SumOrder);
        }

        private void GenerateOrderPdf(Basket basket)
        {
            string orderInfo = GetOrderInfo(basket); // Формируем информацию для QR-кода

            // Генерация QR-кода
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 1,
                    Hints = { { ZXing.EncodeHintType.CHARACTER_SET, "UTF-8" } }
                }
            };
            var qrCodeImage = barcodeWriter.Write(orderInfo);

            // Создаем PDF-документ
            Document doc = new Document(PageSize.A4);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Добавляем QR-код в PDF
                iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(qrCodeImage, System.Drawing.Imaging.ImageFormat.Png);
                qrImage.ScaleToFit(100f, 100f);
                qrImage.SetAbsolutePosition(450f, 600f);  // Устанавливаем позицию QR-кода на странице
                doc.Add(qrImage);

                doc.Close();

                // Получаем путь к папке bin\Debug текущего проекта
                string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Order_" + basket.Id + ".pdf");

                // Сохраняем PDF
                File.WriteAllBytes(outputDirectory, ms.ToArray());

                MessageBox.Show("PDF документ с QR-кодом был успешно создан! Можете найти его по адресу " + outputDirectory, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        // Формируем строку с информацией о заказе для QR-кода
        private string GetOrderInfo(Basket basket)
        {
            return $"Дата заказа: {DateTime.Now.ToShortDateString()}\n" +
                   $"Дата доставки: {DateTime.Now.AddDays(3).ToShortDateString()}\n" +
                   $"Номер заказа: {basket.Id}\n" +
                   $"В заказ входят следующие позиции: {string.Join(", ", basket.Orders.Select(z => z.Book.Name))}\n" +
                   $"Сумма заказа: {basket.SumOrder}\n" +
                   $"Скидка: {basket.Descount}\n" +
                   $"Генерируемый код доставки: {basket.GenericCode}";
        }


        private decimal CalculateDiscountedPrice(decimal originalPrice, decimal? discount)
        {
            if (discount.HasValue && discount.Value > 0)
            {
                // Скидка в процентах
                return originalPrice * (1 - discount.Value / 100);
            }
            else
            {
                return originalPrice;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
