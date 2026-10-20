using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage() { InitializeComponent(); LoadData(); this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) LoadData(); }; }
        private void LoadData() { Dg.ItemsSource = Core.Context.User.ToList(); }
        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddUserPage(null));
        private void BtnEdit_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddUserPage((sender as Button).DataContext as User));
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var items = Dg.SelectedItems.Cast<User>().ToList();
            if (items.Count == 0) { MessageBox.Show("Выберите записи!"); return; }
            if (MessageBox.Show($"Удалить {items.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try { Core.Context.User.RemoveRange(items); Core.Context.SaveChanges(); LoadData(); MessageBox.Show("Удалено!"); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}