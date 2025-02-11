using UnityEngine;
using System.Collections;

namespace NeonBlood
{
    /// <summary>
    /// Enumerado de FadeIn, FadeOut
    /// </summary>
    public enum FadeType { IN, OUT }

    /// <summary>
    /// Clase que controla la musica juego
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        //Instancia para el singleton
        public static MusicManager Instance = null;

        //Volumen de la musica de Ambiente
        public float MusicVolume = 0.75f;

        //Atributo propio
        private AudioSource myAudio;

        void Awake()
        {
            Instance = this;
            this.myAudio = this.GetComponent<AudioSource>();
        }

        public void FadeIn(float timer)
        {
            StartCoroutine(this.FadeSound(timer, FadeType.IN));
        }

        public void FadeOut(float timer)
        {
            StartCoroutine(this.FadeSound(timer, FadeType.OUT));
        }

        /// <summary>
        /// Metodo que realiza el FadeSound de la musica
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="fadetype"></param>
        /// <returns></returns>
        public IEnumerator FadeSound(float timer, FadeType fadetype)
        {
            if (fadetype == FadeType.OUT && this.myAudio.volume == 0)
                yield break;

            float start = (fadetype == FadeType.IN) ? 0.0f : this.MusicVolume;
            float end = (fadetype == FadeType.IN) ? this.MusicVolume : 0.0f;

            float i = 0.0f;
            float step = 1.0f / timer;

            while (i <= 1.0f)
            {
                i += step * Time.deltaTime;
                this.myAudio.volume = Mathf.Lerp(start, end, i);

                yield return null;
            }
        }

        /// <summary>
        /// Metodo que realiza el FadeSound de la musica a un volumen especifico
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="fadetype"></param>
        /// <returns></returns>
        public IEnumerator FadeSound(float timer, FadeType fadetype, float volume)
        {
            float start = (fadetype == FadeType.IN) ? 0.0f : volume;
            float end = (fadetype == FadeType.IN) ? volume : 0.0f;

            float i = 0.0f;
            float step = 1.0f / timer;

            while (i <= 1.0f)
            {
                i += step * Time.deltaTime;
                this.myAudio.volume = Mathf.Lerp(start, end, i);

                yield return null;
            }
        }

        /// <summary>
        /// Metodo que cambia y reproduce un sonido
        /// </summary>
        /// <param name="mySound"></param>
        public void ChangeAndPlaySound(AudioClip mySound)
        {
            this.myAudio.clip = mySound;
            this.myAudio.enabled = true;
            this.myAudio.Play();
            this.myAudio.volume = 0;
        }

        public void StopSound()
        {
            this.myAudio.Stop();
        }

        public void ResumeSound()
        {
            this.myAudio.Play();
        }
    }
}