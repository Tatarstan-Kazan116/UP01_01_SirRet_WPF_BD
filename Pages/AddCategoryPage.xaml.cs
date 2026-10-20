using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class AddCategoryPage : Page
    {
        private Category _item;
        public AddCategoryPage(Category item) { InitializeComponent(); _item = item ?? new Category(); DataContext = _item; }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text)) { MessageBox.Show("Заполните Название!"); return; }
            if (_item.ID == 0) Core.Context.Category.Add(_item);
            Core.Context.SaveChanges(); MessageBox.Show("Сохранено!"); NavigationService.GoBack();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}