using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using Guna.Charts.WinForms;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Supabase;

namespace Diplomchik
{
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();
            InitializeCustomComponents();
            guna2PanelMain.Visible = true;
            guna2PanelSettings.Visible = false;
            guna2PanelStat.Visible = false;
            guna2PanelAdmin.Visible = false;
            if (ClassStorage.role == "user")
            {
                guna2ButtonAdmin.Visible = false;
            }
            else
            {
                guna2ButtonAdmin.Visible = true;
            }
        }

        private Timer timer;
        private bool isPanelOpen = false;
        private const int panelWidth = 500;// Ширина панели в открытом состоянии
        private const int animationStep = 50; // Шаг анимации

        private void InitializeCustomComponents()
        {
            // Инициализация таймера
            timer = new Timer();
            timer.Interval = 10; // Интервал таймера (в миллисекундах)
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPanelOpen)
            {
                // Открытие панели
                if (guna2Panel1.Width < panelWidth)
                {
                    guna2Panel1.Width += animationStep;
                    guna2ButtonMenu.Width += animationStep;
                    guna2ButtonMainPage.Width += animationStep;
                    guna2ButtonStat.Width += animationStep;
                    guna2ButtonSettings.Width += animationStep;
                    guna2ButtonAdmin.Width += animationStep;
                    guna2Button.Width += animationStep;
                }
                else
                {
                    isPanelOpen = true;
                    timer.Stop();
                }
            }
            else
            {
                // Закрытие панели
                if (guna2Panel1.Width > 80)
                {
                    guna2Panel1.Width -= animationStep; 
                    guna2ButtonMenu.Width -= animationStep;
                    guna2ButtonMainPage.Width -= animationStep;
                    guna2ButtonStat.Width -= animationStep;
                    guna2ButtonSettings.Width -= animationStep;
                    guna2ButtonAdmin.Width -= animationStep;
                    guna2Button.Width -= animationStep;
                }
                else
                {
                    isPanelOpen = false;
                    timer.Stop();
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Запускаем анимацию
            if (!isPanelOpen)
            {
                timer.Start();// Открываем панель
                guna2ButtonMenu.ImageAlign = HorizontalAlignment.Left;
                guna2ButtonMainPage.ImageAlign = HorizontalAlignment.Left;
                guna2ButtonStat.ImageAlign = HorizontalAlignment.Left;
                guna2ButtonSettings.ImageAlign = HorizontalAlignment.Left;
                guna2ButtonAdmin.ImageAlign = HorizontalAlignment.Left;
                guna2Button.ImageAlign = HorizontalAlignment.Left;
                guna2ButtonMenu.Text = "Меню";
                guna2ButtonMainPage.Text = "Главная";
                guna2ButtonStat.Text = "Статистика";
                guna2ButtonSettings.Text = "Настройки";
                guna2ButtonAdmin.Text = "Администрирование";
                guna2Button.Text = "Выход";
            }
            else
            {                timer.Start();// Закрываем панель
                guna2ButtonMenu.ImageAlign = HorizontalAlignment.Center;
                guna2ButtonMainPage.ImageAlign = HorizontalAlignment.Center;
                guna2ButtonStat.ImageAlign = HorizontalAlignment.Center;
                guna2ButtonSettings.ImageAlign = HorizontalAlignment.Center;
                guna2ButtonAdmin.ImageAlign = HorizontalAlignment.Center;
                guna2Button.ImageAlign = HorizontalAlignment.Center;
                guna2ButtonMenu.Text = "";
                guna2ButtonMainPage.Text = "";
                guna2ButtonStat.Text = "";
                guna2ButtonSettings.Text = "";
                guna2ButtonAdmin.Text = "";
                guna2Button.Text = "";
            }
        }

        private void guna2Button_Click(object sender, EventArgs e)
        {
            ClassStorage.authorizated = false;
            this.Hide();
            FormAuth form = new FormAuth();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        private string selectedFilePath;
        private void guna2Button9_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            // Устанавливаем начальную директорию (опционально)
            openFileDialog1.InitialDirectory = @"C:\Users\user\Desktop\Условия эксплуатации\ПРС ГНО";

            // Устанавливаем заголовок диалогового окна
            openFileDialog1.Title = "Выберите датасет";

            // Отображаем диалоговое окно и проверяем, был ли выбран файл
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Получаем путь к выбранному файлу
                selectedFilePath = openFileDialog1.FileName;

                // Выводим путь к файлу в MessageBox (или используйте его по своему усмотрению)
                MessageBox.Show("Выбран файл: " + selectedFilePath);

                guna2TextBoxFilepath.Text = selectedFilePath;
            }
        }

        private async void guna2Button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Файл не выбран.");
                return;
            }

            // Очищаем таблицу перед добавлением новых данных
            predictionsTable.Clear();

            string pythonPath = @"C:\Users\user\Desktop\Условия эксплуатации\Python310\python.exe";
            string scriptPath = @"C:\Users\user\Desktop\Условия эксплуатации\main.py";
            string modelPath = @"C:\Users\user\Desktop\Условия эксплуатации\trained_model.pkl";
            string neuralModelPath = @"C:\Users\user\Desktop\Условия эксплуатации\trained_neural_model.h5";
            string featuresPath = @"C:\Users\user\Desktop\Условия эксплуатации\features.pkl";

            try
            {
                await Task.Run(() => RunPythonScript(pythonPath, scriptPath, selectedFilePath, modelPath, neuralModelPath, featuresPath));
                MessageBox.Show("Выполнено");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void RunPythonScript(string pythonPath, string scriptPath, string selectedFilePath, string modelPath, string neuralModelPath, string featuresPath)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            start.Arguments = $"\"{scriptPath}\" \"{selectedFilePath}\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;
            start.EnvironmentVariables["TF_ENABLE_ONEDNN_OPTS"] = "0"; // Отключение oneDNN

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd();
                    string result = reader.ReadToEnd();

                    if (!string.IsNullOrEmpty(stderr))
                    {
                        Console.WriteLine($"Ошибки:\n{stderr}");
                    }
                    else
                    {
                        try
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                ProcessResult();
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при обработке результата: {ex.Message}");
                        }
                    }
                }
            }
        }

        DataTable predictionsTable = new DataTable();
        private void ProcessResult()
        {
            try
            {
                string csvFilePath = @"C:\Users\user\Desktop\Условия эксплуатации\predictions.csv";

                // Очищаем таблицу перед добавлением новых данных
                predictionsTable.Columns.Clear();

                using (StreamReader sr = new StreamReader(csvFilePath, Encoding.UTF8))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        predictionsTable.Columns.Add(header);
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = predictionsTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        predictionsTable.Rows.Add(dr);
                    }
                }

                guna2DataGridView1.DataSource = predictionsTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке результата: {ex.Message}");
            }
        }



        private void CreateWordReport(DataTable predictionsTable)
        {
            string reportPath = @"C:\Users\user\Desktop\Условия эксплуатации\Report.docx";

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(reportPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Добавление заголовка
                Paragraph titleParagraph = new Paragraph();
                Run titleRun = new Run();
                titleRun.Append(new Text("Отчет по прогнозам данных"));
                titleRun.RunProperties = new RunProperties(new FontSize { Val = "24" });
                titleParagraph.Append(titleRun);
                body.Append(titleParagraph);

                // Добавление информации о модели
                string modelInfoPath = @"C:\Users\user\Desktop\Условия эксплуатации\model_info.json";
                if (File.Exists(modelInfoPath))
                {
                    string modelInfoJson = File.ReadAllText(modelInfoPath);
                    dynamic modelInfo = JsonConvert.DeserializeObject<dynamic>(modelInfoJson);

                    Paragraph modelInfoParagraph = new Paragraph();
                    Run modelInfoRun = new Run();
                    modelInfoRun.Append(new Text("Информация о модели:"));
                    modelInfoRun.RunProperties = new RunProperties(new FontSize { Val = "20" });
                    modelInfoParagraph.Append(modelInfoRun);
                    body.Append(modelInfoParagraph);

                    body.Append(new Paragraph(new Run(new Text($"Название модели: {modelInfo[0].model_name}"))));
                    body.Append(new Paragraph(new Run(new Text($"Средняя абсолютная ошибка: {modelInfo[0].mean_absolute_error}"))));
                }

                // Группировка данных по НГДУ
                var groupedData = predictionsTable.AsEnumerable()
                    .GroupBy(row => row.Field<string>("NGDU"))
                    .Select(g => new
                    {
                        NGDU = g.Key,
                        AvgPredicted = g.Average(row => double.TryParse(row.Field<string>("Predicted"), NumberStyles.Any, CultureInfo.InvariantCulture, out double predicted) ? predicted : double.NaN),
                        AvgActual = g.Average(row => double.TryParse(row.Field<string>("Actual"), NumberStyles.Any, CultureInfo.InvariantCulture, out double actual) ? actual : double.NaN),
                        AvgError = g.Average(row => double.TryParse(row.Field<string>("Error"), NumberStyles.Any, CultureInfo.InvariantCulture, out double error) ? error : double.NaN)
                    })
                    .OrderBy(g => g.NGDU);

                // Добавление таблицы
                Table table = new Table();
                TableProperties props = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                    )
                );
                table.AppendChild(props);

                // Добавление заголовков столбцов
                TableRow headerRow = new TableRow();
                headerRow.Append(new TableCell(new Paragraph(new Run(new Text("НГДУ")))));
                headerRow.Append(new TableCell(new Paragraph(new Run(new Text("Среднее предсказание")))));
                headerRow.Append(new TableCell(new Paragraph(new Run(new Text("Среднее фактическое")))));
                headerRow.Append(new TableCell(new Paragraph(new Run(new Text("Средняя ошибка")))));
                table.Append(headerRow);

                // Добавление данных
                foreach (var group in groupedData)
                {
                    TableRow tr = new TableRow();
                    tr.Append(new TableCell(new Paragraph(new Run(new Text(group.NGDU)))));
                    tr.Append(new TableCell(new Paragraph(new Run(new Text(group.AvgPredicted.ToString("F2"))))));
                    tr.Append(new TableCell(new Paragraph(new Run(new Text(group.AvgActual.ToString("F2"))))));
                    tr.Append(new TableCell(new Paragraph(new Run(new Text(group.AvgError.ToString("F2"))))));
                    table.Append(tr);
                }

                body.Append(table);

                // Добавление заключения
                Paragraph conclusionParagraph = new Paragraph();
                Run conclusionRun = new Run();
                conclusionRun.Append(new Text("Заключение:"));
                conclusionRun.RunProperties = new RunProperties(new FontSize { Val = "20" });
                conclusionParagraph.Append(conclusionRun);
                body.Append(conclusionParagraph);

                body.Append(new Paragraph(new Run(new Text("В данном отчете представлены средние значения предсказаний, фактических значений и ошибок для каждого НГДУ."))));
            }

            MessageBox.Show($"Отчет создан: {reportPath}");
        }

        private async void guna2Button7_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.DataSource == null)
            {
                MessageBox.Show("Нет данных для создания отчета.");
                return;
            }

            DataTable predictionsTable = (DataTable)guna2DataGridView1.DataSource;
            CreateWordReport(predictionsTable);
        }

        private void guna2ButtonSettings_Click(object sender, EventArgs e)
        {
            guna2PanelStat.Visible = false;
            guna2PanelMain.Visible = false;
            guna2PanelSettings.Visible = true;
            guna2PanelAdmin.Visible = false;
            labelLogin.Text = $"Login: {ClassStorage.login}";
            labelPassword.Text = $"Password: {ClassStorage.password}";
            labelRole.Text = $"Role: {ClassStorage.role}";
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            try
            {
                string csvFilePath = @"C:\Users\user\Desktop\Условия эксплуатации\predictions.csv";

                // Очищаем таблицу перед добавлением новых данных
                predictionsTable.Columns.Clear();

                using (StreamReader sr = new StreamReader(csvFilePath, Encoding.UTF8))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        predictionsTable.Columns.Add(header);
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = predictionsTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        predictionsTable.Rows.Add(dr);
                    }
                }

                guna2DataGridView1.DataSource = predictionsTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке результата: {ex.Message}");
            }
        }

        private void guna2ButtonChangePassword_Click(object sender, EventArgs e)
        {
            this.Hide();
            ChangePassword form = new ChangePassword();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        private DataTable filteredTable; // Переменная для хранения отфильтрованной таблицы

        private void guna2ButtonStat_Click(object sender, EventArgs e)
        {
            if (predictionsTable != null)
            {
                comboBoxChartType.Items.Clear();
                comboBoxChartType.Items.Add("Line");
                comboBoxChartType.Items.Add("Bar");
                comboBoxChartType.SelectedIndex = 0;
                GenerateDataAndLabels(predictionsTable);
            }
            else
            {
                try
                {
                    string csvFilePath = @"C:\Users\user\Desktop\Условия эксплуатации\predictions.csv";

                    // Очищаем таблицу перед добавлением новых данных
                    predictionsTable.Columns.Clear();

                    using (StreamReader sr = new StreamReader(csvFilePath, Encoding.UTF8))
                    {
                        string[] headers = sr.ReadLine().Split(',');
                        foreach (string header in headers)
                        {
                            predictionsTable.Columns.Add(header);
                        }

                        while (!sr.EndOfStream)
                        {
                            string[] rows = sr.ReadLine().Split(',');
                            DataRow dr = predictionsTable.NewRow();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                dr[i] = rows[i];
                            }
                            predictionsTable.Rows.Add(dr);
                        }
                    }
                    GenerateDataAndLabels(predictionsTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке результата: {ex.Message}");
                }
            }
            guna2PanelStat.Visible = true;
            guna2PanelMain.Visible = false;
            guna2PanelSettings.Visible = false;
            guna2PanelAdmin.Visible = false;
        }

        private void comboBoxChartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gunaChart1.Datasets.Clear();
            string selectedChartType = comboBoxChartType.SelectedItem.ToString();
            switch (selectedChartType)
            {
                case "Line":
                    // Set up GunaLineDataset for Predicted values (Blue)
                    gunaLineDataset1 = new GunaLineDataset();
                    gunaLineDataset1.Label = "Predicted";
                    gunaLineDataset1.LegendBoxFillColor = System.Drawing.Color.DodgerBlue;
                    gunaLineDataset1.FillColor = System.Drawing.Color.DodgerBlue;
                    gunaLineDataset1.BorderColor = System.Drawing.Color.DodgerBlue;
                    gunaLineDataset1.YFormat = "Income {0:C}";
                    gunaChart1.Datasets.Add(gunaLineDataset1);

                    // Set up GunaLineDataset for Error values (Red)
                    gunaLineDataset2 = new GunaLineDataset();
                    gunaLineDataset2.Label = "Error";
                    gunaLineDataset2.LegendBoxFillColor = System.Drawing.Color.Red;
                    gunaLineDataset2.FillColor = System.Drawing.Color.Red;
                    gunaLineDataset2.BorderColor = System.Drawing.Color.Red;
                    gunaLineDataset2.YFormat = "Income {0:C}";
                    gunaChart1.Datasets.Add(gunaLineDataset2);
                    break;
                case "Bar":
                    // Set up GunaBarDataset
                    gunaBarDataset1 = new GunaBarDataset();
                    gunaBarDataset1.Label = "Bar";
                    gunaBarDataset1.LegendBoxFillColor = System.Drawing.Color.MediumSlateBlue;
                    gunaBarDataset1.FillColors.Add(System.Drawing.Color.MediumSlateBlue);
                    gunaBarDataset1.FillColors.Add(System.Drawing.Color.MediumPurple);
                    gunaBarDataset1.YFormat = "C";

                    // Add the datasets to the Datasets collection of GunaChart
                    gunaChart1.Datasets.Add(gunaBarDataset1);
                    break;
            }

            // Используем отфильтрованную таблицу, если она существует, иначе используем predictionsTable
            DataTable tableToUse = filteredTable != null ? filteredTable : predictionsTable;
            GenerateDataAndLabels(tableToUse);
            gunaChart1.Update();
        }

        private void FilterData(object sender, EventArgs e)
        {
            string selectedNGDU = guna2TextBoxNGDU.Text.ToString();
            string selectedCeh = guna2TextBoxCeh.Text.ToString();
            string selectedSkvazh = guna2TextBoxSkvazh.Text.ToString();

            filteredTable = predictionsTable.Clone(); // Создаем копию структуры таблицы

            int index = 0;
            int maxRows = 20; // Ограничиваем количество строк для фильтрации

            foreach (DataRow row in predictionsTable.Rows)
            {
                if (index >= maxRows)
                    break;

                if ((string.IsNullOrEmpty(selectedNGDU) || row["NGDU"].ToString() == selectedNGDU) &&
                    (string.IsNullOrEmpty(selectedCeh) || row["Ceh"].ToString() == selectedCeh) &&
                    (string.IsNullOrEmpty(selectedSkvazh) || row["Skvazh"].ToString() == selectedSkvazh))
                {
                    filteredTable.ImportRow(row);
                }

                index++;
            }

            guna2DataGridView1.DataSource = filteredTable;
            GenerateDataAndLabels(filteredTable); // Передаем отфильтрованную таблицу в метод GenerateDataAndLabels
            gunaChart1.Update();
        }

        private void GenerateDataAndLabels(DataTable table)
        {
            gunaLineDataset1.DataPoints.Clear();
            gunaLineDataset2.DataPoints.Clear();
            gunaBarDataset1.DataPoints.Clear();

            int index = 0;
            int maxRows = 100;
            foreach (DataRow row in table.Rows)
            {
                if (index >= maxRows)
                    break;
                string NGDU = Convert.ToString(row["NGDU"]);
                string Ceh = Convert.ToString(row["Ceh"]);
                string Skvazh = Convert.ToString(row["Skvazh"]);

                // Используем double.TryParse с указанием культуры для корректного преобразования
                double predicted;
                double actual;
                double error;

                if (!double.TryParse(Convert.ToString(row["Predicted"]), NumberStyles.Any, CultureInfo.InvariantCulture, out predicted))
                {
                    // Обработка ошибки, если преобразование не удалось
                    predicted = 0.0; // или другое значение по умолчанию
                }

                if (!double.TryParse(Convert.ToString(row["Actual"]), NumberStyles.Any, CultureInfo.InvariantCulture, out actual))
                {
                    // Обработка ошибки, если преобразование не удалось
                    actual = 0.0; // или другое значение по умолчанию
                }

                if (!double.TryParse(Convert.ToString(row["Error"]), NumberStyles.Any, CultureInfo.InvariantCulture, out error))
                {
                    // Обработка ошибки, если преобразование не удалось
                    error = 0.0; // или другое значение по умолчанию
                }

                gunaLineDataset1.DataPoints.Add(new LPoint()
                {
                    Label = $"{NGDU} - {Ceh} - {Skvazh}",
                    Y = predicted,
                });

                gunaLineDataset2.DataPoints.Add(new LPoint()
                {
                    Label = $"{NGDU} - {Ceh} - {Skvazh}",
                    Y = error,
                });

                gunaBarDataset1.DataPoints.Add(new LPoint()
                {
                    Label = $"{NGDU} - {Ceh} - {Skvazh}",
                    Y = actual,
                });

                index++;
            }

            // Обновление графика после добавления данных
            gunaChart1.Update();
        }




        private void guna2ButtonMainPage_Click(object sender, EventArgs e)
        {
            guna2PanelStat.Visible = false;
            guna2PanelMain.Visible = true;
            guna2PanelSettings.Visible = false;
            guna2PanelAdmin.Visible = false;
        }

        private void guna2ButtonAdmin_Click(object sender, EventArgs e)
        {
            guna2PanelStat.Visible = false;
            guna2PanelMain.Visible = false;
            guna2PanelSettings.Visible = false;
            guna2PanelAdmin.Visible = true;
            LoadUsersIntoDataGridView();
            guna2ComboBoxRole.Items.Clear();
            guna2ComboBoxRole.Items.Add("user");
            guna2ComboBoxRole.Items.Add("admin");
        }
        private async void LoadUsersIntoDataGridView()
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            var supabase = new Supabase.Client(ClassStorage.supabaseUrl, ClassStorage.supabaseKey, options);

            // Инициализация клиента Supabase
            await supabase.InitializeAsync();

            // Получаем все записи из таблицы users
            var result = await supabase
                .From<users>()
                .Select("*")
                .Get();

            if (result.Models.Any())
            {
                // Преобразуем список пользователей в DataTable для привязки к DataGridView
                var dataTable = new DataTable();
                dataTable.Columns.Add("User id", typeof(int));
                dataTable.Columns.Add("Created at", typeof(string));
                dataTable.Columns.Add("Login", typeof(string));
                dataTable.Columns.Add("Password", typeof(string));
                dataTable.Columns.Add("Role", typeof(string));

                foreach (var user in result.Models)
                {
                    dataTable.Rows.Add(user.user_id, user.created_at, user.login, user.password, user.role);
                }

                // Привязываем DataTable к DataGridView
                guna2DataGridView2.DataSource = dataTable;
            }
            else
            {
                MessageBox.Show("Нет пользователей для отображения.");
            }
        }

        private async void guna2ButtonUpdate_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку
            if (guna2DataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = guna2DataGridView2.SelectedRows[0];

                // Получаем ID пользователя из выбранной строки
                int userId = Convert.ToInt32(selectedRow.Cells["User id"].Value);

                // Создаем объект пользователя с обновленными данными
                var updatedUser = new users
                {
                    user_id = userId,
                    login = guna2TextBoxLogin.Text,
                    password = guna2TextBoxPassword.Text,
                    role = guna2ComboBoxRole.Text
                };

                // Обновляем данные в Supabase
                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = true
                };

                var supabase = new Supabase.Client(ClassStorage.supabaseUrl, ClassStorage.supabaseKey, options);
                await supabase.InitializeAsync();

                try
                {
                    var updateResult = await supabase
                        .From<users>()
                        .Where(u => u.user_id == userId)
                        .Set(u => u.login, updatedUser.login)
                        .Set(u => u.password, updatedUser.password)
                        .Set(u => u.role, updatedUser.role)
                        .Update();

                    // Перезагружаем данные в DataGridView
                    LoadUsersIntoDataGridView();
                    Console.WriteLine("Данные успешно обновлены.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для обновления.");
            }
        }

        private void guna2DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = guna2DataGridView2.Rows[e.RowIndex];

                // Заполняем текстовые поля значениями из выбранной строки
                guna2TextBoxLogin.Text = row.Cells["Login"].Value.ToString();
                guna2TextBoxPassword.Text = row.Cells["Password"].Value.ToString();

                // Заполняем guna2ComboBoxRole значением из выбранной строки
                string role = row.Cells["Role"].Value.ToString();
                guna2ComboBoxRole.SelectedItem = role;
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            // Создаем экземпляр SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Устанавливаем фильтр для файлов
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            // Устанавливаем начальную директорию (опционально)
            saveFileDialog.InitialDirectory = @"C:\Users\user\Desktop\Условия эксплуатации\ПРС ГНО";

            // Устанавливаем заголовок диалогового окна
            saveFileDialog.Title = "Сохранить отчет";

            // Отображаем диалоговое окно и проверяем, был ли выбран файл
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Получаем путь к выбранному файлу
                string exportFilePath = saveFileDialog.FileName;

                // Сохраняем данные из predictionsTable в выбранный файл
                SaveDataTableToCsv(predictionsTable, exportFilePath);

                MessageBox.Show("Отчет успешно сохранен.");
            }
        }

        private void SaveDataTableToCsv(DataTable dataTable, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Записываем заголовки столбцов
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sw.Write(dataTable.Columns[i].ColumnName);
                    if (i < dataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.WriteLine();

                // Записываем данные
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        sw.Write(row[i].ToString());
                        if (i < dataTable.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.WriteLine();
                }
            }
        }

        private async void guna2Button8_Click(object sender, EventArgs e)
        {
            // Инициализируем клиент Supabase
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };
            var supabase = new Supabase.Client(ClassStorage.supabaseUrl, ClassStorage.supabaseKey, options);

            try
            {
                // Инициализация клиента Supabase
                await supabase.InitializeAsync();

                

                // Экспортируем данные из predictionsTable в Supabase
                await ExportToSupabase(supabase, predictionsTable);

                MessageBox.Show("Данные успешно экспортированы в Supabase.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте данных в Supabase: {ex.Message}");
                MessageBox.Show($"Ошибка при экспорте данных в Supabase: {ex.Message}");
            }
        }

        private async Task ClearTable(Supabase.Client supabase)
        {
            try
            {
                // Удаляем все записи из таблицы
                await supabase.From<Prediction>().Where(p => !string.IsNullOrEmpty(p.NGDU)).Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при очистке таблицы: {ex.Message}");
            }
        }


        private async Task ExportToSupabase(Supabase.Client supabase, DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    // Проверка и корректировка формата строк перед преобразованием в float
                    string predictedStr = row["Predicted"].ToString().Replace(',', '.');
                    string actualStr = row["Actual"].ToString().Replace(',', '.');
                    string errorStr = row["Error"].ToString().Replace(',', '.');

                    if (!float.TryParse(predictedStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float predicted) ||
                        !float.TryParse(actualStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float actual) ||
                        !float.TryParse(errorStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float error))
                    {
                        Console.WriteLine($"Ошибка формата данных в строке: {row["Skvazh"]}");
                        continue; // Пропускаем эту строку и переходим к следующей
                    }

                    var prediction = new Prediction
                    {
                        NGDU = row["NGDU"].ToString(),
                        Ceh = row["Ceh"].ToString(),
                        Skvazh = row["Skvazh"].ToString(),
                        Predicted = predicted,
                        Actual = actual,
                        Error = error
                    };

                    // Добавление нового предсказания в таблицу
                    var insertResult = await supabase
                        .From<Prediction>()
                        .Insert(prediction);

                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при добавлении предсказания для скважины {row["Skvazh"]}: {ex.Message}");
                }
            }
        }


    }
}
