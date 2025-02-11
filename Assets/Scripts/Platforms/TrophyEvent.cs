using UnityEngine;
using System.Collections;

namespace NeonBlood
{
    /// <summary>
    /// Clase que activa un trofeo al iniciar un nivel
    /// </summary>
    public class TrophyEvent : MonoBehaviour
    {
        //Indica el identificador del trofeo
        public int TrophyID = -1;

        public enum Mode { START, TRIGGER, EVENT, NONE };
        public Mode ActivationMode = Mode.NONE;

        void Start()
        {
            if (this.ActivationMode == Mode.START)
                StartCoroutine(this.ActivateTrophyCo());
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && this.ActivationMode == Mode.TRIGGER)
                this.ActivateTrophy();
        }

        /// <summary>
        /// Metodo para activar el trofeos
        /// </summary>
        public void ActivateTrophy()
        {
// #if UNITY_PS4 && !UNITY_EDITOR
//             PS4Achievements.Instance.UnlockTrophy(this.TrophyID);
// #endif

// #if UNITY_PS5 && !UNITY_EDITOR
//             PS5Achievements.Instance.UnlockTrophy(this.TrophyID);
// #endif

// #if UNITY_STANDALONE && !UNITY_EDITOR
//             if (SteamAchievements.Instance)
//             {
//                 if (SteamManager.Initialized)
//                 {
//                     SteamAchievements.Instance.UnlockSteamAchievement("NEW_ACHIEVEMENT_1_" + this.TrophyID);
//                 }                    
//             }            
// #endif

// #if UNITY_STANDALONE && !UNITY_EDITOR && !EOS_DISABLE
//             if(EOSAchievements.Instance)
//             {
//                 string id = (this.TrophyID < 10) ? "0" + this.TrophyID : "" + this.TrophyID;
//                 id = "Trophy_" + id;

//                 EOSAchievements.Instance.UnlockAchievement(id);
//             }
// #endif

// #if UNITY_GAMECORE && !UNITY_EDITOR
//             XboxAchievements.Instance.UnlockTrophy(this.TrophyID);
// #endif
        }

        public void ActivateTrophy(int trophyID)
        {
            this.TrophyID = trophyID;
            this.ActivateTrophy();
        }

        private IEnumerator ActivateTrophyCo()
        {
            yield return new WaitForSeconds(1);
            this.ActivateTrophy();
        }
    }
}