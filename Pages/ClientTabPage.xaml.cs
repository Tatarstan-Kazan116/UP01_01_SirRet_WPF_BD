using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class ClientTabPage : Page
    {
        public ClientTabPage() { InitializeComponent(); LoadData(); this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) LoadData(); }; }
        private void LoadData() { Dg.ItemsSource = Core.Context.Client.ToList(); }
        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddClientPage(null));
        private void BtnEdit_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddClientPage((sender as Button).DataContext as Client));
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var items = Dg.SelectedItems.Cast<Client>().ToList();
            if (items.Count == 0) { MessageBox.Show("Выберите записи!"); return; }
            if (MessageBox.Show($"Удалить {items.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try { Core.Context.Client.RemoveRange(items); Core.Context.SaveChanges(); LoadData(); MessageBox.Show("Удалено!"); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}