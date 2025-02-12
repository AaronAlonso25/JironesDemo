using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    [System.Serializable]
    public class DataManagerInfo
    {
        //Lista de datos guardados segun el capitulo
        public List<SaveDataChapter> SaveDataChapters;

        //Nombre del nivel actual
        public int CurrentChapter;
        public string CurrentLevel;
        public string CurrentLevelDoor;

        //Language
        public string CurrentLanguage;

        //Coins
        public int CharactersCoins;
        //Events
        public List<string> CharactersEvents;
        //Inventario
        public List<ItemRPGInfo> CharactersItems;

        //Completar

        public DataManagerInfo() { }
    }

    [System.Serializable]
    public class NPCInfo
    {
        public string NameNPC;
        public int StateNPC;
    }

    [System.Serializable]
    public class SaveDataLevel
    {
        public string NameLevel; //MercadoBlind
        public int CurrentCheckPointIndex; //0
        public List<NPCInfo> CurrentPlayerNPCs; //Dani,2/Alex,4
    }

    [System.Serializable]
    public class SaveDataChapter
    {
        public int ChapterIndex;
        public List<SaveDataLevel> SaveDataLevels;
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        [Header("Game Data")]
        [Space(10)]        
        //Lista de datos guardados segun el capitulo
        public List<SaveDataChapter> SaveDataChapters;
        //Nombre del nivel actual -> Guardado
        public int CurrentChapter;
        public string CurrentLevel;
        public string CurrentLevelDoor;

        public string CurrentLanguage = "en-US";

        [Header("Player Data")]
        [Space(10)]
        //Coins -> Guardado
        public int CharactersCoins;
        //Events -> Guardado
        public List<string> CharactersEvents;
        //Inventario -> Guardado
        public List<ItemRPGInfo> CharactersItems;
        //Inventario Templates -> No guardar
        public List<ItemScriptableObject> CharactersItemsTemplates;

        [Header("Info Data")]
        [Space(10)]
        public List<string> MainCharactersDiary;
        public List<string> SecondaryCharactersDiary;
        public List<string> CurrentQuests;
        public List<string> CompletedQuests;
        public List<string> AvailablePlaces;

        [Header("Combat Data")]
        [Space(10)]
        //No guardar
        [HideInInspector] public Vector3 CombatPosition;
        [HideInInspector] public string CombatName;
        [HideInInspector] public string CombatScene;
        [HideInInspector] public bool CombatResult;
        [HideInInspector] public List<string> CombatVictoryCheckPointCommands;
        [HideInInspector] public List<string> CombatVictoryNPCCommands;

        [Header("Combat Data")]
        public List<string> CombatRPGTeamAxel;
        public List<string> CombatRPGTeamEnemy;

        [Header("Debug")]
        //No guardar
        public bool CanLoadSave = false;
        public bool IsDemo = false;

        public static DataManager Instance { get { return m_Instance; } }
        static DataManager m_Instance;

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

            

#if !UNITY_GAMECORE
            this.OnPlayerPrefsInit();
#endif

#if UNITY_GAMECORE
            StartCoroutine(this.OnBlobsInit());
#endif
        }

        private void OnPlayerPrefsInit()
        {
            if (!this.CanLoadSave)
                return;

            //Si no existen datos previos
            if (!PlayerPrefs.HasKey("Chapter_1"))
                this.ClearData();
            else
                this.LoadData();

            //TextsManager.Instance.LoadTexts();
        }

        private IEnumerator OnBlobsInit()
        {
            if (!this.CanLoadSave)
                yield break;

#if UNITY_GAMECORE
            yield return StartCoroutine(XboxAchievements.Instance.GameSave_OnClick_SaveExistsButton());

            //Si no existen datos previos
            if (!XboxAchievements.Instance.m_aGameSaveOperationIsExists)
            {
                this.ClearData();
                this.SaveData();
            }
            else
            {
                this.LoadData();
            }            
#endif

            TextsManager.Instance.LoadTexts();

            yield break;
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!this.CanLoadSave)
                return;

            //Asocio el nivel actual al sistema de guardado si no es una escena de configuracion
            if (scene.name != "Intro" && scene.name != "MainMenu" && scene.name != "LoadingScreen" && scene.name != "Credits" &&
                !scene.name.Contains("Combat"))
            {
                if (this.CurrentLevel != scene.name)
                {
                    //Asocio el nombre del nivel
                    this.CurrentLevel = scene.name;
                    //Guardo el progreso
                    this.SaveData();
                }
            }
        }

        #region CheckPoints

        /// <summary>
        /// Metodo para devolver el CheckPoint del nivel actual
        /// </summary>
        /// <returns></returns>
        public SaveDataLevel GetCheckPoint()
        {
            SaveDataChapter sdc = this.SaveDataChapters.Single(x => x.ChapterIndex == this.CurrentChapter);
            return sdc.SaveDataLevels.Single(x => x.NameLevel == SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Metodo para devolver el CheckPoint del nivel indicado
        /// </summary>
        /// <param name="nameScene"></param>
        /// <returns></returns>
        public SaveDataLevel GetCheckPoint(string nameScene)
        {
            SaveDataChapter sdc = this.SaveDataChapters.Single(x => x.ChapterIndex == this.CurrentChapter);
            return sdc.SaveDataLevels.Single(x => x.NameLevel == nameScene);
        }

        #endregion

        #region IO

        public void SaveData()
        {
            if (!this.CanLoadSave)
                return;

#if UNITY_GAMECORE && !UNITY_EDITOR
            XboxAchievements.Instance.GameSave_OnClick_SaveButton();
            return;
#endif

            //Completar

            //Levels
            //Ejemplo: MercadoBlind/0/Dani_2-Alex_4 , AxelBedroom/0/Dani_2-Alex_4
            foreach (SaveDataChapter sdc in this.SaveDataChapters)
            {
                string chapter = "";
                foreach (SaveDataLevel sdl in sdc.SaveDataLevels)
                {
                    string npcs = "";
                    foreach(NPCInfo ni in sdl.CurrentPlayerNPCs)
                        npcs += ni.NameNPC + "_" + ni.StateNPC + "-";
                    if (npcs.Length > 0)
                        npcs = npcs.Remove(npcs.Length - 1);

                    chapter += sdl.NameLevel + "/" + sdl.CurrentCheckPointIndex + "/" + npcs + ",";
                }
                if (chapter.Length > 0)
                    chapter = chapter.Remove(chapter.Length - 1);

                PlayerPrefs.SetString("Chapter_" + sdc.ChapterIndex, chapter);
            }

            int currentChapter = this.CurrentChapter;
            string currentLevel = this.CurrentLevel;
            string currentLevelDoor = this.CurrentLevelDoor;

            string currentLanguage = this.CurrentLanguage;

            int characterCoins = this.CharactersCoins;

            //Events
            string events = "";
            foreach (string e in this.CharactersEvents)
                events += e + ",";
            if (events.Length > 0)
                events = events.Remove(events.Length - 1);

            //Items
            //Ejemplo: Pocion/2,Llave/1,Basura/3
            string items = "";
            foreach (ItemRPGInfo ii in this.CharactersItems)
                items += ii.ItemName + "/" + ii.ItemAmount + ",";
            if (items.Length > 0)
                items = items.Remove(items.Length - 1);

            PlayerPrefs.SetInt("CurrentChapter", currentChapter);
            PlayerPrefs.SetString("CurrentLevel", currentLevel);
            PlayerPrefs.SetString("CurrentLevelDoor", currentLevelDoor);

            PlayerPrefs.SetString("CurrentLanguage", currentLanguage);

            PlayerPrefs.SetInt("CharactersCoins", characterCoins);
            PlayerPrefs.SetString("CharactersEvents", events);
            PlayerPrefs.SetString("CharactersItems", items);

            PlayerPrefs.Save();

#if UNITY_SWITCH && !UNITY_EDITOR
            FsSaveDataPlayerPrefs.Instance.SavePlayerPrefs();
#endif
        }

        public void SaveLanguage()
        {
            if (!this.CanLoadSave)
                return;

#if UNITY_GAMECORE && !UNITY_EDITOR
            XboxAchievements.Instance.GameSave_OnClick_SaveButton();
            return;
#endif

            string currentLanguage = this.CurrentLanguage;

            PlayerPrefs.SetString("CurrentLanguage", currentLanguage);
            PlayerPrefs.Save();

#if UNITY_SWITCH && !UNITY_EDITOR
            FsSaveDataPlayerPrefs.Instance.SavePlayerPrefs();
#endif
        }

        public void LoadData()
        {
            if (!this.CanLoadSave)
                return;

#if UNITY_GAMECORE && !UNITY_EDITOR
            XboxAchievements.Instance.GameSave_OnClick_LoadButton();
            return;
#endif

#if UNITY_SWITCH && !UNITY_EDITOR
            //FsSaveDataPlayerPrefs.Instance.Load();
#endif

            //Completar

            //Levels
            //Ejemplo: MercadoBlind/0/Dani_2-Alex_4 , AxelBedroom/0/Dani_2-Alex_4
            foreach (SaveDataChapter sdc in this.SaveDataChapters)
            {
                string chapter = PlayerPrefs.GetString("Chapter_" + sdc.ChapterIndex);
                string[] levels = chapter.Split(',');

                for (int i = 0; i < sdc.SaveDataLevels.Count; i++)
                {
                    string[] level = levels[i].Split('/');

                    int checkPoint = int.Parse(level[1]);
                    string[] npcs = level[2].Split('-'); ;

                    sdc.SaveDataLevels[i].CurrentCheckPointIndex = int.Parse(level[1]);
                    for (int j = 0; j < sdc.SaveDataLevels[i].CurrentPlayerNPCs.Count; j++)
                        sdc.SaveDataLevels[i].CurrentPlayerNPCs[j].StateNPC = int.Parse(npcs[j].Split('_')[1]);
                }
            }

            this.CurrentChapter = PlayerPrefs.GetInt("CurrentChapter");
            this.CurrentLevel = PlayerPrefs.GetString("CurrentLevel");
            this.CurrentLevelDoor = PlayerPrefs.GetString("CurrentLevelDoor");

            this.CurrentLanguage = PlayerPrefs.GetString("CurrentLanguage");

            this.CharactersCoins = PlayerPrefs.GetInt("CharactersCoins");

            //Events
            string[] events = PlayerPrefs.GetString("CharactersEvents").Split(',').Where(x => x != "").ToArray();
            foreach (string e in events)
                this.CharactersEvents.Add(e);

            //Items
            //Ejemplo: Pocion/2,Llave/1,Basura/3
            string[] items = PlayerPrefs.GetString("CharactersItems").Split(',').Where(x => x != "").ToArray();
            foreach (string s in items)
            {
                string[] item = s.Split('/');

                ItemRPGInfo ii = new ItemRPGInfo(this.CharactersItemsTemplates.Single(x => x.ItemName == item[0]));
                ii.ItemAmount = int.Parse(item[1]);

                this.CharactersItems.Add(ii);
            }
        }

        /// <summary>
        /// Metodo para borrar los datos guardados
        /// </summary>
        public void ClearData()
        {
            foreach (SaveDataChapter sdc in this.SaveDataChapters)
            {
                foreach (SaveDataLevel sdl in sdc.SaveDataLevels)
                {
                    sdl.CurrentCheckPointIndex = 0;
                    foreach (NPCInfo npci in sdl.CurrentPlayerNPCs)
                        npci.StateNPC = 0;
                }
            }

            this.CurrentChapter = 1;
            this.CurrentLevelDoor = "";
            this.CurrentLevel = "";

            this.CurrentLanguage = "en-US";

            this.CharactersCoins = 0;
            this.CharactersEvents = new List<string>();
            this.CharactersItems = new List<ItemRPGInfo>();

            //Completar

            //Guardo los datos reseteados
            this.SaveData();
        }

        /// <summary>
        /// Metodo para resetear los datos guardados al darle Nueva Partida
        /// </summary>
        public void ResetData()
        {
            foreach (SaveDataChapter sdc in this.SaveDataChapters)
            {
                foreach (SaveDataLevel sdl in sdc.SaveDataLevels)
                {
                    sdl.CurrentCheckPointIndex = 0;
                    foreach (NPCInfo npci in sdl.CurrentPlayerNPCs)
                        npci.StateNPC = 0;
                }
            }

            this.CurrentChapter = 1;
            this.CurrentLevelDoor = "";
            this.CurrentLevel = "";

            this.CharactersCoins = 0;
            this.CharactersEvents = new List<string>();
            this.CharactersItems = new List<ItemRPGInfo>();

            //Completar

            //Guardo los datos reseteados
            this.SaveData();
        }

        #endregion

        #region Minimal Info

        public void SetInfoData(DataManagerInfo saveData)
        {
            this.SaveDataChapters = saveData.SaveDataChapters;
            this.CurrentChapter = saveData.CurrentChapter;
            this.CurrentLevel = saveData.CurrentLevel;
            this.CurrentLevelDoor = saveData.CurrentLevelDoor;

            this.CurrentLanguage = saveData.CurrentLanguage;

            this.CharactersCoins = saveData.CharactersCoins;
            this.CharactersEvents = saveData.CharactersEvents;
            this.CharactersItems = saveData.CharactersItems;

            //Completar
        }

        public DataManagerInfo GetInfoData()
        {
            DataManagerInfo info = new DataManagerInfo();

            info.SaveDataChapters = this.SaveDataChapters;
            info.CurrentChapter = this.CurrentChapter;
            info.CurrentLevel = this.CurrentLevel;
            info.CurrentLevelDoor = this.CurrentLevelDoor;

            info.CurrentLanguage = this.CurrentLanguage;

            info.CharactersCoins = this.CharactersCoins;
            info.CharactersEvents = this.CharactersEvents;
            info.CharactersItems = this.CharactersItems;

            //Completar

            return info;
        }

        public void Start()
        {
            Cursor.visible = false; 
        } 
         public void Update()
        {
            Cursor.visible = false; 
        } 


        #endregion
    }
}