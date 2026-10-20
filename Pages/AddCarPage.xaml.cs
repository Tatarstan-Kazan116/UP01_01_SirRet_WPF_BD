using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class AddCarPage : Page
    {
        private Car _item;
        public AddCarPage(Car item) { InitializeComponent(); _item = item ?? new Car(); DataContext = _item; CbClient.ItemsSource = Core.Context.Client.ToList(); }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbBrand.Text) || string.IsNullOrWhiteSpace(TbPlate.Text)) { MessageBox.Show("Заполните Марку и Гос. номер!"); return; }
            if (_item.ID == 0) Core.Context.Car.Add(_item);
            Core.Context.SaveChanges(); MessageBox.Show("Сохранено!"); NavigationService.GoBack();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}