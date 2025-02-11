using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;



namespace NeonBlood
{
    public class PauseCanvas : MMSingleton<PauseCanvas>
    {
        [Header("Main Panels")]
        public GameObject MainPanel;
        public List<GameObject> Panels;

        [Header("Buttons")]
        public GameObject PauseButton;

        [Header("Idiomas")]

        


        private int indexPanel = 0;

        private bool canPause;
        
        public bool CanPause
        {
            get { return this.canPause; }

            set
            {
                if (value)
                    StartCoroutine(CanPressPauseCo());
                else
                    this.canPause = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.canPause = true;
        }

        void Start()
        {
            
        }

        void Update()
        {
            //Mostrar / Ocultar el menu de pausa
            if (Input.GetButtonDown("Start") && !this.MainPanel.activeSelf)
                this.ShowPausePanel();
            else if (Input.GetButtonDown("Start") && this.MainPanel.activeSelf)
                this.ExitPausePanel();
            else if (Input.GetButtonDown("Circle_UI") && this.MainPanel.activeSelf)
                this.ExitPausePanel();
        }

        public void ShowPausePanel()
        {
            //Comprobar que el panel de Mapa no esta activo
            if (MapCanvas.Instance.ContainerPanel.activeSelf ||
                FastTravelCanvas.Instance.FastTravelPanel.activeSelf ||
                ShopCanvas.Instance.ShopPanel.activeSelf ||
                FindFirstObjectByType<CombatRPGCanvas>() ||
                !CharacterController.Instance.CanMove)
                return;

            if (!this.CanPause)
                return;

            MenuCanvas.Instance.Pause(true);
            this.MainPanel.SetActive(true);

            EventSystem.current.SetSelectedGameObject(this.PauseButton);
        }

        public void ExitPausePanel()
        {
            MenuCanvas.Instance.Pause(false);
            this.MainPanel.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void ShowPanel(int indexPanel)
        {
            this.indexPanel = indexPanel;

            //foreach (GameObject panel in this.Panels)
            //    panel.SetActive(false);
            //this.Panels[this.indexPanel].SetActive(true);

            switch (indexPanel)
            {
                case 0:
                    this.Continue();
                    break;
                case 1:
                    this.Save();
                    break;
                case 2:
                    this.Options();
                    break;
                case 3:
                    this.Exit();
                    break;
            }
        }

        #region Actions

        public void Continue()
        {
            this.ExitPausePanel();
        }

        public void Save()
        {

        }

        public void Options()
        {
            

        }

        public void Exit()
        {
            //DataManager.Instance.SaveData();
            this.ExitPausePanel();

#if UNITY_PS4
            MenuCanvas.Instance.PlayLevel("MainMenuJironesSeda");
#else
            MenuCanvas.Instance.PlayLevelDirectly("MainMenuJironesSeda");
#endif
        }

        #endregion

        #region Tools

        public IEnumerator CanPressPauseCo()
        {
            yield return new WaitForSeconds(0.5f);
            this.canPause = true;
        }

        public void GoToMainMenu()
        {
            MenuCanvas.Instance.PlayLevelDirectly("MainMenuJironesSeda");
        }

        #endregion

        



        
    }
}