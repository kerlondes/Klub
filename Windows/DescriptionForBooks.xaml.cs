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
    /// Логика взаимодействия для DescriptionForBooks.xaml
    /// </summary>
    public partial class DescriptionForBooks : Window
    {
        BDEntities gl = new BDEntities();
        private Book tovar;  // Добавляем поле для хранения товара
        public DescriptionForBooks(Book tovar)
        {
            InitializeComponent();
        }
    }
}
