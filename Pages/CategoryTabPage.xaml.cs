using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class CategoryTabPage : Page
    {
        public CategoryTabPage()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateData();
        }

        private void UpdateData()
        {
            if (!IsInitialized || DgCategories == null) return;

            var q = Core.Context.Category.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TbFilter?.Text))
                q = q.Where(c => c.Name.ToLower().Contains(TbFilter.Text.ToLower()));

            if (CbSort != null)
            {
                DgCategories.ItemsSource = CbSort.SelectedIndex == 0
                    ? q.OrderBy(c => c.Name).ToList()
                    : q.OrderByDescending(c => c.Name).ToList();
            }
            else
            {
                DgCategories.ItemsSource = q.OrderBy(c => c.Name).ToList();
            }
        }

        private void TbFilter_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddCategoryPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgCategories.SelectedItem is Category selected)
                NavigationService.Navigate(new AddCategoryPage(selected));
            else
                MessageBox.Show("Выберите категорию для редактирования!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что выбрана только одна запись
            if (DgCategories.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно удалять только по одному пользователю за раз!");
                return;
            }

            if (DgCategories.SelectedItem is User selected)
            {
                // Проверяем, есть ли связанные платежи
                var relatedPayments = Core.Context.Payment.Where(p => p.UserID == selected.ID).ToList();

                if (relatedPayments.Count > 0)
                {
                    MessageBox.Show($"Нельзя удалить пользователя {selected.FIO}! У него есть {relatedPayments.Count} связанных заказ-нарядов. Сначала удалите или измените эти записи.");
                    return;
                }

                if (MessageBox.Show($"Удалить пользователя {selected.FIO}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Core.Context.User.Remove(selected);
                    Core.Context.SaveChanges();
                    UpdateData();
                    MessageBox.Show("Пользователь удалён!");
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления!");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (TbFilter != null) TbFilter.Text = "";
            if (CbSort != null) CbSort.SelectedIndex = 0;
            UpdateData();
        }
    }
}