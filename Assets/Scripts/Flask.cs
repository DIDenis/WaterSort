using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class Flask : MonoBehaviour, IPointerClickHandler, IComparable
{
    [SerializeField, Tooltip("Должна ли колба быть пустой?")] bool isEmpty;
    [SerializeField, Tooltip("Необязательный параметр. Помечает колбу, если она находится слева")] 
    bool left;
    [SerializeField] Transform spout;
    [SerializeField] Transform bottom;
    public bool IsEmpty => isEmpty;
    public bool Left => left;
    public Transform Spout => spout;
    public int SlotsCount => slots.Count;

    Game game;
    Solver solver;
    SpriteRenderer spillingWaterPrefab;
    BoxCollider2D boxCollider;
    List<SpriteRenderer> slots;
    SpriteRenderer flaskSprite;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        flaskSprite = GetComponent<SpriteRenderer>();
        InitializeSlots();
    }

    [Inject]
    public void Construct(SpriteRenderer prefab, Solver solver, Game game)
    {
        spillingWaterPrefab = prefab;
        this.solver = solver;
        this.game = game;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        solver.Select(this);
    }

    public bool Take()
    {
        if (isEmpty)
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

    void InitializeSlots()
    {
        if (slots == null)
        {
            slots = new List<SpriteRenderer>();
            for (int i = 0; i < bottom.childCount; i++)
                slots.Add(bottom.GetChild(i).GetComponent<SpriteRenderer>());
        }
    }

    ///<summary>
    ///Устанавливает всем спрайтам-слотам колбы определённые цвета
    ///</summary>
    ///<param name="colors">Количество цветов, равное количеству спрайтов-слотов колбы</param>
    ///<exception cref="ArgumentException"></exception>
    public void SetColors(params Color[] colors)
    {
        InitializeSlots();

        if (colors.Length != slots.Count)
            throw new ArgumentException(
                $@"Передано количество цветов, не равное количеству спрайтов-слотов колбы.
                Количество слотов: {slots.Count}"
                );

        for (int i = 0; i < slots.Count; i++)
            slots[i].color = colors[i];
    }

    ///<returns>Возвращает все цвета колбы в виде списка. Возможен возврат пустого списка</returns>
    public List<Color> GetColors()
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i < slots.Count; i++)
            if (slots[i].color != Color.clear)
                colors.Add(slots[i].color);
        return colors;
    }

    ///<returns>Возвращает верхние одинаковые спрайты-слоты колбы. Возможен возврат пустого списка</returns>
    public List<SpriteRenderer> GetEqualSprites()
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].color != Color.clear)
            {
                if (sprites.Count == 0)
                    sprites.Add(slots[i]);
                else if (slots[i].color == sprites[sprites.Count - 1].color)
                    sprites.Add(slots[i]);
                else
                    break;
            }
        }
        return sprites;
    }

    ///<returns>Возвращает верхние пустые спрайты-слоты колбы. Возможен возврат пустого списка</returns>
    ///<param name="targetCount">Количество слотов, которые необходимо вернуть</param>
    public List<SpriteRenderer> GetClearSprites(int targetCount)
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].color == Color.clear)
            {
                sprites.Add(slots[i]);
                targetCount--;
            }
            if (targetCount == 0)
                break;
        }
        return sprites;
    }

    ///<summary>
    ///Вызывается у колбы, в которую наливают воду
    ///</summary>
    ///<param name="color">Цвет вливаемой воды</param>
    ///<param name="count">Количество спрайтов-слотов, в которые необходимо налить воду</param>
    public IEnumerator Inpour(Color color, int count)
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>(GetClearSprites(count));
        color.a = 0;
        while (sprites[0].color.a < 1)
        {
            color.a += Time.smoothDeltaTime * 2f;
            foreach (SpriteRenderer sprite in sprites)
                sprite.color = color;
            yield return null;
        }
        color.a = 1;
        foreach (SpriteRenderer sprite in sprites)
            sprite.color = color;

        isEmpty = false;
    }

    ///<summary>
    ///Вызывается у колбы, из которой выливают воду
    ///</summary>
    ///<param name="flask">Колба, в которую наливают воду</param>
    public IEnumerator Outpour(Flask flask)
    {
        Vector3 identityPosition = transform.position;
        int scalar = flask.Left ? 1 : -1;
        Position position = new Position(transform.position, flask.Spout.position);
        Rotation rotation = new Rotation(transform.rotation, Quaternion.Euler(0, 0, 60 * scalar));
        identityPosition.y--;
        boxCollider.enabled = false;
        flaskSprite.sortingOrder = 1;

        yield return MoveTo();

        rotation.SetStartAndTarget(transform.rotation, Quaternion.Euler(0, 0, 90 * scalar));
        List<SpriteRenderer> sprites = new List<SpriteRenderer>(GetEqualSprites());
        SpriteRenderer spillingWater = Instantiate(spillingWaterPrefab, transform.position, Quaternion.identity);
        Color color = sprites[0].color;
        spillingWater.color = color;
        StartCoroutine(flask.Inpour(color, sprites.Count));
        float step = 0;
        while (sprites[0].color.a > 0)
        {
            step += Time.smoothDeltaTime * 2;
            transform.rotation = Quaternion.Slerp(rotation.start, rotation.target, step);
            color.a = 1 - step;
            foreach (SpriteRenderer sprite in sprites)
                sprite.color = color;
            yield return null;
        }
        color = Color.clear;
        foreach (SpriteRenderer sprite in sprites)
            sprite.color = color;
        isEmpty = GetColors().Count == 0;
        Destroy(spillingWater.gameObject);

        position.SetStartAndTarget(transform.position, identityPosition);
        rotation.SetStartAndTarget(transform.rotation, Quaternion.identity);

        yield return MoveTo();

        solver.CheckFlask();
        boxCollider.enabled = true;
        flaskSprite.sortingOrder = 0;

        IEnumerator MoveTo()
        {
            float maxDistance = (position.start - position.target).magnitude;
            float distance = maxDistance;
            float step = 0;
            while (distance > .01f)
            {
                step += Time.smoothDeltaTime * 2f;
                transform.SetPositionAndRotation(
                    Vector3.Lerp(position.start, position.target, step),
                    Quaternion.Slerp(rotation.start, rotation.target, step)
                );
                distance = (transform.position - position.target).magnitude;
                yield return null;
            }
            transform.SetPositionAndRotation(position.target, rotation.target);
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
