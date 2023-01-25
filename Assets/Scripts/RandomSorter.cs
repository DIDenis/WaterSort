using UnityEngine;
using System.Collections.Generic;

namespace WaterSort
{
    public class RandomSorter
    {
        List<Color> colors = new List<Color>();

        public RandomSorter(Settings settings)
        {
            int index = 0;
            for (int i = 0; i < settings.colorBlocksCount; i++)
            {
                if (index == settings.colors.Length)
                    index = 0;
                colors.Add(settings.colors[index++]);
            }
            colors.RandomSort();
        }

        public Color[] GetColors(int count)
        {
            if (this.colors.Count == 0)
                throw new UnityException("Попытка использовать пустой список цветов");

            List<Color> colors = new List<Color>();
            for (int i = 0; i < count; i++)
            {
                colors.Add(this.colors[0]);
                this.colors.RemoveAt(0);
            }
            return colors.ToArray();
        }

        [System.Serializable]
        public struct Settings
        {
            ///<summary>Количество цветных блоков всех заполняемых колб на уровне</summary>
            [Tooltip("Количество цветных блоков всех заполняемых колб на уровне")]
            public int colorBlocksCount;

            ///<summary>Все возможные на уровне цвета</summary>
            [Tooltip("Все возможные на уровне цвета")]
            public Color[] colors;
        }
    }
}