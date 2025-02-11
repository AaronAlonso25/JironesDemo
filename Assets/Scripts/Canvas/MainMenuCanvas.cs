using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;
using Tayx.Graphy;

namespace NeonBlood
{
    public class MainMenuCanvas : MonoBehaviour
    {
        [Header("Main Panels")]
        public GameObject ContainerPanel;
        public List<GameObject> Panels;

        [Header("SubPanels")]
        public GameObject MainPanel;
        public GameObject LevelsPanel;
        public GameObject OptionsPanel;

        [Header("Levels Panel")]
        public GameObject LevelsLevelPanel_Content;
        public GameObject LevelLevel_Prefab;

        public Image LevelsLevelImage;
        public TMP_Text LevelsLevelName;
        public List<Image> HeaderChapterImages;

        [Header("Buttons")]
        public GameObject MainFirstButton;
        public Color HeaderColorHighlighted;

        [Header("Debug")]
        public string FirstLevelName = "Axel Bedroom";

        [Header("Options")]

        public Slider slider_musica;
        public TMP_Text volumenmusica;

        //First Buttons
        private GameObject levelsFirstButton;

        //General
        private int indexPanel = 0;
        private int indexLevel = 0;
        private int indexChapter = 0;

        private SaveDataChapter currentChapter;

        void Start()
        {

        }

        void Update()
        {
            //if (Input.anyKeyDown && !this.ContainerPanel.activeSelf)
            //    this.ShowMainMenuPanel();

            //if (Input.GetButtonDown("Circle_UI") && this.ContainerPanel.activeSelf && this.MainPanel.activeSelf)
            //    this.ExitMapPanel();
            if (Input.GetButtonDown("Circle_UI") && this.ContainerPanel.activeSelf && !this.MainPanel.activeSelf)
                this.ShowPanel(0);

            //Cambiar de Capitulos
            if (Input.GetButtonDown("L1_UI") && this.LevelsPanel.activeSelf)
            {
                this.NextChapterPanel(-1);
            }
            else if (Input.GetButtonDown("R1_UI") && this.LevelsPanel.activeSelf)
            {
                this.NextChapterPanel(1);
            }
        }

        public void NewGame()
        {
            MenuCanvas.Instance.PlayLevelDirectly(this.FirstLevelName);
        }

        public void PlayLevel()
        {
            DataManager.Instance.CurrentChapter = this.currentChapter.ChapterIndex;
            MenuCanvas.Instance.PlayLevelDirectly(DataManager.Instance.SaveDataChapters[this.indexChapter].SaveDataLevels[this.indexLevel].NameLevel);
        }

        public void ShowMainMenuPanel()
        {
            this.ContainerPanel.SetActive(true);
            this.ShowPanel(0);
        }

        public void ExitMapPanel()
        {
            MenuCanvas.Instance.Pause(false);
            this.ContainerPanel.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ShowPanel(int indexPanel)
        {
            this.indexPanel = indexPanel;

            foreach (GameObject panel in this.Panels)
                panel.SetActive(false);
            this.Panels[this.indexPanel].SetActive(true);

            switch (indexPanel)
            {
                case 0:
                    this.ConfigMainPanel();
                    break;
                case 1:
                    this.ConfigLevelsPanel();
                    break;
                case 2:
                    this.ConfigOptionsPanel();
                    break;
            }
        }

        #region SubPanels

        public void ConfigMainPanel()
        {
            EventSystem.current.SetSelectedGameObject(this.MainFirstButton);
        }

        public void ConfigLevelsPanel()
        {
            //Destruyo los niveles
            foreach (Transform child in this.LevelsLevelPanel_Content.transform)
                Destroy(child.gameObject);

            this.currentChapter = DataManager.Instance.SaveDataChapters[this.indexChapter];

            //Header
            foreach (Image im in this.HeaderChapterImages)
                im.color = Color.white;
            this.HeaderChapterImages[this.currentChapter.ChapterIndex - 1].color = this.HeaderColorHighlighted;

            //Creo los niveles
            for (int i = 0; i < this.currentChapter.SaveDataLevels.Count; i++)
            {
                int indexLevel = i;

                GameObject button = Instantiate(this.LevelLevel_Prefab, this.LevelsLevelPanel_Content.transform);
                button.GetComponent<Button>().onClick.AddListener(() => this.PlayLevel());
                button.GetComponentInChildren<TMP_Text>().text = this.currentChapter.SaveDataLevels[indexLevel].NameLevel;

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
                Resources.Load<Sprite>("Levels/" + this.currentChapter.SaveDataLevels[indexLevel].NameLevel);

            this.LevelsLevelName.text = this.currentChapter.SaveDataLevels[indexLevel].NameLevel;
        }

        private void NextChapterPanel(int sign)
        {
            this.indexChapter += sign;
            this.indexChapter = Mathf.Clamp(this.indexChapter, 0, DataManager.Instance.SaveDataChapters.Count - 1);

            this.ConfigLevelsPanel();
        }

        #endregion

        #region Options

        public void ConfigOptionsPanel()
        {
            FindFirstObjectByType<GraphyManager>().ToggleActive();
        }

        public void ShowOptionsPanel()
        {
            OptionsPanel.SetActive(true);
            MainPanel.SetActive(false);
            LevelsPanel.SetActive(false);


        }

        public void ExitOptionsPanel()
        {
            OptionsPanel.SetActive(false);
            MainPanel.SetActive(true);
            LevelsPanel.SetActive(false);

            
        }

        public void UpdateVolumeLabel()
        {
            volumenmusica.text = $"{slider_musica.value}";  
            GameManager.Instance.SetVolume(slider_musica.value / 10.0f);  
         
        }

        #endregion

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