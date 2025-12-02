using System;
using System.Windows.Forms;

namespace Labb9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Novak_My_Indexer";
        }

        // КЛАС BASKETBALLPLAYER
        // Клас описує дані одного гравця.
        public class BasketballPlayer
        {
            public string FullName { get; set; }  // Ім'я гравця
            public int JerseyNumber { get; set; } // Номер на майці
            public string Position { get; set; }  // Позиція (PG, SG, SF, PF, C)
            public double PPG { get; set; }       // Очки за гру (Points Per Game)

            public BasketballPlayer(string name, int number, string pos, double ppg)
            {
                FullName = name;
                JerseyNumber = number;
                Position = pos;
                PPG = ppg;
            }

            // Перевизначення методу ToString для зручного виводу в список
            public override string ToString()
            {
                return $"#{JerseyNumber} {FullName} ({Position}) - {PPG} очк/гру";
            }
        }

        // КЛАС BASKETBALLTEAM (з Індексатором)
        // Цей клас виступає контейнером для гравців і містить логіку перевірки.
        public class BasketballTeam
        {
            private BasketballPlayer[] roster; // Приватний масив для збереження даних
            public int Length { get; private set; } // Розмір команди

            // Код помилки: 0 - Ок, 1 - Помилка індексу, 2 - Помилка даних гравця
            public int ErrorCode { get; private set; }

            public BasketballTeam(int size)
            {
                roster = new BasketballPlayer[size];
                Length = size;
                ErrorCode = 0;
            }

            // ОГОЛОШЕННЯ ІНДЕКСАТОРА
            // Дозволяє звертатися до об'єкта класу як до масиву: team[i]
            public BasketballPlayer this[int index]
            {
                // Аксесор GET: читання даних
                get
                {
                    if (OkIndex(index))
                    {
                        ErrorCode = 0;
                        return roster[index];
                    }
                    else
                    {
                        ErrorCode = 1; // Код 1: Вихід за межі масиву
                        return null;
                    }
                }

                // Аксесор SET: запис даних
                set
                {
                    // 1. Спочатку перевіряємо правильність індексу
                    if (!OkIndex(index))
                    {
                        ErrorCode = 1; // Код 1: Неправильний індекс
                        return;
                    }

                    // 2. Потім перевіряємо дані самого гравця (бізнес-логіка)
                    // Номер гравця має бути від 0 до 99, ім'я не може бути порожнім
                    if (value == null || value.JerseyNumber < 0 || value.JerseyNumber > 99 || string.IsNullOrEmpty(value.FullName))
                    {
                        ErrorCode = 2; // Код 2: Некоректні дані
                        return; // Не записуємо в масив
                    }

                    // Якщо все добре — записуємо
                    roster[index] = value;
                    ErrorCode = 0;
                }
            }

            // Допоміжний метод для перевірки меж масиву
            private bool OkIndex(int index)
            {
                return index >= 0 && index < Length;
            }
        }

        // ОБРОБНИК КНОПКИ
        private void button1_Click(object sender, EventArgs e)
        {
            // Створює команду на 5 місць (стартова п'ятірка)
            BasketballTeam bulls = new BasketballTeam(5);
            string logMessage = "";

            // --- Створення гравців ---
            // Коректні гравці (Легендарний склад Chicago Bulls)
            BasketballPlayer p1 = new BasketballPlayer("Michael Jordan", 23, "SG", 30.1);
            BasketballPlayer p2 = new BasketballPlayer("Scottie Pippen", 33, "SF", 16.1);
            BasketballPlayer p3 = new BasketballPlayer("Dennis Rodman", 91, "PF", 7.3);

            // Некоректний гравець (Номер 999 неможливий у баскетболі)
            BasketballPlayer badP1 = new BasketballPlayer("Bad Player", 999, "C", 2.0);

            // --- Використання ІНДЕКСАТОРА для додавання ---

            // 1. Додаємо успішно
            bulls[0] = p1;
            logMessage += LogResult("1", bulls.ErrorCode, p1.FullName);

            // 2. Додаємо успішно
            bulls[1] = p2;
            logMessage += LogResult("2", bulls.ErrorCode, p2.FullName);

            // 3. Спроба додати некоректного гравця (має повернути помилку даних)
            bulls[2] = badP1;
            logMessage += LogResult("3", bulls.ErrorCode, badP1.FullName);

            // 4. Спроба додати за межі масиву (індекс 10)
            bulls[10] = p3;
            logMessage += LogResult("4", bulls.ErrorCode, "Unknown");

            // 5. Виправляє: записує Родмана на правильне місце
            bulls[2] = p3;
            logMessage += LogResult("5 (Виправлено)", bulls.ErrorCode, p3.FullName);

            // Вивід логу помилок у label1
            label1.Text = logMessage;

            // Вивід фінального складу команди у label2
            string rosterList = "Склад команди:\n";
            for (int i = 0; i < bulls.Length; i++)
            {
                if (bulls[i] != null)
                    rosterList += $"\n{i + 1}. {bulls[i].ToString()}";
                else
                    rosterList += $"\n{i + 1}. --- Порожньо ---";
            }
            label2.Text = rosterList;
        }

        // Допоміжний метод для формування тексту повідомлень
        private string LogResult(string step, int err, string name)
        {
            if (err == 0) return $"\n Крок {step}: Гравця {name} додано успішно.";
            if (err == 1) return $"\n Крок {step}: ПОМИЛКА! Неправильний індекс масиву.";
            if (err == 2) return $"\n Крок {step}: ПОМИЛКА! Некоректні дані гравця {name}.";
            return "\n Невідома помилка.";
        }

        private void label2_Click(object sender, EventArgs e)
        {
         
        }
    }
}