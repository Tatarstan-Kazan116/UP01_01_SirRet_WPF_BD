using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка 1: Все поля заполнены
            if (string.IsNullOrWhiteSpace(txtbxLog.Text) ||
                string.IsNullOrWhiteSpace(txtbxFIO.Text) ||
                string.IsNullOrWhiteSpace(passBxFrst.Password) ||
                string.IsNullOrWhiteSpace(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // Проверка 2: Логин не занят
            if (Core.Context.User.Any(u => u.Login == txtbxLog.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!");
                return;
            }

            // Проверка 3: Пароли совпадают
            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            
            string pwd = passBxFrst.Password;

            if (pwd.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!");
                return;
            }

            bool hasDigit = false;
            bool hasEnglishLetter = false;
            bool hasInvalidChar = false;

            foreach (char c in pwd)
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

            if (!hasEnglishLetter)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну английскую букву!");
                return;
            }
            

            // Все проверки пройдены - сохранение
            try
            {
                var newUser = new User
                {
                    FIO = txtbxFIO.Text,
                    Login = txtbxLog.Text,
                    Password = GetHash(passBxFrst.Password),
                    Role = (comboBxRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User"
                };

                Core.Context.User.Add(newUser);
                Core.Context.SaveChanges();

                MessageBox.Show("Пользователь успешно зарегистрирован!");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }
    }
}