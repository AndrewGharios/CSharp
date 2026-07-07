using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic; // for InputBox (remove if you replace with custom dialog)

namespace lab3
{
    public partial class Form1 : Form
    {
        // Controls
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private NumericUpDown rows1, cols1, rows2, cols2;
        private RichTextBox richTextBox1;
        private Button add, minus, multiply, del, transpose, determinant, inverse, multinumber, clear;
        private Button matrix2, button1, button2, button3;
        private Panel leftPanel;

        private bool ismatrixMode = false;
        private Size normalSize;
        private Size matrixSize;
        private double[,] matrixA, matrixB, result;
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Матричный калькулятор";
            normalSize = new Size(500, 750);
            matrixSize = new Size(800, 750);
            this.Size = normalSize;
            this.MinimumSize = normalSize;
            this.MaximumSize = normalSize;
            this.StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            leftPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            // Matrix A
            Label labelA = new Label { Text = "Матрица A", Location = new Point(10, 10), AutoSize = true };
            rows1 = new NumericUpDown { Location = new Point(10, 40), Minimum = 1, Maximum = 30, Value = 3, Width = 50 };
            cols1 = new NumericUpDown { Location = new Point(70, 40), Minimum = 1, Maximum = 30, Value = 3, Width = 50 };
            Label labelRows1 = new Label { Text = "Строки:", Location = new Point(10, 25), AutoSize = true };
            Label labelCols1 = new Label { Text = "Столбцы:", Location = new Point(70, 25), AutoSize = true };
            dataGridView1 = new DataGridView { Location = new Point(10, 70), Size = new Size(350, 200), AllowUserToAddRows = false };
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            leftPanel.Controls.AddRange(new Control[] { labelA, labelRows1, labelCols1, rows1, cols1, dataGridView1 });

            // Matrix B
            Label labelB = new Label { Text = "Матрица B", Location = new Point(10, 280), AutoSize = true };
            rows2 = new NumericUpDown { Location = new Point(10, 310), Minimum = 1, Maximum = 30, Value = 3, Width = 50, Visible = false };
            cols2 = new NumericUpDown { Location = new Point(70, 310), Minimum = 1, Maximum = 30, Value = 3, Width = 50, Visible = false };
            Label labelRows2 = new Label { Text = "Строки:", Location = new Point(10, 295), AutoSize = true, Visible = false };
            Label labelCols2 = new Label { Text = "Столбцы:", Location = new Point(70, 295), AutoSize = true, Visible = false };
            dataGridView2 = new DataGridView { Location = new Point(10, 340), Size = new Size(350, 200), AllowUserToAddRows = false, Visible = false };
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.ColumnHeadersVisible = false;
            // Pre‑create matrix B so it's ready when shown
            CreateMatrixFromSize(dataGridView2, (int)rows2.Value, (int)cols2.Value);
            leftPanel.Controls.AddRange(new Control[] { labelB, labelRows2, labelCols2, rows2, cols2, dataGridView2 });

