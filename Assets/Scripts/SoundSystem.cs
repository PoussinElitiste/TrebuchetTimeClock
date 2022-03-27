using UnityEngine;

namespace Game.Run
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundSystem : MonoBehaviour
    {
        [SerializeField]
        private AudioClip clip;

        [SerializeField]
        private AudioSource player;

        [SerializeField]
        [Range(0f, 1f)]
        private float volume = 0.5f;

        public void OnTimeModelSwitch()
        {
            player.PlayOneShot(clip, volume);
        }
    }
}

