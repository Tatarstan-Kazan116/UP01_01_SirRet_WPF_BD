using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            // НЕ вызываем UpdateData() здесь - пусть вызывается после полной инициализации
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Вызываем обновление данных только после загрузки страницы
            UpdateData();
        }

        private void UpdateData()
        {
            if (!IsInitialized || DgUsers == null)
                return;

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
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (TbFilter != null) TbFilter.Text = "";
            if (CbSort != null) CbSort.SelectedIndex = 0;
            if (ChbAdmin != null) ChbAdmin.IsChecked = false;
            UpdateData();
        }
    }
}