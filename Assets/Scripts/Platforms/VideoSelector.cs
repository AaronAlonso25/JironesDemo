using UnityEngine;
using UnityEngine.Video;

namespace NeonBlood
{
    public class VideoSelector : MonoBehaviour
    {        
        public VideoClip Video_720;
        public VideoClip Video_1080;
        public VideoClip Video_SteamDeck;

        void Start()
        {
#if UNITY_STANDALONE && STEAMWORKS_NET
            this.GetComponent<VideoPlayer>().clip = this.Video_1080;
            if (SteamAchievements.Instance)
            {
                if (Steamworks.SteamAPI.IsSteamRunning() && Steamworks.SteamUtils.IsSteamRunningOnSteamDeck())
                    this.GetComponent<VideoPlayer>().clip = this.Video_SteamDeck;
            }
#elif UNITY_PS4 || UNITY_PS5 || UNITY_GAMECORE
            this.GetComponent<VideoPlayer>().clip = this.Video_1080;
#elif UNITY_SWITCH
            this.GetComponent<VideoPlayer>().clip = this.Video_720;
#else
            this.GetComponent<VideoPlayer>().clip = this.Video_1080;
#endif
        }
    }
}