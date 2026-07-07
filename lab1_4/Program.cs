using System;

class Program
{
    static void Main(string[] args)
    {
        double x;
        double eps;
        
   
        Console.WriteLine("Введите x: ");
        while (!double.TryParse(Console.ReadLine(), out x))
        {
            Console.WriteLine("Введите корректное число!");
        }


        Console.WriteLine("Введите положительное малое число epsilon: ");
        while (!double.TryParse(Console.ReadLine(), out eps) || eps <= 0)
        {
            Console.WriteLine("Введите положительное число!");
        }

        double sum = 0.0;
        double term = 1.0;  
        int n = 0;           

        while (Math.Abs(term) >= eps)
        {
            sum += term;          
            n++;                   
            term *= -x * x / n;     // вычисляем следующий член по формуле term(n+1) = term(n) * (-x^2)/(n+1)
        }

        Console.WriteLine($"Сумма ряда с точностью {eps}: {sum}");
        Console.WriteLine($"Количество учтенных членов: {n}");
        Console.WriteLine($"Точное значение exp(-x^2): {Math.Exp(-x * x)}");
        Console.WriteLine($"Разница: {Math.Abs(sum - Math.Exp(-x * x))}");
    }
}