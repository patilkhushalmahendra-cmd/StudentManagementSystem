using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudentManagementSystem
{
    public class MainForm : Form
    {
        private readonly StudentStore store = new StudentStore();
        private int selectedId = -1;
        private List<Student> students;
        private DataGridView grid;
        private TextBox searchBox, nameBox, emailBox, phoneBox;
        private ComboBox courseBox, statusBox, semesterBox;
        private NumericUpDown percentageBox;
        private Label totalValue, activeValue, averageValue, formTitle;
        private readonly Color navy = Color.FromArgb(25, 39, 66);
        private readonly Color blue = Color.FromArgb(55, 105, 165);
        private readonly Color canvas = Color.FromArgb(244, 247, 251);
        private readonly Color border = Color.FromArgb(224, 230, 239);

        public MainForm()
        {
            students = store.Load();
            Text = "CampusDesk | Student Management";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1250, 800);
            MinimumSize = new Size(1080, 720);
            BackColor = canvas;
            Font = new Font("Segoe UI", 9F);
            BuildLayout();
            RefreshGrid();
        }

        private void BuildLayout()
        {
            // Top branding bar
            var header = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = navy, Padding = new Padding(28, 10, 24, 8) };
            Controls.Add(header);

            var logo = new Label {
                Text = "CAMPUSDESK", ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(24, 10), AutoSize = true
            };
            var subtitle = new Label {
                Text = "STUDENT INFORMATION PORTAL",
                ForeColor = Color.FromArgb(194, 208, 228),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(27, 43), AutoSize = true
            };
            header.Controls.Add(logo);
            header.Controls.Add(subtitle);

            // Main content area
            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 18, 24, 22), BackColor = canvas };
            Controls.Add(body);
            body.BringToFront();
            header.BringToFront();

            var pageLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                BackColor = canvas, Margin = new Padding(0), Padding = new Padding(0)
            };
            pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 94));
            pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            body.Controls.Add(pageLayout);

            var headingPanel = new Panel { Dock = DockStyle.Fill, BackColor = canvas };
            pageLayout.Controls.Add(headingPanel, 0, 0);
            headingPanel.Controls.Add(new Label {
                Text = "Dashboard", AutoSize = true, Location = new Point(0, 0),
                Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = navy
            });
            headingPanel.Controls.Add(new Label {
                Text = "Manage student records and monitor academic performance.",
                AutoSize = true, Location = new Point(2, 34),
                Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(105, 117, 135)
            });

            var stats = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1,
                BackColor = canvas, Padding = new Padding(0, 2, 0, 8), Margin = new Padding(0)
            };
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            pageLayout.Controls.Add(stats, 0, 1);
            AddStat(stats, "TOTAL STUDENTS", out totalValue, 0, Color.FromArgb(55, 105, 165));
            AddStat(stats, "ACTIVE STUDENTS", out activeValue, 1, Color.FromArgb(31, 145, 104));
            AddStat(stats, "AVERAGE SCORE", out averageValue, 2, Color.FromArgb(199, 133, 40));

            // White main card
            var mainCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(18), Margin = new Padding(0, 4, 0, 0) };
            mainCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, mainCard.ClientRectangle, border, ButtonBorderStyle.Solid);
            pageLayout.Controls.Add(mainCard, 0, 2);

            var mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                BackColor = Color.White, Margin = new Padding(0), Padding = new Padding(0)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            mainCard.Controls.Add(mainLayout);

            // Table heading and search/action row
            var toolbar = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1,
                BackColor = Color.White, Margin = new Padding(0), Padding = new Padding(0, 0, 0, 8)
            };
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 270));
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            mainLayout.Controls.Add(toolbar, 0, 0);

            toolbar.Controls.Add(new Label {
                Text = "Student Records", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = navy
            }, 0, 0);

            searchBox = new TextBox {
                Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10),
                Margin = new Padding(6, 7, 12, 7)
            };
            searchBox.TextChanged += (s, e) => RefreshGrid();
            toolbar.Controls.Add(searchBox, 1, 0);

            var add = MakeButton("+  Add Student", blue);
            add.Dock = DockStyle.Fill;
            add.Margin = new Padding(0, 4, 0, 4);
            add.Click += (s, e) => ClearForm();
            toolbar.Controls.Add(add, 2, 0);

            // Responsive student grid
            grid = new DataGridView {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(232, 236, 242),
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Margin = new Padding(0, 0, 0, 12)
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 243, 249);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = navy;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.ColumnHeadersHeight = 36;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 250);
            grid.DefaultCellStyle.SelectionForeColor = navy;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            grid.RowTemplate.Height = 32;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);
            AddColumn("Id", "ID", 7);
            AddColumn("FullName", "STUDENT NAME", 18);
            AddColumn("Course", "COURSE", 10);
            AddColumn("Semester", "SEM", 7);
            AddColumn("Email", "EMAIL ADDRESS", 22);
            AddColumn("Percentage", "SCORE %", 10, "0.0");
            AddColumn("Status", "STATUS", 10);
            grid.CellClick += GridCellClick;
            mainLayout.Controls.Add(grid, 0, 1);

            // Bottom details area
            var details = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                BackColor = Color.White, Padding = new Padding(0, 10, 0, 0), Margin = new Padding(0)
            };
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            details.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
            details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.Controls.Add(details, 0, 2);

            formTitle = new Label {
                Text = "Student Details  |  Add or edit a record", Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = navy,
                TextAlign = ContentAlignment.MiddleLeft
            };
            details.Controls.Add(formTitle, 0, 0);

            var fields = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 2,
                BackColor = Color.White, Padding = new Padding(0, 4, 0, 0), Margin = new Padding(0)
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            details.Controls.Add(fields, 0, 1);

            nameBox = AddTextField(fields, "FULL NAME", 0, 0);
            emailBox = AddTextField(fields, "EMAIL ADDRESS", 1, 0);
            phoneBox = AddTextField(fields, "PHONE", 2, 0);
            courseBox = AddComboField(fields, "COURSE", new[] { "BCA", "BSc IT", "BCom", "BBA", "MCA" }, 3, 0);
            semesterBox = AddComboField(fields, "SEMESTER", new[] { "1", "2", "3", "4", "5", "6", "7", "8" }, 4, 0);
            statusBox = AddComboField(fields, "STATUS", new[] { "Active", "Inactive" }, 5, 0);

            var scorePanel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 3, 8, 0) };
            fields.Controls.Add(scorePanel, 0, 1);
            scorePanel.Controls.Add(new Label {
                Text = "SCORE %", Dock = DockStyle.Top, Height = 20,
                ForeColor = Color.FromArgb(105, 117, 135), Font = new Font("Segoe UI", 8, FontStyle.Bold)
            });
            percentageBox = new NumericUpDown {
                Dock = DockStyle.Top, Minimum = 0, Maximum = 100, DecimalPlaces = 1,
                Value = 75, Width = 120, Font = new Font("Segoe UI", 10)
            };
            scorePanel.Controls.Add(percentageBox);

            var actions = new FlowLayoutPanel {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false, BackColor = Color.White, Padding = new Padding(0, 6, 0, 0), Margin = new Padding(0)
            };
            details.Controls.Add(actions, 0, 2);
            var save = MakeButton("Save Student", Color.FromArgb(31, 145, 104));
            save.Size = new Size(132, 34); save.Click += (s, e) => SaveStudent(); actions.Controls.Add(save);
            var delete = MakeButton("Delete Selected", Color.FromArgb(183, 67, 67));
            delete.Size = new Size(142, 34); delete.Click += (s, e) => DeleteStudent(); actions.Controls.Add(delete);
            var reset = MakeButton("Clear Form", Color.FromArgb(95, 108, 128));
            reset.Size = new Size(112, 34); reset.Click += (s, e) => ClearForm(); actions.Controls.Add(reset);
            var export = MakeButton("Export CSV", navy);
            export.Size = new Size(130, 34); export.Margin = new Padding(12, 0, 0, 0);
            export.Click += (s, e) => ExportCsv(); actions.Controls.Add(export);
        }

        private void AddStat(TableLayoutPanel parent, string title, out Label value, int column, Color accent)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0, 0, 12, 0), Padding = new Padding(16, 8, 8, 6) };
            card.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, border, ButtonBorderStyle.Solid);
            parent.Controls.Add(card, column, 0);
            var stripe = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent };
            card.Controls.Add(stripe);
            card.Controls.Add(new Label {
                Text = title, Location = new Point(29, 10), AutoSize = true,
                ForeColor = Color.FromArgb(105, 117, 135), Font = new Font("Segoe UI", 8, FontStyle.Bold)
            });
            value = new Label {
                Text = "0", Location = new Point(29, 30), AutoSize = true,
                ForeColor = navy, Font = new Font("Segoe UI", 20, FontStyle.Bold)
            };
            card.Controls.Add(value);
        }

        private void AddColumn(string property, string title, float fillWeight, string format = null)
        {
            var column = new DataGridViewTextBoxColumn {
                DataPropertyName = property, HeaderText = title, FillWeight = fillWeight,
                MinimumWidth = 45
            };
            if (format != null) column.DefaultCellStyle.Format = format;
            grid.Columns.Add(column);
        }

        private TextBox AddTextField(TableLayoutPanel parent, string label, int column, int row)
        {
            var panel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 3, 8, 0) };
            parent.Controls.Add(panel, column, row);
            panel.Controls.Add(new Label {
                Text = label, Dock = DockStyle.Top, Height = 20,
                ForeColor = Color.FromArgb(105, 117, 135), Font = new Font("Segoe UI", 8, FontStyle.Bold)
            });
            var box = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            panel.Controls.Add(box);
            return box;
        }

        private ComboBox AddComboField(TableLayoutPanel parent, string label, string[] options, int column, int row)
        {
            var panel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 3, 8, 0) };
            parent.Controls.Add(panel, column, row);
            panel.Controls.Add(new Label {
                Text = label, Dock = DockStyle.Top, Height = 20,
                ForeColor = Color.FromArgb(105, 117, 135), Font = new Font("Segoe UI", 8, FontStyle.Bold)
            });
            var combo = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            combo.Items.AddRange(options);
            combo.SelectedIndex = 0;
            panel.Controls.Add(combo);
            return combo;
        }

        private Button MakeButton(string text, Color color)
        {
            var button = new Button {
                Text = text, BackColor = color, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand, UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void RefreshGrid()
        {
            if (grid == null) return;
            string query = searchBox == null ? "" : searchBox.Text.Trim().ToLowerInvariant();
            var view = students.Where(s =>
                (s.FullName + " " + s.Course + " " + s.Email + " " + s.Id)
                .ToLowerInvariant().Contains(query)).OrderBy(s => s.Id).ToList();
            grid.DataSource = null;
            grid.DataSource = view;
            totalValue.Text = students.Count.ToString();
            activeValue.Text = students.Count(s => s.Status == "Active").ToString();
            averageValue.Text = (students.Count == 0 ? 0 : students.Average(s => s.Percentage)).ToString("0.0") + "%";
        }

        private void GridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var student = grid.Rows[e.RowIndex].DataBoundItem as Student;
            if (student == null) return;
            selectedId = student.Id;
            nameBox.Text = student.FullName;
            emailBox.Text = student.Email;
            phoneBox.Text = student.Phone;
            courseBox.SelectedItem = student.Course;
            semesterBox.SelectedItem = student.Semester.ToString();
            statusBox.SelectedItem = student.Status;
            percentageBox.Value = (decimal)student.Percentage;
            formTitle.Text = "Student Details  |  Editing ID " + student.Id;
        }

        private void SaveStudent()
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                MessageBox.Show("Please enter the student's full name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameBox.Focus(); return;
            }
            if (!string.IsNullOrWhiteSpace(emailBox.Text) && !emailBox.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailBox.Focus(); return;
            }
            Student student = selectedId < 0
                ? new Student { Id = students.Count == 0 ? 1001 : students.Max(x => x.Id) + 1 }
                : students.FirstOrDefault(x => x.Id == selectedId);
            if (student == null) return;
            student.FullName = nameBox.Text.Trim();
            student.Email = emailBox.Text.Trim();
            student.Phone = phoneBox.Text.Trim();
            student.Course = Convert.ToString(courseBox.SelectedItem);
            student.Semester = int.Parse(Convert.ToString(semesterBox.SelectedItem));
            student.Status = Convert.ToString(statusBox.SelectedItem);
            student.Percentage = (double)percentageBox.Value;
            if (selectedId < 0) students.Add(student);
            store.Save(students);
            RefreshGrid();
            ClearForm();
            MessageBox.Show("Student record saved successfully.", "CampusDesk", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteStudent()
        {
            if (selectedId < 0)
            {
                MessageBox.Show("Select a student from the table first.");
                return;
            }
            var answer = MessageBox.Show("Delete the selected student record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;
            students.RemoveAll(s => s.Id == selectedId);
            store.Save(students);
            RefreshGrid();
            ClearForm();
        }

        private void ClearForm()
        {
            selectedId = -1;
            if (nameBox == null) return;
            nameBox.Clear(); emailBox.Clear(); phoneBox.Clear();
            courseBox.SelectedIndex = 0; semesterBox.SelectedIndex = 0; statusBox.SelectedIndex = 0;
            percentageBox.Value = 75;
            formTitle.Text = "Student Details  |  Add or edit a record";
            if (grid != null) grid.ClearSelection();
        }

        private void ExportCsv()
        {
            using (var dialog = new SaveFileDialog {
                Filter = "CSV file (*.csv)|*.csv", FileName = "StudentRecords.csv",
                Title = "Export student records"
            })
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;
                var lines = new List<string> { "ID,Full Name,Course,Semester,Email,Phone,Percentage,Status" };
                lines.AddRange(students.Select(s => string.Join(",", new[] {
                    s.Id.ToString(), Csv(s.FullName), Csv(s.Course), s.Semester.ToString(),
                    Csv(s.Email), Csv(s.Phone), s.Percentage.ToString("0.0"), Csv(s.Status)
                })));
                System.IO.File.WriteAllLines(dialog.FileName, lines, System.Text.Encoding.UTF8);
                MessageBox.Show("CSV exported successfully.", "CampusDesk", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string Csv(string value)
        {
            return "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
        }
    }
}
