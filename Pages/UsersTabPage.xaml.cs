using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateData();
        }

        private void UpdateData()
        {
            if (!IsInitialized || DgUsers == null) return;

            var q = Core.Context.User.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TbFilter?.Text))
                q = q.Where(u => u.FIO.ToLower().Contains(TbFilter.Text.ToLower()));

            if (ChbAdmin != null && ChbAdmin.IsChecked == true)
                q = q.Where(u => u.Role == "Admin");

            if (CbSort != null)
            {
                DgUsers.ItemsSource = CbSort.SelectedIndex == 0
                    ? q.OrderBy(u => u.FIO).ToList()
                    : q.OrderByDescending(u => u.FIO).ToList();
            }
            else
            {
                DgUsers.ItemsSource = q.OrderBy(u => u.FIO).ToList();
            }
        }

        private void TbFilter_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();
        private void ChbAdmin_Changed(object sender, RoutedEventArgs e) => UpdateData();

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddUserPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgUsers.SelectedItem is User selected)
                NavigationService.Navigate(new AddUserPage(selected));
            else
                MessageBox.Show("Выберите пользователя для редактирования!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что выбрана только одна запись
            if (DgUsers.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно удалять только по одному пользователю за раз!");
                return;
            }

            if (DgUsers.SelectedItem is User selected)
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
            if (ChbAdmin != null) ChbAdmin.IsChecked = false;
            UpdateData();
        }
    }
}