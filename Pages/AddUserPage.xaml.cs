using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UP01.Pages
{
    public partial class AddUserPage : Page
    {
        private User _user;

        public AddUserPage(User user)
        {
            InitializeComponent();
            _user = user ?? new User();
            DataContext = _user;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(TbFIO.Text) || string.IsNullOrWhiteSpace(TbLogin.Text))
            {
                MessageBox.Show("Заполните ФИО и Логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Проверка уникальности логина
            if (Core.Context.User.Any(u => u.Login == TbLogin.Text && u.ID != _user.ID))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Проверка пароля
            if (!string.IsNullOrWhiteSpace(PbPass.Password))
            {
                string pwd = PbPass.Password;

                if (pwd.Length < 6)
                {
                    MessageBox.Show("Ошибка: Пароль должен содержать минимум 6 символов.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show("Ошибка: Пароль должен содержать ТОЛЬКО английские буквы и цифры.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!hasDigit)
                {
                    MessageBox.Show("Ошибка: Пароль должен содержать хотя бы одну цифру.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!hasEnglishLetter)
                {
                    MessageBox.Show("Ошибка: Пароль должен содержать хотя бы одну английскую букву.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Хешируем новый пароль, если он прошел все проверки
                using (var h = System.Security.Cryptography.SHA1.Create())
                {
                    _user.Password = string.Concat(h.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pwd)).Select(x => x.ToString("X2")));
                }
            }
            else
            {
                // Если поле пароля пустое
                if (_user.ID == 0) // Это новый пользователь
                {
                    MessageBox.Show("Ошибка: Пароль обязателен для нового пользователя!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Если это редактирование (_user.ID != 0), мы просто не меняем поле _user.Password, оно останется старым из БД.
            }

            // 4. Сохранение остальных полей
            _user.FIO = TbFIO.Text;
            _user.Login = TbLogin.Text;
            var selectedRole = CbRole.SelectedItem as ComboBoxItem;
            _user.Role = selectedRole?.Content.ToString() ?? "User";

            try
            {
                if (_user.ID == 0)
                    Core.Context.User.Add(_user);

                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}