using UnityEngine;

namespace WaterSort
{
    public static class ColorExtensions
    {
        ///<summary>
        ///Сравнивает 2 цвета по RGB-каналам, игнорируя альфа-канал
        ///</summary>
        public static bool EqualsColor(this Color thisColor, Color otherColor)
        {
            thisColor.a = 0;
            otherColor.a = 0;
            return thisColor.Equals(otherColor);
        }
    }
}