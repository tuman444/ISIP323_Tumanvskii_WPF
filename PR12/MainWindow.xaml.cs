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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR12
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Return_click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
                Reduce();
            }
        }

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Progress != null)
            {
                if (Progress.Value == 5)
                {
                    var result = MessageBox.Show("Есть несохранённые изменения. Покинуть страницу?", "Подтверждение",
                      MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        Add();
                        e.Cancel = true;
                    }
                }
            }
        }

        public void Add()
        {
            Progress.Value += 1;
        }
        public void Reduce()
        {
            Progress.Value -= 1;
        }

        private void MainFrame_NavigationStopped(object sender, NavigationEventArgs e)
        {
            Add();
        }
    }
}
