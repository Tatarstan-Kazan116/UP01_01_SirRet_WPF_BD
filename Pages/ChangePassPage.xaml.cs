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
        public ChangePassPage()
        {
            InitializeComponent();
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка заполнения всех полей
            if (string.IsNullOrWhiteSpace(TbLogin.Text) ||
                string.IsNullOrWhiteSpace(PbOld.Password) ||
                string.IsNullOrWhiteSpace(PbNew.Password) ||
                string.IsNullOrWhiteSpace(PbConf.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // 2. Сначала вычисляем хэш ДО запроса к БД
            string hashedOldPassword = GetHash(PbOld.Password);

            // 3. Ищем пользователя в БД
            var user = Core.Context.User.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hashedOldPassword);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или текущий пароль!");
                return;
            }

            // 4. Проверка совпадения нового пароля и подтверждения
            if (PbNew.Password != PbConf.Password)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают!");
                return;
            }

           
            string newPwd = PbNew.Password;

            if (newPwd.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!");
                return;
            }

            bool hasDigit = false;
            bool hasEnglishLetter = false;
            bool hasInvalidChar = false;

            foreach (char c in newPwd)
            {
                if (c >= '0' && c <= '9')
                {
                    hasDigit = true; // Нашли цифру
                }
                else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    hasEnglishLetter = true; // Нашли английскую букву
                }
                else
                {
                    hasInvalidChar = true; // Нашли русскую букву, пробел или спецсимвол
                    break;
                }
            }

            if (hasInvalidChar)
            {
                MessageBox.Show("Пароль должен содержать ТОЛЬКО английские буквы и цифры!");
                return;
            }

            if (!hasDigit)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!");
                return;
            }

            // Проверка заблокирует "123456"
            if (!hasEnglishLetter)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну английскую букву!");
                return;
            }
            

            // 5. Сохраняем новый хешированный пароль
            user.Password = GetHash(PbNew.Password);
            Core.Context.SaveChanges();

            MessageBox.Show("Пароль успешно изменен!");
            NavigationService.Navigate(new AuthPage());
        }
    }
}