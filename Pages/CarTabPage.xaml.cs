using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class CarTabPage : Page
    {
        public CarTabPage() { InitializeComponent(); LoadData(); this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) LoadData(); }; }
        private void LoadData() { Dg.ItemsSource = Core.Context.Car.ToList(); }
        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddCarPage(null));
        private void BtnEdit_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddCarPage((sender as Button).DataContext as Car));
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var items = Dg.SelectedItems.Cast<Car>().ToList();
            if (items.Count == 0) { MessageBox.Show("Выберите записи!"); return; }
            if (MessageBox.Show($"Удалить {items.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try { Core.Context.Car.RemoveRange(items); Core.Context.SaveChanges(); LoadData(); MessageBox.Show("Удалено!"); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}