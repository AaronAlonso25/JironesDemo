using UnityEngine;
using Rewired;

namespace NeonBlood
{
    public class ResolutionConfigManager : MonoBehaviour
    {
        void Start()
        {
#if UNITY_STANDALONE

            //string s = PlayerPrefs.GetString("OptionsResolution", "1920*1080");

            //int width = int.Parse(s.Split('*')[0]);
            //int height = int.Parse(s.Split('*')[1]);
            //if (Screen.fullScreen)
            //    Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow, 60);
            //else
            //    Screen.SetResolution(width, height, FullScreenMode.Windowed, 60);

            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 3;
            Application.targetFrameRate = 60;

            //Activo los mapas de Teclado y Raton
            //ReInput.players.GetPlayer(0).controllers.maps.SetAllMapsEnabled(true);

#elif UNITY_PS4

            Screen.SetResolution(1920, 1080, true);
            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 3;
            Application.targetFrameRate = 60;  
            Application.runInBackground = true;

#elif UNITY_PS5

            Screen.SetResolution(2560, 1440, true);
            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 3;
            Application.targetFrameRate = 60;
            Application.runInBackground = true;

#elif UNITY_SWITCH

            Screen.SetResolution(1280, 720, true);
            QualitySettings.vSyncCount = 2;
            QualitySettings.maxQueuedFrames = 3;
            Application.targetFrameRate = 30;  
            
#endif
        }
    }
}