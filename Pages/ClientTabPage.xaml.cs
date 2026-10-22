using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class ClientTabPage : Page
    {
        public ClientTabPage()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateData();
        }

        private void UpdateData()
        {
            if (!IsInitialized || DgClients == null) return;

            var q = Core.Context.Client.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TbFilter?.Text))
                q = q.Where(c => c.FIO.ToLower().Contains(TbFilter.Text.ToLower()));

            if (CbSort != null)
            {
                DgClients.ItemsSource = CbSort.SelectedIndex == 0
                    ? q.OrderBy(c => c.FIO).ToList()
                    : q.OrderByDescending(c => c.FIO).ToList();
            }
            else
            {
                DgClients.ItemsSource = q.OrderBy(c => c.FIO).ToList();
            }
        }

        private void TbFilter_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AddClientPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgClients.SelectedItem is Client selected)
                NavigationService.Navigate(new AddClientPage(selected));
            else
                MessageBox.Show("Выберите клиента для редактирования!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что выбрана только одна запись
            if (DgClients.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно удалять только по одному пользователю за раз!");
                return;
            }

            if (DgClients.SelectedItem is User selected)
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