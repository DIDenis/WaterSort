using UnityEngine;
using System.Collections;
using System;

namespace WaterSort
{
    public class ColorBlock : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;

        ///<remarks>При установке альфа-канала на 0 RGB-каналы также устанавливаются на 0</remarks>
        public Color Color
        {
            get
            {
                Validate();
                return spriteRenderer.color;
            }
            set
            {
                Validate();
                Color color = value;
                color.a = Mathf.Clamp(color.a, 0, 1);
                if (color.a == 0)
                    color = Color.clear;
                spriteRenderer.color = color;
            }
        }
        public bool IsClear
        {
            get
            {
                Validate();
                return spriteRenderer.color == Color.clear;
            }
        }

        void Validate()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        ///<summary>Устанавливает цвет с асинхронной модификацией альфа-канала</summary>
        ///<param name="color">Устанавливаемый цвет</param>
        ///<param name="predicate">Выражение для модификации альфа канала</param>
        ///<param name="speed">Скорость модификации (value/sec). При отрицательном значении альфа-канал уменьшается</param>
        public IEnumerator SetColorAsync(Color color, Predicate<float> predicate, float speed)
        {
            while (predicate(Color.a))
            {
                color.a += Time.smoothDeltaTime * speed;
                Color = color;
                yield return null;
            }
        }
    }
}