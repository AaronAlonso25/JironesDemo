using UnityEngine;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace NeonBlood
{
    public class MenuInfo
    {
        //Datos del menu del XML
        public string Title { get; set; }
        public List<string> Buttons { get; set; }
        public List<ContainerInfo> Container { get; set; }

        public MenuInfo(string title, List<string> buttons, List<ContainerInfo> container)
        {
            this.Title = title;
            this.Buttons = buttons;
            this.Container = container;
        }
    }

    public class ContainerInfo
    {
        //Datos del contenedor del XML
        public string Title { get; set; }
        public string Text { get; set; }

        public ContainerInfo(string title, string text)
        {
            this.Title = title;
            this.Text = text;
        }
    }

    public class ObjectInfo
    {
        //Datos del contenedor del XML
        public string Title { get; set; }
        public string Short { get; set; }
        public string Text { get; set; }

        public ObjectInfo(string title, string shortText, string longText)
        {
            this.Title = title;
            this.Short = shortText;
            this.Text = longText;
        }
    }

    public class QuestInfo
    {
        //Datos del contenedor del XML
        public string Title { get; set; }
        public string Text { get; set; }
        public List<string> Objectives { get; set; }

        public QuestInfo(string title, string text, List<string> objectives)
        {
            this.Title = title;
            this.Text = text;
            this.Objectives = objectives;
        }
    }

    [System.Serializable]
    public class TextsLanguage
    {
        public string Language;
        public TextAsset File;
    }

    /// <summary>
    /// Clase que carga los datos de los niveles y los menus del XML
    /// </summary>
    public class TextsManager : MMPersistentSingleton<TextsManager>
    {
        //Archivos XML
        public List<TextsLanguage> Files;
        public TextAsset CombatRPGFile;

        public Dictionary<string, MenuInfo> MenusInfo { get; set; }
        public Dictionary<string, QuestInfo> QuestsInfo { get; set; }
        public Dictionary<string, ContainerInfo> CharactersInfo { get; set; }
        public Dictionary<string, ContainerInfo> LocationsInfo { get; set; }
        public Dictionary<string, ContainerInfo> TutorialsInfo { get; set; }
        public Dictionary<string, ObjectInfo> ObjectsInfo { get; set; }

        //Combat
        public Dictionary<string, WeaponRPGInfo> WeaponsCombatInfo { get; set; }
        public Dictionary<string, WeaponRPGInfo> SpecialWeaponsCombatInfo { get; set; }
        public Dictionary<string, AbilityRPGInfo> AbilitiesCombatInfo { get; set; }
        public Dictionary<string, CharacterRPGInfo> CharactersCombatInfo { get; set; }

        protected override void Awake()
        {
            base.Awake();

            this.LoadTexts();
        }

        #region Texts

        /// <summary>
        /// Metodo para ejecutar la carga del menu
        /// </summary>
        public void LoadTexts()
        {
            this.MenusInfo = new Dictionary<string, MenuInfo>();
            this.QuestsInfo = new Dictionary<string, QuestInfo>();
            this.CharactersInfo = new Dictionary<string, ContainerInfo>();
            this.LocationsInfo = new Dictionary<string, ContainerInfo>();
            this.TutorialsInfo = new Dictionary<string, ContainerInfo>();
            this.ObjectsInfo = new Dictionary<string, ObjectInfo>();

            this.GetInfo();
        }

        private void GetInfo()
        {
            string currentLanguage = "Spanish";
            TextAsset currentFile = this.Files.Single(x => x.Language == currentLanguage).File;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(currentFile.text);
            XmlNodeList levelsList = xmlDoc.GetElementsByTagName("language");

            foreach (XmlNode levelIndex in levelsList)
            {
                foreach (XmlNode levelInfo in levelIndex)
                {
                    //if (levelInfo.Name == "name" && levelInfo.InnerText != DataManager.Instance.CurrentLanguage)
                    //    break;

                    if (levelInfo.Name == "menus")
                    {
                        this.GetInfoMenus(levelInfo);
                    }
                    else if (levelInfo.Name == "quests")
                    {
                        this.GetInfoQuests(levelInfo);
                    }
                    else if (levelInfo.Name == "locations")
                    {
                        this.GetInfoLocations(levelInfo);
                    }
                    else if (levelInfo.Name == "characters")
                    {
                        this.GetInfoCharacters(levelInfo);
                    }
                    else if (levelInfo.Name == "tutorials")
                    {
                        this.GetInfoTutorials(levelInfo);
                    }
                    else if (levelInfo.Name == "objects")
                    {
                        this.GetInfoObjects(levelInfo);
                    }
                }
            }
        }

        private void GetInfoMenus(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;
                string title = "";
                List<string> buttons = new List<string>();
                List<ContainerInfo> containers = new List<ContainerInfo>();

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "title")
                        title = levelDialoguesInfo.InnerText;
                    if (levelDialoguesInfo.Name == "buttons")
                    {
                        foreach (XmlNode levelDialoguesInfoInner in levelDialoguesInfo.ChildNodes)
                            buttons.Add(levelDialoguesInfoInner.InnerText);
                    }
                    if (levelDialoguesInfo.Name == "container")
                    {
                        string titleInner = "";
                        string text = "";

                        foreach (XmlNode levelDialoguesInfoInner in levelDialoguesInfo.ChildNodes)
                        {
                            if (levelDialoguesInfoInner.Name == "title")
                                titleInner = levelDialoguesInfoInner.InnerText;
                            else if (levelDialoguesInfoInner.Name == "text")
                                text = levelDialoguesInfoInner.InnerText;
                        }

                        ContainerInfo ci = new ContainerInfo(titleInner, text);
                        containers.Add(ci);
                    }
                }

                MenuInfo mi = new MenuInfo(title, buttons, containers);
                this.MenusInfo.Add(key, mi);
            }
        }

        private void GetInfoQuests(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;
                string title = "";
                string description = "";
                List<string> objectives = new List<string>();

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "title")
                        title = levelDialoguesInfo.InnerText;
                    if (levelDialoguesInfo.Name == "text")
                        description = levelDialoguesInfo.InnerText;
                    if (levelDialoguesInfo.Name == "objectives")
                    {
                        foreach (XmlNode levelDialoguesInfoInner in levelDialoguesInfo.ChildNodes)
                            objectives.Add(levelDialoguesInfoInner.InnerText);
                    }
                }

                QuestInfo qi = new QuestInfo(title, description, objectives);
                this.QuestsInfo.Add(key, qi);
            }
        }

        private void GetInfoLocations(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;
                string title = "";
                string text = "";

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "title")
                        title = levelDialoguesInfo.InnerText;
                    else if (levelDialoguesInfo.Name == "text")
                        text = levelDialoguesInfo.InnerText;
                }

                ContainerInfo oi = new ContainerInfo(title, text);
                this.LocationsInfo.Add(key, oi);
            }
        }

        private void GetInfoCharacters(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;
                string title = "";
                string text = "";

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "name")
                        title = levelDialoguesInfo.InnerText;
                    else if (levelDialoguesInfo.Name == "description")
                        text = levelDialoguesInfo.InnerText;
                }

                ContainerInfo oi = new ContainerInfo(title, text);
                this.CharactersInfo.Add(key, oi);
            }
        }

        private void GetInfoObjects(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;
                string title = "";
                string shortText = "";
                string longText = "";

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "title")
                        title = levelDialoguesInfo.InnerText;
                    else if (levelDialoguesInfo.Name == "short")
                        shortText = levelDialoguesInfo.InnerText;
                    else if (levelDialoguesInfo.Name == "description")
                        longText = levelDialoguesInfo.InnerText;
                }

                ObjectInfo oi = new ObjectInfo(title, shortText, longText);
                this.ObjectsInfo.Add(key, oi);
            }
        }

        private void GetInfoTutorials(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;
                string title = "";
                string text = "";

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "title")
                        title = levelDialoguesInfo.InnerText;
                    else if (levelDialoguesInfo.Name == "text")
                        text = levelDialoguesInfo.InnerText;
                }

                ContainerInfo oi = new ContainerInfo(title, text);
                this.TutorialsInfo.Add(key, oi);
            }
        }

        #endregion

        #region CombatRPG

        public void LoadCombatTexts()
        {
            this.WeaponsCombatInfo = new Dictionary<string, WeaponRPGInfo>();
            this.SpecialWeaponsCombatInfo = new Dictionary<string, WeaponRPGInfo>();
            this.AbilitiesCombatInfo = new Dictionary<string, AbilityRPGInfo>();
            this.CharactersCombatInfo = new Dictionary<string, CharacterRPGInfo>();

            this.GetCombatInfo();
        }

        private void GetCombatInfo()
        {
            //string currentLanguage = "Spanish";
            TextAsset currentFile = this.CombatRPGFile;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(currentFile.text);
            XmlNodeList levelsList = xmlDoc.GetElementsByTagName("language");

            foreach (XmlNode levelIndex in levelsList)
            {
                foreach (XmlNode levelInfo in levelIndex)
                {
                    //if (levelInfo.Name == "name" && levelInfo.InnerText != DataManager.Instance.CurrentLanguage)
                    //    break;

                    if (levelInfo.Name == "weapons")
                    {
                        this.GetInfoCombatWeapons(levelInfo);
                    }
                    else if (levelInfo.Name == "specialWeapons")
                    {
                        this.GetInfoCombatSpecialWeapons(levelInfo);
                    }
                    else if (levelInfo.Name == "abilities")
                    {
                        this.GetInfoCombatAbilities(levelInfo);
                    }
                    else if (levelInfo.Name == "characters")
                    {
                        this.GetInfoCombatCharacters(levelInfo);
                    }
                }
            }
        }

        private void GetInfoCombatWeapons(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;

                string name = "";
                Dice attack = new Dice(0, 0);
                int recharge = 0;
                int uses = 0;

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "name")
                        name = levelDialoguesInfo.InnerText;
                    if (levelDialoguesInfo.Name == "attack")
                    {
                        string[] dice = levelDialoguesInfo.InnerText.Split(',');
                        attack = new Dice(int.Parse(dice[0]), int.Parse(dice[1]));
                    }
                    if (levelDialoguesInfo.Name == "recharge")
                        recharge = int.Parse(levelDialoguesInfo.InnerText);
                    if (levelDialoguesInfo.Name == "uses")
                        uses = int.Parse(levelDialoguesInfo.InnerText);
                }

                WeaponRPGInfo wi = new WeaponRPGInfo(key, name, attack, recharge, uses);
                this.WeaponsCombatInfo.Add(key, wi);
            }
        }

        private void GetInfoCombatSpecialWeapons(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;

                string name = "";
                Dice attack = new Dice(0, 0);
                int recharge = 0;
                int uses = 0;

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "name")
                        name = levelDialoguesInfo.InnerText;
                    if (levelDialoguesInfo.Name == "attack")
                    {
                        string[] dice = levelDialoguesInfo.InnerText.Split(',');
                        attack = new Dice(int.Parse(dice[0]), int.Parse(dice[1]));
                    }
                    if (levelDialoguesInfo.Name == "recharge")
                        recharge = int.Parse(levelDialoguesInfo.InnerText);
                    if (levelDialoguesInfo.Name == "uses")
                        uses = int.Parse(levelDialoguesInfo.InnerText);
                }

                WeaponRPGInfo wi = new WeaponRPGInfo(key, name, attack, recharge, uses);
                this.SpecialWeaponsCombatInfo.Add(key, wi);
            }
        }

        private void GetInfoCombatAbilities(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;
                string name = "";

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "name")
                        name = levelDialoguesInfo.InnerText;
                }

                AbilityRPGInfo ai = new AbilityRPGInfo(key, name);
                this.AbilitiesCombatInfo.Add(key, ai);
            }
        }

        private void GetInfoCombatCharacters(XmlNode levelInfo)
        {
            foreach (XmlNode levelDialogues in levelInfo.ChildNodes)
            {
                string key = levelDialogues.Attributes["key"].Value;

                string name = "";
                string prefab = "";
                CharacterRPGStats stats = null;
                WeaponRPGInfo weapon = null;
                List<WeaponRPGInfo> specialWeapons = new List<WeaponRPGInfo>();
                List<AbilityRPGInfo> abilities = new List<AbilityRPGInfo>();

                foreach (XmlNode levelDialoguesInfo in levelDialogues.ChildNodes)
                {
                    if (levelDialoguesInfo.Name == "name")
                        name = levelDialoguesInfo.InnerText;
                    if (levelDialoguesInfo.Name == "prefab")
                        prefab = levelDialoguesInfo.InnerText;
                    if (levelDialoguesInfo.Name == "stats")
                    {
                        int health = 0;
                        int totalHealth = 0;
                        int initiative = 0;
                        Dice evasion = new Dice(0, 0);
                        int armor = 0;

                        foreach (XmlNode levelDialoguesInnerInfo in levelDialoguesInfo.ChildNodes)
                        {
                            if (levelDialoguesInnerInfo.Name == "health")
                                health = int.Parse(levelDialoguesInnerInfo.InnerText);
                            if (levelDialoguesInnerInfo.Name == "totalHealth")
                                totalHealth = int.Parse(levelDialoguesInnerInfo.InnerText);
                            if (levelDialoguesInnerInfo.Name == "initiative")
                                initiative = int.Parse(levelDialoguesInnerInfo.InnerText);
                            if (levelDialoguesInnerInfo.Name == "evasion")
                            {
                                string[] dice = levelDialoguesInnerInfo.InnerText.Split(',');
                                evasion = new Dice(int.Parse(dice[0]), int.Parse(dice[1]));
                            }
                            if (levelDialoguesInnerInfo.Name == "armor")
                                armor = int.Parse(levelDialoguesInnerInfo.InnerText);
                        }

                        stats = new CharacterRPGStats(health, totalHealth, initiative, evasion, armor);
                    }
                    if (levelDialoguesInfo.Name == "weapon")
                    {
                        WeaponRPGInfo wp = new WeaponRPGInfo(this.WeaponsCombatInfo[levelDialoguesInfo.InnerText]);
                        weapon = new WeaponRPGInfo(wp);
                    }
                    if (levelDialoguesInfo.Name == "specialWeapons")
                    {
                        foreach (XmlNode levelDialoguesInnerInfo in levelDialoguesInfo.ChildNodes)
                        {
                            if (levelDialoguesInnerInfo.Name == "weapon")
                            {
                                WeaponRPGInfo wp = new WeaponRPGInfo(this.SpecialWeaponsCombatInfo[levelDialoguesInnerInfo.InnerText]);
                                specialWeapons.Add(new WeaponRPGInfo(wp));
                            }
                        }
                    }
                    if (levelDialoguesInfo.Name == "abilities")
                    {
                        foreach (XmlNode levelDialoguesInnerInfo in levelDialoguesInfo.ChildNodes)
                        {
                            if (levelDialoguesInnerInfo.Name == "ability")
                            {
                                AbilityRPGInfo ability = new AbilityRPGInfo(this.AbilitiesCombatInfo[levelDialoguesInnerInfo.InnerText]);
                                abilities.Add(new AbilityRPGInfo(ability));
                            }
                        }
                    }
                }

                CharacterRPGInfo ci = new CharacterRPGInfo(key, name, prefab, stats, new CharacterRPGStates(), weapon, specialWeapons, abilities);
                this.CharactersCombatInfo.Add(key, ci);
            }
        }

        #endregion
    }
}