using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System;

public class RandomSorter
{
    List<Color> colors;
    List<Flask> flasks;

    public RandomSorter(List<Flask> flasks, List<Color> colors)
    {
        this.flasks = new List<Flask>(flasks);
        this.colors = new List<Color>(colors);
        Sort();
    }

    void Sort()
    {   
        flasks.Sort();
        flasks.RemoveLast(2);

        if (flasks.Count != colors.Count)
            throw new UnityException(
                $"Количество колб не равно количеству цветов. Количество колб: {flasks.Count} Количество цветов: {colors.Count}"
            );
        List<Color> sortColors = new List<Color>();
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < colors.Count; j++)
                sortColors.Add(colors[j]);
        sortColors.RandomSort();

        for (int i = 0, j = 0; i < sortColors.Count; )
            flasks[j++].SetColors(sortColors[i++], sortColors[i++], sortColors[i++], sortColors[i++]);
    }
}
