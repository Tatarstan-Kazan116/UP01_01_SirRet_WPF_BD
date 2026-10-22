using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace UP01.Pages
{
    public partial class DiagrammPage : Page
    {
        private Chart _chart;

        public DiagrammPage()
        {
            InitializeComponent();

            // Создаем диаграмму программно
            _chart = new Chart();
            _chart.Dock = System.Windows.Forms.DockStyle.Fill;
            _chart.ChartAreas.Add(new ChartArea("Main"));
            _chart.Series.Add(new Series("Сумма") { IsValueShownAsLabel = true });
            _chart.Legends.Add(new Legend());

            WfhChart.Child = _chart;

            // Загружаем данные в ComboBox
            try
            {
                var users = Core.Context.User.ToList();
                CmbUser.ItemsSource = users;
                CmbUser.DisplayMemberPath = "FIO";

                if (users.Count > 0)
                {
                    CmbUser.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("В базе данных нет пользователей!");
                }

                CmbType.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
                if (CmbType.Items.Count > 0)
                {
                    CmbType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CmbUser.SelectedItem is User currentUser && CmbType.SelectedItem is SeriesChartType chartType)
                {
                    var series = _chart.Series[0];
                    series.ChartType = chartType;
                    series.Points.Clear();

                    var categories = Core.Context.Category.ToList();
                    foreach (var cat in categories)
                    {
                        // Используем nullable double и оператор ?? для замены null на 0
                        double? sum = Core.Context.Payment
                            .Where(p => p.UserID == currentUser.ID && p.CategoryID == cat.ID)
                            .Sum(p => (double?)(p.Price * p.Num));

                        // Если sum == null (нет платежей), используем 0
                        double finalSum = sum ?? 0;

                        // Добавляем точку только если есть данные
                        if (finalSum > 0)
                        {
                            series.Points.AddXY(cat.Name, finalSum);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка построения диаграммы: " + ex.Message);
            }
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var users = Core.Context.User.ToList();

                if (users.Count == 0)
                {
                    MessageBox.Show("Нет пользователей для экспорта!");
                    return;
                }

                // Проверяем количество платежей
                int totalPayments = Core.Context.Payment.Count();
                MessageBox.Show($"Всего платежей в базе: {totalPayments}");

                var app = new Word.Application { Visible = true };
                var doc = app.Documents.Add();

                foreach (var user in users)
                {
                    // Заголовок с ФИО
                    var p = doc.Paragraphs.Add();
                    p.Range.Text = user.FIO;
                    p.Range.Font.Bold = 1;
                    p.Range.Font.Size = 14;
                    p.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    p.Range.InsertParagraphAfter();

                    // Проверяем количество платежей у пользователя
                    var payments = Core.Context.Payment.Where(pay => pay.UserID == user.ID).ToList();
                    MessageBox.Show($"У пользователя {user.FIO} найдено платежей: {payments.Count}");

                    if (payments.Count > 0)
                    {
                        // Создаем таблицу
                        var table = doc.Tables.Add(p.Range, payments.Count + 1, 3);
                        table.Borders.Enable = 1;

                        // Заголовки столбцов
                        table.Cell(1, 1).Range.Text = "Услуга";
                        table.Cell(1, 2).Range.Text = "Кол-во";
                        table.Cell(1, 3).Range.Text = "Сумма";
                        table.Rows[1].Range.Font.Bold = 1;

                        // Заполняем данными
                        for (int i = 0; i < payments.Count; i++)
                        {
                            table.Cell(i + 2, 1).Range.Text = payments[i].Name;
                            table.Cell(i + 2, 2).Range.Text = payments[i].Num.ToString();
                            table.Cell(i + 2, 3).Range.Text = (payments[i].Price * payments[i].Num).ToString("N2") + " руб.";
                        }
                    }
                    else
                    {
                        var noData = doc.Paragraphs.Add();
                        noData.Range.Text = "Нет платежей";
                        noData.Range.InsertParagraphAfter();
                    }

                    // Разрыв страницы между пользователями (кроме последнего)
                    if (user != users.Last())
                    {
                        doc.Paragraphs.Add().Range.InsertBreak(Word.WdBreakType.wdPageBreak);
                    }
                }

                MessageBox.Show("Экспорт в Word завершён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта в Word: " + ex.Message);
            }
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var users = Core.Context.User.OrderBy(u => u.FIO).ToList();

                if (users.Count == 0)
                {
                    MessageBox.Show("Нет пользователей для экспорта!");
                    return;
                }

                var app = new Excel.Application { Visible = true };
                app.SheetsInNewWorkbook = users.Count;
                var wb = app.Workbooks.Add(Type.Missing);

                for (int i = 0; i < users.Count; i++)
                {
                    // Выносим значения в переменные ДО LINQ-запроса
                    var currentUser = users[i];
                    int currentUserId = currentUser.ID;
                    string currentUserName = currentUser.FIO;

                    Excel.Worksheet ws = wb.Worksheets.Item[i + 1];
                    ws.Name = currentUserName.Length > 30 ? currentUserName.Substring(0, 30) : currentUserName;

                    // Заголовки столбцов
                    ws.Cells[1, 1] = "Дата";
                    ws.Cells[1, 2] = "Услуга";
                    ws.Cells[1, 3] = "Кол-во";
                    ws.Cells[1, 4] = "Цена";
                    ws.Cells[1, 5] = "Сумма";

                    // Форматирование заголовков
                    Excel.Range headerRange = ws.Range[ws.Cells[1, 1], ws.Cells[1, 5]];
                    headerRange.Font.Bold = true;
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    var payments = Core.Context.Payment.Where(p => p.UserID == currentUserId).ToList();

                    int row = 2;
                    foreach (var payment in payments)
                    {
                        ws.Cells[row, 1] = payment.Date.ToString("dd.MM.yyyy");
                        ws.Cells[row, 2] = payment.Name;
                        ws.Cells[row, 3] = payment.Num;
                        ws.Cells[row, 4] = payment.Price;
                        ws.Cells[row, 5] = payment.Price * payment.Num;
                        row++;
                    }

                    // Автоподбор ширины столбцов
                    ws.Columns.AutoFit();
                }

                MessageBox.Show("Экспорт в Excel завершён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта в Excel: " + ex.Message);
            }
        }
    }
}