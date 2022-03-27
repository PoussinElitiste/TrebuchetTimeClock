using UnityEngine;

namespace Game.Run
{
    public class SoundSystem : MonoBehaviour
    {
        [SerializeField]
        private AudioClip clip;

        [SerializeField]
        private AudioSource player;

        [SerializeField]
        [Range(0f, 1f)]
        private float volume = 0.5f;

        private void Start()
        {
            player.PlayOneShot(clip, volume);
        }

        public void OnTimeModelSwitch()
        {
            player.PlayOneShot(clip, volume);
        }
    }
}

