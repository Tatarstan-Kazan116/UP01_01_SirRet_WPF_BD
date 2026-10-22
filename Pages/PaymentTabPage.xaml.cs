using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class PaymentTabPage : Page
    {
        public PaymentTabPage()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateData();
        }

        private void UpdateData()
        {
            if (!IsInitialized || DgPayments == null) return;

            var q = Core.Context.Payment.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TbFilter?.Text))
                q = q.Where(p => p.Name.ToLower().Contains(TbFilter.Text.ToLower()));

            if (CbSort != null)
            {
                DgPayments.ItemsSource = CbSort.SelectedIndex == 0
                    ? q.OrderBy(p => p.Date).ToList()
                    : q.OrderByDescending(p => p.Price * p.Num).ToList();
            }
            else
            {
                DgPayments.ItemsSource = q.OrderBy(p => p.Date).ToList();
            }
        }

        private void TbFilter_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddPaymentPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgPayments.SelectedItem is Payment selected)
                NavigationService.Navigate(new AddPaymentPage(selected));
            else
                MessageBox.Show("Выберите заказ-наряд для редактирования!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что выбрана только одна запись
            if (DgPayments.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно удалять только по одному пользователю за раз!");
                return;
            }

            if (DgPayments.SelectedItem is User selected)
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