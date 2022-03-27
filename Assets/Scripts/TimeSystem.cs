using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Run
{
    public class TimeSystem : MonoBehaviour
    {
        private const float MILLISEC = 1000f;
        private const float MIN_TO_ANGLE = 360f / 60f;
        private const float HOUR_TO_ANGLE = 360f / 24f * 2f; //affichage sur 12h
        private const float MIN_TO_HOUR_ANGLE = MIN_TO_ANGLE / 12f; // minutes pour 1heure 

        [SerializeField]
        private GameObject clockworkPrefab;
        private TimeClock clockwork;

        [SerializeField]
        private int minuteAngle;
        [SerializeField]
        private float hourAngle;

        [SerializeField]
        private PlayerInput playerInput;
        private InputAction m_switchAction;

        [SerializeField]
        public UnityEvent onSwitchModel;

        private void Start()
        {
            var clockInstance = Instantiate(clockworkPrefab);
            clockInstance.transform.parent = transform;
            clockwork = clockInstance.GetComponent<TimeClock>();

            SetCurrentTime();
            // sync to system time update interval
            var syncStart = (MILLISEC - DateTime.Now.Millisecond) / MILLISEC;
            Debug.Log("start sync : " + syncStart.ToString() + "s");
            InvokeRepeating(nameof(SetCurrentTime), syncStart, 1f);

            m_switchAction = playerInput.currentActionMap["Switch"];
            m_switchAction.performed += context =>
            {
                clockwork.SwitchModel();
                onSwitchModel?.Invoke();
            };
        }

        private void SetCurrentTime()
        {
            DateTime dt = DateTime.Now;
            minuteAngle = Mathf.RoundToInt(dt.Minute * MIN_TO_ANGLE);
            hourAngle = Mathf.Round((dt.Hour * HOUR_TO_ANGLE) + (dt.Minute * MIN_TO_HOUR_ANGLE));
            clockwork.SetCurrentTime(minuteAngle, hourAngle, dt.ToString("HH:mm:ss"));
        }
    }
}
