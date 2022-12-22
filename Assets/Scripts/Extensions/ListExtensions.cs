using System.Collections.Generic;
using System;

public static class ListExtensions
{
    ///<summary>
    ///Удаляет несколько элементов из списка, начиная с конца
    ///</summary>
    ///<param name="count">Количество элементов, которые необходимо удалить</param>
    ///<exception cref="ArgumentException"></exception>
    public static void RemoveLast<T>(this List<T> thisList, int count)
    {
        if (count <= 0)
            throw new ArgumentException("Количество удаляемых элементов должно быть больше нуля");
        thisList.RemoveRange(thisList.Count - count, count);
    }

    ///<summary>
    ///Сортирует список случайным образом
    ///</summary>
    public static void RandomSort<T>(this List<T> thisList)
    {
        Random rnd = new Random();
        for (int a = thisList.Count - 1; a > 0; a--)
        {
            int b = rnd.Next(a);
            T value = thisList[b];
            thisList[b] = thisList[a];
            thisList[a] = value;
        }
    }
}
