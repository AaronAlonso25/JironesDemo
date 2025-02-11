using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    /// <summary>
    /// Clase que inicia las cartelas del principio del juego
    /// </summary>
    public class InitCanvas : MonoBehaviour
    {
        [Header("Logos")]        
        public List<VideoClip> Videos_720;
        public List<VideoClip> Videos_1080;
        public List<VideoClip> Videos_SteamDeck;

        public string NextLevel = "MainMenu";

        private VideoPlayer video;

        IEnumerator Start()
        {
            this.video = this.GetComponent<VideoPlayer>();
            yield return StartCoroutine(this.PlayVideo(0));

            MenuCanvas.Instance.PlayLevelDirectly(this.NextLevel);
        }

        void Update()
        {
            if(Input.anyKeyDown)
            {
                this.video.Stop();
                MenuCanvas.Instance.PlayLevelDirectly(this.NextLevel);
            }
        }

        private IEnumerator PlayVideo(int indexVideo)
        {
#if UNITY_STANDALONE && STEAMWORKS_NET
            this.video.clip = this.Videos_1080[indexVideo];
            if (SteamAchievements.Instance)
            {
                if (Steamworks.SteamAPI.IsSteamRunning() && Steamworks.SteamUtils.IsSteamRunningOnSteamDeck())
                    this.video.clip = this.Videos_SteamDeck[indexVideo];
            }
#elif UNITY_PS4 || UNITY_PS5 || UNITY_GAMECORE
            this.video.clip = this.Videos_1080[indexVideo];
#elif UNITY_SWITCH
            this.video.clip = this.Videos_720[indexVideo];
#else
            this.video.clip = this.Videos_1080[indexVideo];
#endif

            this.video.Prepare();

            while (!this.video.isPrepared)
                yield return null;

            this.video.Play();

            while (this.video.isPlaying)
                yield return null;

            this.video.Stop();
        }
    }
}