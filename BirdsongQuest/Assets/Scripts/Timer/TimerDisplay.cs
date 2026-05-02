using Assets.Scripts.Timer;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField] private BaseTimer timer;
        [SerializeField] private TextMeshProUGUI timerText;

        private void OnEnable()
        {
            if (timer != null)
                timer.OnTimeChanged += UpdateDisplay;
        }

        private void OnDisable()
        {
            if (timer != null)
                timer.OnTimeChanged -= UpdateDisplay;
        }


        private void UpdateDisplay(float currentTime)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
