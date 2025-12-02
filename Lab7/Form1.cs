using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace LABB7
{
    public partial class Form1 : Form
    {
        public static string PlanText = "";
        public static string ResultText = "";
        public static string nl = "\r\n";
        public static int step = 1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Кнопка "Start" запускає нашу демонстрацію ООП.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Очищення полів
            PlanText = "--- ПЛАН ДЕМОНСТРАЦІЇ ---" + nl;
            ResultText = "--- РЕЗУЛЬТАТ ВИКОНАННЯ ---" + nl;
            step = 1;

            // --- Демонстрація Інкапсуляції та Конструктора ---
            PlanText += "1. Створити 'UserAccount' (Конструктор)." + nl;
            UserAccount user = new UserAccount("Alice", "password123");

            PlanText += "2. Отримати логін (Інкапсуляція - 'get')." + nl;
            ResultText += $"[Крок 2] Логін користувача: {user.Login}" + nl;

            PlanText += "3. Спроба змінити логін (Інкапсуляція - 'private set')." + nl;
            ResultText += $"[Крок 3] Логін не можна змінити ззовні." + nl;

            // --- Демонстрація Успадкування та Поліморфізму ---
            PlanText += "4. Створити 'PremiumAccount' (Успадкування, Конструктор)." + nl;
            PremiumAccount vipUser = new PremiumAccount("Bob", "qwerty");

            PlanText += "5. Викликати 'DisplayInfo' у 'PremiumAccount' (Поліморфізм - 'override')." + nl;
            vipUser.DisplayInfo();

            // --- Демонстрація Перевантаження ---
            PlanText += "6. Викликати 'ChangePassword(string)' у 'vipUser' (Перевантаження)." + nl;
            vipUser.ChangePassword("new_strong_password");

            PlanText += "7. Викликати 'ChangePassword(string, bool)' у 'vipUser' (Перевантаження)." + nl;
            vipUser.ChangePassword("another_pass", true);

            // --- Демонстрація Поліморфізму через базовий клас ---
            PlanText += "8. Створити список акаунтів (поліморфізм)." + nl;
            UserAccount[] accounts = new UserAccount[2];
            accounts[0] = user;
            accounts[1] = vipUser;

            PlanText += "9. Викликати 'DisplayInfo' для кожного акаунта в циклі." + nl;
            foreach (UserAccount acc in accounts)
            {
                acc.DisplayInfo();
            }

            label1.Text = PlanText;
            label2.Text = ResultText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
    }

    /// <summary>
    /// 1. БАЗОВИЙ КЛАС
    /// </summary>
    public class UserAccount
    {
        private string _password;
        public string Login { get; private set; }

        public UserAccount(string login, string password)
        {
            Login = login;
            _password = password;
            Form1.ResultText += $"[Крок {Form1.step++}] Створено UserAccount: {Login}" + Form1.nl;
        }

        public virtual void DisplayInfo()
        {
            Form1.ResultText += $"[Крок {Form1.step++}] **Звичайний акаунт**: {Login}" + Form1.nl;
        }

        public void ChangePassword(string newPassword)
        {
            _password = newPassword;
            Form1.ResultText += $"[Крок {Form1.step++}] Пароль для {Login} змінено." + Form1.nl;
        }

        public void ChangePassword(string newPassword, bool notifyByEmail)
        {
            _password = newPassword;
            if (notifyByEmail)
            {
                Form1.ResultText += $"[Крок {Form1.step++}] Пароль для {Login} змінено. Надіслано E-mail." + Form1.nl;
            }
            else
            {
                Form1.ResultText += $"[Крок {Form1.step++}] Пароль для {Login} змінено." + Form1.nl;
            }
        }
    }

    /// <summary>
    /// 2. ПОХІДНИЙ КЛАС
    /// </summary>
    public class PremiumAccount : UserAccount
    {
        public DateTime ExpiryDate { get; set; }

        public PremiumAccount(string login, string password) : base(login, password)
        {
            ExpiryDate = DateTime.Now.AddYears(1);
            Form1.ResultText += $"[Інфо] Акаунт підвищено до Premium. Діє до {ExpiryDate.ToShortDateString()}" + Form1.nl;
        }

        public override void DisplayInfo()
        {
            Form1.ResultText += $"[Крок {Form1.step++}] **PREMIUM АКАУНТ**: {Login} (VIP)" + Form1.nl;
        }
    }
}
