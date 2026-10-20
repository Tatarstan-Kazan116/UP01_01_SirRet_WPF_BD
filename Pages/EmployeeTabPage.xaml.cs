using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class EmployeeTabPage : Page
    {
        public EmployeeTabPage() { InitializeComponent(); LoadData(); this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) LoadData(); }; }
        private void LoadData() { Dg.ItemsSource = Core.Context.Employee.ToList(); }
        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddEmployeePage(null));
        private void BtnEdit_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddEmployeePage((sender as Button).DataContext as Employee));
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var items = Dg.SelectedItems.Cast<Employee>().ToList();
            if (items.Count == 0) { MessageBox.Show("Выберите записи!"); return; }
            if (MessageBox.Show($"Удалить {items.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try { Core.Context.Employee.RemoveRange(items); Core.Context.SaveChanges(); LoadData(); MessageBox.Show("Удалено!"); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}