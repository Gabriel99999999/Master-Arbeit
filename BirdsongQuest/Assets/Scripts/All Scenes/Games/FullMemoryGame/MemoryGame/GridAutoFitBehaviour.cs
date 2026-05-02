using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridAutoFitBehaviour : UIBehaviour
    {
        public RectTransform container;      // i. d. R. das gleiche RectTransform wie dieses
        public GridLayoutGroup grid;         // GridLayoutGroup auf demselben Objekt
        public float spacing = 20f;
        public Vector2 minPadding = new Vector2(30, 30);

        // Quelle für die Item-Anzahl (meist die ChildCount von gridRoot)
        public int ItemCountOverride = -1;   // <0 = nimm childCount

        protected override void Awake()
        {
            if (!container) container = GetComponent<RectTransform>();
            if (!grid) grid = GetComponent<GridLayoutGroup>();
        }

        protected override void Start()
        {
            Refit();
        }

        // Wird von Unity aufgerufen, wenn sich die Rect-Abmessungen ändern (Fenstergröße, Layout, DPI…)
        protected override void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled) return;
            Refit();
        }

        public void Refit()
        {
            if (!container || !grid) return;

            int count = ItemCountOverride >= 0 ? ItemCountOverride : container.childCount;
            // Ein Frame warten, falls die Kinder gerade erst erzeugt wurden
            StartCoroutine(RefitNextFrame(count));
        }

        
        private System.Collections.IEnumerator RefitNextFrame(int count)
        {
            yield return null; // Layout & rect erst aktualisieren lassen
            GridAutoSizer.FitSquareCells(container, grid, count, spacing, minPadding);
        }
    }
}
