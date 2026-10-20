using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class AddPaymentPage : Page
    {
        private Payment _item;
        public AddPaymentPage(Payment item)
        {
            InitializeComponent();
            _item = item ?? new Payment();
            DataContext = _item;

            CbUser.ItemsSource = Core.Context.User.ToList();
            CbCategory.ItemsSource = Core.Context.Category.ToList();

            CbEmployee.ItemsSource = Core.Context.Employee.ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text) || DpDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните Название и Дату!");
                return;
            }

            if (_item.ID == 0)
                Core.Context.Payment.Add(_item);

            Core.Context.SaveChanges();
            MessageBox.Show("Сохранено!");
            NavigationService.GoBack();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}