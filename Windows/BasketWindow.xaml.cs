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
        Basket korzina;

        public BasketWindow(Basket korzina)
        {
            InitializeComponent();
            this.korzina = korzina;

            // Загружаем товары в корзине
            foreach (var zakaz in korzina.Orders)
            {
                var tovar = zakaz.Book;

                var item = new StackPanel { Orientation = Orientation.Horizontal };

                var itemText = new TextBlock { Text = $"{tovar.Name} - {zakaz.Quantity} шт. - {zakaz.SumOrder.ToString("C")}" };
                var increaseButton = new Button { Content = "+" };
                var decreaseButton = new Button { Content = "-" };
                var deleteButton = new Button { Content = "Удалить" };

                increaseButton.Click += (s, e) => UpdateItemQuantity(zakaz, 1);
                decreaseButton.Click += (s, e) => UpdateItemQuantity(zakaz, -1);
                deleteButton.Click += (s, e) => RemoveItemFromCart(zakaz);

                item.Children.Add(itemText);
                item.Children.Add(increaseButton);
                item.Children.Add(decreaseButton);
                item.Children.Add(deleteButton);

                ItemsListBox.Items.Add(item);
            }
        }


        private void UpdateItemQuantity(Order zakaz, int delta)
        {
            // Обновляем количество товара
            zakaz.Quantity += delta;

            if (zakaz.Quantity <= 0)
            {
                RemoveItemFromCart(zakaz);
            }
            else
            {
                zakaz.SumOrder = Convert.ToInt32(CalculateDiscountedPrice(zakaz.Book.Prise, zakaz.Book.Discount) * zakaz.Quantity);

                bd.SaveChanges();
                RefreshItemsList();
            }
        }

        private void RemoveItemFromCart(Order zakaz)
        {
            var trackedZakaz = bd.Orders.Local.FirstOrDefault(z => z.Id == zakaz.Id);

            if (trackedZakaz == null)
            {
                trackedZakaz = bd.Orders.FirstOrDefault(z => z.Id == zakaz.Id);
            }

            if (trackedZakaz != null)
            {
                // Удаляем товар из базы данных
                bd.Orders.Remove(trackedZakaz);
                bd.SaveChanges();  // Сохраняем изменения в базе данных
            }
            korzina.Orders.Remove(zakaz);
            RefreshItemsList();
        }

        private void RefreshItemsList()
        {
            // Очищаем ListBox, если это необходимо (по желанию)
            ItemsListBox.Items.Clear();

            // Добавляем обновленные элементы обратно в ListBox
            foreach (var zakaz in korzina.Orders)
            {
                var tovar = zakaz.Book;

                // Проверяем, есть ли уже элемент для этого товара в ListBox
                var existingItem = ItemsListBox.Items.Cast<StackPanel>().FirstOrDefault(item =>
                {
                    var itemText = item.Children.OfType<TextBlock>().FirstOrDefault();
                    return itemText != null && itemText.Text.Contains(tovar.Name);
                });

                if (existingItem != null)
                {
                    // Если товар уже существует в ListBox, обновляем его текст
                    var itemText = existingItem.Children.OfType<TextBlock>().First();
                    itemText.Text = $"{tovar.Name} - {zakaz.Quantity} шт. - {zakaz.SumOrder.ToString("C")}";
                }
                else
                {
                    // Если товар не найден в ListBox, добавляем новый элемент
                    var item = new StackPanel { Orientation = Orientation.Horizontal };

                    var itemText = new TextBlock { Text = $"{tovar.Name} - {zakaz.Quantity} шт. - {zakaz.SumOrder.ToString("C")}" };
                    var increaseButton = new Button { Content = "+" };
                    var decreaseButton = new Button { Content = "-" };
                    var deleteButton = new Button { Content = "Удалить" };

                    // При нажатии на "+" увеличиваем количество товара в корзине
                    increaseButton.Click += (s, e) => UpdateItemQuantity(zakaz, 1);
                    // При нажатии на "-" уменьшаем количество товара в корзине
                    decreaseButton.Click += (s, e) => UpdateItemQuantity(zakaz, -1);
                    // При нажатии на "Удалить" удаляем товар из корзины
                    deleteButton.Click += (s, e) => RemoveItemFromCart(zakaz);

                    item.Children.Add(itemText);
                    item.Children.Add(increaseButton);
                    item.Children.Add(decreaseButton);
                    item.Children.Add(deleteButton);

                    ItemsListBox.Items.Add(item);
                }
            }
        }
        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            // Рассчитываем итоговую цену без скидки (цена товара без учета скидки)
            decimal totalPriceWithoutDiscount = korzina.Orders.Sum(z => z.Quantity * z.Book.Prise);
            // Рассчитываем итоговую цену со скидкой (цена товара с учетом скидки)
            decimal totalPriceWithDiscount = korzina.Orders.Sum(z => z.Quantity * CalculateDiscountedPrice(z.Book.Prise, z.Book.Discount));
            // Сумма скидки
            decimal totalDiscount = (totalPriceWithoutDiscount - totalPriceWithDiscount);

            // Проверяем наличие товара на складе
            bool allAvailable = korzina.Orders.All(z => z.Book.Remains.HasValue && z.Book.Remains.Value >= z.Quantity);
            int deliveryDays = korzina.Orders.Count >= 3 && allAvailable ? 3 : 6;
            DateTime deliveryDate = DateTime.Now.AddDays(deliveryDays);

            foreach (var zakaz in korzina.Orders)
            {
                var tovar = zakaz.Book;

                if (tovar.Remains.HasValue && tovar.Remains.Value >= zakaz.Quantity)
                {
                    // Уменьшаем остаток товара
                    tovar.Remains -= zakaz.Quantity;
                }
                else
                {
                    MessageBox.Show($"Недостаточно товара {tovar.Name} на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Если недостаточно товара, прерываем оформление
                }
            }

            korzina.SumOrder = totalPriceWithDiscount;
            korzina.Descount = totalDiscount;
            korzina.Delivery_time = deliveryDays;
            korzina.Id_status = 2; // Статус заказа (предполагаем, что 2 — это статус, который означает подтверждение заказа)

            // Перезагружаем объект корзины из базы данных для обновления всех данных
            var reloadedKorzina = bd.Baskets.FirstOrDefault(k => k.Id == korzina.Id);
            if (reloadedKorzina != null)
            {
                reloadedKorzina.SumOrder = korzina.SumOrder;
                reloadedKorzina.Descount = korzina.Descount;
                reloadedKorzina.Delivery_time = korzina.Delivery_time;
                reloadedKorzina.Id_status = korzina.Id_status;
                // Сохраняем изменения в базе данных
                bd.SaveChanges();
            }

            // Сохраняем изменения в базе данных для каждого товара (обновленные остатки)
            bd.SaveChanges();

            MessageBox.Show($"Итоговая сумма: {totalPriceWithDiscount.ToString("C")}\nСумма скидки: {totalDiscount.ToString("C")}\nДата доставки: {deliveryDate.ToShortDateString()}", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Information);

            // Генерация PDF с QR-кодом
            GenerateOrderPdf(korzina);

            // Переходим в главное окно
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
            decimal finalSum = korzina.Orders.Sum(z => z.SumOrder);
        }

        private void GenerateOrderPdf(Basket korzina)
        {
            string orderInfo = GetOrderInfo(korzina); // Формируем информацию для QR-кода

            // Генерация QR-кода
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300
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
                string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Order_" + korzina.Id + ".pdf");

                // Сохраняем PDF
                File.WriteAllBytes(outputDirectory, ms.ToArray());

                MessageBox.Show("PDF документ с QR-кодом был успешно создан! Можете найти его по адресу " + outputDirectory, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        // Формируем строку с информацией о заказе для QR-кода
        private string GetOrderInfo(Basket korzina)
        {
            return $"Data order: {DateTime.Now.ToShortDateString()}\n" +
                   $"Data dostavki: {DateTime.Now.AddDays(3).ToShortDateString()}\n" +
                   $"Number order: {korzina.Id}\n" +
                   $"Sostav order: {string.Join(", ", korzina.Orders.Select(z => z.Book.Name))}\n" +
                   $"Sum order: {korzina.SumOrder}\n" +
                   $"Sum sale: {korzina.Descount}\n" +
                   $"Kod poluchenija: {korzina.GenericCode}";
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