            Panel rightPanel = new Panel { Dock = DockStyle.Fill };
            richTextBox1 = new RichTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(340, 450),
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                WordWrap = false   // keep lines from wrapping, ensures alignment
            };
            rightPanel.Controls.Add(richTextBox1);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(10) };
            matrix2 = new Button { Text = "Две матрицы", Size = new Size(100, 30) };
            button1 = new Button { Text = "Вставить в A", Size = new Size(100, 30) };
            button2 = new Button { Text = "Случайные", Size = new Size(100, 30) };
            button3 = new Button { Text = "Вставить в B", Size = new Size(100, 30) };
            add = new Button { Text = "+", Size = new Size(60, 30), Visible = false };
            minus = new Button { Text = "-", Size = new Size(60, 30), Visible = false };
            multiply = new Button { Text = "*", Size = new Size(60, 30), Visible = false };
            del = new Button { Text = "/", Size = new Size(60, 30), Visible = false };
            transpose = new Button { Text = "Трансп.", Size = new Size(80, 30) };
            determinant = new Button { Text = "Определитель", Size = new Size(100, 30) };
            inverse = new Button { Text = "Обратная", Size = new Size(80, 30) };
            multinumber = new Button { Text = "× число", Size = new Size(80, 30) };
            clear = new Button { Text = "Очистить", Size = new Size(80, 30) };

            buttonPanel.Controls.AddRange(new Control[] { matrix2, button1, button2, button3, add, minus, multiply, del, transpose, determinant, inverse, multinumber, clear });

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightPanel, 1, 0);
            mainLayout.Controls.Add(buttonPanel, 0, 1);
            mainLayout.SetColumnSpan(buttonPanel, 2);
            this.Controls.Add(mainLayout);

            // Events
            rows1.ValueChanged += (s, e) => CreateMatrixFromSize(dataGridView1, (int)rows1.Value, (int)cols1.Value);
            cols1.ValueChanged += (s, e) => CreateMatrixFromSize(dataGridView1, (int)rows1.Value, (int)cols1.Value);
            rows2.ValueChanged += (s, e) => { if (ismatrixMode) CreateMatrixFromSize(dataGridView2, (int)rows2.Value, (int)cols2.Value); };
            cols2.ValueChanged += (s, e) => { if (ismatrixMode) CreateMatrixFromSize(dataGridView2, (int)rows2.Value, (int)cols2.Value); };

            matrix2.Click += Matrix2_Click;
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
            add.Click += Add_Click;
            minus.Click += Minus_Click;
            multiply.Click += Multiply_Click;
            del.Click += Del_Click;
            transpose.Click += Transpose_Click;
            determinant.Click += Determinant_Click;
            inverse.Click += Inverse_Click;
            multinumber.Click += Multinumber_Click;
            clear.Click += Clear_Click;

            CreateMatrixFromSize(dataGridView1, 3, 3);
        }

        private void CreateMatrixFromSize(DataGridView grid, int rows, int cols)
        {
            grid.RowCount = rows;
            grid.ColumnCount = cols;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (grid.Rows[i].Cells[j].Value == null)
                        grid.Rows[i].Cells[j].Value = 0;
        }

        private double[,] GetMatrixFromGrid(DataGridView grid, int rows, int cols, string name)
        {
            double[,] m = new double[rows, cols];
            bool error = false;
            string msg = "";
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    object val = grid.Rows[i].Cells[j].Value;
                    if (val == null || val.ToString() == "") m[i, j] = 0;
                    else if (!double.TryParse(val.ToString(), out double d))
                    {
                        error = true;
                        msg += $"{name}[{i + 1},{j + 1}] = '{val}' – не число\n";
                        m[i, j] = 0;
                    }
                    else m[i, j] = d;
                }
            if (error) MessageBox.Show(msg, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return m;
        }

        private void DisplayResult(double[,] m)
        {
            richTextBox1.Clear();
            if (m == null) return;

            int rows = m.GetLength(0);
            int cols = m.GetLength(1);
            const int columnWidth = 12; // enough for numbers up to ±999999.99

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    string numStr = m[i, j].ToString("F2");
                    richTextBox1.Text += numStr.PadLeft(columnWidth);
                }
                richTextBox1.Text += "\n";
            }
        }

        private void Matrix2_Click(object sender, EventArgs e)
        {
            ismatrixMode = !ismatrixMode;
            if (ismatrixMode)
            {
                this.Size = matrixSize;
                this.MinimumSize = matrixSize;
                this.MaximumSize = matrixSize;
                dataGridView2.Visible = true;
                rows2.Visible = true;
                cols2.Visible = true;
                // Show labels for rows2/cols2
                foreach (Control c in leftPanel.Controls)
                {
                    if (c is Label lbl && (lbl.Text == "Строки:" || lbl.Text == "Столбцы:") && lbl.Location.Y == 295)
                        lbl.Visible = true;
                }
                add.Visible = minus.Visible = multiply.Visible = del.Visible = true;
                matrix2.Text = "Одна матрица";
                CreateMatrixFromSize(dataGridView2, (int)rows2.Value, (int)cols2.Value);
            }
            else
            {
                this.Size = normalSize;
                this.MinimumSize = normalSize;
                this.MaximumSize = normalSize;
                dataGridView2.Visible = false;
                rows2.Visible = false;
                cols2.Visible = false;
                foreach (Control c in leftPanel.Controls)
                {
                    if (c is Label lbl && (lbl.Text == "Строки:" || lbl.Text == "Столбцы:") && lbl.Location.Y == 295)
                        lbl.Visible = false;
                }
                add.Visible = minus.Visible = multiply.Visible = del.Visible = false;
                matrix2.Text = "Две матрицы";
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (result == null) { MessageBox.Show("Нет результата для вставки"); return; }
            int rows = result.GetLength(0), cols = result.GetLength(1);
            rows1.Value = rows; cols1.Value = cols;
            CreateMatrixFromSize(dataGridView1, rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    dataGridView1.Rows[i].Cells[j].Value = result[i, j];
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            int rowsA = (int)rows1.Value, colsA = (int)cols1.Value;
            for (int i = 0; i < rowsA; i++)
                for (int j = 0; j < colsA; j++)
                    dataGridView1.Rows[i].Cells[j].Value = random.Next(-10, 11);
            if (ismatrixMode && dataGridView2.Visible)
            {
                int rowsB = (int)rows2.Value, colsB = (int)cols2.Value;
                for (int i = 0; i < rowsB; i++)
                    for (int j = 0; j < colsB; j++)
                        dataGridView2.Rows[i].Cells[j].Value = random.Next(-10, 11);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (result == null) { MessageBox.Show("Нет результата для вставки"); return; }
            if (!ismatrixMode) { MessageBox.Show("Включите режим двух матриц"); return; }
            int rows = result.GetLength(0), cols = result.GetLength(1);
            rows2.Value = rows; cols2.Value = cols;
            CreateMatrixFromSize(dataGridView2, rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    dataGridView2.Rows[i].Cells[j].Value = result[i, j];
        }

        private void Add_Click(object sender, EventArgs e)
        {
            int ra = (int)rows1.Value, ca = (int)cols1.Value;
            int rb = (int)rows2.Value, cb = (int)cols2.Value;
            if (ra != rb || ca != cb) { MessageBox.Show("Размеры матриц не совпадают"); return; }
            matrixA = GetMatrixFromGrid(dataGridView1, ra, ca, "A");
            matrixB = GetMatrixFromGrid(dataGridView2, rb, cb, "B");
            result = new double[ra, ca];
            for (int i = 0; i < ra; i++)
                for (int j = 0; j < ca; j++)
                    result[i, j] = matrixA[i, j] + matrixB[i, j];
            DisplayResult(result);
        }

        private void Minus_Click(object sender, EventArgs e)
        {
            int ra = (int)rows1.Value, ca = (int)cols1.Value;
            int rb = (int)rows2.Value, cb = (int)cols2.Value;
            if (ra != rb || ca != cb) { MessageBox.Show("Размеры матриц не совпадают"); return; }
            matrixA = GetMatrixFromGrid(dataGridView1, ra, ca, "A");
            matrixB = GetMatrixFromGrid(dataGridView2, rb, cb, "B");
            result = new double[ra, ca];
            for (int i = 0; i < ra; i++)
                for (int j = 0; j < ca; j++)
                    result[i, j] = matrixA[i, j] - matrixB[i, j];
            DisplayResult(result);
        }

        private void Multiply_Click(object sender, EventArgs e)
        {
            int ra = (int)rows1.Value, ca = (int)cols1.Value;
            int rb = (int)rows2.Value, cb = (int)cols2.Value;
            if (ca != rb) { MessageBox.Show("Число столбцов A ≠ числу строк B"); return; }
            matrixA = GetMatrixFromGrid(dataGridView1, ra, ca, "A");
            matrixB = GetMatrixFromGrid(dataGridView2, rb, cb, "B");
            result = new double[ra, cb];
            for (int i = 0; i < ra; i++)
                for (int j = 0; j < cb; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < ca; k++)
                        sum += matrixA[i, k] * matrixB[k, j];
                    result[i, j] = sum;
                }
            DisplayResult(result);
        }

        private void Del_Click(object sender, EventArgs e)
        {
            int ra = (int)rows1.Value, ca = (int)cols1.Value;
            int rb = (int)rows2.Value, cb = (int)cols2.Value;
            if (rb != cb) { MessageBox.Show("Матрица B должна быть квадратной"); return; }
            if (ca != rb) { MessageBox.Show("Столбцы A ≠ размеру B"); return; }
            matrixA = GetMatrixFromGrid(dataGridView1, ra, ca, "A");
            matrixB = GetMatrixFromGrid(dataGridView2, rb, cb, "B");
            double det = Determinant(matrixB);
            if (Math.Abs(det) < 1e-10) { MessageBox.Show("Определитель B = 0, деление невозможно"); return; }
            double[,] invB = Inverse(matrixB);
            result = MultiplyMatrices(matrixA, invB);
            DisplayResult(result);
        }

        private double[,] MultiplyMatrices(double[,] A, double[,] B)
        {
            int ra = A.GetLength(0), ca = A.GetLength(1);
            int cb = B.GetLength(1);
            double[,] res = new double[ra, cb];
            for (int i = 0; i < ra; i++)
                for (int j = 0; j < cb; j++)
                    for (int k = 0; k < ca; k++)
                        res[i, j] += A[i, k] * B[k, j];
            return res;
        }

        private void Transpose_Click(object sender, EventArgs e)
        {
            int rows = (int)rows1.Value, cols = (int)cols1.Value;
            matrixA = GetMatrixFromGrid(dataGridView1, rows, cols, "A");
            result = new double[cols, rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[j, i] = matrixA[i, j];
            DisplayResult(result);
        }

        private void Determinant_Click(object sender, EventArgs e)
        {
            int n = (int)rows1.Value;
            if (n != (int)cols1.Value) { MessageBox.Show("Матрица не квадратная"); return; }
            matrixA = GetMatrixFromGrid(dataGridView1, n, n, "A");
            double det = Determinant(matrixA);
            richTextBox1.Clear();
            richTextBox1.Text = $"Определитель = {det:F4}\n";
        }

        // -------------------- NEW: Gaussian elimination for determinant (O(n³)) --------------------
        private double Determinant(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] a = (double[,])matrix.Clone();
            double det = 1.0;
            for (int i = 0; i < n; i++)
            {
                // Find pivot
                int pivot = i;
                for (int j = i + 1; j < n; j++)
                    if (Math.Abs(a[j, i]) > Math.Abs(a[pivot, i]))
                        pivot = j;
                if (Math.Abs(a[pivot, i]) < 1e-12)
                    return 0; // singular
                if (pivot != i)
                {
                    // swap rows
                    for (int k = i; k < n; k++)
                    {
                        double t = a[i, k];
                        a[i, k] = a[pivot, k];
                        a[pivot, k] = t;
                    }
                    det = -det;
                }
                det *= a[i, i];
                // eliminate below
                for (int j = i + 1; j < n; j++)
                {
                    double factor = a[j, i] / a[i, i];
                    for (int k = i + 1; k < n; k++)
                        a[j, k] -= factor * a[i, k];
                    // a[j,i] becomes zero, no need to set explicitly
                }
            }
            return det;
        }

        // -------------------- NEW: Gauss-Jordan elimination for inverse (O(n³)) --------------------
        private double[,] Inverse(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] a = new double[n, 2 * n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    a[i, j] = matrix[i, j];
            for (int i = 0; i < n; i++)
                a[i, n + i] = 1.0;

            for (int i = 0; i < n; i++)
            {
                // Find pivot
                int pivot = i;
                for (int j = i + 1; j < n; j++)
                    if (Math.Abs(a[j, i]) > Math.Abs(a[pivot, i]))
                        pivot = j;
                if (Math.Abs(a[pivot, i]) < 1e-12)
                    throw new InvalidOperationException("Матрица вырождена, обратной не существует.");
                if (pivot != i)
                {
                    // swap rows
                    for (int k = i; k < 2 * n; k++)
                    {
                        double t = a[i, k];
                        a[i, k] = a[pivot, k];
                        a[pivot, k] = t;
                    }
                }
                // normalize row i
                double factor = a[i, i];
                for (int k = i; k < 2 * n; k++)
                    a[i, k] /= factor;
                // eliminate other rows
                for (int j = 0; j < n; j++)
                {
                    if (j == i) continue;
                    double coeff = a[j, i];
                    if (coeff == 0) continue;
                    for (int k = i; k < 2 * n; k++)
                        a[j, k] -= coeff * a[i, k];
                }
            }
            double[,] inv = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inv[i, j] = a[i, n + j];
            return inv;
        }

        private void Inverse_Click(object sender, EventArgs e)
        {
            int n = (int)rows1.Value;
            if (n != (int)cols1.Value) { MessageBox.Show("Матрица не квадратная"); return; }
            matrixA = GetMatrixFromGrid(dataGridView1, n, n, "A");
            try
            {
                result = Inverse(matrixA);
                DisplayResult(result);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Multinumber_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Введите число:", "Умножение на число", "2");
            if (!double.TryParse(input, out double num)) { MessageBox.Show("Неверное число"); return; }
            int rows = (int)rows1.Value, cols = (int)cols1.Value;
            matrixA = GetMatrixFromGrid(dataGridView1, rows, cols, "A");
            result = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = matrixA[i, j] * num;
            DisplayResult(result);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            int ra = (int)rows1.Value, ca = (int)cols1.Value;
            for (int i = 0; i < ra; i++)
                for (int j = 0; j < ca; j++)
                    dataGridView1.Rows[i].Cells[j].Value = 0;
            if (ismatrixMode && dataGridView2.Visible)
            {
                int rb = (int)rows2.Value, cb = (int)cols2.Value;
                for (int i = 0; i < rb; i++)
                    for (int j = 0; j < cb; j++)
                        dataGridView2.Rows[i].Cells[j].Value = 0;
            }
            result = null;
        }
    }
}