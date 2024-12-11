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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        BDEntities bd = new BDEntities();
        public AuthWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = log.Text;
            string password = pas.Password;

            // Поиск пользователя в базе данных по логину и паролю
            var user = bd.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                CurrentUser.UserId = user.Id;
                CurrentUser.UserRole = user.Role.Name;

                if (user.Id_Role == 2)  // Если роль - менеджер
                {
                    ManagerWindows.ManagerWindow managerWindow = new ManagerWindows.ManagerWindow();
                    managerWindow.Show();
                }
                else if (user.Id_Role == 3)  // Если роль - администратор
                {
                    AdminWindows.AdminWindow managerWindow = new AdminWindows.AdminWindow();
                    managerWindow.Show();
                }
                else
                {
                    // Open MainWindow for other roles
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }

                // Close the authorization window
                this.Close();
            }
            else
            {
                // If user not found, show an error message
                MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
