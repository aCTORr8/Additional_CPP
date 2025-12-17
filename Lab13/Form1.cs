using SkladProject;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace SkladProject
{
    public partial class Form1 : Form
    {
        private TSklad mySklad;
        private DateTimePicker dtp;

        public Form1()
        {
            InitializeComponent();

            mySklad = new TSklad();

            btnAdd.Click += BtnAdd_Click;

            miSave.Click += (s, e) => SaveDataWithDialog();
            miLoad.Click += (s, e) => LoadDataWithDialog();
            miPrint.Click += MenuPrint_Click;
            miExit.Click += (s, e) => Close();

            miEditGroups.Click += MenuEditDict_Click;
            miEditPost.Click += MenuEditDict_Click;
            miEditOd.Click += MenuEditDict_Click;

            miStats.Click += MenuStats_Click;

            tsbSave.Click += (s, e) => SaveDataWithDialog();
            tsbStats.Click += MenuStats_Click;

            treeWarehouses.AfterSelect += TreeWarehouses_AfterSelect;

            InitDataBindings();
            InitGridCalendar();

            SetupSearchLogic();
        }

        private void SetupSearchLogic()
        {

            tscbSearchField.Items.Clear();

            tscbSearchField.Items.AddRange(new object[] {
                "Всюди",
                "Назва",
                "Група",
                "Виробник",
                "Постачальник",
                "Ціна",
                "Кількість",
                "Валюта",
                "Од. виміру", 
                "Дата"        
            });

            tscbSearchField.SelectedIndex = 0;
            tstbSearch.TextChanged += (s, e) => UpdateGlobalFilter();
            tscbSearchField.SelectedIndexChanged += (s, e) => UpdateGlobalFilter();
        }

        private void UpdateGlobalFilter()
        {
            string filter = "";

            if (treeWarehouses.SelectedNode != null && treeWarehouses.SelectedNode.Text != "Всі склади")
            {
                string sklad = treeWarehouses.SelectedNode.Text.Replace("'", "''");
                filter = $"SkladID = '{sklad}'";
            }

            string searchText = tstbSearch.Text.Trim().Replace("'", "''");
            string selectedField = tscbSearchField.SelectedItem?.ToString() ?? "Всюди";

            if (!string.IsNullOrEmpty(searchText))
            {
                if (filter.Length > 0) filter += " AND ";

                switch (selectedField)
                {
                    case "Всюди":
                        filter += "(" +
                                  $"Nazva LIKE '%{searchText}%' OR " +
                                  $"Grupa LIKE '%{searchText}%' OR " +
                                  $"Vyrobnyk LIKE '%{searchText}%' OR " +
                                  $"Postachalnyk LIKE '%{searchText}%' OR " +
                                  $"Valuta LIKE '%{searchText}%' OR " +
                                  $"OdVymiru LIKE '%{searchText}%' OR " + 
                                  $"Convert(Cina, 'System.String') LIKE '%{searchText}%' OR " +
                                  $"Convert(Kilkist, 'System.String') LIKE '%{searchText}%' OR " +
                                  $"Convert(Date, 'System.String') LIKE '%{searchText}%'" + 
                                  ")";
                        break;

                    case "Назва":
                        filter += $"Nazva LIKE '%{searchText}%'";
                        break;
                    case "Група":
                        filter += $"Grupa LIKE '%{searchText}%'";
                        break;
                    case "Виробник":
                        filter += $"Vyrobnyk LIKE '%{searchText}%'";
                        break;
                    case "Постачальник":
                        filter += $"Postachalnyk LIKE '%{searchText}%'";
                        break;
                    case "Ціна":
                        filter += $"Convert(Cina, 'System.String') LIKE '%{searchText}%'";
                        break;
                    case "Кількість":
                        filter += $"Convert(Kilkist, 'System.String') LIKE '%{searchText}%'";
                        break;
                    case "Валюта":
                        filter += $"Valuta LIKE '%{searchText}%'";
                        break;

                    case "Од. виміру":
                        filter += $"OdVymiru LIKE '%{searchText}%'";
                        break;
                    case "Дата":
                        filter += $"Convert(Date, 'System.String') LIKE '%{searchText}%'";
                        break;
                }
            }

            try
            {
                mySklad.ApplyFilter(filter);
                UpdateSums();
            }
            catch
            {
              
            }
        }

        private void InitGridCalendar()
        {
            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Short;
            dtp.Visible = false;
            dtp.Width = 100;

            dtp.ValueChanged += (s, e) =>
            {
                if (dgSklad.CurrentCell != null)
                {
                    dgSklad.CurrentCell.Value = dtp.Value;
                }
            };
            dgSklad.Controls.Add(dtp);

            dgSklad.CellClick += DgSklad_CellClick;
            dgSklad.Scroll += (s, e) => dtp.Visible = false;
            dgSklad.ColumnWidthChanged += (s, e) => dtp.Visible = false;

            dgSklad.CellValueChanged += DgSklad_CellValueChanged;
            dgSklad.CurrentCellDirtyStateChanged += DgSklad_CurrentCellDirtyStateChanged;
            dgSklad.CellValidating += DgSklad_CellValidating;
            dgSklad.DataError += DgSklad_DataError;

            dgSklad.AllowUserToAddRows = false;
            dgSklad.DefaultValuesNeeded += DgSklad_DefaultValuesNeeded;
        }

        private void InitDataBindings()
        {
            BindCombo(cbGrupa, mySklad.DovGrupa, "Grupa");
            BindCombo(cbPostachalnyk, mySklad.DovPostachalnyk, "Postachalnyk");
            BindCombo(cbOdVymiru, mySklad.DovOdVymiru, "OdVymiru");
            BindCombo(cbValuta, mySklad.DovValuta, "Valuta");

            dgSklad.AutoGenerateColumns = false;
            SetupGridColumns();
            dgSklad.DataSource = mySklad.SkladView;

            treeWarehouses.Nodes.Add("Всі склади");
            treeWarehouses.Nodes[0].Nodes.Add("Склад №1 (Основний)");
            treeWarehouses.Nodes[0].Nodes.Add("Склад №2 (Резервний)");
            treeWarehouses.ExpandAll();
        }

        private void BindCombo(ComboBox cb, DataTable dt, string col)
        {
            cb.DataSource = dt;
            cb.DisplayMember = col;
            cb.ValueMember = col;
        }

        private void SetupGridColumns()
        {
            dgSklad.Columns.Clear();

            AddTextCol("N_pp", "№");
            dgSklad.Columns["N_pp"].ReadOnly = true;

            AddComboCol("Grupa", "Група", mySklad.DovGrupa, "Grupa");
            AddTextCol("Nazva", "Назва");
            AddTextCol("Vyrobnyk", "Виробник");
            AddComboCol("Postachalnyk", "Постачальник", mySklad.DovPostachalnyk, "Postachalnyk");
            AddComboCol("OdVymiru", "Од. вим.", mySklad.DovOdVymiru, "OdVymiru");
            AddTextCol("Cina", "Ціна");
            AddComboCol("Valuta", "Валюта", mySklad.DovValuta, "Valuta");
            AddTextCol("Kilkist", "К-сть");

            AddTextCol("Vartist", "Вартість");
            dgSklad.Columns["Vartist"].ReadOnly = true;
            dgSklad.Columns["Vartist"].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            AddTextCol("Date", "Дата");
            dgSklad.Columns["Date"].ReadOnly = true;
        }

        private void AddTextCol(string dataProp, string header)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = dataProp;
            col.Name = dataProp;
            col.HeaderText = header;
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            dgSklad.Columns.Add(col);
        }

        private void AddComboCol(string dataProp, string header, DataTable source, string display)
        {
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.DataPropertyName = dataProp;
            col.Name = dataProp;
            col.HeaderText = header;
            col.DataSource = source;
            col.DisplayMember = display;
            col.ValueMember = display;
            col.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            dgSklad.Columns.Add(col);
        }

        // --- ОБРОБНИКИ ПОДІЙ ---

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                decimal cina = decimal.Parse(tbCina.Text);
                int kilk = int.Parse(tbKilkist.Text);

                string skladName = treeWarehouses.SelectedNode != null ? treeWarehouses.SelectedNode.Text : "Склад №1 (Основний)";
                if (skladName == "Всі склади") skladName = "Склад №1 (Основний)";

                mySklad.AddRow(
                    skladName,
                    cbGrupa.Text,
                    tbNazva.Text,
                    tbVyrobnyk.Text,
                    cbPostachalnyk.Text,
                    cbOdVymiru.Text,
                    cina,
                    cbValuta.Text,
                    kilk
                );
                UpdateSums();
            }
            catch
            {
                MessageBox.Show("Перевірте правильність вводу числових даних (Ціна, Кількість)!");
            }
        }

        private void DgSklad_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            string selectedSklad = treeWarehouses.SelectedNode != null ? treeWarehouses.SelectedNode.Text : "Склад №1 (Основний)";
            if (selectedSklad == "Всі склади") selectedSklad = "Склад №1 (Основний)";

            e.Row.Cells["SkladID"].Value = selectedSklad;
            e.Row.Cells["Date"].Value = DateTime.Now;
            e.Row.Cells["Cina"].Value = 0;
            e.Row.Cells["Kilkist"].Value = 0;
            e.Row.Cells["Vartist"].Value = 0;
        }

        private void UpdateSums()
        {
            dgSums.DataSource = mySklad.GetSums();
        }

        private void TreeWarehouses_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateGlobalFilter();
        }

        private void MenuEditDict_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            string tag = item.Tag.ToString();

            DataTable targetDt = null;
            if (tag == "Grupa") targetDt = mySklad.DovGrupa;
            if (tag == "Post") targetDt = mySklad.DovPostachalnyk;
            if (tag == "Od") targetDt = mySklad.DovOdVymiru;

            if (targetDt != null)
            {
                FEditDict f = new FEditDict(targetDt, item.Text);
                f.ShowDialog();
                ((CurrencyManager)BindingContext[targetDt]).Refresh();
            }
        }

        private void MenuPrint_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();

            pd.PrintPage += (s, args) =>
            {
                Graphics g = args.Graphics;
                float yPos = 50;
                int leftMargin = 50;
                int rightMargin = 50;
                int PageWidth = args.PageBounds.Width;

                Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                Font bodyFont = new Font("Arial", 12);
                Font boldBodyFont = new Font("Arial", 12, FontStyle.Bold);
                Font dateFont = new Font("Arial", 10, FontStyle.Regular);

                decimal totalSumUAH = 0;
                decimal rateUSD = 41.5m;
                decimal rateEUR = 44.0m;

                string title = "ЗВІТ ПО СКЛАДУ";
                SizeF titleSize = g.MeasureString(title, titleFont);
                float centerX = (PageWidth - titleSize.Width) / 2;

                g.DrawString(title, titleFont, Brushes.Black, centerX, yPos);
                yPos += 40;

                g.DrawLine(Pens.Black, leftMargin, yPos, PageWidth - rightMargin, yPos);
                yPos += 30;

                foreach (DataRowView row in mySklad.SkladView)
                {
                    string nazva = row["Nazva"]?.ToString() ?? "(без назви)";
                    string grupa = row["Grupa"]?.ToString() ?? "";
                    string cina = row["Cina"]?.ToString() ?? "0";
                    string valuta = row["Valuta"]?.ToString() ?? "";
                    string kilk = row["Kilkist"]?.ToString() ?? "0";
                    string od = row["OdVymiru"]?.ToString() ?? "";
                    string vartist1 = row["Vartist"]?.ToString() ?? "0";

                    decimal vartist = 0;
                    decimal.TryParse(row["Vartist"].ToString(), out vartist);

                    if (valuta == "дол.") totalSumUAH += vartist * rateUSD;
                    else if (valuta == "євро") totalSumUAH += vartist * rateEUR;
                    else totalSumUAH += vartist;

                    string lineText = $"{nazva} ({grupa}) — {cina} {valuta} x {kilk} {od} = {vartist1} {valuta}";
                    g.DrawString(lineText, bodyFont, Brushes.Black, leftMargin, yPos);
                    yPos += 25;
                }

                yPos += 20;
                g.DrawLine(Pens.Black, leftMargin, yPos, PageWidth - rightMargin, yPos);
                yPos += 20;

                string sumText = $"Загальна вартість складу: {totalSumUAH:N2} грн.";
                g.DrawString(sumText, boldBodyFont, Brushes.Black, leftMargin, yPos);

                yPos += 40;

                string dateText = $"Дата формування: {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}";
                SizeF dateSize = g.MeasureString(dateText, dateFont);
                float dateX = PageWidth - rightMargin - dateSize.Width;
                g.DrawString(dateText, dateFont, Brushes.Gray, dateX, yPos);
            };

            PrintPreviewDialog ppd = new PrintPreviewDialog();
            ppd.Document = pd;
            ppd.Width = 800;
            ppd.Height = 600;
            ppd.ShowDialog();
        }

        private void MenuStats_Click(object sender, EventArgs e)
        {
            decimal totalUAH = 0;
            int count = mySklad.SkladView.Count;

            decimal rateUSD = 41.5m;
            decimal rateEUR = 44.0m;

            foreach (DataRowView row in mySklad.SkladView)
            {
                decimal vartist = 0;
                decimal.TryParse(row["Vartist"].ToString(), out vartist);
                string valuta = row["Valuta"].ToString();

                if (valuta == "дол.") totalUAH += vartist * rateUSD;
                else if (valuta == "євро") totalUAH += vartist * rateEUR;
                else totalUAH += vartist;
            }

            MessageBox.Show(
                $"Статистика по поточному виду (фільтру):\n" +
                $"----------------------------------------\n" +
                $"Кількість записів: {count}\n" +
                $"Загальна вартість: {totalUAH:N2} грн.",
                "Фінансова статистика",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void SaveDataWithDialog()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML файли (*.xml)|*.xml|Всі файли (*.*)|*.*";
            sfd.Title = "Зберегти базу даних складу";
            sfd.FileName = "SkladData.xml";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                mySklad.SaveToFile(sfd.FileName);
            }
        }

        private void LoadDataWithDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML файли (*.xml)|*.xml|Всі файли (*.*)|*.*";
            ofd.Title = "Відкрити базу даних складу";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                mySklad.LoadFromFile(ofd.FileName);
                UpdateSums();
            }
        }

        // --- ЛОГІКА РОБОТИ З ТАБЛИЦЕЮ ---

        private void DgSklad_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;

            string colName = dgSklad.Columns[e.ColumnIndex].Name;

            if (colName == "Date")
            {
                MessageBox.Show("Ви ввели некоректну дату. Будь ласка, використовуйте календар.", "Помилка формату");
            }
            else if (colName == "Cina" || colName == "Kilkist")
            {
                MessageBox.Show("У це поле можна вводити тільки числа!", "Помилка формату");
            }
        }

        private void DgSklad_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgSklad.Rows[e.RowIndex].IsNewRow) return;

            string colName = dgSklad.Columns[e.ColumnIndex].Name;

            if (colName == "Cina")
            {
                if (!decimal.TryParse(e.FormattedValue.ToString(), out _))
                {
                    e.Cancel = true;
                    dgSklad.Rows[e.RowIndex].ErrorText = "Ціна має бути числом!";
                    MessageBox.Show("Помилка: Ціна має бути числовим значенням.");
                }
                else
                {
                    dgSklad.Rows[e.RowIndex].ErrorText = "";
                }
            }

            if (colName == "Kilkist")
            {
                if (!int.TryParse(e.FormattedValue.ToString(), out _))
                {
                    e.Cancel = true;
                    dgSklad.Rows[e.RowIndex].ErrorText = "Кількість має бути цілим числом!";
                    MessageBox.Show("Помилка: Кількість має бути цілим числом.");
                }
                else
                {
                    dgSklad.Rows[e.RowIndex].ErrorText = "";
                }
            }
        }

        private void DgSklad_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgSklad.Columns[e.ColumnIndex].Name == "Date")
            {
                Rectangle cellRect = dgSklad.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                dtp.Size = new Size(cellRect.Width, cellRect.Height);
                dtp.Location = new Point(cellRect.X, cellRect.Y);

                if (dgSklad.CurrentCell.Value != null && DateTime.TryParse(dgSklad.CurrentCell.Value.ToString(), out DateTime date))
                {
                    dtp.Value = date;
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }

                dtp.Visible = true;
                dtp.Focus();
                SendKeys.Send("%{DOWN}");
            }
            else
            {
                dtp.Visible = false;
            }
        }

        private void DgSklad_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgSklad.IsCurrentCellDirty)
            {
                dgSklad.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgSklad_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgSklad.Columns[e.ColumnIndex].Name;

            if (colName == "Cina" || colName == "Kilkist")
            {
                DataGridViewRow row = dgSklad.Rows[e.RowIndex];
                decimal price = 0;
                int count = 0;

                decimal.TryParse(row.Cells["Cina"].Value?.ToString(), out price);
                int.TryParse(row.Cells["Kilkist"].Value?.ToString(), out count);

                row.Cells["Vartist"].Value = price * count;
            }

            if (colName == "Cina" || colName == "Kilkist" || colName == "Valuta" || colName == "Grupa" || colName == "Vartist")
            {
                UpdateSums();
            }
        }

        private void pInput_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tscbSearchField_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}