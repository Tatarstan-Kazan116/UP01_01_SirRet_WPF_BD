using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage() { InitializeComponent(); }
        private void BtnNav_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            Page p = tag switch
            {
                "User" => new UsersTabPage(),
                "Client" => new ClientTabPage(),
                "Car" => new CarTabPage(),
                "Employee" => new EmployeeTabPage(),
                "Category" => new CategoryTabPage(),
                "Payment" => new PaymentTabPage(),
                "Diagramm" => new DiagrammPage(),
                _ => null
            };
            if (p != null) NavigationService.Navigate(p);
        }
    }
}