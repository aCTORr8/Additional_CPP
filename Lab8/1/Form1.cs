using System;
using System.Collections;
using System.Windows.Forms;

namespace Labb8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // --- Клас баскетболіста ---
        public class Player
        {
            public int Number { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Position { get; set; }
            public string Team { get; set; }

            public Player(int number, string name, int age, string position, string team)
            {
                Number = number;
                Name = name;
                Age = age;
                Position = position;
                Team = team;
            }

            public override string ToString()
            {
                return $"#{Number} {Name} | Вік: {Age} | Позиція: {Position} | Команда: {Team}";
            }
        }

        // --- Клас BasketballTeam1 (успадковує ArrayList) ---
        public class BasketballTeam1 : ArrayList
        {
            public Player[] PlayersArray { get; set; }

            public BasketballTeam1(int count)
            {
                PlayersArray = new Player[count];
            }
        }

        // --- Клас BasketballTeam2 (реалізує IEnumerable) ---
        public class BasketballTeam2 : IEnumerable
        {
            public Player[] PlayersArray { get; set; }

            public BasketballTeam2(int count)
            {
                PlayersArray = new Player[count];
            }

            public IEnumerator GetEnumerator()
            {
                return PlayersArray.GetEnumerator();
            }
        }

        // --- Клас BasketballTeam3 (реалізує IEnumerable та IEnumerator) ---
        public class BasketballTeam3 : IEnumerable, IEnumerator
        {
            BasketballTeam3[] teamArray;
            int maxPlayers = 0;
            int currentIndex = 0;

            int Number;
            string Name;
            int Age;
            string Position;
            string Team;

            int position = -1;

            public BasketballTeam3(int count)
            {
                teamArray = new BasketballTeam3[count];
                maxPlayers = count;
            }

            public BasketballTeam3(int count, int number, string name, int age, string position, string team)
            {
                teamArray = new BasketballTeam3[count];
                maxPlayers = count;
                Number = number;
                Name = name;
                Age = age;
                Position = position;
                Team = team;
            }

            public BasketballTeam3 this[int index]
            {
                get
                {
                    if (index >= 0 && index < maxPlayers)
                        return teamArray[index];
                    else return null;
                }
                set
                {
                    if (index < maxPlayers)
                        teamArray[index] = value;
                }
            }

            public IEnumerator GetEnumerator()
            {
                foreach (BasketballTeam3 p in teamArray)
                {
                    yield return (IEnumerator)p;
                }
            }

            public object Current
            {
                get
                {
                    if (position >= 0 && position < maxPlayers)
                        return teamArray[position];
                    else return null;
                }
            }

            public bool MoveNext()
            {
                position++;
                return (position < maxPlayers);
            }

            public void Reset()
            {
                position = -1;
            }

            // --- Додати баскетболіста ---
            public void Add(int number, string name, int age, string position, string team, ref int KodError)
            {
                if (currentIndex < maxPlayers)
                {
                    teamArray[currentIndex] = new BasketballTeam3(1);
                    teamArray[currentIndex].Number = number;
                    teamArray[currentIndex].Name = name;
                    teamArray[currentIndex].Age = age;
                    teamArray[currentIndex].Position = position;
                    teamArray[currentIndex].Team = team;
                    currentIndex++;
                    KodError = 0;
                }
                else KodError = 1;
            }

            public override string ToString()
            {
                return $"#{Number} {Name} | Вік: {Age} | Позиція: {Position} | Команда: {Team}";
            }
        }

        // --- Подія натискання кнопки ---
        private void button1_Click(object sender, EventArgs e)
        {
            string ss = "\n Вивід для класу BasketballTeam1 \n\n";
            BasketballTeam1 team1 = new BasketballTeam1(100);

            team1.PlayersArray[0] = new Player(1, "Іван Петренко", 25, "Розігруючий", "Динамо");
            team1.PlayersArray[1] = new Player(3, "Сергій Коваль", 28, "Форвард", "Київ-Баскет");
            team1.PlayersArray[2] = new Player(7, "Олег Шевчук", 22, "Центровий", "Будівельник");

            foreach (Player p in team1.PlayersArray)
            {
                if (p != null) ss += p.ToString() + "\n";
            }

            ss += "\n Вивід для класу BasketballTeam2 \n\n";
            BasketballTeam2 team2 = new BasketballTeam2(100);
            team2.PlayersArray[0] = new Player(9, "Максим Бондар", 24, "Атакувальний захисник", "Дніпро");
            team2.PlayersArray[1] = new Player(10, "Олександр Гнатюк", 26, "Форвард", "Черкаські Мавпи");
            team2.PlayersArray[2] = new Player(18, "Владислав Мороз", 23, "Центровий", "Хімік");

            foreach (Player p in team2.PlayersArray)
            {
                if (p != null) ss += p.ToString() + "\n";
            }

            ss += "\n Вивід для класу BasketballTeam3 \n\n";
            BasketballTeam3 team3 = new BasketballTeam3(100);
            int KodError = 0;

            team3.Add(4, "Євген Павлюк", 23, "Форвард", "Будівельник", ref KodError);
            team3.Add(5, "Марлон Гомес", 21, "Розігруючий", "Динамо", ref KodError);
            team3.Add(6, "Хоакінете", 28, "Центровий", "Київ-Баскет", ref KodError);
            team3.Add(8, "Денис Кузик", 23, "Атакувальний захисник", "Черкаські Мавпи", ref KodError);

            foreach (BasketballTeam3 p in team3)    
            {
                if (p != null) ss += p.ToString() + "\n";
            }

            team3.MoveNext();
            ss += "\n Поточний елемент:\n" + team3.Current?.ToString() + "\n";
            team3.MoveNext();
            ss += "\n Ще раз MoveNext:\n" + team3.Current?.ToString();

            label1.Text = ss;
        }
    }
}
