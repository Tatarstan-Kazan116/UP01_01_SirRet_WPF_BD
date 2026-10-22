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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            if (!IsInitialized || DgCategories == null)
                return;

            var q = Core.Context.Category.AsQueryable();

            // Фильтр по названию категории
            if (!string.IsNullOrWhiteSpace(TbFilter?.Text))
                q = q.Where(c => c.Name.ToLower().Contains(TbFilter.Text.ToLower()));

            // Сортировка
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

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (TbFilter != null) TbFilter.Text = "";
            if (CbSort != null) CbSort.SelectedIndex = 0;
            UpdateData();
        }
    }
}