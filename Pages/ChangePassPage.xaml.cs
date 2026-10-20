using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace UP01.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage() { InitializeComponent(); }
        public static string GetHash(string pwd) { using (var h = SHA1.Create()) return string.Concat(h.ComputeHash(Encoding.UTF8.GetBytes(pwd)).Select(x => x.ToString("X2"))); }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text) || string.IsNullOrWhiteSpace(PbOld.Password) || string.IsNullOrWhiteSpace(PbNew.Password)) { MessageBox.Show("Заполните все поля!"); return; }
            var user = Core.Context.User.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == GetHash(PbOld.Password));
            if (user == null) { MessageBox.Show("Неверный логин или пароль!"); return; }
            if (PbNew.Password != PbConf.Password) { MessageBox.Show("Пароли не совпадают!"); return; }
            user.Password = GetHash(PbNew.Password); Core.Context.SaveChanges();
            MessageBox.Show("Пароль изменен!"); NavigationService.Navigate(new AuthPage());
        }
    }
}