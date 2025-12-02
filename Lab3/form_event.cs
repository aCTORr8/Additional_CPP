using System;
using System.Windows.Forms;

public class MyForm : Form 
{
    private Button myButton;

    public MyForm()
    {
        this.Text = "Додаткове завдання ЛР3";
        this.Size = new System.Drawing.Size(300, 200);

        myButton = new Button();
        myButton.Text = "Натисни мене!";
        myButton.Location = new System.Drawing.Point(10, 10);
        myButton.Size = new System.Drawing.Size(150, 40);

        myButton.Click += new EventHandler(Button_Click);

        this.Controls.Add(myButton);
    }
    private void Button_Click(object sender, EventArgs e)
    {
        myButton.Text = "Кнопку натиснуто!";
        Console.WriteLine("Кнопка була натиснута.");
    }

    [STAThread]
    static void Main()
    {
        Application.Run(new MyForm());
    }
}
