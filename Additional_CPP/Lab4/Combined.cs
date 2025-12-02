using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb4
{
    class Program
    {

        public static double f(double x)
        {
            return x * x - 4;
        }

        public static double fp(double x)
        {
            return 2 * x;
        }

        public static double f2p(double x)
        {
            return 2;
        }
        public static List<Tuple<double, double>> FindIntervals(double start, double end, double step)
        {
            List<Tuple<double, double>> intervals = new List<Tuple<double, double>>();
            double x_prev = start;
            double f_prev = f(start);

            Console.WriteLine("\n--- Результати Табулювання ---");
            Console.WriteLine($"|     X     |     f(X)    |");

            Console.WriteLine($"| {start,9:F3} | {f_prev,9:F3} |");

            if (step <= 0)
            {
                Console.WriteLine("Помилка: Крок табулювання має бути додатним.");
                return intervals;
            }

            double correction = step * 0.000001;
            for (double x = start + step; x <= end + correction; x += step)
            {
                double current_x = Math.Min(x, end);
                double f_curr = f(current_x);
                Console.WriteLine($"| {current_x,9:F3} | {f_curr,9:F3} |");

                if (f_prev * f_curr <= 0)
                {
                    intervals.Add(new Tuple<double, double>(x_prev, current_x));
                }

                x_prev = current_x;
                f_prev = f_curr;

                if (current_x == end) break;
            }
            Console.WriteLine("-----------------------------");

            return intervals;
        }
        public static void MethodBisection(double a, double b, double Eps)
        {
            double c;
            int Lich = 0;

            if (f(a) * f(b) > 0)
            {
                Console.WriteLine("Помилка: f(a)*f(b) > 0. Уточнiть iнтервал.");
                return;
            }

            while (Math.Abs(b - a) >= Eps)
            {
                c = 0.5 * (a + b);
                Lich++;

                if (Math.Abs(f(c)) < Eps)
                    break;

                if (f(a) * f(c) < 0)
                {
                    b = c;
                }
                else
                {
                    a = c;
                }
            }

            double root = (a + b) / 2;
            int precision = (int)Math.Ceiling(-Math.Log10(Eps));

            Console.WriteLine($"\nМЕТОД ДIЛЕННЯ НАВПIЛ (МДН):");
            Console.WriteLine(string.Format("Корiнь: x = {0:F" + precision + "}", root));
            Console.WriteLine($"Кiлькiсть iтерацiй (подiлiв): {Lich}");
        }
        public static void MethodNewton(double a, double b, double Eps, int Kmax)
        {
            double x = (f(b) * f2p(b) > 0) ? b : a;
            int iterations = 0;

            if (f(x) * f2p(x) < 0)
            {
                Console.WriteLine("Увага: Умова збiжностi МН (f(x0) * f''(x0) > 0) не виконана.");
            }

            for (int i = 1; i <= Kmax; i++)
            {
                iterations = i;

                if (Math.Abs(fp(x)) < double.Epsilon)
                {
                    Console.WriteLine("Помилка: Похiдна f'(x) близька до нуля. Метод Ньютона незастосовний.");
                    return;
                }

                double Dx = f(x) / fp(x);
                x = x - Dx;

                if (Math.Abs(Dx) <= Eps)
                {
                    int precision = (int)Math.Ceiling(-Math.Log10(Eps));

                    Console.WriteLine($"\nМЕТОД НЬЮТОНА (МН):");
                    Console.WriteLine(string.Format("Корiнь: x = {0:F" + precision + "}", x));
                    Console.WriteLine($"Кiлькiсть iтерацiй: {iterations}");
                    return;
                }
            }

            Console.WriteLine($"Помилка: Корiнь не знайдено за {Kmax} iтерацiй.");
        }

        static void Main(string[] args)
        {
            double startInterval, endInterval, step;
            double Eps;
            int Kmax = 100;

            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("Розв'язання нелiнiйного рiвняння x^2 - 4 = 0");
            Console.WriteLine("ДОДАТКОВI ЗАВДАННЯ 1 та 2");

            try
            {
                Console.Write("\nВведiть початкове значення X для табулювання (напр., -5): ");
                startInterval = Convert.ToDouble(Console.ReadLine());
                Console.Write("Введiть кiнцеве значення X для табулювання (напр., 5): ");
                endInterval = Convert.ToDouble(Console.ReadLine());
                Console.Write("Введiть крок табулювання (напр., 0,5): ");
                step = Convert.ToDouble(Console.ReadLine());
                Console.Write("Введiть необхiдну точнiсть Eps (напр., 1e-5): ");
                Eps = Convert.ToDouble(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Помилка вводу. Перезапустiть програму та введiть дiйснi числа.");
                Console.ReadLine();
                return;
            }

            List<Tuple<double, double>> intervals = FindIntervals(startInterval, endInterval, step);

            if (intervals.Count == 0)
            {
                Console.WriteLine("Коренi (змiна знаку) в заданому дiапазонi не знайдено. Спробуйте змiнити межi або логiку.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nЗНАЙДЕНI IНТЕРВАЛИ (де f(a)*f(b) <= 0):");
            for (int i = 0; i < intervals.Count; i++)
            {
                Console.WriteLine($"{i + 1}. [{intervals[i].Item1}, {intervals[i].Item2}]");
            }

            Console.Write("Оберiть номер iнтервалу для розрахунку: ");
            if (!int.TryParse(Console.ReadLine(), out int choiceInterval) || choiceInterval < 1 || choiceInterval > intervals.Count)
            {
                Console.WriteLine("Некоректний вибiр. Використано перший знайдений iнтервал.");
                choiceInterval = 1;
            }

            double a = intervals[choiceInterval - 1].Item1;
            double b = intervals[choiceInterval - 1].Item2;
            Console.WriteLine($"\nОбрано iнтервал: [{a}, {b}]");

            Console.WriteLine("\n--- ВИБIР МЕТОДУ РОЗВ'ЯЗАННЯ ---");
            Console.WriteLine("1. Метод Дiлення Навпiл (МДН)");
            Console.WriteLine("2. Метод Ньютона (МН)");
            Console.Write("Ваш вибi (1 або 2): ");

            string choiceMethod = Console.ReadLine();

            if (choiceMethod == "1")
            {
                MethodBisection(a, b, Eps);
            }
            else if (choiceMethod == "2")
            {
                MethodNewton(a, b, Eps, Kmax);
            }
            else
            {
                Console.WriteLine("Некоректний вибiр методу.");
            }

            Console.WriteLine("\nНатиснiть Enter для виходу.");
            Console.ReadLine();
        }
    }
}