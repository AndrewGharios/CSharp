using System;
using System.IO;

namespace Task13_Simple
{
    struct NOTE
    {
        public string lastName;
        public string firstName;
        public string phoneNumber;
        public int day;
        public int month;
        public int year;
    }

    class Program
    {
        static NOTE[] notes = new NOTE[100]; // max 100 entries
        static int count = 0;
        static string dataFile = "notes.txt";

        static void Main()
        {
            LoadFromFile();
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== МЕНЮ ===");
                Console.WriteLine("1. Добавить запись");
                Console.WriteLine("2. Показать все записи (по алфавиту)");
                Console.WriteLine("3. Найти по месяцу рождения");
                Console.WriteLine("4. Выйти");
                Console.Write("Выберите опцию: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddNote(); break;
                    case "2": DisplayAll(); break;
                    case "3": SearchByMonth(); break;
                    case "4": exit = true; break;
                    default: Console.WriteLine("Неверный ввод. Нажмите любую клавишу..."); Console.ReadKey(); break;
                }
            }
        }

        static void LoadFromFile()
        {
            if (File.Exists(dataFile))
            {
                string[] lines = File.ReadAllLines(dataFile);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split('|');
                    if (parts.Length == 6)
                    {
                        NOTE n = new NOTE();
                        n.lastName = parts[0].Trim();
                        n.firstName = parts[1].Trim();
                        n.phoneNumber = parts[2].Trim();
                        if (int.TryParse(parts[3], out int d) && int.TryParse(parts[4], out int m) && int.TryParse(parts[5], out int y))
                        {
                            n.day = d;
                            n.month = m;
                            n.year = y;
                            if (n.day >= 1 && n.day <= 31 && n.month >= 1 && n.month <= 12)
                                notes[count++] = n;
                        }
                    }
                }
                Console.WriteLine($"Загружено {count} записей.");
            }
            else
                Console.WriteLine("Файл notes.txt не найден. Начинаем с пустого списка.");
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static void AddNote()
        {
            Console.Clear();
            if (count >= notes.Length)
            {
                Console.WriteLine("Достигнут максимум записей.");
                Console.ReadKey();
                return;
            }

            NOTE n = new NOTE();
            Console.Write("Фамилия: "); n.lastName = Console.ReadLine();
            Console.Write("Имя: "); n.firstName = Console.ReadLine();
            Console.Write("Телефон: "); n.phoneNumber = Console.ReadLine();


            do { Console.Write("День рождения (1-31): "); }
            while (!int.TryParse(Console.ReadLine(), out n.day) || n.day < 1 || n.day > 31);

            do { Console.Write("Месяц рождения (1-12): "); }
            while (!int.TryParse(Console.ReadLine(), out n.month) || n.month < 1 || n.month > 12);

            Console.Write("Год рождения: ");
            int.TryParse(Console.ReadLine(), out n.year);

            notes[count++] = n;
            Console.WriteLine("Запись добавлена!");
            Console.ReadKey();
        }

        static void DisplayAll()
        {
            Console.Clear();
            if (count == 0)
            {
                Console.
                WriteLine("Нет записей.");
                Console.ReadKey();
                return;
            }

            // Simple bubble sort by last name
            for (int i = 0; i < count - 1; i++)
                for (int j = i + 1; j < count; j++)
                    if (string.Compare(notes[i].lastName, notes[j].lastName) > 0)
                    {
                        NOTE temp = notes[i];
                        notes[i] = notes[j];
                        notes[j] = temp;
                    }

            Console.WriteLine("Список записей (по алфавиту):\n");
            Console.WriteLine($"{"Фамилия",-15} {"Имя",-15} {"Телефон",-15} Дата рождения");
            Console.WriteLine(new string('-', 60));
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"{notes[i].lastName,-15} {notes[i].firstName,-15} {notes[i].phoneNumber,-15} {notes[i].day:D2}.{notes[i].month:D2}.{notes[i].year}");
            }
            Console.ReadKey();
        }

        static void SearchByMonth()
        {
            Console.Clear();
            if (count == 0)
            {
                Console.WriteLine("Нет записей.");
                Console.ReadKey();
                return;
            }

            int month;
            do { Console.Write("Введите месяц (1-12): "); }
            while (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12);

            string monthName = GetMonthName(month);
            Console.WriteLine($"\nЛюди, родившиеся в {monthName}:\n");
            bool found = false;
            for (int i = 0; i < count; i++)
            {
                if (notes[i].month == month)
                {
                    Console.WriteLine($"{notes[i].lastName} {notes[i].firstName} - {notes[i].day:D2}.{notes[i].month:D2}.{notes[i].year} - Тел.: {notes[i].phoneNumber}");
                    found = true;
                }
            }
            if (!found) Console.WriteLine("Таких записей нет.");
            Console.ReadKey();
        }

        static string GetMonthName(int m)
        {
            string[] months = { "январь", "февраль", "март", "апрель", "май", "июнь",
                                "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" };
            return months[m - 1];
        }
    }
}