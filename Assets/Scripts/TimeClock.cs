using System;
using System.Collections;
using System.Collections.Generic;
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

        private float day;


        private void Start()
        {
            DateTime dt = DateTime.Now;
            Debug.Log(dt.ToString("G"));
        }

        // Update is called once per frame
        void Update()
        {
            DateTime dt = DateTime.Now;

            day += Time.deltaTime / REAL_SECOND_PER_INGAME_DAY;
            float dayNormalized = day % 1f;
            float rotationDegreesperDay = 360f;
            hourHand.eulerAngles = new Vector3(0, 0, -dayNormalized * rotationDegreesperDay);

            float hoursPerDay = 24f;
            minuteHand.eulerAngles = new Vector3(0, 0, -dayNormalized * rotationDegreesperDay * hoursPerDay);

            timeText.text = dt.Minute.ToString("00") + ":"+ dt.Hour.ToString("00") + ":" + dt.Second.ToString("00");
        }
    }
}
