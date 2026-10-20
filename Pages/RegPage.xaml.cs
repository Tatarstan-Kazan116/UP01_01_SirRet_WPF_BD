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
            // 1. Проверка заполнения всех полей
            if (string.IsNullOrWhiteSpace(txtbxLog.Text) ||
                string.IsNullOrWhiteSpace(txtbxFIO.Text) ||
                string.IsNullOrWhiteSpace(passBxFrst.Password) ||
                string.IsNullOrWhiteSpace(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ВАЖНО: прерываем выполнение!
            }

            // 2. Проверка на существующий логин
            if (Core.Context.User.Any(u => u.Login == txtbxLog.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ВАЖНО: прерываем выполнение!
            }

            // 3. ПРОВЕРКА ПАРОЛЯ
            string pwd = passBxFrst.Password;

            // Проверка 1: Минимум 6 символов
            if (pwd.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ВАЖНО: прерываем выполнение!
            }

            bool hasDigit = false;
            bool hasInvalidChar = false;

            foreach (char c in pwd)
            {
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    // Английская буква - OK
                }
                else
                {
                    // Русский символ или спецсимвол
                    hasInvalidChar = true;
                    break;
                }
            }

            // Проверка 2: Только английская раскладка
            if (hasInvalidChar)
            {
                MessageBox.Show("Используйте только английскую раскладку (латинские буквы и цифры)!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ВАЖНО: прерываем выполнение!
            }

            // Проверка 3: Хотя бы одна цифра
            if (!hasDigit)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ВАЖНО: прерываем выполнение!
            }

            // 4. Проверка совпадения паролей
            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ВАЖНО: прерываем выполнение!
            }

            // 5. Если все проверки пройдены - сохраняем
            try
            {
                var newUser = new User
                {
                    FIO = txtbxFIO.Text,
                    Login = txtbxLog.Text,
                    Password = GetHash(passBxFrst.Password), // Хешируем!
                    Role = (comboBxRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User"
                };

                Core.Context.User.Add(newUser);
                Core.Context.SaveChanges();

                MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}