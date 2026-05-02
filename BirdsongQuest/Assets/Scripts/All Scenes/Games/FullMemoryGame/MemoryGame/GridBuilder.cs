using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    public class GridBuilder : MonoBehaviour
    {
        [SerializeField] private RectTransform grid;
        [SerializeField] private Spawner spawner;

        public void BuildGrid(List<(Bird bird, CardType type)> deck)
        {
            ClearGrid();

            foreach (var (bird, type) in deck)
            {
                var cardPrefab = spawner.SpawnPrefab(grid);
                if (cardPrefab.TryGetComponent(out IMemoryCard card))
                    card.Init(bird, type);
            }

            grid.GetComponent<GridAutoFitBehaviour>()?.Refit();
        }

        public void ClearGrid()
        {
            foreach (Transform child in grid)
                Destroy(child.gameObject);
        }
    }
}
