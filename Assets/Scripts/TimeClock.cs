using System;

using TMPro;
using UnityEngine;

namespace Game.Run
{
    public class TimeClock : MonoBehaviour
    {
        [SerializeField]
        private Transform minuteHand;
        [SerializeField]
        private Transform hourHand;

        [SerializeField]
        private TextMeshPro timeText;

        public void SetCurrentTime(int minuteAngle, float hourAngle, string display)
        {
            minuteHand.eulerAngles = new Vector3(0, 0, -minuteAngle);
            hourHand.eulerAngles = new Vector3(0, 0, -hourAngle);

            timeText.text = display;
        }
    }
}
