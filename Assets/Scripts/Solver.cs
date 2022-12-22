using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Zenject;

public class Solver : MonoBehaviour
{
    Flask selectedFlask;
    List<Flask> transfusionedFlasks;
    List<Flask> flasks;
    Game game;

    void Awake()
    {
        transfusionedFlasks = new List<Flask>();
    }

    [Inject]
    public void Construct(Game game, List<Flask> flasks)
    {
        this.game = game;
        this.flasks = flasks;
    }

    public void Select(Flask flask)
    {
        foreach (Flask transfusionedFlask in transfusionedFlasks)
        {
            if (transfusionedFlask == flask && selectedFlask == null)
                return;
        }

        if (selectedFlask == null)
        {
            if (flask.Take())
                selectedFlask = flask;
        }
        else if (selectedFlask == flask)
        {
            flask.Put();
            selectedFlask = null;
        }
        else
            ValidateInput();

        void ValidateInput()
        {
            if (flask.IsEmpty)
            {
                StartCoroutine(TransfusionInto());
                selectedFlask = null;
            }
            else
            {
                List<SpriteRenderer> equalSprites = selectedFlask.GetEqualSprites();
                List<SpriteRenderer> clearSprites = flask.GetClearSprites(flask.SlotsCount);
                bool equalColors = equalSprites[0].color.EqualsColor(flask.GetColors()[0]);
                bool overflowCheck = equalSprites.Count <= clearSprites.Count;

                if (equalColors && overflowCheck)
                {
                    StartCoroutine(TransfusionInto());
                    selectedFlask = null;
                }
                else
                {
                    selectedFlask.Put();
                    flask.Take();
                    selectedFlask = flask;
                }
            }            

            IEnumerator TransfusionInto()
            {
                transfusionedFlasks.Add(flask);
                yield return selectedFlask.Outpour(flask);
                transfusionedFlasks.Remove(flask);
            }
        }
    }

    ///<summary>
    ///Проверяет количество полностью заполненных колб одним цветом.
    ///При заполнении всех колб уровень завершается
    ///</summary>
    public void CheckFlask()
    {
        int completedFlasksCount = 0;
        int emptyFlasks = 0;
        for (int i = 0; i < flasks.Count; i++)
        {
            if (flasks[i].GetEqualSprites().Count == flasks[i].SlotsCount)
                completedFlasksCount++;
            if (flasks[i].IsEmpty)
                emptyFlasks++;
        }

        if (completedFlasksCount == flasks.Count - emptyFlasks)
            game.CompleteLevel();
    }
}
