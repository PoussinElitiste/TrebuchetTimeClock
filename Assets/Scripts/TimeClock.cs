using System;

using TMPro;
using UnityEngine;

namespace Game.Run
{
    public class TimeClock : MonoBehaviour
    {
        private const float REAL_SECOND_PER_INGAME_DAY = 60f;
        [SerializeField]
        private Transform minuteHand;
        [SerializeField]
        private Transform hourHand;

        [SerializeField]
        private TextMeshPro timeText;

        private const float MILLISEC = 1000f;
        private const float MIN_TO_ANGLE = 360f / 60f;
        private const float HOUR_TO_ANGLE = 360f / 24f * 2f; //affichage sur 12h
        private const float MIN_TO_HOUR_ANGLE = MIN_TO_ANGLE / 12f; // minutes pour 1heure 

        [SerializeField]
        private int minuteAngle;
        [SerializeField]
        private float hourAngle;

        private void Start()
        {
            SetCurrentTime();
            // sync to system time update interval
            var syncStart = (MILLISEC - DateTime.Now.Millisecond)/ MILLISEC;
            Debug.Log("start sync : " + syncStart.ToString() + "s");
            InvokeRepeating(nameof(SetCurrentTime), syncStart, 1f);
        }

        private void SetCurrentTime()
        {
            DateTime dt = DateTime.Now;
            
            minuteAngle = Mathf.RoundToInt(dt.Minute * MIN_TO_ANGLE);
            hourAngle = Mathf.Round((dt.Hour * HOUR_TO_ANGLE) + (dt.Minute * MIN_TO_HOUR_ANGLE));
            minuteHand.eulerAngles = new Vector3(0, 0, -minuteAngle);
            hourHand.eulerAngles = new Vector3(0, 0, -hourAngle);

            timeText.text = dt.ToString("HH:mm:ss");
        }
    }
}
