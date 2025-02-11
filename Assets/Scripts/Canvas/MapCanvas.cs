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
    public class MapCanvas : MMSingleton<MapCanvas>
    {
        [Header("Main Panels")]
        public GameObject ContainerPanel;
        public List<GameObject> Panels;

        [Header("SubPanels")]
        public GameObject MainPanel;
        public GameObject InventoryPanel;
        public GameObject TeamPanel;
        public GameObject QuestsPanel;
        public GameObject MapPanel;
        public GameObject DiaryPanel;
        public GameObject SavePanel;

        [Header("Inventory Panel")]
        public GameObject InventoryObjectsPanel_Content;
        public GameObject InventoryObject_Prefab;

        public Image InventoryObjectImage;
        public TMP_Text InventoryObjectName;
        public TMP_Text InventoryObjectShort;
        public TMP_Text InventoryObjectDescription;
        public List<Image> HeaderItemsImages;

        [Header("Quests Panel")]
        public GameObject QuestsQuestPanel_Content;
        public GameObject QuestsQuest_Prefab;

        public Image QuestsQuestImage;
        public TMP_Text QuestsQuestName;
        public TMP_Text QuestsQuestDescription;
        public List<Image> HeaderQuestsImages;

        [Header("Diary Panel")]
        public GameObject DiaryCharactersPanel_Content;
        public GameObject DiaryCharacter_Prefab;

        public Image DiaryCharacterImage;
        public TMP_Text DiaryCharacterName;
        public TMP_Text DiaryCharacterDescription;
        public List<Image> HeaderCharactersImages;

        [Header("Buttons")]
        public GameObject MainFirstButton;
        public Color HeaderColorHighlighted;

        //First Buttons
        private GameObject inventoryFirstButton;
        private GameObject questsFirstButton;
        private GameObject diaryFirstButton;

        //General
        private int indexPanel = 0;

        //Inventory
        private List<ItemRPGInfo> filterItems = new List<ItemRPGInfo>();
        private ITEMS filterType = ITEMS.CONSUMABLES;

        //Quests
        private int indexQuestPanel = 0;
        private List<string> filterQuests = new List<string>();

        //Diary
        private int indexDiaryPanel = 0;
        private List<string> filterCharacters = new List<string>();

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {

        }

        void Update()
        {
            //Mostrar / Ocultar el menu de pausa
            if (Input.GetButtonDown("Select") && !this.ContainerPanel.activeSelf)
                this.ShowMapPanel();
            else if (Input.GetButtonDown("Select") && this.ContainerPanel.activeSelf)
                this.ExitMapPanel();

            if (Input.GetButtonDown("Circle_UI") && this.ContainerPanel.activeSelf && this.MainPanel.activeSelf)
                this.ExitMapPanel();
            else if (Input.GetButtonDown("Circle_UI") && this.ContainerPanel.activeSelf && !this.MainPanel.activeSelf)
                this.ShowPanel(0);

            //Filtrar objetos del inventario
            if (Input.GetButtonDown("L1_UI") && this.InventoryPanel.activeSelf)
            {
                if (this.filterType != ITEMS.CONSUMABLES)
                {
                    this.filterType--;
                    this.ConfigInventoryPanel(this.filterType);
                }
            }
            else if (Input.GetButtonDown("R1_UI") && this.InventoryPanel.activeSelf)
            {
                if (this.filterType != ITEMS.VARIOUS)
                {
                    this.filterType++;
                    this.ConfigInventoryPanel(this.filterType);
                }
            }

            //Filtrar misiones
            if (Input.GetButtonDown("L1_UI") && this.QuestsPanel.activeSelf)
            {
                this.indexQuestPanel = 0;
                this.ConfigQuestsPanel();
            }
            else if (Input.GetButtonDown("R1_UI") && this.QuestsPanel.activeSelf)
            {
                this.indexQuestPanel = 1;
                this.ConfigQuestsPanel();
            }

            //Filtrar personajes
            if (Input.GetButtonDown("L1_UI") && this.DiaryPanel.activeSelf)
            {
                this.indexDiaryPanel = 0;
                this.ConfigDiaryPanel();
            }
            else if (Input.GetButtonDown("R1_UI") && this.DiaryPanel.activeSelf)
            {
                this.indexDiaryPanel = 1;
                this.ConfigDiaryPanel();
            }
        }

        public void ShowMapPanel()
        {
            //Comprobar que el panel de Pausa no esta activo
            if (PauseCanvas.Instance.MainPanel.activeSelf ||
                FastTravelCanvas.Instance.FastTravelPanel.activeSelf ||
                ShopCanvas.Instance.ShopPanel.activeSelf ||
                FindFirstObjectByType<CombatRPGCanvas>() ||
                !CharacterController.Instance.CanMove)
                return;

            MenuCanvas.Instance.Pause(true);
            this.ContainerPanel.SetActive(true);

            this.ShowPanel(0);
        }

        public void ExitMapPanel()
        {
            MenuCanvas.Instance.Pause(false);
            this.ContainerPanel.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void ShowPanel(int indexPanel)
        {
            this.indexPanel = indexPanel;

            this.filterType = ITEMS.CONSUMABLES;
            this.indexQuestPanel = 0;
            this.indexDiaryPanel = 0;

            foreach (GameObject panel in this.Panels)
                panel.SetActive(false);
            this.Panels[this.indexPanel].SetActive(true);

            switch (indexPanel)
            {
                case 0:
                    this.ConfigMainPanel();
                    break;
                case 1:
                    this.ConfigInventoryPanel(this.filterType);
                    break;
                case 2:
                    this.ConfigTeamPanel();
                    break;
                case 3:
                    this.ConfigQuestsPanel();
                    break;
                case 4:
                    this.ConfigMapPanel();
                    break;
                case 5:
                    this.ConfigDiaryPanel();
                    break;
                case 6:
                    this.ConfigSavePanel();
                    break;
            }
        }

        #region SubPanels

        public void ConfigMainPanel()
        {
            EventSystem.current.SetSelectedGameObject(this.MainFirstButton);
        }

        public void ConfigInventoryPanel(ITEMS type)
        {
            //Destruyo las habilidades previas
            foreach (Transform child in this.InventoryObjectsPanel_Content.transform)
                Destroy(child.gameObject);

            this.filterItems = DataManager.Instance.CharactersItems.Where(x => x.ItemType == type).ToList();

            //Header
            foreach (Image im in this.HeaderItemsImages)
                im.color = Color.white;
            this.HeaderItemsImages[(int)type].color = this.HeaderColorHighlighted;

            //Creo los objetos nuevos
            for (int i = 0; i < this.filterItems.Count; i++)
            {
                int indexItem = i;

                GameObject button = Instantiate(this.InventoryObject_Prefab, this.InventoryObjectsPanel_Content.transform);
                //button.GetComponent<Button>().onClick.AddListener(() => this.ShowObjectInfoPanel(indexItem));

                button.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Inventory/" + this.filterItems[indexItem].ItemName);
                button.GetComponentInChildren<TMP_Text>().text = "x" + this.filterItems[indexItem].ItemAmount;

                this.AddItemEventTriggerListener(button.GetComponent<EventTrigger>(), EventTriggerType.Select, indexItem);

                if (indexItem == 0)
                    this.inventoryFirstButton = button;
            }

            EventSystem.current.SetSelectedGameObject(this.inventoryFirstButton);

            //Si hay algun objeto, muestro el primero
            if (filterItems.Count > 0)
                this.ShowObjectInfoPanel(0);
        }

        private void ShowObjectInfoPanel(int indexItem)
        {
            this.InventoryObjectImage.sprite = Resources.Load<Sprite>("Inventory/" + this.filterItems[indexItem].ItemName);

            this.InventoryObjectName.text =
                TextsManager.Instance.ObjectsInfo[this.filterItems[indexItem].ItemName].Title;
            this.InventoryObjectShort.text =
                TextsManager.Instance.ObjectsInfo[this.filterItems[indexItem].ItemName].Short;
            this.InventoryObjectDescription.text =
                TextsManager.Instance.ObjectsInfo[this.filterItems[indexItem].ItemName].Text;
        }

        public void ConfigTeamPanel()
        {

        }

        public void ConfigQuestsPanel()
        {
            //Destruyo las misiones previas
            foreach (Transform child in this.QuestsQuestPanel_Content.transform)
                Destroy(child.gameObject);

            this.filterQuests = (this.indexQuestPanel == 0) ?
                DataManager.Instance.CurrentQuests : DataManager.Instance.CompletedQuests;

            //Header
            foreach (Image im in this.HeaderQuestsImages)
                im.color = Color.white;
            this.HeaderQuestsImages[this.indexQuestPanel].color = this.HeaderColorHighlighted;

            //Creo las misiones nuevas
            for (int i = 0; i < this.filterQuests.Count; i++)
            {
                int indexQuest = i;

                GameObject button = Instantiate(this.QuestsQuest_Prefab, this.QuestsQuestPanel_Content.transform);
                //button.GetComponent<Button>().onClick.AddListener(() => this.ShowQuestInfoPanel(indexQuest));
                button.GetComponentInChildren<TMP_Text>().text =
                    TextsManager.Instance.QuestsInfo[this.filterQuests[indexQuest]].Title;

                this.AddQuestEventTriggerListener(button.GetComponent<EventTrigger>(), EventTriggerType.Select, indexQuest);

                if (indexQuest == 0)
                    this.questsFirstButton = button;
            }

            EventSystem.current.SetSelectedGameObject(this.questsFirstButton);
            this.ShowQuestInfoPanel(0);
        }

        private void ShowQuestInfoPanel(int indexQuest)
        {
            this.QuestsQuestImage.sprite = Resources.Load<Sprite>("Quests/" + this.filterQuests[indexQuest]);

            this.QuestsQuestName.text = TextsManager.Instance.QuestsInfo[this.filterQuests[indexQuest]].Title;
            this.QuestsQuestDescription.text = TextsManager.Instance.QuestsInfo[this.filterQuests[indexQuest]].Text;
        }

        public void ConfigMapPanel()
        {

        }

        public void ConfigDiaryPanel()
        {
            //Destruyo las habilidades previas
            foreach (Transform child in this.DiaryCharactersPanel_Content.transform)
                Destroy(child.gameObject);

            this.filterCharacters = (this.indexDiaryPanel == 0) ?
                DataManager.Instance.MainCharactersDiary : DataManager.Instance.SecondaryCharactersDiary;

            //Header
            foreach (Image im in this.HeaderCharactersImages)
                im.color = Color.white;
            this.HeaderCharactersImages[this.indexDiaryPanel].color = this.HeaderColorHighlighted;

            //Creo los personajes nuevos
            for (int i = 0; i < this.filterCharacters.Count; i++)
            {
                int indexCharacter = i;

                GameObject button = Instantiate(this.DiaryCharacter_Prefab, this.DiaryCharactersPanel_Content.transform);
                //button.GetComponent<Button>().onClick.AddListener(() => this.ShowCharacterInfoPanel(indexCharacter));
                button.GetComponentInChildren<TMP_Text>().text =
                    TextsManager.Instance.CharactersInfo[this.filterCharacters[indexCharacter]].Title;

                this.AddCharacterEventTriggerListener(button.GetComponent<EventTrigger>(), EventTriggerType.Select, indexCharacter);

                if (indexCharacter == 0)
                    this.diaryFirstButton = button;
            }

            EventSystem.current.SetSelectedGameObject(this.diaryFirstButton);
            this.ShowCharacterInfoPanel(0);
        }

        private void ShowCharacterInfoPanel(int indexCharacter)
        {
            this.DiaryCharacterImage.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
                "Diary/" + this.filterCharacters[indexCharacter] + "DiaryAnimator");

            this.DiaryCharacterName.text = TextsManager.Instance.CharactersInfo[this.filterCharacters[indexCharacter]].Title;
            this.DiaryCharacterDescription.text = TextsManager.Instance.CharactersInfo[this.filterCharacters[indexCharacter]].Text;
        }

        public void ConfigSavePanel()
        {

        }

        #endregion

        #region Tools

        private void AddItemEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, int indexItem)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener((functionIWant) => { this.ShowObjectInfoPanel(indexItem); });
            trigger.triggers.Add(entry);
        }

        private void AddQuestEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, int indexQuest)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener((functionIWant) => { this.ShowQuestInfoPanel(indexQuest); });
            trigger.triggers.Add(entry);
        }

        private void AddCharacterEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, int indexCharacter)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener((functionIWant) => { this.ShowCharacterInfoPanel(indexCharacter); });
            trigger.triggers.Add(entry);
        }

        #endregion
    }
}