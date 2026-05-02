using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.All_Scenes.Games.TestScene
{
    public class MyLogger : MonoBehaviour
    {
        
       
        public static MyLogger Instance { get; private set; }

        [Header("Configuration")]

        string fileName = $"BirdQuest_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";


        private DateTime? _codeStartTimeUtc;
        private StreamWriter _writer;
        private string _csvPath;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeFile();

            var fs = new FileStream(_csvPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            _writer = new StreamWriter(fs, encoding: new UTF8Encoding(false))
            {
                AutoFlush = true
            };
        }

        private void OnDestroy()
        {
            if (Instance != this) return;

            _writer.WriteLine($"[{UtcNowStamp()}] game was closed");
            _writer.Flush();

            try
            {
                _writer?.Flush();
                _writer?.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        private static string UtcNowStamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }

        //Method for every Game to log the start
        public void LogStartGame(string Game, string levelButtonNumber, string levelNumber)
        {
            _writer.WriteLine($"[{UtcNowStamp()}] START Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}");
            _writer.Flush();
        }

        //For the Memory Game
        public void LogMemoryGameMove(string Game, string levelButtonNumber, string levelNumber, string currentMoveNumber, string foundPair, bool correct, string movesSinceTheLastPairWasFound, string currentTimeFromStopwatch, string timeSinceTheLastPairWasFound)
        {
            _writer.WriteLine($"[{UtcNowStamp()}] During Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, Game Move: {currentMoveNumber}, FoundPair: {foundPair}, Correct: {correct.ToString()}, movesSinceTheLastPairWasFound: {movesSinceTheLastPairWasFound}, currentTimeFromStopWatch: {currentTimeFromStopwatch}, timeSinceTheLastPairWasFound: {timeSinceTheLastPairWasFound}");
            _writer.Flush();
        }

        public void LogMemoryEndGame(string Game, string levelButtonNumber, string levelNumber, string currentMoveNumber, string timeNeededForTheLevel)
        {
            _writer.WriteLine($"[{UtcNowStamp()}] END Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, CurrentMoveNumber: {currentMoveNumber}, TimeNeededForTheLevel: {timeNeededForTheLevel}");
            _writer.Flush();
        }


        //For the Quiz Game and for the Test Quizes (beginning and end Test);
        public void LogQuizGameQuestion(string Game, string levelButtonNumber, string levelNumber, string questionNumber,  string questionedBirdSong, string questionedBird, string[] answerPossibilities, string givenAnswer, bool wasAnswerCorrect, string timeNeededForThisQuestion)
        {
            string answerPossibilitiesStr = $"answerPossibility1: {answerPossibilities[0]}";
            for (int i = 1; i < answerPossibilities.Length; i++)
            {
                answerPossibilitiesStr += $", answerPossibility{i + 1}: {answerPossibilities[i]}";
            }
            _writer.WriteLine($"[{UtcNowStamp()}] Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, QuestionNumber: {questionNumber}, questionedBirdSong: {questionedBirdSong}, questionedBird: {questionedBird}, answerPossibilities: {answerPossibilitiesStr}, givenAnswer: {givenAnswer}, wasAnswerCorrect: {wasAnswerCorrect}, timeNeededForThisAnswer: {timeNeededForThisQuestion}");
            _writer.Flush();
        }

        public void LogQuizEndGame(string Game, string levelButtonNumber, string levelNumber, string timeNeededForTheLevel, string amountOfQuestions, string correctAnswers, string incorrectAnswers, string successrate, string levelPassed)
        {
            _writer.WriteLine($"[{UtcNowStamp()}] END Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, totalTimeNeeded: {timeNeededForTheLevel}, amountOfQuestions: {amountOfQuestions}, correctAnswers: {correctAnswers}, incorrectAnswers: {incorrectAnswers}, successrate: {successrate}, levelPassed: {levelPassed}");
            _writer.Flush();
        }

        public void LogBirdChorSyncQuestion(string Game, string levelButtonNumber, string levelNumber, string questionNumber, string[] singingBirds, string[] answerPossibilities, string[] givenAnswers, bool[] wasAnswerCorrect, string timeNeededForTheQuestion)
        {
            //array hat fix sachen drinnen
            string singingBirdsStr = $"singingBird1: {singingBirds[0]}";
            for(int i = 1; i< singingBirds.Length; i++)
            {
                singingBirdsStr += $", singingBird{i+1}: {singingBirds[i]}";
            }

            string answerPossibilitiesStr = $"answerPossibility1: {answerPossibilities[0]}";
            for (int i = 1; i < answerPossibilities.Length; i++)
            {
                answerPossibilitiesStr += $", answerPossibility{i + 1}: {answerPossibilities[i]}";
            }

            //array könnte auch leer sein
            string givenAnswerStr = "";
            if (givenAnswers.Length > 0)
                givenAnswerStr = $"givenAnswer1: {givenAnswers[0]} wasCorrect: {wasAnswerCorrect[0]}";
            else
                givenAnswerStr = $"givenAnswers: None";

            for (int i = 1; i < givenAnswers.Length; i++)
            {
                givenAnswerStr += $", givenAnswer{i + 1}: {givenAnswers[i]}  wasCorrect: {wasAnswerCorrect[i]}";
            }
            _writer.WriteLine($"[{UtcNowStamp()}] Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, QuestionNumber: {questionNumber}, {singingBirdsStr}, {answerPossibilitiesStr}, {givenAnswerStr}, timeNeededForTheQuestion: {timeNeededForTheQuestion}");
            _writer.Flush();
        }

        public void LogBirdChorSyncEnd(string Game, string levelButtonNumber, string levelNumber, string timeNeededForTheLevel, string achievedPoints, string achieveablePoints, string successrate, string levelPassed)
        {
            _writer.WriteLine($"[{UtcNowStamp()}] END Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, TimeNeededForTheLevel: {timeNeededForTheLevel}, AchievedPoints: {achievedPoints}, AchieveablePoints: {achieveablePoints}, Successrate: {successrate}, levelPassed: {levelPassed}");
            _writer.Flush();

        }

        public void LogBirdChorOrderQuestion(string Game, string levelButtonNumber, string levelNumber, string questionNumber, string[] singingBirdsInCorrectOrder, (Bird bird, bool wasCorrect)[] orderWhichWasCommited, string timeNeededForTheQuestion)
        {
            //array hat fix sachen drinnen
            string correctOrderStr = $"singingBird1: {singingBirdsInCorrectOrder[0]}";
            for (int i = 1; i < singingBirdsInCorrectOrder.Length; i++)
            {
                correctOrderStr += $", singingBird{i + 1}: {singingBirdsInCorrectOrder[i]}";
            }

            (Bird bird, bool wasCorrect) item = orderWhichWasCommited[0];
            string oderWhichWasCommitedStr = $"Bird1: {item.bird.displayName} wasCorrect: {item.wasCorrect}";
            for (int i = 1; i < orderWhichWasCommited.Length; i++)
            {
                item = orderWhichWasCommited[i];
                oderWhichWasCommitedStr += $", Bird{i+1}: {item.bird.displayName} wasCorrect: {item.wasCorrect}";
            }

            _writer.WriteLine($"[{UtcNowStamp()}] Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, QuestionNumber: {questionNumber}, {correctOrderStr}, {oderWhichWasCommitedStr}, timeNeededForTheQuestion: {timeNeededForTheQuestion}");
            _writer.Flush();
        }

        public void LogBirdChorOrderEnd(string Game, string levelButtonNumber, string levelNumber, string timeNeededForTheLevel, string achievedPoints, string achieveablePoints, string successrate, string levelPassed)
        {
            _writer.WriteLine($"[{UtcNowStamp()}] END Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, TimeNeededForTheLevel: {timeNeededForTheLevel}, AchievedPoints: {achievedPoints}, AchieveablePoints: {achieveablePoints}, Successrate: {successrate}, levelPassed: {levelPassed}");
            _writer.Flush();

        }

        public void LogFindeDieAmselQuestion(string Game, string levelButtonNumber, string levelNumber, string questionNumber, string[] birdsWhichWereSelectable, string questionedBird, string givenAnswer, string wasAnswerCorrect, string timeNeededForTheQuestion)
        {
            string selectableBirdsStr = $"selectableBird1: {birdsWhichWereSelectable[0]}";
            for (int i = 1; i < birdsWhichWereSelectable.Length; i++)
            {
                selectableBirdsStr += $", selectableBird{i + 1}: {birdsWhichWereSelectable[i]}";
            }
            _writer.WriteLine($"[{UtcNowStamp()}] Game: {Game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, QuestionNumber: {questionNumber}, {selectableBirdsStr}, questionedBird: {questionedBird}, givenAnswer: {givenAnswer}, wasAnswerCorrect: {wasAnswerCorrect}, timeNeededForTheQuestion: {timeNeededForTheQuestion}");
            _writer.Flush();
        }

        public void LogFindeDieAmselEnd(string game, string levelButtonNumber, string levelNumber, string timeNeededForTheLevel, string amountOfQuestions, string richtigeAnswersCounter, string falseAnswersCounter, string successrate, string levelPassed)
        {
            _writer.WriteLine($"[{UtcNowStamp()}] END Game: {game}, LevelButtonNumber: {levelButtonNumber}, Level: {levelNumber}, TimeNeededForTheLevel: {timeNeededForTheLevel}, amountOfQuestions: {amountOfQuestions}, richtigeAnswersCounter: {richtigeAnswersCounter}, falseAnswersCounter: {falseAnswersCounter}, successrate: {successrate}, levelPassed: {levelPassed}");
            _writer.Flush();
        }

        private void InitializeFile()
        {
            _csvPath = Path.Combine(Application.persistentDataPath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(_csvPath)!);

            var fs = new FileStream(_csvPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            _writer = new StreamWriter(fs, encoding: new UTF8Encoding(false))
            {
                AutoFlush = true
            };

            //var writeHeader = !File.Exists(_csvPath) || new FileInfo(_csvPath).Length == 0;

            /*if (writeHeader)
            {
                _writer.WriteLine("timestamp,Game,GameMode,levelNumber,value,attempt_index,attempts_remaining,time_since_level_start,system_type");
                _writer.Flush();
            }*/
        }
    }
}
