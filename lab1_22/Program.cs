using System;
using System.Collections.Generic;

namespace lab22
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Задание 22: Формирование списка L = L1 \\ L2 ===\n");
            
            // Example with integers
            Console.WriteLine("Пример с целыми числами:");
            List<int> L1_int = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<int> L2_int = new List<int> { 3, 5, 7, 9, 11, 13 };
            
            List<int> L_int = GetDifference(L1_int, L2_int);
            
            PrintList(L1_int, "L1");
            PrintList(L2_int, "L2");
            PrintList(L_int, "L (L1 \\ L2)");
            
            Console.WriteLine();
            
            // Example with strings
            Console.WriteLine("Пример со строками:");
            List<string> L1_str = new List<string> { "яблоко", "груша", "банан", "апельсин", "манго" };
            List<string> L2_str = new List<string> { "банан", "киви", "манго", "ананас" };
            
            List<string> L_str = GetDifference(L1_str, L2_str);
            
            PrintList(L1_str, "L1");
            PrintList(L2_str, "L2");
            PrintList(L_str, "L (L1 \\ L2)");
            
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
            
            // Interactive version
            RunInteractiveVersion();
        }
        
        // Generic method to get difference between two lists
        static List<T> GetDifference<T>(List<T> L1, List<T> L2)
        {
            List<T> result = new List<T>();
            
            foreach (T element in L1)
            {
                // Check if element is not in L2
                if (!Contains(L2, element))
                {
                    // Check if not already in result (for uniqueness)
                    if (!Contains(result, element))
                    {
                        result.Add(element);
                    }
                }
            }
            
            return result;
        }
        
        // Manual Contains method (since we're not using LINQ)
        static bool Contains<T>(List<T> list, T value)
        {
            foreach (T item in list)
            {
                if (EqualityComparer<T>.Default.Equals(item, value))
                {
                    return true;
                }
            }
            return false;
        }
        
        static void PrintList<T>(List<T> list, string name)
        {
            Console.Write($"{name}: [");
            for (int i = 0; i < list.Count; i++)
            {
                Console.Write(list[i]);
                if (i < list.Count - 1)
                    Console.Write(", ");
            }
            Console.WriteLine("]");
        }
        
        static void RunInteractiveVersion()
        {
            Console.Clear();
            Console.WriteLine("=== Интерактивный режим ===\n");
            
            // Input L1
            Console.Write("Введите элементы списка L1 через пробел: ");
            string[] input1 = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<string> L1 = new List<string>(input1);
            
            // Input L2
            Console.Write("Введите элементы списка L2 через пробел: ");
            string[] input2 = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<string> L2 = new List<string>(input2);
            
            Console.WriteLine("\nИсходные списки:");
            PrintList(L1, "L1");
            PrintList(L2, "L2");
            
            // Get difference
            List<string> L = GetDifference(L1, L2);
            
            Console.WriteLine("\nРезультат (элементы, которые есть в L1, но нет в L2):");
            if (L.Count > 0)
            {
                PrintList(L, "L");
            }
            else
            {
                Console.WriteLine("L: пустой список");
            }
            
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}