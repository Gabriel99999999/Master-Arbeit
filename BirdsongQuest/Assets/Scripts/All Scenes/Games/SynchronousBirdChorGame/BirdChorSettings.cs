using Assets.Scripts.All_Scenes.Games.FullMemoryGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.All_Scenes.Games.BirdChorGame
{
    public class BirdChorSettings
    {
        public int levelNumberForBestTimeAndNextBird;
        private BirdChorLevel _level;
        private BirdChorMode _mode;

        public BirdChorLevel Level => _level;
        public BirdChorMode Mode => _mode;

        public bool nextLevelAddsANewBird;
        public BirdChorSettings(BirdChorLevel level, BirdChorMode mode, bool nextLevelAddsANewBird, int levelNumberForBestTimeAndNextBird)
        {
            this.levelNumberForBestTimeAndNextBird = levelNumberForBestTimeAndNextBird;
            _level = level;
            _mode = mode;
            this.nextLevelAddsANewBird = nextLevelAddsANewBird;
        }
    }
}
