using UnityEngine;
using Rewired;

namespace NeonBlood
{
    [System.Serializable]
    public class OptionsManagerInfo
    {
        //Completar

        public OptionsManagerInfo() { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OptionsManager : MonoBehaviour
    {
        public enum Platform { STEAM, PS4, PS5, SWITCH, GAMECORE };
        public enum Input { KEYBOARD, CONTROLLER };

        [Header("Platform")]
        public Platform CurrentPlatform;
        public Input CurrentInput;

        private Controller currentController;
        private Player currentPlayer;

        public static OptionsManager Instance { get { return m_Instance; } }
        static OptionsManager m_Instance;

        void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this);
                return;
            }
            else
            {
                m_Instance = this;
                DontDestroyOnLoad(this);
            }

            this.SetPlatform();
            this.SetInput();
        }

        void Update()
        {
#if UNITY_STANDALONE

            if(this.currentPlayer == null && ReInput.isReady)
                this.currentPlayer = ReInput.players.GetPlayer(0);

            if(this.currentPlayer != null)
            {
                this.currentController = this.currentPlayer.controllers.GetLastActiveController();
                if (this.currentController != null)
                {
                    switch (this.currentController.type)
                    {
                        case ControllerType.Keyboard:
                            this.CurrentInput = Input.KEYBOARD;
                            break;
                        case ControllerType.Mouse:
                            this.CurrentInput = Input.KEYBOARD;
                            break;
                        case ControllerType.Joystick:
                            this.CurrentInput = Input.CONTROLLER;
                            break;
                    }
                }
            }   
            
#endif
        }

        #region Platform

        private void SetPlatform()
        {
#if UNITY_STANDALONE
            this.CurrentPlatform = Platform.STEAM;
#elif UNITY_PS4
            this.CurrentPlatform = Platform.PS4;
#elif UNITY_PS5
            this.CurrentPlatform = Platform.PS5;
#elif UNITY_SWITCH
            this.CurrentPlatform = Platform.SWITCH;
#elif UNITY_GAMECORE
            this.CurrentPlatform = Platform.GAMECORE;
#endif
        }

        #endregion

        #region Input

        private void SetInput()
        {
#if UNITY_STANDALONE
            this.CurrentInput = Input.KEYBOARD;
#elif UNITY_PS4 || UNITY_PS5 || UNITY_SWITCH || UNITY_GAMECORE
            this.CurrentInput = Input.CONTROLLER;
#endif
        }

        #endregion

        #region IO

        /// <summary>
        /// Metodo para guardar el progreso del juego
        /// </summary>
        public void SaveData()
        {
            if (!DataManager.Instance.CanLoadSave)
                return;

#if UNITY_GAMECORE && !UNITY_EDITOR
            XboxAchievements.Instance.GameSave_OnClick_SaveButton();
            return;
#endif

            //Completar

#if UNITY_SWITCH && !UNITY_EDITOR
            FsSaveDataPlayerPrefs.Instance.SavePlayerPrefs();
#endif
        }

        public void SaveLanguages()
        {
#if UNITY_GAMECORE && !UNITY_EDITOR
            return;
#endif

            //Completar

#if UNITY_SWITCH && !UNITY_EDITOR
            FsSaveDataPlayerPrefs.Instance.SavePlayerPrefs();
#endif
        }

        /// <summary>
        /// Metodo para cargar el progreso del juego
        /// </summary>
        public void LoadData()
        {
            if (!DataManager.Instance.CanLoadSave)
                return;

#if UNITY_GAMECORE && !UNITY_EDITOR
            return;
#endif

#if UNITY_SWITCH && !UNITY_EDITOR
            FsSaveDataPlayerPrefs.Instance.Load();
#endif

            //Completar
        }

        /// <summary>
        /// Metodo para borrar los datos guardados
        /// </summary>
        public void ClearData()
        {
            //Completar
        }

        #endregion

        #region Minimal Info

        public void SetInfoData(OptionsManagerInfo saveData)
        {
            //Completar
        }

        public OptionsManagerInfo GetInfoData()
        {
            OptionsManagerInfo info = new OptionsManagerInfo();

            //Completar

            return info;
        }

        #endregion
    }
}