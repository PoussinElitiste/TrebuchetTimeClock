using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Run
{
    // ALIGNED WITH CLOCK HAND MODELS!
    enum HandleType
    {
        Cube = 0,
        Cylinder,
        Tetra,
        Triangle
    }

    public class TimeClock : MonoBehaviour
    {
        [System.Serializable]
        struct ClockHandModel
        {
            public Mesh minute;
            public Mesh hour;
        }

        [SerializeField]
        private Transform minuteHand;
        private MeshFilter minuteHandMesh;

        [SerializeField]
        private Transform hourHand;
        private MeshFilter hourHandMesh;

        [SerializeField]
        private TextMeshPro timeText;

        [SerializeField]
        private HandleType defaultClockModel = HandleType.Cube;
        [SerializeField]
        private List<ClockHandModel> clockHandModels = new ();

        private void Start()
        {
            minuteHandMesh = minuteHand.GetComponentInChildren<MeshFilter>();
            hourHandMesh = hourHand.GetComponentInChildren<MeshFilter>();

            minuteHandMesh.sharedMesh = clockHandModels[(int)defaultClockModel].minute;
            hourHandMesh.sharedMesh = clockHandModels[(int)defaultClockModel].hour;
        }

        public void SetCurrentTime(int minuteAngle, float hourAngle, string display)
        {
            minuteHand.eulerAngles = new Vector3(0, 0, -minuteAngle);
            hourHand.eulerAngles = new Vector3(0, 0, -hourAngle);

            timeText.text = display;
        }
    }
}
