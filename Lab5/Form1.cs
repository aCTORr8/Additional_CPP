using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Math;

namespace Lab5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label7.Visible = false;
            textBox6.Visible = false;
        }

        private double f(double x, ref int k1)
        {
            switch (k1)
            {
                case 0: return x * x - 4;
                case 1: return 3 * x - 4 * Log(x) - 5;
                case 2: return Sin(x) - 0.5;
            }
            return 0;
        }
        private double fp(double x, double d, ref int k1)
        {
            return (f(x + d, ref k1) - f(x, ref k1)) / d;
        }
        private double f2p(double x, double d, ref int k1)
        {
            return (f(x + d, ref k1) + f(x - d, ref k1) - 2 * f(x, ref k1)) / (d * d);
        }
        private void MDP_void(double a, double b, double Eps, ref int k1, ref int L)
        {
            double c = 0, Fc;
            while (b - a > Eps)
            {
                c = 0.5 * (b - a) + a;
                L++;
                Fc = f(c, ref k1);

                if (f(a, ref k1) * Fc > 0)
                    a = c;
                else
                    b = c;
            }
            textBox5.Text = Convert.ToString(c);
            textBox4.Text = Convert.ToString(L);
        }
        private void MN_void(double a, double b, double Eps, ref int k1, int Kmax, ref int L)
        {
            double x, Dx, D;
            int i;
            Dx = 0.0;
            D = Eps / 100.0;
            x = b;

            if (f(x, ref k1) * f2p(x, D, ref k1) < 0)
                x = a;

            if (f(x, ref k1) * f2p(x, D, ref k1) < 0)
                MessageBox.Show("Для цього рівняння збіжність ітерацій не гарантована");

            for (i = 1; i <= Kmax; i++)
            {
                Dx = f(x, ref k1) / fp(x, D, ref k1);
                x = x - Dx;
                if (Abs(Dx) < Eps)
                {
                    L = i;
                    textBox5.Text = Convert.ToString(x);
                    textBox4.Text = Convert.ToString(L);
                    return;
                }
            }
            MessageBox.Show("За задану кількість ітерацій кореня не знайдено");
            textBox5.Text = "-";
            textBox4.Text = "N/A";
        }
        private void radioMethod_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox6.Clear();

            label7.Visible = false;
            textBox6.Visible = false;

            if (radioMN.Checked)
            {
                label7.Visible = true;
                textBox6.Visible = true;
            }
        }
        private bool FindCorrectInterval(ref double a, ref double b, ref int k1, double step)
        {
            double Fa = f(a, ref k1);
            double x;

            if (Abs(Fa) < 1e-9) return true;

            for (x = a + step; x <= b; x += step)
            {
                double Fx = f(x, ref k1);
                if (Fa * Fx < 0)
                {
                    b = x;
                    return true;
                }
                a = x;
                Fa = Fx;
            }

            if (f(a, ref k1) * f(b, ref k1) < 0)
                return true;

            return false;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            int L, k = -1, Kmax = 0, m = -1;
            double D = 0, Eps = 0, a_orig, b_orig, a, b;
            L = 0;

            if (radioMDP.Checked)
            {
                m = 0;
            }
            else if (radioMN.Checked)
            {
                m = 1;
            }

            if (m == -1)
            {
                MessageBox.Show("Оберіть метод!");
                return;
            }

            switch (comboBox2.SelectedIndex)
            {
                case 0: k = 0; break;
                case 1: k = 1; break;
                case 2: k = 2; break;
            }

            if (k == -1)
            {
                MessageBox.Show("Оберіть рівняння!");
                comboBox2.Focus();
                return;
            }

            if (textBox1.Text == "") { MessageBox.Show("Введіть число в textBox1"); textBox1.Focus(); return; }
            a = Convert.ToDouble(textBox1.Text);

            if (textBox2.Text == "") { MessageBox.Show("Введіть число в textBox2"); textBox2.Focus(); return; }
            b = Convert.ToDouble(textBox2.Text);

            a_orig = a;
            b_orig = b;

            if (a > b)
            {
                D = a;
                a = b;
                b = D;
                textBox1.Text = Convert.ToString(a);
                textBox2.Text = Convert.ToString(b);
            }

            if (textBox3.Text == "") { MessageBox.Show("Введіть число в textBox3"); textBox3.Focus(); return; }
            Eps = Convert.ToDouble(textBox3.Text);

            if ((Eps > 1e-1) || (Eps <= 0))
            {
                Eps = 1e-4;
            }
            textBox3.Text = Convert.ToString(Eps);

            if (m == 0)
            {

                if ((f(a, ref k)) * (f(b, ref k)) > 0)
                {
                    double step = (b - a) / 100.0;

                    double temp_a = a;
                    double temp_b = b;

                    if (!FindCorrectInterval(ref temp_a, ref temp_b, ref k, step))
                    {
                        MessageBox.Show("На інтервалі [" + a_orig.ToString() + ", " + b_orig.ToString() + "] кореня немає або він не може бути знайдений методом ділення навпіл.");
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox1.Focus();
                        return;
                    }
                    else
                    {
                        a = temp_a;
                        b = temp_b;
                    }
                }
            }

            switch (m)
            {
                case 0:
                    {
                        MDP_void(a, b, Eps, ref k, ref L);
                    }
                    break;

                case 1:
                    {
                        if (textBox6.Text == "") { MessageBox.Show("Введіть число в Kmax"); textBox6.Focus(); return; }
                        Kmax = Convert.ToInt32(textBox6.Text);

                        MN_void(a, b, Eps, ref k, Kmax, ref L);
                    }
                    break;
            }
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox6.Clear();
            textBox5.Clear();
            textBox4.Clear();

            if (radioMN.Checked)
            {
                label7.Visible = true;
                textBox6.Visible = true;
            }
            else
            {
                label7.Visible = false;
                textBox6.Visible = false;
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            Close();
        }
        private void label8_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}