using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class UserPage : Page
    {
        public UserPage() { InitializeComponent(); UpdateData(); }
        private void UpdateData()
        {
            var q = Core.Context.User.AsQueryable();
            if (!string.IsNullOrWhiteSpace(TbFilter.Text)) q = q.Where(u => u.FIO.ToLower().Contains(TbFilter.Text.ToLower()));
            if (ChbAdmin != null && ChbAdmin.IsChecked == true)
                q = q.Where(u => u.Role == "Admin");
            DgUsers.ItemsSource = CbSort.SelectedIndex == 0 ? q.OrderBy(u => u.FIO).ToList() : q.OrderByDescending(u => u.FIO).ToList();
        }
        private void TbFilter_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();
        private void ChbAdmin_Changed(object sender, RoutedEventArgs e) => UpdateData();
        private void BtnClear_Click(object sender, RoutedEventArgs e) { TbFilter.Text = ""; CbSort.SelectedIndex = 0; ChbAdmin.IsChecked = false; UpdateData(); }
    }
}