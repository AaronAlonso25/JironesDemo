using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;

namespace NeonBlood
{
    public class FastTravelCanvas : MMSingleton<FastTravelCanvas>
    {
        [Header("Main Panels")]
        public GameObject FastTravelPanel;

        [Header("Fast Travel Panel")]
        public GameObject LevelsLevelPanel_Content;
        public GameObject LevelLevel_Prefab;

        public Image LevelsLevelImage;
        public TMP_Text LevelsLevelName;

        //First Buttons
        private GameObject levelsFirstButton;

        //General
        private int indexLevel = 0;
        private FastTravelEvent fte;

        void Start()
        {

        }

        void Update()
        {
            if (Input.GetButtonDown("Circle_UI") && this.FastTravelPanel.activeSelf)
                this.ExitFastTravelPanel();
        }

        public void PlayLevel()
        {
            this.ExitFastTravelPanel();
            DataManager.Instance.CurrentLevelDoor = "FastTravel";
            MenuCanvas.Instance.PlayLevel(DataManager.Instance.AvailablePlaces[this.indexLevel]);
        }

        public IEnumerator ShowFastTravelPanel(FastTravelEvent fte)
        {
            //Comprobar que el panel de Mapa no esta activo
            if (MapCanvas.Instance.ContainerPanel.activeSelf ||
                PauseCanvas.Instance.MainPanel.activeSelf)
                yield break;

            yield return new WaitForEndOfFrame();

            MenuCanvas.Instance.Pause(true);
            this.FastTravelPanel.SetActive(true);
            PauseCanvas.Instance.CanPause = false;

            this.fte = fte;

            this.ConfigLevelsPanel();
        }

        public void ExitFastTravelPanel()
        {
            MenuCanvas.Instance.Pause(false);
            this.FastTravelPanel.SetActive(false);
            PauseCanvas.Instance.CanPause = true;

            this.fte.ReactiveEvent();
            this.fte = null;

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void ConfigLevelsPanel()
        {
            //Destruyo los niveles
            foreach (Transform child in this.LevelsLevelPanel_Content.transform)
                Destroy(child.gameObject);

            //Creo los niveles
            for (int i = 0; i < DataManager.Instance.AvailablePlaces.Count; i++)
            {
                int indexLevel = i;

                GameObject button = Instantiate(this.LevelLevel_Prefab, this.LevelsLevelPanel_Content.transform);
                button.GetComponent<Button>().onClick.AddListener(() => this.PlayLevel());
                button.GetComponentInChildren<TMP_Text>().text = DataManager.Instance.AvailablePlaces[indexLevel];

                this.AddLevelEventTriggerListener(button.GetComponent<EventTrigger>(), EventTriggerType.Select, indexLevel);

                if (indexLevel == 0)
                    this.levelsFirstButton = button;
            }

            EventSystem.current.SetSelectedGameObject(this.levelsFirstButton);
            this.ShowLevelInfoPanel(0);
        }

        private void ShowLevelInfoPanel(int indexLevel)
        {
            this.indexLevel = indexLevel;

            this.LevelsLevelImage.sprite =
                Resources.Load<Sprite>("Levels/" + DataManager.Instance.AvailablePlaces[indexLevel]);

            this.LevelsLevelName.text = DataManager.Instance.AvailablePlaces[indexLevel];
        }

        #region Tools

        private void AddLevelEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, int indexLevel)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener((functionIWant) => { this.ShowLevelInfoPanel(indexLevel); });
            trigger.triggers.Add(entry);
        }

        #endregion
    }
}