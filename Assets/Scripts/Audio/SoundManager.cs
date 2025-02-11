using UnityEngine;
using System.Collections;

namespace NeonBlood
{
    /// <summary>
    /// Clase que controla la musica juego
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance = null;

        //Volumen de la musica de Ambiente
        public float SoundVolume = 0.5f;

        //Atributo propio
        private AudioSource myAudio;

        public bool IsEnd { get; set; }

        void Awake()
        {
            Instance = this;
            this.myAudio = this.GetComponent<AudioSource>();
        }

        public void PlaySound(AudioClip clip)
        {
            if (this.IsEnd)
                return;
            
            this.myAudio.clip = clip;
            this.myAudio.loop = false;
            this.myAudio.Play();
        }

        public void PlaySoundLoop(AudioClip clip)
        {
            if (this.IsEnd)
                return;

            this.myAudio.clip = clip;
            this.myAudio.loop = true;
            this.myAudio.Play();
        }

        public void PlaySound(AudioClip clip, float volume = 1, bool loop = false)
        {
            if (this.IsEnd)
                return;

            this.myAudio.clip = clip;
            this.myAudio.volume = volume;
            this.myAudio.loop = loop;
            this.myAudio.Play();
        }

        public void StopSound()
        {
            this.myAudio.Stop();
        }

        public void ResumeSound()
        {
            this.myAudio.Play();
        }

        public void MuteAudio()
        {
            this.myAudio.volume = 0;
        }        
    }
}