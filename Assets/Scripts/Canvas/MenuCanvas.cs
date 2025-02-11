using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using MoreMountains.Tools;

namespace NeonBlood
{
    /// <summary>
    /// Clase que controla todos los canvas
    /// </summary>
    public class MenuCanvas : MMSingleton<MenuCanvas>
    {
        //Canvas propio
        private Canvas myCanvas;

        //Imagen de fundido
        public Image FaderImage;

        //Tiempo de fundido 
        public float FadeTime = 1f;

        void Start()
        {
            base.Awake();
            this.myCanvas = this.GetComponent<Canvas>();

            if (this.FaderImage)
                this.Fade(0, " ", true);
        }

        void Update()
        {
            //if(Input.GetKeyDown(KeyCode.R))
            // {
            // DataManager.Instance.ResetData();
            // this.PlayLevel("Intro");
            //}
        }

        /// <summary>
        /// Metodo que pausa el juego
        /// </summary>
        public void Pause(bool pause)
        {
            Time.timeScale = (pause) ? 0.0f : 1.0f;
        }

        /// <summary>
        /// Metodo para iniciar un nivel
        /// </summary>
        /// <param name="level"></param>
        public void PlayLevel(string level)
        {
            this.Fade(1, level, true);
        }

        /// <summary>
        /// Metodo para iniciar un nivel directamente
        /// </summary>
        /// <param name="level"></param>
        public void PlayLevelDirectly(string level)
        {
            this.Fade(1, level, false);
        }

        /// <summary>
        /// Metodo para iniciar un nivel con opciones
        /// </summary>
        /// <param name="level"></param>
        /// <param name="loading"></param>
        public void PlayLevelOptions(string level, bool loading = true)
        {
            this.Fade(1, level, loading);
        }

        /// <summary>
        /// Metodo para reiniciar el nivel
        /// </summary>
        public void RebootLevel()
        {
            this.Fade(1, SceneManager.GetActiveScene().name, false);
        }

        /// <summary>
        /// Metodo para cambiar entre canvas
        /// </summary>
        public void ChangeCanvas(Canvas myFutureCanvas)
        {
            this.myCanvas.enabled = false;
            myFutureCanvas.enabled = true;
        }

        /// <summary>
        /// Metodo para hacer un fundido en la pantalla
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="nameLevel"></param>
        /// <param name="loading"></param>
        private void Fade(int alpha, string nameLevel, bool loading)
        {
            this.FaderImage.gameObject.SetActive(true);
            StartCoroutine(MMFade.FadeImage(this.FaderImage, this.FadeTime, new Color(0, 0, 0, alpha)));

            if (alpha == 1)
            {
                StartCoroutine(this.PlayLevelCo(this.FadeTime, nameLevel, loading));
            }
            else
            {
                if (MusicManager.Instance)
                    StartCoroutine(MusicManager.Instance.FadeSound(this.FadeTime, FadeType.IN));
                if (AmbientMusicManager.Instance)
                    StartCoroutine(AmbientMusicManager.Instance.FadeSound(this.FadeTime, FadeType.IN));
            }
        }

        /// <summary>
        /// Metodo para cargar un nivel pasado un tiempo determinado
        /// </summary>
        /// <param name="time"></param>
        /// <param name="nameLevel"></param>
        /// <param name="loading"></param>
        /// <returns></returns>
        private IEnumerator PlayLevelCo(float time, string nameLevel, bool loading)
        {
            if (MusicManager.Instance)
                StartCoroutine(MusicManager.Instance.FadeSound(this.FadeTime, FadeType.OUT));
            if (AmbientMusicManager.Instance)
                StartCoroutine(AmbientMusicManager.Instance.FadeSound(this.FadeTime, FadeType.OUT));

            yield return new WaitForSeconds(time);

            if (loading)
                LoadingSceneManager_Instance.LoadScene(nameLevel);
            else
                SceneManager.LoadScene(nameLevel);
        }

        /// <summary>
        /// Metodo para realizar un FadeIn / FadeOut
        /// </summary>
        public IEnumerator FadeInFadeOut(float timeDelay)
        {
            this.FaderImage.gameObject.SetActive(true);
            yield return StartCoroutine(MMFade.FadeImage(this.FaderImage, this.FadeTime, new Color(0, 0, 0, 1)));

            yield return new WaitForSeconds(timeDelay);

            this.FaderImage.gameObject.SetActive(true);
            yield return StartCoroutine(MMFade.FadeImage(this.FaderImage, this.FadeTime, new Color(0, 0, 0, 0)));
        }

        /// <summary>
        /// Metodo para que sea accesible desde los UnityEvents
        /// </summary>
        public void FadeInOutFunction(float timeDelay)
        {
            StartCoroutine(FadeInFadeOut(timeDelay));
        }
    }
}