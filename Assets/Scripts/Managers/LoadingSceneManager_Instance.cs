using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace NeonBlood
{
    /// <summary>
    /// Clase que controla la pantalla de Cargando
    /// </summary>
    public class LoadingSceneManager_Instance : MonoBehaviour
    {
        [Header("Binding")]
        public static string LoadingScreenSceneName = "LoadingScreen";

        [Header("GameObjects")]
        public Image BackgroundImage;
        public List<Sprite> BackgroundRandomImages;
        public CanvasGroup LoadingProgressBar;
        public Image Fader;

        [Header("Time")]
        public float StartFadeDuration = 0.2f;
        public float ProgressBarSpeed = 2f;
        public float ExitFadeDuration = 0.2f;
        public float LoadCompleteDelay = 0.5f;

        protected AsyncOperation _asyncOperation;
        protected static string _sceneToLoad = "";
        protected float _fadeDuration = 0.5f;
        protected float _fillTarget = 0f;

        protected virtual void Start()
        {
            if (_sceneToLoad != "")
                StartCoroutine(LoadAsynchronously());
        }

        protected virtual void Update()
        {
            LoadingProgressBar.GetComponent<Image>().fillAmount = MMMaths.Approach(LoadingProgressBar.GetComponent<Image>().fillAmount, _fillTarget, Time.deltaTime * ProgressBarSpeed);
        }

        /// <summary>
        /// Call this static method to load a scene from anywhere
        /// </summary>
        /// <param name="sceneToLoad">Level name.</param>
        public static void LoadScene(string sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
            //Application.backgroundLoadingPriority = ThreadPriority.High;
            if (LoadingScreenSceneName != null)
            {
                SceneManager.LoadScene(LoadingScreenSceneName);
                LoadingScreenSceneName = "LoadingScreen";
            }
        }

        /// <summary>
        /// Loads the scene to load asynchronously.
        /// </summary>
        protected virtual IEnumerator LoadAsynchronously()
        {
            // we setup our various visual elements
            LoadingSetup();

            yield return new WaitForSeconds(1);
            System.GC.Collect();
#if UNITY_PS4
            yield return new WaitForSeconds(5);
#endif
            yield return Resources.UnloadUnusedAssets();

            // we start loading the scene
            _asyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);
            _asyncOperation.allowSceneActivation = false;

            // while the scene loads, we assign its progress to a target that we'll use to fill the progress bar smoothly
            while (_asyncOperation.progress < 0.9f)
            {
                _fillTarget = _asyncOperation.progress;
                yield return null;
            }
            // when the load is close to the end (it'll never reach it), we set it to 100%
            _fillTarget = 1f;

            // we wait for the bar to be visually filled to continue
            while (LoadingProgressBar.GetComponent<Image>().fillAmount != _fillTarget)
            {
                yield return null;
            }

            // the load is now complete, we replace the bar with the complete animation
            LoadingComplete();
            yield return new WaitForSeconds(LoadCompleteDelay);

            // we fade to black
            FaderOn(true, ExitFadeDuration);
            yield return new WaitForSeconds(ExitFadeDuration);

            // we switch to the new scene
            _asyncOperation.allowSceneActivation = true;
        }

        /// <summary>
        /// Sets up all visual elements, fades from black at the start
        /// </summary>
        protected virtual void LoadingSetup()
        {
            Fader.gameObject.SetActive(true);
            Fader.color = new Color(0, 0, 0, 1f);
            FaderOn(false, ExitFadeDuration);

            LoadingProgressBar.GetComponent<Image>().fillAmount = 0f;
        }

        /// <summary>
        /// Triggered when the actual loading is done, replaces the progress bar with the complete animation
        /// </summary>
        protected virtual void LoadingComplete()
        {
            StartCoroutine(MMFade.FadeCanvasGroup(LoadingProgressBar, 0.1f, 1f));
        }

        /// <summary>
		/// Fades the fader in or out depending on the state
		/// </summary>
		/// <param name="state">If set to <c>true</c> fades the fader in, otherwise out if <c>false</c>.</param>
		protected virtual void FaderOn(bool state, float duration)
        {
            if (Fader != null)
            {
                Fader.gameObject.SetActive(true);
                if (state)
                    StartCoroutine(MMFade.FadeImage(Fader, duration, new Color(0, 0, 0, 1)));
                else
                    StartCoroutine(MMFade.FadeImage(Fader, duration, new Color(0, 0, 0, 0)));
            }
        }
    }
}