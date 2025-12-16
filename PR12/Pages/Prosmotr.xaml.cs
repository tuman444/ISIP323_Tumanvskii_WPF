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

namespace PR12.Pages
{
    /// <summary>
    /// Логика взаимодействия для Prosmotr.xaml
    /// </summary>
    public partial class Prosmotr : Page
    {
        public Prosmotr()
        {
            InitializeComponent();
        }
        private void B_click(object sender, RoutedEventArgs e)
        {

            Info.MVobj.Add();
            Frame MF = Info.MVobj.MainFrame;

            if (MF.CanGoForward)
            {
                MF.GoForward();
                return;
            }
            NavigationService.Navigate(new Credit());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            konf.Text = $"{Info.model}\n{Info.engine}\n{Info.color}";
            Opt.Text = $"{Info.options}";
        }
    }
}
