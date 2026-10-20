using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class AddClientPage : Page
    {
        private Client _item;
        public AddClientPage(Client item) { InitializeComponent(); _item = item ?? new Client(); DataContext = _item; }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbFIO.Text) || string.IsNullOrWhiteSpace(TbPhone.Text)) { MessageBox.Show("Заполните ФИО и Телефон!"); return; }
            if (_item.ID == 0) Core.Context.Client.Add(_item);
            Core.Context.SaveChanges(); MessageBox.Show("Сохранено!"); NavigationService.GoBack();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
} 