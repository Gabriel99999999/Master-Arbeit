using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    public static class GridAutoSizer
    {
        /// Findet für itemCount die beste (cols, rows)-Kombi, setzt eine quadratische cellSize
        /// UND richtet das Grid zentriert aus (symmetrische Padding-Ränder).
        public static void FitSquareCells(
            RectTransform container,
            GridLayoutGroup grid,
            int itemCount,
            float spacing = 20f,
            Vector2? minPadding = null)
        {
            if (container == null || grid == null || itemCount <= 0)
                return;

            // Basis-Setup
            var padMin = minPadding ?? new Vector2(20, 20);
            grid.spacing = new Vector2(spacing, spacing);
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

            // ACHTUNG: container.rect ist erst korrekt, wenn das Layout einmal durch ist
            // (deshalb rufen wir Fit NICHT im selben Frame wie die Instantiation auf).
            float W = Mathf.Max(0, container.rect.width);
            float H = Mathf.Max(0, container.rect.height);
            if (W <= 0f || H <= 0f) return;

            int bestCols = 1;
            int bestRows = itemCount;
            float bestCell = -1f;

            // brute-force über mögliche Spaltenanzahlen
            for (int cols = 1; cols <= itemCount; cols++)
            {
                int rows = Mathf.CeilToInt(itemCount / (float)cols);

                float availW = W - padMin.x * 2f - grid.spacing.x * (cols - 1);
                float availH = H - padMin.y * 2f - grid.spacing.y * (rows - 1);
                if (availW <= 0 || availH <= 0) continue;

                float maxCell = Mathf.Floor(Mathf.Min(availW / cols, availH / rows)); // quadratisch & int
                if (maxCell > bestCell)
                {
                    bestCell = maxCell;
                    bestCols = cols;
                    bestRows = rows;
                }
            }

            // Fallback (kleine Container)
            if (bestCell < 1f) bestCell = 1f;

            // Grid anwenden
            grid.constraintCount = bestCols;
            grid.cellSize = new Vector2(bestCell, bestCell);

            // Zentrierung: verbleibenden Platz gleichmäßig als Padding verteilen
            float usedW = bestCols * bestCell + grid.spacing.x * (bestCols - 1);
            float usedH = bestRows * bestCell + grid.spacing.y * (bestRows - 1);

            float leftRight = Mathf.Max(padMin.x, Mathf.Floor((W - usedW) * 0.5f));
            float topBottom = Mathf.Max(padMin.y, Mathf.Floor((H - usedH) * 0.5f));

            grid.padding.left = (int)leftRight;
            grid.padding.right = (int)leftRight;
            grid.padding.top = (int)topBottom;
            grid.padding.bottom = (int)topBottom;
        }
    }
}
