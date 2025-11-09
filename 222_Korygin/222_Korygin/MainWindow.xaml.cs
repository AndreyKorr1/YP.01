using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _222_Korygin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.AuthPage());

            /// <summary>
            /// Код для добавления даты и времени
            /// </summary>
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) =>
            {
                DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            };
            timer.Start();

            
            ApplyLightTheme();
            UpdateThemeButton();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?",
                              "Подтверждение",
                              MessageBoxButton.YesNo,
                              MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void ThemeToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;

            if (isDarkTheme)
            {
                ApplyDarkTheme();
            }
            else
            {
                ApplyLightTheme();
            }

            UpdateThemeButton();
        }

        private void ApplyLightTheme()
        {
            ChangeTheme("Light.xaml");
        }

        private void ApplyDarkTheme()
        {
            ChangeTheme("Dark.xaml");
        }

        /// <summary>
        /// Выполняет смену цветовой темы приложения путем загрузки нового ResourceDictionary.
        /// </summary>
        /// <param name="themeFile">
        /// </param>
        /// <exception cref="UriFormatException">
        /// Выбрасывается когда themeFile содержит невалидный URI.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Выбрасывается когда файл темы не найден или недоступен для чтения.
        /// </exception>

        private void ChangeTheme(string themeFile)
        {
            try
            {
                
                var uri = new Uri(themeFile + "?t=" + DateTime.Now.Ticks, UriKind.Relative);

                
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

                
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Clear();

               
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене темы: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateThemeButton()
        {
            if (isDarkTheme)
            {
                ThemeToggleBtn.Content = "Светлая тема";
                ThemeToggleBtn.ToolTip = "Переключиться на светлую тему";
            }
            else
            {
                ThemeToggleBtn.Content = "Тёмная тема";
                ThemeToggleBtn.ToolTip = "Переключиться на тёмную тему";
            }
        }
    }
}