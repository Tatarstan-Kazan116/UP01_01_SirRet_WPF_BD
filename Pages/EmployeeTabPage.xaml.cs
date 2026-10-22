using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class EmployeeTabPage : Page
    {
        public EmployeeTabPage()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateData();
        }

        private void UpdateData()
        {
            if (!IsInitialized || DgEmployees == null) return;

            var q = Core.Context.Employee.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TbFilter?.Text))
                q = q.Where(e => e.FIO.ToLower().Contains(TbFilter.Text.ToLower()));

            if (CbSort != null)
            {
                DgEmployees.ItemsSource = CbSort.SelectedIndex == 0
                    ? q.OrderBy(e => e.FIO).ToList()
                    : q.OrderByDescending(e => e.FIO).ToList();
            }
            else
            {
                DgEmployees.ItemsSource = q.OrderBy(e => e.FIO).ToList();
            }
        }

        private void TbFilter_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddEmployeePage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is Employee selected)
                NavigationService.Navigate(new AddEmployeePage(selected));
            else
                MessageBox.Show("Выберите сотрудника для редактирования!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что выбрана только одна запись
            if (DgEmployees.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно удалять только по одному пользователю за раз!");
                return;
            }

            if (DgEmployees.SelectedItem is User selected)
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