using System;
using System.Collections;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    public interface IMemoryCard
    {
        string BirdId { get; }
        CardType Type { get; }
        bool IsMatched { get; }

        void Init(Bird bird, CardType type);
        void MarkAsFound();

        void Flip(bool showFront);

        void AddFoundCounter();

        void MarkBirdAsFoundForCollectionBook();

        //public static event Action<IMemoryCard> OnCardClickedGlobal;
    }
}
