using System;

class Program
{
    static void Main()
    {
        Console.Write("Введите предложение: ");
        string sentence = Console.ReadLine();
        Console.Write("Введите длину слов: ");
        int len = int.Parse(Console.ReadLine());
        
        string[] allWords = sentence.Split(' ');
        
        // Count words of required length
        int count = 0;
        for (int i = 0; i < allWords.Length; i++)
        {
            if (allWords[i].Length == len)
                count++;
        }
        
        if (count < 2)
        {
            Console.WriteLine("Недостаточно слов нужной длины");
            return;
        }
        
        // Collect words of required length
        string[] words = new string[count];
        int index = 0;
        for (int i = 0; i < allWords.Length; i++)
        {
            if (allWords[i].Length == len)
            {
                words[index] = allWords[i];
                index++;
            }
        }
        
        // Find pair with maximum distance
        string word1 = "", word2 = "";
        int maxDist = -1;
        
        for (int i = 0; i < words.Length - 1; i++)
        {
            for (int j = i + 1; j < words.Length; j++)
            {
                int dist = 0;
                for (int k = 0; k < len; k++)
                {
                    if (words[i][k] != words[j][k])
                        dist++;
                }
                
                if (dist > maxDist)
                {
                    maxDist = dist;
                    word1 = words[i];
                    word2 = words[j];
                }
            }
        }
        
        Console.WriteLine($"Пара слов: \"{word1}\" и \"{word2}\"");
        Console.WriteLine($"Максимальное расстояние: {maxDist}");
    }
}