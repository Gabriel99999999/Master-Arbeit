using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings
{
    public class MemorySettings
    {
        public int levelNumberForBestTimeAndNextBird;
        private MemoryLevel _level;

        public MemoryLevel Level => _level;

        public bool nextLevelAddsANewBird;

        public MemorySettings(MemoryLevel level, bool nextLevelAddsANewBird, int levelNumberForBestTimeAndNextBird)
        {
            this.levelNumberForBestTimeAndNextBird = levelNumberForBestTimeAndNextBird;
            _level = level;
            this.nextLevelAddsANewBird = nextLevelAddsANewBird;
        }
    }

    
}
