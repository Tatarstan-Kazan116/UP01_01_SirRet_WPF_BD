using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UP01.Pages;
namespace UP01
{
    public partial class MainWindow : Window
    {
        private bool isDark = false;
        public MainWindow() { InitializeComponent(); MainFrame.Navigate(new AuthPage()); TxtPageTitle.Text = "Авторизация"; MainFrame.Navigated += (s, e) => { if (MainFrame.Content is Page p) TxtPageTitle.Text = p.Title ?? "Автосервис"; }; }
        private void Window_Loaded(object sender, RoutedEventArgs e) { var t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) }; t.Tick += (o, a) => DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); t.Start(); }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { if (MessageBox.Show("Выйти из приложения?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) e.Cancel = true; }
        private void BtnBack_Click(object sender, RoutedEventArgs e) { if (MainFrame.CanGoBack) MainFrame.GoBack(); }
        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            isDark = !isDark; // Инвертируем флаг

            // Определяем путь к нужному файлу словаря
            string themeFile = isDark ? "DictionaryDark.xaml" : "Dictionary1.xaml";
            var uri = new Uri(themeFile, UriKind.Relative);

            // Загружаем новый словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            // Очищаем текущие словари и добавляем новый
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

            // Меняем текст кнопки для наглядности
            BtnTheme.Content = isDark ? "Светлая тема" : "Темная тема";
        }
    }
}