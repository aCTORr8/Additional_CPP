using System;
using System.Drawing;
using System.Windows.Forms;

namespace labbb8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Клас, що успадковує TextBox і реалізує ICloneable
        public class MyTextBox : TextBox, ICloneable
        {
            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Створює екземпляр класу MyTextBox, задає значення властивостей:
            MyTextBox tb2 = new MyTextBox();
            tb2.Text = "Це текст з TextBox2";
            tb2.BackColor = Color.LightBlue;

            // Властивість Location задає розміщення контрола на формі
            tb2.Location = new System.Drawing.Point(50, 110);

            // Властивість Size задає розміри контрола на формі
            tb2.Size = new System.Drawing.Size(170, 30);

            // Метод Clone створить екземпляр типу object, який можна привести до типу TextBox
            TextBox clonedTextBox = (TextBox)tb2.Clone();

            // Додаємо новостворений екземпляр на форму
            this.Controls.Add(clonedTextBox);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}