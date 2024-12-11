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
    /// Логика взаимодействия для AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        BDEntities bd = new BDEntities();
        public AddBookWindow()
        {
            InitializeComponent();
            // Заполняем ComboBox производителями
            autor.ItemsSource = bd.Suppliers.ToList();
            autor.DisplayMemberPath = "Name"; // Замените на имя поля, которое вы хотите показывать в ComboBox
            autor.SelectedValuePath = "ID";  // Это будет значение, которое будет использоваться в коде
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Name = nameTextBox.Text;
            string Opis = descriptionTextBlock.Text;

            // Проверка на пустые значения
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Opis) || string.IsNullOrEmpty(priceTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            decimal Prise;
            if (!decimal.TryParse(priceTextBox.Text, out Prise))
            {
                MessageBox.Show("Пожалуйста, введите корректную цену.");
                return;
            }

            // Получаем выбранного производителя из ComboBox
            var selectedProizvoditel = (Supplier)autor.SelectedItem;
            if (selectedProizvoditel == null)
            {
                MessageBox.Show("Пожалуйста, выберите производителя.");
                return;
            }

            // Получаем путь к изображению
            string imagePath = GetRelativeImagePath(image.Source != null ? image.Source.ToString() : string.Empty);

            // Создаем новый товар
            var newTovar = new Book
            {
                Name = Name,
                Description = Opis,
                Prise = Prise,
                Id_Status = 3, // предполагаем, что статус 3 означает "в наличии"
                Remains = 0,  // количество товара
                Id_Supplier = selectedProizvoditel.Id, // Устанавливаем производителя
                Image = imagePath  // Используем полученный путь к изображению
            };

            // Добавляем товар в базу данных
            bd.Books.Add(newTovar);
            bd.SaveChanges(); // Сохраняем изменения в базе данных

            MessageBox.Show("Товар успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close(); // Закрываем окно после добавления товара
        }

        private string GetRelativeImagePath(string imageFileName)
        {
            // Убираем префикс "file:///" если он есть
            if (imageFileName.StartsWith("file://"))
            {
                imageFileName = new Uri(imageFileName).LocalPath;
            }

            // Путь к папке с изображениями (относительно корня проекта)
            string imagesFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            // Проверка существования папки
            if (!System.IO.Directory.Exists(imagesFolderPath))
            {
                // Если папка не существует, создаем её
                System.IO.Directory.CreateDirectory(imagesFolderPath);
            }

            // Если файл изображения существует, сохраняем его в папке "Images"
            if (System.IO.File.Exists(imageFileName))
            {
                string fileName = System.IO.Path.GetFileName(imageFileName);
                string destinationPath = System.IO.Path.Combine(imagesFolderPath, fileName);

                if (!System.IO.File.Exists(destinationPath))
                {
                    System.IO.File.Copy(imageFileName, destinationPath);
                }

                // Возвращаем только имя файла с расширением
                return fileName;
            }

            // Если файл не существует, возвращаем имя изображения по умолчанию
            return "default_image.png";
        }


        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалог выбора изображения
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // фильтр для изображений

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                // Устанавливаем путь к изображению
                string imagePath = dlg.FileName;
                image.Source = new BitmapImage(new Uri(imagePath)); // Загружаем картинку в Image
            }
        }
    }
}
