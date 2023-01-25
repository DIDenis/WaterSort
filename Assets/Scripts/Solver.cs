using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Zenject;

namespace WaterSort
{
    public class Solver : MonoBehaviour
    {
        Flask selectedFlask;
        List<Flask> transfusionedFlasks = new List<Flask>();
        List<Flask> flasks;
        Game game;

        [Inject]
        public void Construct(Game game, List<Flask> flasks)
        {
            this.game = game;
            this.flasks = flasks;
        }

        public void Select(Flask flask)
        {
            // Выбранная колба не берётся, если в неё уже вливают воду
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
                    List<ColorBlock> equalSprites = selectedFlask.GetEqualBlocks();
                    List<ColorBlock> clearSprites = flask.GetClearBlocks();
                    bool equalColors = equalSprites[0].Color.EqualsColor(flask.GetEqualBlocks()[0].Color);
                    bool overflowCheck = equalSprites.Count <= clearSprites.Count;

                    if (equalColors && overflowCheck)
                    {
                        StartCoroutine(TransfusionInto());
                        selectedFlask = null;
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

        ///<summary>Проверяет количество полностью заполненных колб одним цветом.</summary>
        ///<remarks>При заполнении всех колб уровень завершается</remarks>
        public void CheckFlask()
        {
            int completedFlasksCount = 0;
            int emptyFlasks = 0;
            for (int i = 0; i < flasks.Count; i++)
            {
                if (flasks[i].IsFilled)
                    completedFlasksCount++;
                else if (flasks[i].IsEmpty)
                    emptyFlasks++;
            }

            if (completedFlasksCount == flasks.Count - emptyFlasks)
                game.CompleteLevel();
        }
    }
}