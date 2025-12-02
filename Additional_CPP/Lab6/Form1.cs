using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab6
{
    public partial class Form1 : Form
    {
        int N = 1;
        int i = 0;
        int j = 0;
        int Change;
        double[,] A = new double[6, 6];
        double[] B = new double[6];
        double[] X = new double[6];

        public Form1()
        {
            InitializeComponent();
        }

        #region Метод LU-Перетворення (З ПЕРЕВІРКАМИ)
        private void Decomp(int N, ref int Change)
        {
            int k;
            double maxEl;
            int maxRow;
            Change = 1;

            maxEl = Math.Abs(A[1, 1]);
            maxRow = 1;
            for (i = 2; i <= N; i++)
            {
                if (Math.Abs(A[i, 1]) > maxEl)
                {
                    maxEl = Math.Abs(A[i, 1]);
                    maxRow = i;
                }
            }

            if (maxRow != 1)
            {
                for (j = 1; j <= N; j++)
                {
                    double temp = A[1, j];
                    A[1, j] = A[maxRow, j];
                    A[maxRow, j] = temp;
                }
                Change = maxRow;
            }

            if (Math.Abs(A[1, 1]) < 1E-10)
            {
            }

            for (i = 2; i <= N; i++)
            {
                if (Math.Abs(A[1, 1]) > 1E-10)
                {
                    A[1, i] = A[1, i] / A[1, 1];
                }
            }

            for (i = 2; i <= N; i++)
            {
                for (k = i; k <= N; k++)
                {
                    double sum = 0;
                    for (j = 1; j <= i - 1; j++)
                    {
                        sum += A[k, j] * A[j, i];
                    }
                    A[k, i] = A[k, i] - sum;
                }

                for (k = i + 1; k <= N; k++)
                {
                    double sum = 0;
                    for (j = 1; j <= i - 1; j++)
                    {
                        sum += A[i, j] * A[j, k];
                    }

                    if (Math.Abs(A[i, i]) > 1E-10)
                    {
                        A[i, k] = (A[i, k] - sum) / A[i, i];
                    }
                    else
                    {
                    }
                }
            }

            for (i = 0; i < N; i++)
                for (j = 0; j < N; j++)
                {
                    C_matrix_dgv.Rows[i].Cells[j].Value = Convert.ToString(A[i + 1, j + 1]);
                }
        }

        private void Solve(int Change, int N)
        {
            if (Change != 1)
            {
                double temp = B[1];
                B[1] = B[Change];
                B[Change] = temp;
            }

            if (Math.Abs(A[1, 1]) < 1E-10)
            {
                MessageBox.Show("Помилка (Solve): Матриця вироджена. Ділення на 0.");
                for (int m = 0; m < N; m++) X_vector_dgv[0, m].Value = "Error";
                return;
            }
            B[1] = B[1] / A[1, 1];

            for (i = 2; i <= N; i++)
            {
                double sum = 0;
                for (j = 1; j <= i - 1; j++)
                {
                    sum += A[i, j] * B[j];
                }

                if (Math.Abs(A[i, i]) < 1E-10)
                {
                    if (Math.Abs(B[i] - sum) < 1E-10)
                    {
                        MessageBox.Show("Система має безліч розв'язків (випадок 0 / 0).");
                        for (int m = 0; m < N; m++) X_vector_dgv[0, m].Value = "Infinite";
                    }
                    else
                    {
                        MessageBox.Show("Система не має розв'язків (випадок X / 0).");
                        for (int m = 0; m < N; m++) X_vector_dgv[0, m].Value = "No Solution";
                    }
                    for (i = 0; i < N; i++) X_vector_dgv[0, i].Value = "Error";
                    return;
                }

                B[i] = (B[i] - sum) / A[i, i];
            }

            X[N] = B[N];
            for (i = N - 1; i >= 1; i--)
            {
                double sum = 0;
                for (j = i + 1; j <= N; j++)
                {
                    sum += A[i, j] * X[j];
                }
                X[i] = B[i] - sum;
            }
        }
        #endregion

        /// <summary>
        /// Розв'язує СЛАР методом Гауса
        /// </summary>
        private void SolveGauss()
        {

            for (int k = 1; k <= N - 1; k++) 
            {
                int maxRow = k;
                double maxVal = Math.Abs(A[k, k]);

                for (int i = k + 1; i <= N; i++)
                {
                    if (Math.Abs(A[i, k]) > maxVal)
                    {
                        maxVal = Math.Abs(A[i, k]);
                        maxRow = i;
                    }
                }

                if (maxVal < 1E-10)
                {
                    MessageBox.Show("Помилка: Матриця вироджена. Розв'язок неможливий або неєдиний.");
                    for (int m = 0; m < N; m++) X_vector_dgv[0, m].Value = "Error";
                    return;
                }

                if (maxRow != k)
                {
                    for (int j = k; j <= N; j++)
                    {
                        double temp = A[k, j];
                        A[k, j] = A[maxRow, j];
                        A[maxRow, j] = temp;
                    }
                    double tempB = B[k];
                    B[k] = B[maxRow];
                    B[maxRow] = tempB;
                }

                for (int i = k + 1; i <= N; i++)
                {
                    double factor = A[i, k] / A[k, k];
                    for (int j = k + 1; j <= N; j++)
                    {
                        A[i, j] = A[i, j] - factor * A[k, j];
                    }

                    B[i] = B[i] - factor * B[k];

                    A[i, k] = 0;
                }
            }

            for (i = 0; i < N; i++)
                for (j = 0; j < N; j++)
                {
                    C_matrix_dgv.Rows[i].Cells[j].Value = Convert.ToString(A[i + 1, j + 1]);
                }

            for (i = N; i >= 1; i--)
            {
                double sum = 0;
                for (j = i + 1; j <= N; j++)
                {
                    sum += A[i, j] * X[j];
                }

                if (Math.Abs(A[i, i]) < 1E-10)
                {
                    MessageBox.Show($"Помилка: Ділення на 0 на кроці {i}. Матриця вироджена.");
                    for (int m = 0; m < N; m++) X_vector_dgv[0, m].Value = "Error";
                    return;
                }

                X[i] = (B[i] - sum) / A[i, i];
            }
        }
        #endregion

        private void BCreateGrid_Click(object sender, EventArgs e)
        {
            bool exc_A = false;
            bool exc_B = false;

            for (i = 1; i <= N; i++)
                for (j = 1; j <= N; j++)
                {
                    try
                    {
                        A[i, j] = Convert.ToDouble(A_matrix_dgv[j - 1, i - 1].Value);
                    }
                    catch
                    {
                        A_matrix_dgv[j - 1, i - 1].Style.ForeColor = Color.Red;
                        exc_A = true;
                    }
                }

            for (j = 0; j < N; j++)
            {
                try
                {
                    B[j + 1] = Convert.ToDouble(B_vector_dgv[0, j].Value);
                }
                catch
                {
                    B_vector_dgv[0, j].Style.ForeColor = Color.Red;
                    exc_B = true;
                }
            }

            if (exc_A || exc_B)
            {
                MessageBox.Show("Помилка введення!");
                return;
            }

            if (MethodSelector_cb.SelectedIndex == 0)
            {
                Decomp(N, ref Change);
                Solve(Change, N);
            }
            else
            {
                SolveGauss();
            }


            for (i = 0; i < N; i++)
            {
                X_vector_dgv[0, i].Value = X[i + 1].ToString();
            }
            MessageBox.Show("Розв'язок знайдено");
        }

        private void BClear_Click(object sender, EventArgs e)
        {
            for (i = 0; i < N; i++)
                for (j = 0; j < N; j++)
                {
                    A_matrix_dgv[j, i].Value = "";
                    C_matrix_dgv[j, i].Value = "";
                }
            for (j = 0; j < N; j++)
            {
                B_vector_dgv[0, j].Value = "";
                X_vector_dgv[0, j].Value = "";
            }
        }

        private void BClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            X_vector_dgv.ReadOnly = true;
            A_matrix_dgv.AllowUserToAddRows = false;
            B_vector_dgv.AllowUserToAddRows = false;
            X_vector_dgv.AllowUserToAddRows = false;
            C_matrix_dgv.AllowUserToAddRows = false;

            A_matrix_dgv.ColumnCount = 1;
            A_matrix_dgv.RowCount = 1;
            X_vector_dgv.ColumnCount = 1;
            X_vector_dgv.RowCount = 1;
            B_vector_dgv.ColumnCount = 1;
            B_vector_dgv.RowCount = 1;
            C_matrix_dgv.ColumnCount = 1;
            C_matrix_dgv.RowCount = 1;

            MethodSelector_cb.Items.Add("Метод LU-перетворення");
            MethodSelector_cb.Items.Add("Метод Гауса");
            MethodSelector_cb.SelectedIndex = 0;
        }

        private void NUD_rozmir_ValueChanged(object sender, EventArgs e)
        {
            N = Convert.ToInt16(NUD_rozmir.Value);
            A_matrix_dgv.RowCount = N;
            A_matrix_dgv.ColumnCount = N;
            X_vector_dgv.RowCount = N;
            B_vector_dgv.RowCount = N;
            C_matrix_dgv.RowCount = N;
            C_matrix_dgv.ColumnCount = N;
        }

        private void A_matrix_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            A_matrix_dgv.CurrentCell.Style.ForeColor = Color.Black;
        }

        private void B_vector_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            B_vector_dgv.CurrentCell.Style.ForeColor = Color.Black;
        }

        private void MethodSelector_cb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}