using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace UP01.Pages
{
    public partial class AuthPage : Page
    {
        private int fails = 0;
        public AuthPage() { InitializeComponent(); }
        public static string GetHash(string pwd) { using (var h = SHA1.Create()) return string.Concat(h.ComputeHash(Encoding.UTF8.GetBytes(pwd)).Select(x => x.ToString("X2"))); }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text) || string.IsNullOrWhiteSpace(PbPassword.Password)) { MessageBox.Show("Заполните поля!"); return; }
            var hash = GetHash(PbPassword.Password);
            var user = Core.Context.User.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hash);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!"); fails++;
                if (fails >= 3) { ShowCaptcha(); }
                return;
            }
            MessageBox.Show("Успешный вход!");
            if (user.Role == "Admin") ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new AdminPage());
            else ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new UserPage());
        }
        private void ShowCaptcha()
        {
            LoginPanel.Visibility = Visibility.Hidden; CaptchaPanel.Visibility = Visibility.Visible;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; Random r = new Random();
            TxtCaptcha.Text = new string(Enumerable.Repeat(chars, 6).Select(s => s[r.Next(s.Length)]).ToArray());
        }
        private void BtnCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (TbCaptchaInput.Text != TxtCaptcha.Text) { MessageBox.Show("Неверно!"); ShowCaptcha(); }
            else { MessageBox.Show("Верно!"); LoginPanel.Visibility = Visibility.Visible; CaptchaPanel.Visibility = Visibility.Hidden; fails = 0; }
        }
        private void BtnReg_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new RegPage());
        private void BtnChangePass_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ChangePassPage());
    }
}