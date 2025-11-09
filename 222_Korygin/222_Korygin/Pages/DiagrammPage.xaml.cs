using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace _222_Korygin.Pages
{
    /// <summary>
    /// Логика взаимодействия для DiagrammPage.xaml
    /// </summary>
    public partial class DiagrammPage : Page
    {
        private Korygin_DB_PaymentEntities _context = new Korygin_DB_PaymentEntities();

        public DiagrammPage()
        {
            InitializeComponent();

            // Инициализация диаграммы
            ChartPayments.ChartAreas.Add(new ChartArea("Main"));

            var currentSeries = new Series("Платежи")
            {
                IsValueShownAsLabel = true
            };
            ChartPayments.Series.Add(currentSeries);

            // Загрузка данных в ComboBox
            CmbUser.ItemsSource = _context.User.ToList();
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));

            // Установка значений по умолчанию
            if (CmbUser.Items.Count > 0)
                CmbUser.SelectedIndex = 0;
            if (CmbDiagram.Items.Count > 0)
                CmbDiagram.SelectedIndex = 0;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is User currentUser && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartPayments.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                var categoriesList = _context.Category.ToList();
                foreach (var category in categoriesList)
                {
                    var sum = _context.Payment.ToList()
                        .Where(u => u.UserID == currentUser.ID && u.CategoryID == category.ID)
                        .Sum(u => u.Price * u.Num);

                    currentSeries.Points.AddXY(category.Name, sum);
                }
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var excelApp = new Excel.Application();
                excelApp.Visible = true;
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = workbook.ActiveSheet;

                // Заголовки
                worksheet.Cells[1, 1] = "Категория";
                worksheet.Cells[1, 2] = "Сумма";

                // Данные
                if (CmbUser.SelectedItem is User currentUser)
                {
                    var categoriesList = _context.Category.ToList();
                    int row = 2;

                    foreach (var category in categoriesList)
                    {
                        var sum = _context.Payment.ToList()
                            .Where(u => u.UserID == currentUser.ID && u.CategoryID == category.ID)
                            .Sum(u => u.Price * u.Num);

                        worksheet.Cells[row, 1] = category.Name;
                        worksheet.Cells[row, 2] = sum;
                        row++;
                    }
                }

                MessageBox.Show("Данные экспортированы в Excel!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}");
            }
        }

        private void BtnExportWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wordApp = new Word.Application();
                wordApp.Visible = true;
                Word.Document document = wordApp.Documents.Add();

                // Заголовок
                Word.Paragraph title = document.Paragraphs.Add();
                title.Range.Text = "Отчет по платежам";
                title.Range.Font.Bold = 1;
                title.Range.Font.Size = 16;
                title.Format.SpaceAfter = 24;
                title.Range.InsertParagraphAfter();

                // Данные
                if (CmbUser.SelectedItem is User currentUser)
                {
                    Word.Paragraph userInfo = document.Paragraphs.Add();
                    userInfo.Range.Text = $"Пользователь: {currentUser.FIO}";
                    userInfo.Format.SpaceAfter = 12;
                    userInfo.Range.InsertParagraphAfter();

                    var categoriesList = _context.Category.ToList();

                    foreach (var category in categoriesList)
                    {
                        var sum = _context.Payment.ToList()
                            .Where(u => u.UserID == currentUser.ID && u.CategoryID == category.ID)
                            .Sum(u => u.Price * u.Num);

                        Word.Paragraph data = document.Paragraphs.Add();
                        data.Range.Text = $"{category.Name}: {sum} руб.";
                        data.Format.SpaceAfter = 6;
                        data.Range.InsertParagraphAfter();
                    }
                }

                MessageBox.Show("Данные экспортированы в Word!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Word: {ex.Message}");
            }
        }
    }
}