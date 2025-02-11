using UnityEngine;
using System.Collections.Generic;

namespace NeonBlood
{
    [System.Serializable]
    public class PlatformChange
    {
        public List<GameObject> ObjectsToDeactivate;
        public List<GameObject> ObjectsToActivate;
    }

    public class PlatformObjectsManager : MonoBehaviour
    {
        [Header("Platforms")]
        public PlatformChange SteamObjects;
        public PlatformChange PS4Objects;
        public PlatformChange PS5Objects;
        public PlatformChange SwitchObjects;
        public PlatformChange GameCoreObjects;

        void Start()
        {
            //Plataformas
            this.CheckPlatform();
        }

        private void CheckPlatform()
        {
#if UNITY_PS4
            foreach (GameObject go in this.PS4Objects.ObjectsToDeactivate)
                go.SetActive(false);
            foreach (GameObject go in this.PS4Objects.ObjectsToActivate)
                go.SetActive(true);
#elif UNITY_PS5
            foreach (GameObject go in this.PS5Objects.ObjectsToDeactivate)
                go.SetActive(false);
            foreach (GameObject go in this.PS5Objects.ObjectsToActivate)
                go.SetActive(true);
#elif UNITY_STANDALONE
            foreach (GameObject go in this.SteamObjects.ObjectsToDeactivate)
                go.SetActive(false);
            foreach (GameObject go in this.SteamObjects.ObjectsToActivate)
                go.SetActive(true);
#elif UNITY_SWITCH
            foreach (GameObject go in this.SwitchObjects.ObjectsToDeactivate)
                go.SetActive(false);
            foreach (GameObject go in this.SwitchObjects.ObjectsToActivate)
                go.SetActive(true);
#elif UNITY_GAMECORE
            foreach (GameObject go in this.GameCoreObjects.ObjectsToDeactivate)
                go.SetActive(false);
            foreach (GameObject go in this.GameCoreObjects.ObjectsToActivate)
                go.SetActive(true);
#endif
        }
    }
}