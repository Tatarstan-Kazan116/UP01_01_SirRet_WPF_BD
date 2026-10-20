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
        private Chart _chart; // Объявляем поле для диаграммы

        public DiagrammPage()
        {
            InitializeComponent();

            // Создаем диаграмму программно
            _chart = new Chart();
            _chart.Dock = System.Windows.Forms.DockStyle.Fill; // Заполняет весь WindowsFormsHost

            // Настраиваем область и серию данных
            _chart.ChartAreas.Add(new ChartArea("Main"));
            _chart.Series.Add(new Series("Сумма") { IsValueShownAsLabel = true });

            // Добавляем легенду
            _chart.Legends.Add(new Legend());

            // Присваиваем диаграмму как содержимое WindowsFormsHost
            WfhChart.Child = _chart;

            // Загружаем данные в ComboBox
            CmbUser.ItemsSource = Core.Context.User.ToList();
            CmbUser.DisplayMemberPath = "FIO";
            CmbType.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is User u && CmbType.SelectedItem is SeriesChartType t)
            {
                var s = _chart.Series[0];
                s.ChartType = t;
                s.Points.Clear();

                foreach (var c in Core.Context.Category.ToList())
                {
                    double sum = Core.Context.Payment
                        .Where(p => p.UserID == u.ID && p.CategoryID == c.ID)
                        .Sum(p => (double)(p.Price * p.Num));
                    if (sum > 0) s.Points.AddXY(c.Name, sum);
                }
            }
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            var app = new Word.Application { Visible = true };
            var doc = app.Documents.Add();

            foreach (var u in Core.Context.User.ToList())
            {
                var p = doc.Paragraphs.Add();
                p.Range.Text = u.FIO;
                p.Range.Font.Bold = 1;
                p.Range.InsertParagraphAfter();

                var pays = Core.Context.Payment.Where(x => x.UserID == u.ID).ToList();
                if (pays.Any())
                {
                    var tbl = doc.Tables.Add(p.Range, pays.Count + 1, 3);
                    tbl.Cell(1, 1).Range.Text = "Услуга";
                    tbl.Cell(1, 2).Range.Text = "Кол-во";
                    tbl.Cell(1, 3).Range.Text = "Сумма";

                    for (int i = 0; i < pays.Count; i++)
                    {
                        tbl.Cell(i + 2, 1).Range.Text = pays[i].Name;
                        tbl.Cell(i + 2, 2).Range.Text = pays[i].Num.ToString();
                        tbl.Cell(i + 2, 3).Range.Text = (pays[i].Price * pays[i].Num).ToString("N2");
                    }
                }
                doc.Paragraphs.Add().Range.InsertBreak(Word.WdBreakType.wdPageBreak);
            }
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            var users = Core.Context.User.OrderBy(u => u.FIO).ToList();
            var app = new Excel.Application { Visible = true, SheetsInNewWorkbook = users.Count };
            var wb = app.Workbooks.Add(Type.Missing);

            for (int i = 0; i < users.Count; i++)
            {
                Excel.Worksheet ws = wb.Worksheets.Item[i + 1];
                ws.Name = users[i].FIO.Length > 30 ? users[i].FIO.Substring(0, 30) : users[i].FIO;
                ws.Cells[1, 1] = "Дата";
                ws.Cells[1, 2] = "Услуга";
                ws.Cells[1, 3] = "Кол-во";
                ws.Cells[1, 4] = "Цена";
                ws.Cells[1, 5] = "Сумма";

                int row = 2;
                foreach (var p in Core.Context.Payment.Where(x => x.UserID == users[i].ID))
                {
                    ws.Cells[row, 1] = p.Date.ToString("dd.MM.yyyy");
                    ws.Cells[row, 2] = p.Name;
                    ws.Cells[row, 3] = p.Num;
                    ws.Cells[row, 4] = p.Price;
                    ws.Cells[row, 5] = p.Price * p.Num;
                    row++;
                }
                ws.Columns.AutoFit();
            }
        }
    }
}