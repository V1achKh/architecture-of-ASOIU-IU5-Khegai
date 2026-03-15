using System;

Console.WriteLine("Программа вычисления расстояния Дамерау-Левенштейна");

while (true)
{
    Console.Write("\nВведите первую строку (или 'exit' для выхода): ");
    string str1 = Console.ReadLine();

    if (str1 == null || str1.ToLower() == "exit")
    {
        break;
    }

    Console.Write("Введите вторую строку: ");
    string str2 = Console.ReadLine();

    int distance = dist_Leven_Dam(str1, str2);

    if (distance == -1)
    {
        Console.WriteLine("Ошибка вычисления (null строки).");
    }
    else
    {
        Console.WriteLine($"Расстояние Дамерау-Левенштейна: {distance}");
    }
}

/// <summary>
/// Вычисление расстояния Дамерау-Левенштейна
/// </summary>
int dist_Leven_Dam(string str1, string str2)
{
    if (str1 == null || str2 == null)
    {
        return -1;
    }

    int len_str1 = str1.Length;
    int len_str2 = str2.Length;

    if (len_str1 == 0) return len_str2;
    if (len_str2 == 0) return len_str1;


    int[,] Matrix = new int[len_str1 + 1, len_str2 + 1];

    for (int i = 0; i <= len_str1; ++i)
    {
        Matrix[i, 0] = i;
    }
    for (int j = 0; j <= len_str2; ++j)
    {
        Matrix[0, j] = j;
    }

    for (int i = 1; i <= len_str1; ++i)
    {
        for (int j = 1; j <= len_str2; ++j)
        {
            int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;

            int ins = Matrix[i, j - 1] + 1;     
            int del = Matrix[i - 1, j] + 1;      
            int subst = Matrix[i - 1, j - 1] + cost; 

            Matrix[i, j] = Math.Min(Math.Min(ins, del), subst);

            if (i > 1 && j > 1 &&
                s1[i - 1] == s2[j - 2] &&
                s1[i - 2] == s2[j - 1])
            {
                Matrix[i, j] = Math.Min(Matrix[i, j], Matrix[i - 2, j - 2] + cost);
            }
        }
    }

    return Matrix[len_str1, len_str2];
}