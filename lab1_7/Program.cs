using System;

class Program
{
    static void Main()
    {
        Console.Write("Введите количество строк n: ");
        int n = int.Parse(Console.ReadLine());
        
        Console.Write("Введите количество столбцов m: ");
        int m = int.Parse(Console.ReadLine());
        
        double[,] a = new double[n, m];
        
        Console.WriteLine("Введите элементы матрицы:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                Console.Write($"a[{i},{j}] = ");
                a[i, j] = double.Parse(Console.ReadLine());
            }
        }

        //вивод матр
        Console.WriteLine();
        for (int i = 0; i < n; i++)
        {   
            for (int j = 0; j < m; j++)
            {
                Console.Write(a[i, j] + " ");
            }
            Console.WriteLine();
        }

        
        double[] minRow = new double[n];
        int[] minCol = new int[n];
        
        for (int i = 0; i < n; i++)
        {
            minRow[i] = a[i, 0];
            minCol[i] = 0;
            
            for (int j = 1; j < m; j++)
            {
                if (a[i, j] < minRow[i])
                {
                    minRow[i] = a[i, j];
                    minCol[i] = j;
                }
            }
        }
        
        double maxMin = minRow[0];
        int maxRow = 0;
        
        for (int i = 1; i < n; i++)
        {
            if (minRow[i] > maxMin)
            {
                maxMin = minRow[i];
                maxRow = i;
            }
        }
        
        int maxCol = minCol[maxRow];
        
        Console.WriteLine($"\nИндексы искомого элемента: [{maxRow}, {maxCol}]");
        Console.WriteLine($"Значение элемента: {a[maxRow, maxCol]}");
    }
}