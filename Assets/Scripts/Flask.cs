using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Zenject;

namespace WaterSort
{
    public class Flask : MonoBehaviour, IPointerClickHandler, IComparable
    {
        ///<summary>При отключении параметра колба не заполняется. По умолчанию включено</summary>
        [Tooltip("При отключении параметра колба не заполняется. По умолчанию включено")]
        [SerializeField] bool fill = true;

        ///<summary>Необязательный параметр. Помечает колбу, если она находится слева</summary>
        [Tooltip("Необязательный параметр. Помечает колбу, если она находится слева")]
        [SerializeField] bool left;
        [SerializeField] Transform spout;

        public bool IsEmpty
        {
            get => GetClearBlocks().Count == colorBlocks.Length;
        }
        public bool IsFilled
        {
            get => GetClearBlocks().Count == 0;
        }
        public bool Left => left;
        public Transform Spout => spout;

        Game game;
        Solver solver;
        SpriteRenderer spillingWaterPrefab;
        ColorBlock[] colorBlocks;

        [Inject]
        public void Construct(SpriteRenderer prefab, Solver solver, Game game, RandomSorter sorter)
        {
            colorBlocks = GetComponentsInChildren<ColorBlock>();
            spillingWaterPrefab = prefab;
            this.solver = solver;
            this.game = game;
            if (fill)
                SetColors(sorter);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            solver.Select(this);
        }

        public bool Take()
        {
            if (IsEmpty)
                return false;
            var position = transform.position;
            position.y++;
            transform.position = position;
            return true;
        }

        public void Put()
        {
            var position = transform.position;
            position.y--;
            transform.position = position;
        }

        ///<summary>Устанавливает всем цветовым блокам колбы определённые цвета</summary>
        public void SetColors(RandomSorter sorter)
        {
            Color[] colors = sorter.GetColors(colorBlocks.Length);
            for (int i = 0; i < colorBlocks.Length; i++)
                colorBlocks[i].Color = colors[i];
        }

        ///<returns>Возвращает верхние одинаковые цветовые блоки колбы. Возможен возврат пустого списка</returns>
        public List<ColorBlock> GetEqualBlocks()
        {
            List<ColorBlock> blocks = new List<ColorBlock>();
            for (int i = 0; i < colorBlocks.Length; i++)
            {
                if (!colorBlocks[i].IsClear)
                {
                    if (blocks.Count == 0)
                        blocks.Add(colorBlocks[i]);
                    else if (colorBlocks[i].Color == blocks[blocks.Count - 1].Color)
                        blocks.Add(colorBlocks[i]);
                    else
                        break;
                }
            }
            return blocks;
        }

        ///<returns>Возвращает нижние пустые цветовые блоки колбы. Возможен возврат пустого списка</returns>
        ///<param name="targetCount">Количество блоков, которые необходимо вернуть</param>
        public List<ColorBlock> GetClearBlocks(int targetCount = int.MaxValue)
        {
            List<ColorBlock> blocks = new List<ColorBlock>();
            for (int i = colorBlocks.Length - 1; i >= 0; i--)
            {
                if (colorBlocks[i].IsClear)
                {
                    blocks.Add(colorBlocks[i]);
                    targetCount--;
                }
                if (targetCount == 0)
                    break;
            }
            return blocks;
        }

        ///<summary>Вызывается у колбы, в которую наливают воду</summary>
        ///<param name="color">Цвет вливаемой воды</param>
        ///<param name="count">Количество спрайтов-слотов, в которые необходимо налить воду</param>
        public void Inpour(Color color, int count)
        {
            List<ColorBlock> blocks = new List<ColorBlock>(GetClearBlocks(count));
            color.a = 0;
            foreach (ColorBlock block in blocks)
                StartCoroutine(block.SetColorAsync(color, (float alpha) => alpha < 1, 2));
        }

        ///<summary>Вызывается у колбы, из которой выливают воду</summary>
        ///<param name="flask">Колба, в которую наливают воду</param>
        public IEnumerator Outpour(Flask flask)
        {
            Vector3 identityPosition = transform.position;
            identityPosition.y--;
            int scalar = flask.Left ? 1 : -1;
            Position position = new Position(transform.position, flask.Spout.position);
            Rotation rotation = new Rotation(transform.rotation, Quaternion.Euler(0, 0, 60 * scalar));
            enabled = false;

            yield return MoveTo();

            rotation.SetStartAndTarget(transform.rotation, Quaternion.Euler(0, 0, 90 * scalar));
            List<ColorBlock> blocks = new List<ColorBlock>(GetEqualBlocks());
            SpriteRenderer spillingWater = Instantiate(spillingWaterPrefab, transform.position, Quaternion.identity);
            Color color = blocks[0].Color;
            spillingWater.color = color;
            flask.Inpour(color, blocks.Count);

            foreach (ColorBlock block in blocks)
                StartCoroutine(block.SetColorAsync(block.Color, (float alpha) => alpha > 0, -2));

            float time = Time.time;
            while (transform.rotation != rotation.target)
            {
                transform.rotation = Quaternion.Slerp(rotation.start, rotation.target, (Time.time - time) * 2f);
                yield return null;
            }

            Destroy(spillingWater.gameObject);

            position.SetStartAndTarget(transform.position, identityPosition);
            rotation.SetStartAndTarget(transform.rotation, Quaternion.identity);

            yield return MoveTo();

            solver.CheckFlask();
            enabled = true;

            IEnumerator MoveTo()
            {
                float maxDistance = (position.start - position.target).magnitude;
                float distance = maxDistance;
                float step = 0;
                while (distance > 0)
                {
                    step += Time.smoothDeltaTime * 2f;
                    transform.SetPositionAndRotation(
                        Vector3.Lerp(position.start, position.target, step),
                        Quaternion.Slerp(rotation.start, rotation.target, step)
                    );
                    distance = (transform.position - position.target).magnitude;
                    yield return null;
                }
            }
        }

        public int CompareTo(object o)
        {
            if (o is Flask flask) return IsEmpty.CompareTo(flask.IsEmpty);
            else throw new ArgumentException(
                $"Попытка сравнения объекта типа Flask c объектом типа {o.GetType()}"
            );
        }

        struct Position
        {
            public Vector3 start;
            public Vector3 target;

            public Position(Vector3 start, Vector3 target)
            {
                this.start = start;
                this.target = target;
            }

            public void SetStartAndTarget(Vector3 start, Vector3 target)
            {
                this.start = start;
                this.target = target;
            }
        }
        struct Rotation
        {
            public Quaternion start;
            public Quaternion target;

            public Rotation(Quaternion start, Quaternion target)
            {
                this.start = start;
                this.target = target;
            }

            public void SetStartAndTarget(Quaternion start, Quaternion target)
            {
                this.start = start;
                this.target = target;
            }
        }
    }
}