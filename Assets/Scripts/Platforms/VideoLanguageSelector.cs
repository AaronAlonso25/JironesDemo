using UnityEngine;
using UnityEngine.Video;
using System.Linq;
using System.Collections.Generic;

namespace NeonBlood
{
    [System.Serializable]
    public class VideoLanguageInfo
    {
        /**
         * zh-Hans
         * zh-Hant
         * en-US
         * fr-FR
         * de-DE
         * it-IT
         * ja-JP
         * ko-KR
         * pt-PT
         * es-ES
         * sv-SE
         * tr-TR
         */
        public string Language;        
        public VideoClip Video_720;
        public VideoClip Video_1080;
        public VideoClip Video_SteamDeck;
    }

    public class VideoLanguageSelector : MonoBehaviour
    {
        public List<VideoLanguageInfo> VideosLanguage;

        void Start()
        {
            VideoLanguageInfo vli = null;

            string language = DataManager.Instance.CurrentLanguage;
            if (this.VideosLanguage.Any(x => x.Language == language))
                vli = this.VideosLanguage.Single(x => x.Language == language);
            else
                vli = this.VideosLanguage.Single(x => x.Language == "en-US");

#if UNITY_STANDALONE && STEAMWORKS_NET
            this.GetComponent<VideoPlayer>().clip = vli.Video_1080;
            if (SteamAchievements.Instance)
            {
                if (Steamworks.SteamAPI.IsSteamRunning() && Steamworks.SteamUtils.IsSteamRunningOnSteamDeck())
                    this.GetComponent<VideoPlayer>().clip = vli.Video_SteamDeck;
            }
#elif UNITY_PS4 || UNITY_PS5 || UNITY_GAMECORE
            this.GetComponent<VideoPlayer>().clip = vli.Video_1080;
#elif UNITY_SWITCH
            this.GetComponent<VideoPlayer>().clip = vli.Video_720;
#else
            this.GetComponent<VideoPlayer>().clip = vli.Video_1080;
#endif
        }
    }
}