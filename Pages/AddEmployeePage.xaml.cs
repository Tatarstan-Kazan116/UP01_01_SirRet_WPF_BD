using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class AddEmployeePage : Page
    {
        private Employee _item;
        public AddEmployeePage(Employee item) { InitializeComponent(); _item = item ?? new Employee(); DataContext = _item; }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbFIO.Text) || string.IsNullOrWhiteSpace(TbPosition.Text)) { MessageBox.Show("Заполните ФИО и Должность!"); return; }
            if (_item.ID == 0) Core.Context.Employee.Add(_item);
            Core.Context.SaveChanges(); MessageBox.Show("Сохранено!"); NavigationService.GoBack();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}