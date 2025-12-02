using System;
using System.Globalization;
using System.Threading;

class Bisection
{
    static double f(double x)
    {
        return x * x * x - 2 * x - 5;
    }

    static void Main()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        double a = 2.0;
        double b = 3.0;
        double tolerance = 0.0001;
        double c = 0.0;
        int maxIterations = 100;

        Console.WriteLine("=== Метод дiлення навпiл ===");
        Console.WriteLine($"Функцiя: x^3 - 2x - 5. Iнтервал: [{a}, {b}].");

        if (f(a) * f(b) >= 0)
        {
            Console.WriteLine("Метод непридатний: f(a) та f(b) одного знаку.");
            return;
        }

        for (int i = 0; i < maxIterations; i++)
        {
            c = (a + b) / 2;

            if (Math.Abs(f(c)) < tolerance || (b - a) / 2 < tolerance)
            {
                break;
            }

            if (f(c) * f(a) < 0)
            {
                b = c;
            }
            else
            {
                a = c;
            }
        }
        
        Console.WriteLine($"Корiнь знайдено: x = {c:F4}");
        Console.WriteLine($"Значення f(x) у коренi: {f(c):F6}");
        Console.WriteLine("\nPress <ENTER> to continue...");
        Console.ReadLine();
    }
}
