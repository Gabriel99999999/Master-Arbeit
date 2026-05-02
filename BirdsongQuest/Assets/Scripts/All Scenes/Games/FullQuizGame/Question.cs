using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame
{
    public class Question
    {
        public Bird Bird { get; private set; }
        public string question { get; private set; }
        public QuestionType questionType { get; private set; }

        public AudioClip call { get; private set; }

        public Question(QuestionType questionType, Bird bird)
        {
            this.questionType = questionType;

            if (questionType == QuestionType.Audio)
            {
                question = "Welcher Vogel singt hier?";
            }
            else
            {
                question = "Erkennst du diesen Vogel?";
            }

            int randomCallIndex = UnityEngine.Random.Range(0, bird.calls.Length);
            call = bird.calls[randomCallIndex];

            this.Bird = bird;
        }
    }
}
