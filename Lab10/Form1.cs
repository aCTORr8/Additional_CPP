using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int N = 3;
        double[,] Ja;
        double[] x0;
        double[] X;
        double[] F;
        double[] Dx;
        double[] Fp;

        public Form1()
        {
            InitializeComponent();

            InitData();
        }
        private void InitData()
        {
            N = (int)nudN.Value;

            Ja = new double[N + 1, N + 1];
            x0 = new double[N + 1];
            X = new double[N + 1];
            F = new double[N + 1];
            Dx = new double[N + 1];
            Fp = new double[N + 1];

            dgvX0.RowCount = N;
            dgvX0.ColumnHeadersVisible = false;
            dgvX0.RowHeadersVisible = false;
            dgvX0.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.RowCount = N;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void nudN_ValueChanged(object sender, EventArgs e)
        {
            InitData();
        }
        public void FM(double[] X, ref double[] f)
        {
            f[1] = X[1] + Math.Exp(X[1] - 1.0) + (X[2] + X[3]) * (X[2] + X[3]) - 27.0;
            f[2] = X[1] * Math.Exp(X[2] - 2.0) + X[3] * X[3] - 10.0;
            f[3] = X[3] + Math.Sin(X[2] - 2.0) + X[2] * X[2] - 7.0;
        }
        public double[,] Jacob(double[] X)
        {
            int i, j;
            double Q = 0.000001;

            FM(X, ref F);

            for (j = 1; j <= N; j++)
            {
                X[j] = X[j] + Q;
                FM(X, ref Fp);

                for (i = 1; i <= N; i++)
                {
                    Ja[i, j] = (Fp[i] - F[i]) / Q;
                }
                X[j] = X[j] - Q;
            }
            return Ja;
        }
        public void GAUSS(double[,] A, double[] B, ref double[] X, int N)
        {
            double[,] tempA = new double[N + 1, N + 1];
            double[] tempB = new double[N + 1];

            for (int r = 1; r <= N; r++)
            {
                for (int c = 1; c <= N; c++)
                {
                    tempA[r, c] = A[r, c];
                }
                tempB[r] = B[r];
                X[r] = 0;
            }

            for (int i = 1; i <= N; i++)
            {
                double maxVal = Math.Abs(tempA[i, i]);
                int pivotRow = i;
                for (int k = i + 1; k <= N; k++)
                {
                    if (Math.Abs(tempA[k, i]) > maxVal)
                    {
                        maxVal = Math.Abs(tempA[k, i]);
                        pivotRow = k;
                    }
                }

                if (pivotRow != i)
                {
                    for (int j = i; j <= N; j++)
                    {
                        double temp = tempA[i, j];
                        tempA[i, j] = tempA[pivotRow, j];
                        tempA[pivotRow, j] = temp;
                    }
                    double tempBVal = tempB[i];
                    tempB[i] = tempB[pivotRow];
                    tempB[pivotRow] = tempBVal;
                }

                if (Math.Abs(tempA[i, i]) < 1e-12)
                {
                    MessageBox.Show("Матриця є виродженою (або близькою до неї)! Розв'язок може бути неточним.");
                }

                for (int k = i + 1; k <= N; k++)
                {
                    double factor = tempA[k, i] / tempA[i, i];
                    for (int j = i; j <= N; j++)
                    {
                        tempA[k, j] -= factor * tempA[i, j];
                    }
                    tempB[k] -= factor * tempB[i];
                }
            }

            for (int i = N; i >= 1; i--)
            {
                double sum = 0;
                for (int j = i + 1; j <= N; j++)
                {
                    sum += tempA[i, j] * X[j];
                }
                if (Math.Abs(tempA[i, i]) > 1e-12)
                    X[i] = (tempB[i] - sum) / tempA[i, i];
                else
                    X[i] = 0;
            }
        }
        public void LU_Decomp(double[,] A, int N, ref int Change)
        {
            int i, j, k;
            double sum;
            Change = 1;

            double maxVal = Math.Abs(A[1, 1]);
            int pivotRow = 1;

            for (i = 2; i <= N; i++)
            {
                if (Math.Abs(A[i, 1]) > maxVal)
                {
                    maxVal = Math.Abs(A[i, 1]);
                    pivotRow = i;
                }
            }

            if (pivotRow != 1)
            {
                Change = pivotRow;
                for (j = 1; j <= N; j++)
                {
                    double temp = A[1, j];
                    A[1, j] = A[pivotRow, j];
                    A[pivotRow, j] = temp;
                }
            }

            if (Math.Abs(A[1, 1]) < 1e-12)
            {
                return;
            }

            for (j = 2; j <= N; j++)
            {
                A[1, j] = A[1, j] / A[1, 1];
            }

            for (i = 2; i <= N; i++)
            {
                for (k = i; k <= N; k++)
                {
                    sum = 0;
                    for (j = 1; j <= i - 1; j++)
                    {
                        sum += A[k, j] * A[j, i];
                    }
                    A[k, i] = A[k, i] - sum;
                }

                if (Math.Abs(A[i, i]) < 1e-12) return;

                for (k = i + 1; k <= N; k++)
                {
                    sum = 0;
                    for (j = 1; j <= i - 1; j++)
                    {
                        sum += A[i, j] * A[j, k];
                    }
                    A[i, k] = (A[i, k] - sum) / A[i, i];
                }
            }
        }

        public void LU_Solve(double[,] A, double[] B, ref double[] X, int N, int Change)
        {
            int i, j;
            double sum;
            double[] Y = new double[N + 1];
            double[] bLocal = (double[])B.Clone();

            if (Change != 1)
            {
                double temp = bLocal[1];
                bLocal[1] = bLocal[Change];
                bLocal[Change] = temp;
            }

            for (i = 1; i <= N; i++)
            {
                sum = 0;
                for (j = 1; j < i; j++)
                {
                    sum += A[i, j] * Y[j];
                }
                Y[i] = (bLocal[i] - sum) / A[i, i];
            }

            for (i = N; i >= 1; i--)
            {
                sum = 0;
                for (j = i + 1; j <= N; j++)
                {
                    sum += A[i, j] * X[j];
                }
                X[i] = Y[i] - sum;
            }
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            for (int i = 0; i < N; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = "";
            }

            double Eps;
            int KMax;

            try
            {
                Eps = Convert.ToDouble(textBox1.Text);
                KMax = Convert.ToInt32(textBox2.Text);

                for (int i = 0; i < N; i++)
                {
                    if (dgvX0.Rows[i].Cells[0].Value != null)
                        x0[i + 1] = Convert.ToDouble(dgvX0.Rows[i].Cells[0].Value);
                    else
                        x0[i + 1] = 0;
                }
            }
            catch
            {
                MessageBox.Show("Перевірте коректність введених даних!");
                return;
            }

            X = (double[])x0.Clone();
            double DxMax = 0;

            int pivotChange = 1;
            if (checkBox1.Checked)
            {
                Ja = Jacob(X);
                LU_Decomp(Ja, N, ref pivotChange);
            }

            for (int k = 1; k <= KMax; k++)
            {
                FM(X, ref F);

                if (checkBox1.Checked)
                {
                    LU_Solve(Ja, F, ref Dx, N, pivotChange);
                }
                else
                {
                    Ja = Jacob(X);
                    GAUSS(Ja, F, ref Dx, N);
                }

                DxMax = 0;
                bool isError = false;

                for (int i = 1; i <= N; i++)
                {
                    X[i] = X[i] - Dx[i];

                    if (double.IsNaN(X[i]) || double.IsInfinity(X[i]))
                    {
                        isError = true;
                    }

                    if (Math.Abs(Dx[i]) > DxMax)
                        DxMax = Math.Abs(Dx[i]);
                }

                if (isError)
                {
                    MessageBox.Show("Процес розбігається! Спробуйте інші початкові умови.", "Помилка математики");
                    return;
                }

                if (DxMax <= Eps)
                {
                    textBox3.Text = k.ToString();
                    for (int i = 0; i < N; i++)
                    {
                        dataGridView1.Rows[i].Cells[0].Value = X[i + 1];
                    }

                    MessageBox.Show(checkBox1.Checked ?
                        "Розв'язок Модифікованим методом знайдено!" :
                        "Розв'язок Класичним методом знайдено!");
                    return;
                }
            }

            MessageBox.Show("Не вдалося знайти розв'язок за задану кількість ітерацій (KMax). \nСпробуйте збільшити KMax.", "Ліміт ітерацій");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < N; i++)
            {
                dgvX0.Rows[i].Cells[0].Value = 0;
                dataGridView1.Rows[i].Cells[0].Value = "";
            }
            textBox3.Text = "";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}