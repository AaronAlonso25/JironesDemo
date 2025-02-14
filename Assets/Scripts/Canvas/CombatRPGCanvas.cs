using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;
using DamageNumbersPro;
using UnityEngine.SceneManagement;

namespace NeonBlood
{
    public enum COMBAT_RPGACTION { NONE, ATTACK, WEAPON, ABILITY, DEFEND };
    public enum COMBAT_RPGPERMISSIONS { NONE, ATTACK, WEAPON, ABILITY, DEFEND, ALL }

    public enum COMBAT_RPGATTACKSTATE { NONE, MISS, CRITICAL, BLOCKED };

    [System.Serializable]
    public struct CombatRPGAttackInfo
    {
        public string AttackIntro;
        public Dice AttackDice;
        public int AttackResult;
        public int AttackArmour;
        public COMBAT_RPGATTACKSTATE AttackState;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CombatRPGCanvas : MonoBehaviour
    {
        [Header("Art")]
        public Transform TeamAxelParent;
        public Transform TeamEnemyParent;
        public List<Transform> TeamAxelPositions;
        public List<Transform> TeamEnemyPositions;

        [Header("Combat")]
        public List<CharacterRPGInfo> TeamAxel;
        public List<CharacterRPGInfo> TeamEnemy;

        [Header("Cameras")]
        public GameObject CameraTeamAxel;
        public GameObject CameraTeamEnemy;

        [Header("UI")]
        public GameObject CombatPanel;
        public CanvasGroup CombatCanvasGroup;
        public GameObject PriorityPanel;
        public GameObject PriorityImagePrefab;
        public GameObject StatsAllyPanel;
        public GameObject StatsEnemyPanel;
        public GameObject StatsAllyPrefab;
        public GameObject StatsEnemyPrefab;
        public GameObject AbilitiesPanel;
        public GameObject AbilitiesContentPanel;
        public GameObject AbilitiesPrefab;
        public GameObject WeaponsPanel;
        public GameObject WeaponsContentPanel;
        public GameObject WeaponsPrefab;
        public GameObject InfoPanel;

        [Header("Main Buttons")]
        public Button MainAttackButton;
        public Button MainWeaponsButton;
        public Button MainAbilitiesButton;
        public Button MainDefendButton;

        [Header("First Buttons")]
        public GameObject FirstCombatButton;
        public GameObject FirstAbilityButton;
        public GameObject FirstWeaponButton;

        [Header("VFX Texts")]
        public GameObject VFXSawNeonRedPrefab;
        public GameObject VFXSawNeonGreenPrefab;
        public GameObject VFXNeonTextMissPrefab;
        public GameObject VFXNeonTextCriticalPrefab;
        public GameObject VFXNeonTextBlockedPrefab;
        public GameObject VFXHitPrefab;
        public GameObject VFXHealPrefab;
        public Vector3 VFXSawOffset = new Vector3(0, -1, 0);
        public Vector3 VFXNeonOffset = new Vector3(0, -0.5f, 0);
        public Vector3 VFXHitOffset = new Vector3(0, 0, 0);
        public Vector3 VFXHealOffset = new Vector3(0, 0, 0);

        [Header("LOG")]
        public TMP_Text LogText;

        [Header("Animations")]
        public Animator StatsPanelAnimator;

        [Header("Debug")]
        public bool CanConfirmAction = false;
        public COMBAT_RPGACTION CombatAction = COMBAT_RPGACTION.NONE;
        public COMBAT_RPGPERMISSIONS CombatPermission = COMBAT_RPGPERMISSIONS.ALL;
        public CombatRPGAttackInfo AttackLogInfo;
        public bool isSelectorActive = false;
        public int indexSelector = 0;
        public bool isJoystickLeft = false;

        private int indexAbility = -1;
        private int indexWeapon = -1;

        void Start()
        {
            TextsManager.Instance.LoadCombatTexts();
            this.InitializeCombat(
                DataManager.Instance.CombatRPGTeamAxel,
                DataManager.Instance.CombatRPGTeamEnemy);
        }

        void Update()
        {
            //Si el selector esta activado
            if (this.isSelectorActive)
            {
                //Muevo el selector
                if (Input.GetAxis("Left Stick Vertical") > 0 && !this.isJoystickLeft)
                {
                    this.isJoystickLeft = true;
                    if (this.CombatAction == COMBAT_RPGACTION.ATTACK ||
                        this.CombatAction == COMBAT_RPGACTION.ABILITY ||
                        this.CombatAction == COMBAT_RPGACTION.WEAPON)
                        this.NextSelectorEnemy(1);
                }
                else if (Input.GetAxis("Left Stick Vertical") < 0 && !this.isJoystickLeft)
                {
                    this.isJoystickLeft = true;
                    if (this.CombatAction == COMBAT_RPGACTION.ATTACK ||
                        this.CombatAction == COMBAT_RPGACTION.ABILITY ||
                        this.CombatAction == COMBAT_RPGACTION.WEAPON)
                        this.NextSelectorEnemy(-1);
                }
                if (Input.GetAxis("Left Stick Vertical") == 0 && this.isJoystickLeft)
                    this.isJoystickLeft = false;

                //Confirmo el ataque
                if (Input.GetButtonDown("Cross_UI") && this.CanConfirmAction)
                {
                    if (this.CombatAction == COMBAT_RPGACTION.ATTACK)
                        StartCoroutine(this.ExecuteTurn(COMBAT_RPGACTION.ATTACK));
                    else if (this.CombatAction == COMBAT_RPGACTION.WEAPON)
                        StartCoroutine(this.ExecuteTurn(COMBAT_RPGACTION.WEAPON));
                    else if (this.CombatAction == COMBAT_RPGACTION.ABILITY)
                        StartCoroutine(this.ExecuteTurn(COMBAT_RPGACTION.ABILITY));
                    this.isSelectorActive = false;
                }

                //Confirmar la cancelacion
                if (Input.GetButtonDown("Circle_UI") && this.CanConfirmAction)
                {
                    if (this.CombatAction == COMBAT_RPGACTION.ATTACK)
                    {
                        this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(false);
                        this.TeamEnemy[this.indexSelector].CharacterHealthBarObject.SetActive(false);
                    }
                    else if (this.CombatAction == COMBAT_RPGACTION.ABILITY)
                    {
                        this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(false);
                        this.TeamEnemy[this.indexSelector].CharacterHealthBarObject.SetActive(false);

                        this.AbilitiesPanel.SetActive(true);
                        this.indexAbility = -1;
                    }
                    else if (this.CombatAction == COMBAT_RPGACTION.WEAPON)
                    {
                        this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(false);
                        this.TeamEnemy[this.indexSelector].CharacterHealthBarObject.SetActive(false);

                        this.WeaponsPanel.SetActive(true);
                        this.indexWeapon = -1;
                    }

                    this.CombatPanel.SetActive(true);

                    this.CombatAction = COMBAT_RPGACTION.NONE;
                    this.CanConfirmAction = false;
                    this.isSelectorActive = false;

                    //Desactivo la camara del Enemigo
                    if (this.CameraTeamEnemy)
                    {
                        this.CameraTeamEnemy.SetActive(false);
                    }
                }
            }

            //Si el panel de habilidades esta activo
            if (this.AbilitiesPanel.activeSelf)
            {
                if (Input.GetButtonDown("Circle_UI"))
                {
                    this.AbilitiesPanel.SetActive(false);
                    this.InfoPanel.SetActive(false);
                    this.CombatCanvasGroup.interactable = true;
                    EventSystem.current.SetSelectedGameObject(this.FirstCombatButton);
                }
            }

            //Si el panel de objetos esta activo
            if (this.WeaponsPanel.activeSelf)
            {
                if (Input.GetButtonDown("Circle_UI"))
                {
                    this.WeaponsPanel.SetActive(false);
                    this.InfoPanel.SetActive(false);
                    this.CombatCanvasGroup.interactable = true;
                    EventSystem.current.SetSelectedGameObject(this.FirstCombatButton);
                }
            }

            //Mostrar StatsPanel
            if(Input.GetButton("L2"))
            {
                this.StatsPanelAnimator.SetBool("Show", true);
            }
            else if(Input.GetButtonUp("L2"))
            {
                this.StatsPanelAnimator.SetBool("Show", false);
            }
        }

        #region Combat

        public void InitializeCombat(List<string> teamAxel, List<string> teamEnemy)
        {
            //Automatizo la busqueda de los objetos no relacionados con el combate
            this.TeamAxelParent = GameObject.Find("AxelParty").transform;
            this.TeamEnemyParent = GameObject.Find("EnemiesParty").transform;


            foreach (Transform child in this.TeamAxelParent)
                this.TeamAxelPositions.Add(child);
            foreach (Transform child in this.TeamEnemyParent)
                this.TeamEnemyPositions.Add(child);

            foreach (string ch in teamAxel)
                this.TeamAxel.Add(new CharacterRPGInfo(TextsManager.Instance.CharactersCombatInfo[ch]));
            foreach (string ch in teamEnemy)
                this.TeamEnemy.Add(new CharacterRPGInfo(TextsManager.Instance.CharactersCombatInfo[ch]));

            //Crear los personajes del equipo de Axel
            for (int i = 0; i < this.TeamAxel.Count; i++)
            {
                GameObject player = Instantiate(
                    Resources.Load<GameObject>("Combat/Prefabs/" + this.TeamAxel[i].CharacterPrefab + "Combat"),
                    this.TeamAxelParent);
                player.transform.position = this.TeamAxelPositions[i].position;

                //Asocio el arte
                this.TeamAxel[i].CharacterCombatObject = player;
                this.TeamAxel[i].CharacterSelectorObject = player.transform.GetChild(1).gameObject;

                //Instanciar el panel de estadisticas
                this.TeamAxel[i].CharacterStatsObject = Instantiate(this.StatsAllyPrefab, this.StatsAllyPanel.transform);
                this.TeamAxel[i].CharacterStatsObject.name = "StatsPanel_" + this.TeamAxel[i].CharacterName;
                this.TeamAxel[i].CharacterStatsObject.GetComponentsInChildren<Image>()[2].sprite = Resources.Load<Sprite>("Combat/UI/CharacterImage_" + this.TeamAxel[i].CharacterName);

                ////Asocio las imagenes de prioridad
                //this.TeamAxel[i].CharacterPrioritySpriteOff = Resources.Load<Sprite>("Combat/UI/PriorityImage_" + this.TeamAxel[i].CharacterName + " Sprite OFF");
                //this.TeamAxel[i].CharacterPrioritySpriteOn = Resources.Load<Sprite>("Combat/UI/PriorityImage_" + this.TeamAxel[i].CharacterName + " Sprite ON");

                //Desactivo el selector
                this.TeamAxel[i].CharacterSelectorObject.SetActive(false);
            }

            //Crear los personajes del equipo de los enemigos
            for (int i = 0; i < this.TeamEnemy.Count; i++)
            {
                GameObject player = Instantiate(
                    Resources.Load<GameObject>("Combat/Prefabs/" + this.TeamEnemy[i].CharacterPrefab + "Combat"),
                    this.TeamEnemyParent);
                player.transform.position = this.TeamEnemyPositions[i].position;

                //Asocio el arte
                this.TeamEnemy[i].CharacterCombatObject = player;
                this.TeamEnemy[i].CharacterSelectorObject = player.transform.GetChild(1).gameObject;

                //Instanciar el panel de estadisticas
                this.TeamEnemy[i].CharacterStatsObject = Instantiate(this.StatsEnemyPrefab, this.StatsEnemyPanel.transform);
                this.TeamEnemy[i].CharacterStatsObject.name = "StatsPanel_" + this.TeamEnemy[i].CharacterName;
                this.TeamEnemy[i].CharacterStatsObject.GetComponentsInChildren<Image>()[2].sprite = Resources.Load<Sprite>("Combat/UI/CharacterImage_" + this.TeamEnemy[i].CharacterName);

                //Asocio la barra de vida
                this.TeamEnemy[i].CharacterHealthBarObject = player.GetComponentInChildren<ProgressBar>().gameObject;
                this.TeamEnemy[i].CharacterHealthBarObject.SetActive(false);

                ////Asocio las imagenes de prioridad
                //this.TeamEnemy[i].CharacterPrioritySpriteOff = Resources.Load<Sprite>("Combat/UI/PriorityImage_" + this.TeamEnemy[i].CharacterName + " Sprite OFF");
                //this.TeamEnemy[i].CharacterPrioritySpriteOn = Resources.Load<Sprite>("Combat/UI/PriorityImage_" + this.TeamEnemy[i].CharacterName + " Sprite ON");

                //Desactivo el selector
                this.TeamEnemy[i].CharacterSelectorObject.SetActive(false);
            }

            this.SetAlliesStatsPanel();
            this.SetEnemiesStatsPanel();

            //Empezamos combate por turnos
            this.NextTurn();
        }

        public void EndCombat(bool victory)
        {
            DataManager.Instance.CombatResult = victory;

            this.CombatPanel.SetActive(false);

            if (victory)
            {
                //Ejecuto los comandos de CheckPoints
                foreach (string command in DataManager.Instance.CombatVictoryCheckPointCommands)
                    this.SetCheckPoint(command);
                foreach (string command in DataManager.Instance.CombatVictoryNPCCommands)
                    this.SetNPCState(command);

                //Voy al nivel
#if UNITY_PS4
                MenuCanvas.Instance.PlayLevel(DataManager.Instance.CurrentLevel);
#else
                MenuCanvas.Instance.PlayLevelDirectly(DataManager.Instance.CurrentLevel);
#endif
            }
            else
                MenuCanvas.Instance.PlayLevelDirectly("EndCombat");
        }

        #endregion

        #region Turn

        public void NextTurn()
        {
            this.CombatPanel.SetActive(true);

            ////Reseteamos los paneles de stats de los aliados y las imagenes de prioridad
            //foreach (CharacterInfo ci in TeamAxel)
            //{
            //    ci.CharacterStatsObject.transform.localScale = Vector3.one * 1.15f;
            //    if (ci.CharacterPriorityObject)
            //    {
            //        ci.CharacterPriorityObject.transform.localScale = Vector3.one;
            //        ci.CharacterPriorityObject.GetComponent<Image>().sprite = ci.CharacterPrioritySpriteOff;
            //    }
            //}
            //foreach (CharacterInfo ci in TeamEnemy)
            //{
            //    if (ci.CharacterPriorityObject)
            //    {
            //        ci.CharacterPriorityObject.transform.localScale = Vector3.one;
            //        ci.CharacterPriorityObject.GetComponent<Image>().sprite = ci.CharacterPrioritySpriteOff;
            //    }
            //}

            //this.indexPriority++;

            ////Si el indice de prioridad supera a la lista resetea la lista
            //if (indexPriority > PriorityOrder.Count - 1)
            //{
            //    this.indexPriority = 0;
            //    this.PriorityOrder = new List<int>();
            //    SetPriorityOrder();
            //}

            ////Si es TeamAxel o Enemigos activo/desactivo el panel
            //CharacterInfo currentFighter = GetFighter(this.PriorityOrder[this.indexPriority]);
            //if (currentFighter.CharacterStats.Health <= 0) //Si esta muerto el currentFighter, paso turno
            //{
            //    //Finalizo el turno
            //    this.EndTurn();
            //}
            //else //Si no esta muerto el currentFighter, hago cosas
            //{
            //    //Destacamos la imagen de prioridad actual
            //    currentFighter.CharacterPriorityObject.transform.localScale = Vector3.one * 1.5f;
            //    currentFighter.CharacterPriorityObject.GetComponent<Image>().sprite = currentFighter.CharacterPrioritySpriteOn;

            //    if (IsTeamAxel(currentFighter))
            //    {
            //        CombatPanel.SetActive(true);

            //        //Destacamos el stats panel del personaje
            //        currentFighter.CharacterStatsObject.transform.localScale = Vector3.one * 1.5f;

            //        //Aumento el Boost del personaje
            //        currentFighter.CharacterStats.Boost = Mathf.Clamp(currentFighter.CharacterStats.Boost + 1, 0, 5);
            //        Debug.Log("Boost " + currentFighter.CharacterStats.Boost);

            //        //Activo la camera del TeamAxel
            //        if (this.CameraTeamAxel != null)
            //            this.CameraTeamAxel.SetActive(true);
            //    }
            //    else
            //    {
            //        CombatPanel.SetActive(false);

            //        //IA
            //        this.AttackIA();

            //        //Activo la camera del TeamEnemy
            //        if (this.CameraTeamEnemy != null)
            //            this.CameraTeamEnemy.SetActive(true);
            //    }
            //}
        }

        private IEnumerator ExecuteTurn(COMBAT_RPGACTION action)
        {
            //Oculto todos los selectores
            this.ShowAllSelectors(false);

            //Si es ABILITY, DEFEND o WEAPON -> Axel Initiative = 999
            if (
                action == COMBAT_RPGACTION.ABILITY ||
                action == COMBAT_RPGACTION.WEAPON ||
                action == COMBAT_RPGACTION.DEFEND
                )
            {
                this.GetAxel().CharacterStats.CharacterInitiative = 999;
            }

            //Si es ABILITY y es Preparation -> Axel Initiative = 0
            if (
                action == COMBAT_RPGACTION.ABILITY &&
                this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Preparation"
                )
            {
                this.GetAxel().CharacterStats.CharacterInitiative = 0;
            }

            foreach (CharacterRPGInfo ci in this.GetPriorityOrder())
            {
                if (this.IsTeamAxel(ci))
                {
                    if (action == COMBAT_RPGACTION.ATTACK)
                    {
                        yield return StartCoroutine(this.AttackAxelCo());
                    }
                    else if (action == COMBAT_RPGACTION.DEFEND)
                    {
                        yield return StartCoroutine(this.DefendAxelCo());
                    }
                    else if (action == COMBAT_RPGACTION.WEAPON)
                    {
                        yield return StartCoroutine(this.WeaponAxelCo(this.indexWeapon));
                    }
                    else if (action == COMBAT_RPGACTION.ABILITY)
                    {
                        yield return StartCoroutine(this.AbilityAxelCo(this.indexAbility));
                    }
                }
                else
                {
                    //Si la habilidad de Axel es Speed
                    if (action == COMBAT_RPGACTION.ABILITY && this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Speed")
                    {
                        //El enemigo no puede hacer nada
                    }
                    else
                    {
                        yield return StartCoroutine(this.AttackIACo(ci));
                    }
                }
            }

            this.EndTurn();

            yield return null;
        }

        private void EndTurn()
        {
            //Recorremos los personajes
            foreach (CharacterRPGInfo ci in this.GetPriorityOrder())
            {
                //Comprobamos si hay algun muerto y quitamos el arte
                if (ci.CharacterStats.CharacterHealth <= 0)
                {
                    StartCoroutine(this.DeathCo(ci));
                }

                //Aumentamos la recarga de las armas especiales
                foreach (WeaponRPGInfo wpi in ci.CharacterSpecialWeapons)
                    wpi.WeaponActualRecharge = Mathf.Clamp(wpi.WeaponActualRecharge + 1, 0, wpi.WeaponRecharge);

                //Reseteamos los estados
                ci.CharacterResetStats();
            }

            //Actualizo las interfaces 
            this.SetAlliesStatsPanel();
            this.SetEnemiesStatsPanel();

            //Comprobamos si el combate ha terminado
            if (this.IsVictory())
            {
                Debug.Log("VICTORY");
               // this.EndCombat(true);
              
                SceneManager.LoadScene("SceneCombat2");
                return;
            }

            if (this.IsDefeat())
            {
                Debug.Log("DEFEAT");
               // this.EndCombat(false);
                 SceneManager.LoadScene("CombatEnd");
                return;
            }

            //Configuro los permisos del siguiente turno 
            this.SetPermissions(COMBAT_RPGPERMISSIONS.ALL);

            //Si vengo de WEAPON, solo permito Esquivar
            if (this.CombatAction == COMBAT_RPGACTION.WEAPON)
                this.SetPermissions(COMBAT_RPGPERMISSIONS.DEFEND);

            //Activo el panel
            this.CombatPanel.SetActive(true);
            this.CombatCanvasGroup.interactable = true;
            EventSystem.current.SetSelectedGameObject(this.GetFirstCombatButton(this.CombatPermission));

            //Oculto el selector y lo reinicio
            this.indexSelector = 0;
            this.indexAbility = -1;
            this.indexWeapon = -1;
            this.isSelectorActive = false;
            this.CanConfirmAction = false;
            this.CombatAction = COMBAT_RPGACTION.NONE;

            //Desactivo la camara del Enemigo y la de Axel
            if (this.CameraTeamEnemy)
                this.CameraTeamEnemy.SetActive(false);
            if (this.CameraTeamAxel)
                this.CameraTeamAxel.SetActive(false);

            this.LogText.text = "";

            //Siguiente turno
            this.NextTurn();
        }

        #endregion

        #region Config

        private List<CharacterRPGInfo> GetPriorityOrder()
        {
            List<CharacterRPGInfo> fighters = new List<CharacterRPGInfo>();
            fighters.AddRange(this.TeamAxel);
            fighters.AddRange(this.TeamEnemy);

            fighters = fighters.OrderByDescending(x => x.CharacterStats.CharacterInitiative).ToList();
            fighters.RemoveAll(x => x.CharacterStats.CharacterHealth <= 0);

            return fighters;
        }

        private CharacterRPGInfo GetFighter(int indexPriority)
        {
            if (indexPriority < this.TeamAxel.Count)
                return this.TeamAxel[indexPriority];
            else
                return this.TeamEnemy[indexPriority - this.TeamAxel.Count];
        }

        private CharacterRPGInfo GetAxel()
        {
            return this.TeamAxel[0];
        }

        private bool IsTeamAxel(CharacterRPGInfo ci)
        {
            if (this.TeamAxel.Contains(ci))
                return true;
            else
                return false;
        }

        private bool IsVictory()
        {
            return this.TeamEnemy.All(x => x.CharacterStats.CharacterHealth <= 0);
        }

        private bool IsDefeat()
        {
            return this.TeamAxel.All(x => x.CharacterStats.CharacterHealth <= 0);
        }

        private void SetAlliesStatsPanel()
        {
            foreach (CharacterRPGInfo ci in this.TeamAxel)
            {
                //Nombre
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[0].text = ci.CharacterName;

                //Progress Bar
                ci.CharacterStatsObject.GetComponentInChildren<ProgressBar>().ProgressBar_MinValue = 0;
                ci.CharacterStatsObject.GetComponentInChildren<ProgressBar>().ProgressBar_MaxValue = ci.CharacterStats.CharacterTotalHealth;
                ci.CharacterStatsObject.GetComponentInChildren<ProgressBar>().SetValue(ci.CharacterStats.CharacterHealth);

                //Armadura
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[3].text = "ARMOUR: " + ci.CharacterStats.CharacterArmor;
                //Iniciativa
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[4].text = "INIT: " + ci.CharacterStats.CharacterInitiative;
                //Weapon Name
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[5].text = "WEAPON: " + ci.CharacterWeapon.WeaponName;
                //Weapon Attack
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[6].text = "ATTACK: " + ci.CharacterWeapon.WeaponAttack.ToString();
            }
        }

        private void SetEnemiesStatsPanel()
        {
            foreach (CharacterRPGInfo ci in this.TeamEnemy)
            {
                //Nombre
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[0].text = ci.CharacterName;

                //Progress Bar
                ci.CharacterStatsObject.GetComponentInChildren<ProgressBar>().ProgressBar_MinValue = 0;
                ci.CharacterStatsObject.GetComponentInChildren<ProgressBar>().ProgressBar_MaxValue = ci.CharacterStats.CharacterTotalHealth;
                ci.CharacterStatsObject.GetComponentInChildren<ProgressBar>().SetValue(ci.CharacterStats.CharacterHealth);

                //Progress Bar InGame
                ci.CharacterHealthBarObject.GetComponent<ProgressBar>().ProgressBar_MinValue = 0;
                ci.CharacterHealthBarObject.GetComponent<ProgressBar>().ProgressBar_MaxValue = ci.CharacterStats.CharacterTotalHealth;
                ci.CharacterHealthBarObject.GetComponent<ProgressBar>().SetValue(ci.CharacterStats.CharacterHealth);

                //Armadura
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[3].text = "ARMOUR: " + ci.CharacterStats.CharacterArmor;
                //Iniciativa
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[4].text = "INIT: " + ci.CharacterStats.CharacterInitiative;
                //Weapon Name
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[5].text = "WEAPON: " + ci.CharacterWeapon.WeaponName;
                //Weapon Attack
                ci.CharacterStatsObject.GetComponentsInChildren<TMP_Text>()[6].text = "ATTACK: " + ci.CharacterWeapon.WeaponAttack.ToString();
            }
        }

        private void SetPermissions(COMBAT_RPGPERMISSIONS perm)
        {
            this.CombatPermission = perm;
            if (this.CombatPermission == COMBAT_RPGPERMISSIONS.NONE)
            {
                this.MainAttackButton.interactable = false;
                this.MainWeaponsButton.interactable = false;
                this.MainAbilitiesButton.interactable = false;
                this.MainDefendButton.interactable = false;
            }
            else if (this.CombatPermission == COMBAT_RPGPERMISSIONS.ALL)
            {
                this.MainAttackButton.interactable = true;
                this.MainWeaponsButton.interactable = true;
                this.MainAbilitiesButton.interactable = true;
                this.MainDefendButton.interactable = true;
            }
            else
            {
                this.MainAttackButton.interactable = (this.CombatPermission == COMBAT_RPGPERMISSIONS.ATTACK);
                this.MainWeaponsButton.interactable = (this.CombatPermission == COMBAT_RPGPERMISSIONS.WEAPON);
                this.MainAbilitiesButton.interactable = (this.CombatPermission == COMBAT_RPGPERMISSIONS.ABILITY);
                this.MainDefendButton.interactable = (this.CombatPermission == COMBAT_RPGPERMISSIONS.DEFEND);
            }
        }

        private GameObject GetFirstCombatButton(COMBAT_RPGPERMISSIONS perm)
        {
            if (perm == COMBAT_RPGPERMISSIONS.ATTACK)
                return this.MainAttackButton.gameObject;
            if (perm == COMBAT_RPGPERMISSIONS.WEAPON)
                return this.MainWeaponsButton.gameObject;
            if (perm == COMBAT_RPGPERMISSIONS.ABILITY)
                return this.MainAbilitiesButton.gameObject;
            if (perm == COMBAT_RPGPERMISSIONS.DEFEND)
                return this.MainDefendButton.gameObject;
            else
                return this.MainAttackButton.gameObject;
        }

        #endregion

        #region Actions Axel

        public void Attack()
        {
            //Oculto el panel
            this.CombatPanel.SetActive(false);

            //Asocio la accion
            this.CombatAction = COMBAT_RPGACTION.ATTACK;

            //Muestro el selector
            StartCoroutine(this.ShowSelectorEnemyCo());
        }

        public void Defend()
        {
            //Oculto el panel
            this.CombatPanel.SetActive(false);

            //Asocio la accion  
            this.CombatAction = COMBAT_RPGACTION.DEFEND;

            //Activo la defensa de Axel
            this.GetAxel().CharacterStates.IsDefend = true;
            this.GetAxel().CharacterStates.DefendValue = this.GetAxel().CharacterStats.CharacterEvasion.DiceRoll();

            //LOG
            Debug.Log("Axel esta evadiendo con un valor de " + this.GetAxel().CharacterStates.DefendValue);

            StartCoroutine(this.ExecuteTurn(COMBAT_RPGACTION.DEFEND));
        }

        public void Weapons()
        {
            //Destruyo las armas previas
            foreach (Transform child in this.WeaponsContentPanel.transform)
                Destroy(child.gameObject);

            //Si no hay armas, me salgo
            if (this.GetAxel().CharacterSpecialWeapons.Count == 0)
                return;

            //Creo las armas nuevas y las configuro
            for (int i = 0; i < this.GetAxel().CharacterSpecialWeapons.Count; i++)
            {
                int indexWeapon = i;

                GameObject button = Instantiate(this.WeaponsPrefab, this.WeaponsContentPanel.transform);
                button.GetComponent<Button>().onClick.AddListener(() => this.Weapon(indexWeapon));
                button.GetComponentInChildren<TMP_Text>().text =
                    this.GetAxel().CharacterSpecialWeapons[i].WeaponName;

                this.AddWeaponEventTriggerListener(button.GetComponent<EventTrigger>(), EventTriggerType.Select, indexWeapon);

                //Si ya no tiene usos
                if (this.GetAxel().CharacterSpecialWeapons[i].WeaponUses == 0 ||
                    this.GetAxel().CharacterSpecialWeapons[i].WeaponActualRecharge != this.GetAxel().CharacterSpecialWeapons[i].WeaponRecharge)
                {
                    button.GetComponentsInChildren<Image>()[1].enabled = true;
                    button.GetComponent<Image>().color = Color.gray;
                }                    

                if (indexWeapon == 0)
                    this.FirstWeaponButton = button;
            }

            //Activo el panel de armas y asocio el primer boton
            this.WeaponsPanel.SetActive(true);
            this.CombatCanvasGroup.interactable = false;
            EventSystem.current.SetSelectedGameObject(this.FirstWeaponButton);

            //Activo el panel de info
            this.InfoPanel.SetActive(true);
            this.ShowWeaponInfoPanel(0);
        }

        public void Weapon(int indexWeapon)
        {
            //Si ya no tiene usos
            if (this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponUses == 0 ||
                this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponActualRecharge != this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponRecharge)
                return;

            //Oculto el panel
            this.CombatPanel.SetActive(false);
            this.WeaponsPanel.SetActive(false);
            this.InfoPanel.SetActive(false);

            //Asocio la accion
            this.CombatAction = COMBAT_RPGACTION.WEAPON;
            this.indexWeapon = indexWeapon;

            //Muestro el selector
            StartCoroutine(this.ShowSelectorEnemyCo());
        }

        public void Abilities()
        {
            //Destruyo las habilidades previas
            foreach (Transform child in this.AbilitiesContentPanel.transform)
                Destroy(child.gameObject);

            //Si no hay habilidades, me salgo
            if (this.GetAxel().CharacterAbilities.Count == 0)
                return;

            //Creo las habilidades nuevas y las configuro
            for (int i = 0; i < this.GetAxel().CharacterAbilities.Count; i++)
            {
                int indexAbility = i;

                GameObject button = Instantiate(this.AbilitiesPrefab, this.AbilitiesContentPanel.transform);
                button.GetComponent<Button>().onClick.AddListener(() => this.Ability(indexAbility));
                button.GetComponentInChildren<TMP_Text>().text =
                    this.GetAxel().CharacterAbilities[i].AbilityName;

                this.AddAbilityEventTriggerListener(button.GetComponent<EventTrigger>(), EventTriggerType.Select, indexAbility);

                if (indexAbility == 0)
                    this.FirstAbilityButton = button;
            }

            //Activo el panel de habilidades y asocio el primer boton
            this.AbilitiesPanel.SetActive(true);
            this.CombatCanvasGroup.interactable = false;
            EventSystem.current.SetSelectedGameObject(this.FirstAbilityButton);

            //Activo el panel de info
            this.InfoPanel.SetActive(true);
            this.ShowAbilityInfoPanel(0);
        }

        public void Ability(int indexAbility)
        {
            //Oculto el panel
            this.CombatPanel.SetActive(false);
            this.AbilitiesPanel.SetActive(false);
            this.InfoPanel.SetActive(false);

            //Asocio la accion
            this.CombatAction = COMBAT_RPGACTION.ABILITY;
            this.indexAbility = indexAbility;

            //Muestro el selector si la habilidad no es MetabolicAdaptation
            if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID != "MetabolicAdaptation")
                StartCoroutine(this.ShowSelectorEnemyCo());
            else
                StartCoroutine(this.ExecuteTurn(COMBAT_RPGACTION.ABILITY));
        }

        private IEnumerator AttackAxelCo()
        {
            //Desactivo la camara del Enemigo
            if (this.CameraTeamEnemy)
            {
                this.CameraTeamEnemy.SetActive(false);
                yield return new WaitForSeconds(1);
            }

            float damage = this.AttackFormula(this.GetAxel(), this.TeamEnemy[this.indexSelector]);
            this.TeamEnemy[this.indexSelector].ApplyDamage(damage);

            //LOG
            Debug.Log("Axel ataca a " + this.TeamEnemy[this.indexSelector].CharacterName + " con el arma " + this.GetAxel().CharacterWeapon.WeaponName +
                " inflingiendo " + damage + " de da�o");

            this.AttackLogInfo.AttackIntro = "Axel Attack " + this.GetAxel().CharacterWeapon.WeaponName;
           // this.ShowLog(this.AttackLogInfo);

            //Desactivo el selector
            this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(false);

            //Animaciones
            yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Normal Attack"));
            yield return StartCoroutine(this.FlickerCharacter(this.TeamEnemy[this.indexSelector])); //�No hay animacion de Hit?

            //VFX
            this.ExecuteDamageVFX(
                this.VFXSawNeonRedPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, damage, Color.white);
            this.ExecuteAttackStateVFX(
                this.AttackLogInfo.AttackState, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, Color.white);
            this.ExecuteHitVFX(
                this.VFXHitPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position);

            //Actualizamos interfaz
            this.SetEnemiesStatsPanel();

            yield return null;
        }

        private IEnumerator DefendAxelCo()
        {
            //Activo la camara de Axel
            if (this.CameraTeamAxel)
            {
                this.CameraTeamAxel.SetActive(true);
                yield return new WaitForSeconds(1);
            }

            this.LogText.text = "Axel Defend";

            //Animaciones
            yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Defense"));

            //Desactivo la camara de Axel
            if (this.CameraTeamAxel)
                this.CameraTeamAxel.SetActive(false);

            yield return null;
        }

        private IEnumerator WeaponAxelCo(int indexWeapon)
        {
            /*
             * SmokeBomb                OK
             * Stunner                  OK
             * Grenade                  OK                 
             * ArmorPiercingBulletGun   OK
             * Antielectronics          OK
             */

            //Activo la camara de Axel
            if (this.CameraTeamAxel)
            {
                this.CameraTeamAxel.SetActive(true);
                yield return new WaitForSeconds(1);
            }

            //Desactivo la camara del Enemigo
            if (this.CameraTeamEnemy)
            {
                this.CameraTeamEnemy.SetActive(false);
            }

            this.LogText.text = "Axel Uses " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponName;

            if (this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponID == "SmokeBomb")
            {
                this.TeamEnemy[this.indexSelector].CharacterStates.IsBlind = true;

                //Animaciones
                yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Normal Attack"));
                yield return StartCoroutine(this.FlickerCharacter(this.TeamEnemy[this.indexSelector])); //�No hay animacion de Hit?
            }
            else if (this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponID == "Stunner")
            {
                Dice stunnerDice = this.StunnerDice(this.GetAxel().CharacterSpecialWeapons[indexWeapon]);

                float damage = this.AttackWeaponFormula(this.GetAxel(), indexWeapon, this.TeamEnemy[this.indexSelector], stunnerDice, 0);
                this.TeamEnemy[this.indexSelector].ApplyDamage(damage);

                //LOG
                Debug.Log("Axel ataca a " + this.TeamEnemy[this.indexSelector].CharacterName + " con el arma " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponName +
                    " inflingiendo " + damage + " de da�o");

                //Animaciones
                yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Normal Attack"));
                yield return StartCoroutine(this.FlickerCharacter(this.TeamEnemy[this.indexSelector])); //�No hay animacion de Hit?

                //VFX
                this.ExecuteDamageVFX(
                    this.VFXSawNeonRedPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, damage, Color.white);
                this.ExecuteAttackStateVFX(
                    this.AttackLogInfo.AttackState, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, Color.white);
                this.ExecuteHitVFX(
                    this.VFXHitPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position);
            }
            else if (this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponID == "Grenade")
            {
                float flatDamage = this.AttackWeaponMinimalFormula(this.GetAxel(), indexWeapon);

                List<float> damages = new List<float>();
                foreach (CharacterRPGInfo enemy in this.GrenadeArea())
                {
                    float damage = this.AttackWeaponFormula(this.GetAxel(), indexWeapon, enemy, null, flatDamage);
                    enemy.ApplyDamage(damage);

                    damages.Add(damage);

                    //LOG
                    Debug.Log("Axel ataca a " + enemy.CharacterName + " con el arma " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponName +
                        " inflingiendo " + damage + " de da�o");
                }

                //Animaciones
                yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Normal Attack"));
                for (int i = 0; i < this.GrenadeArea().Count; i++)
                {
                    StartCoroutine(this.FlickerCharacter(this.GrenadeArea()[i])); //�No hay animacion de Hit?

                    //VFX
                    this.ExecuteDamageVFX(
                        this.VFXSawNeonRedPrefab, this.GrenadeArea()[i].CharacterSelectorObject.transform.position, damages[i], Color.white);
                    this.ExecuteAttackStateVFX(
                        this.AttackLogInfo.AttackState, this.GrenadeArea()[i].CharacterSelectorObject.transform.position, Color.white);
                    this.ExecuteHitVFX(
                        this.VFXHitPrefab, this.GrenadeArea()[i].CharacterSelectorObject.transform.position);
                }
            }
            else
            {
                float damage = this.AttackWeaponFormula(this.GetAxel(), indexWeapon, this.TeamEnemy[this.indexSelector]);
                this.TeamEnemy[this.indexSelector].ApplyDamage(damage);

                //LOG
                Debug.Log("Axel ataca a " + this.TeamEnemy[this.indexSelector].CharacterName + " con el arma " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponName +
                    " inflingiendo " + damage + " de da�o");

                //Animaciones
                yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Normal Attack"));
                yield return StartCoroutine(this.FlickerCharacter(this.TeamEnemy[this.indexSelector])); //�No hay animacion de Hit?

                //VFX
                this.ExecuteDamageVFX(
                    this.VFXSawNeonRedPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, damage, Color.white);
                this.ExecuteAttackStateVFX(
                    this.AttackLogInfo.AttackState, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, Color.white);
                this.ExecuteHitVFX(
                        this.VFXHitPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position);
            }

            //Reducimos la recarga y el uso del arma
            this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponActualRecharge = 0;
            this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponUses =
                Mathf.Clamp(this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponUses - 1, 0, this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponUses);

            //Actualizamos interfaz
            this.SetEnemiesStatsPanel();

            //Desactivo el selector
            this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(false);

            //Desactivo la camara de Axel
            if (this.CameraTeamAxel)
                this.CameraTeamAxel.SetActive(false);

            yield return null;
        }

        private IEnumerator AbilityAxelCo(int indexAbility)
        {
            /*
             * Preparation              OK
             * Initiative               OK
             * Anticipation             OK     
             * ProtectiveField          OK
             * Speed                    OK
             * MetabolicAdaptation      OK
             */

            //Activo la camara de Axel
            if (this.CameraTeamAxel)
            {
                this.CameraTeamAxel.SetActive(true);
                yield return new WaitForSeconds(1);
            }

            //Desactivo la camara del Enemigo
            if (this.CameraTeamEnemy)
            {
                this.CameraTeamEnemy.SetActive(false);
            }

            this.LogText.text = "Axel Uses " + this.GetAxel().CharacterAbilities[indexAbility].AbilityName;

            float damage = 0;

            if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Preparation" ||
                this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Initiative" ||
                this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Anticipation"
                )
            {
                damage = this.AttackAbilityFormula(this.GetAxel(), indexAbility, this.TeamEnemy[this.indexSelector]);
                this.TeamEnemy[this.indexSelector].ApplyDamage(damage);

                //LOG
                Debug.Log("Axel ataca a " + this.TeamEnemy[this.indexSelector].CharacterName + " con la habilidad " + this.GetAxel().CharacterAbilities[indexAbility].AbilityName +
                    " inflingiendo " + damage + " de da�o");
            }
            else if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "ProtectiveField")
            {
                //Ciego a los enemigos
                foreach (CharacterRPGInfo ci in this.GetPriorityOrder())
                {
                    if (!this.IsTeamAxel(ci))
                        ci.CharacterStates.IsBlind = true;
                }

                damage = this.AttackAbilityFormula(this.GetAxel(), indexAbility, this.TeamEnemy[this.indexSelector]);
                this.TeamEnemy[this.indexSelector].ApplyDamage(damage);

                //LOG
                Debug.Log("Axel ataca a " + this.TeamEnemy[this.indexSelector].CharacterName + " con la habilidad " + this.GetAxel().CharacterAbilities[indexAbility].AbilityName +
                    " inflingiendo " + damage + " de da�o");
            }
            else if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Speed")
            {
                //Reduccion de Vida
                Dice healthDice = new Dice(1, 10);
                this.GetAxel().ApplyDamage(healthDice.DiceRoll());

                //Damage
                damage = this.AttackAbilityFormula(this.GetAxel(), indexAbility, this.TeamEnemy[this.indexSelector]);
                this.TeamEnemy[this.indexSelector].ApplyDamage(damage);

                //LOG
                Debug.Log("Axel ataca a " + this.TeamEnemy[this.indexSelector].CharacterName + " con la habilidad " + this.GetAxel().CharacterAbilities[indexAbility].AbilityName +
                    " inflingiendo " + damage + " de da�o");
            }
            else if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "MetabolicAdaptation")
            {
                //Aumento de Vida
                Dice healthDice = new Dice(2, 8);

                damage = -healthDice.DiceRoll();
                this.GetAxel().ApplyDamage(damage);
            }
            else
            {
                damage = this.AttackWeaponFormula(this.GetAxel(), indexWeapon, this.TeamEnemy[this.indexSelector]);
                this.TeamEnemy[this.indexSelector].ApplyDamage(damage);

                //LOG
                Debug.Log("Axel ataca a " + this.TeamEnemy[this.indexSelector].CharacterName + " con el arma " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponName +
                    " inflingiendo " + damage + " de da�o");
            }

            //Animaciones
            if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID != "MetabolicAdaptation")
            {
                yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Normal Attack"));
                yield return StartCoroutine(this.FlickerCharacter(this.TeamEnemy[this.indexSelector])); //�No hay animacion de Hit?
            }

            //VFX
            if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID != "MetabolicAdaptation")
            {
                this.ExecuteDamageVFX(
                    this.VFXSawNeonRedPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, damage, Color.white);
                this.ExecuteAttackStateVFX(
                    this.AttackLogInfo.AttackState, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position, Color.white);
                this.ExecuteHitVFX(
                        this.VFXHitPrefab, this.TeamEnemy[this.indexSelector].CharacterSelectorObject.transform.position);
            }
            else
            {
                this.ExecuteDamageVFX(
                    this.VFXSawNeonGreenPrefab, this.GetAxel().CharacterSelectorObject.transform.position, -damage, Color.white);
                this.ExecuteHealVFX(
                    this.VFXHealPrefab, this.GetAxel().CharacterSelectorObject.transform.position);
                yield return new WaitForSeconds(0.5f);
            }

            //Actualizamos interfaz
            this.SetEnemiesStatsPanel();

            //Desactivo el selector
            this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(false);

            //Desactivo la camara de Axel
            if (this.CameraTeamAxel)
                this.CameraTeamAxel.SetActive(false);

            yield return null;
        }

        private IEnumerator DeathCo(CharacterRPGInfo character)
        {
            yield return StartCoroutine(this.ExecuteCharacterAnimation(character, "Death"));
        }

        #endregion

        #region Actions IA

        private IEnumerator AttackIACo(CharacterRPGInfo enemy)
        {
            //Desactivo la camara del Enemigo
            if (this.CameraTeamEnemy)
            {
                this.CameraTeamEnemy.SetActive(false);
            }

            float damage = this.AttackFormula(enemy, this.GetAxel());
            this.GetAxel().ApplyDamage(damage);

            //LOG
            Debug.Log(enemy.CharacterName + " ataca a Axel con el arma " + enemy.CharacterWeapon.WeaponName +
                " inflingiendo " + damage + " de da�o");

            this.AttackLogInfo.AttackIntro = enemy.CharacterName + " Attack " + enemy.CharacterWeapon.WeaponName;
           // this.ShowLog(this.AttackLogInfo);

            //Animaciones
            yield return StartCoroutine(this.ExecuteCharacterAnimation(enemy, "Normal Attack"));

            //VFX
            this.ExecuteDamageVFX(
                    this.VFXSawNeonRedPrefab, this.GetAxel().CharacterSelectorObject.transform.position, damage, Color.white);
            this.ExecuteAttackStateVFX(
                this.AttackLogInfo.AttackState, this.GetAxel().CharacterSelectorObject.transform.position, Color.white);
            this.ExecuteHitVFX(
                    this.VFXHitPrefab, this.GetAxel().CharacterSelectorObject.transform.position);

            //Animaci�n Hit de Axel
            yield return StartCoroutine(this.ExecuteCharacterAnimation(this.GetAxel(), "Hit"));

            //Actualizamos interfaz
            this.SetAlliesStatsPanel();

            yield return null;
        }

        private IEnumerator DefendIACo(CharacterRPGInfo enemy)
        {
            yield return null;
        }

        private IEnumerator WeaponIACo()
        {
            yield return null;
        }

        #endregion

        #region Selector

        private IEnumerator ShowSelectorEnemyCo()
        {
            //Encuentro a un target vivo. Mientras este muerto, sigo buscando
            this.indexSelector = this.TeamEnemy.IndexOf(this.TeamEnemy.First(x => x.CharacterStats.CharacterHealth > 0));
            this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(true);
            this.TeamEnemy[this.indexSelector].CharacterHealthBarObject.SetActive(true);

            //Activo la camara del Enemigo
            if (this.CameraTeamEnemy)
            {
                this.CameraTeamEnemy.SetActive(true);
                yield return new WaitForSeconds(0.3f);
            }

            //Permito mover el selector
            this.isSelectorActive = true;
            //Delay para poder confirmar el ataque
            yield return new WaitForEndOfFrame();
            //Permito atacar
            this.CanConfirmAction = true;
        }

        private void NextSelectorEnemy(int sign)
        {
            //Desactivo el selector en la posicion antigua
            this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(false);
            this.TeamEnemy[this.indexSelector].CharacterHealthBarObject.SetActive(false);

            //Itero el selector
            this.indexSelector = Mathf.Clamp(this.indexSelector + sign, 0, this.TeamEnemy.Count - 1);

            //Activo el selector en la posicion nueva
            this.TeamEnemy[this.indexSelector].CharacterSelectorObject.SetActive(true);
            this.TeamEnemy[this.indexSelector].CharacterHealthBarObject.SetActive(true);
        }

        private void ShowAllSelectors(bool active)
        {
            foreach (CharacterRPGInfo character in this.TeamEnemy)
            {
                character.CharacterSelectorObject.SetActive(active);
                character.CharacterHealthBarObject.SetActive(active);
            }                
        }

        #endregion

        #region Animations

        private IEnumerator FlickerCharacter(CharacterRPGInfo ci)
        {
            SpriteRenderer sp = ci.CharacterCombatObject.GetComponentInChildren<SpriteRenderer>();
            sp.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sp.enabled = true;
        }

        private IEnumerator ExecuteMoveAnimation(int sign)
        {
            //Animacion del personaje de aproximamiento
            //this.GetFighter(this.PriorityOrder[this.indexPriority]).CharacterCombatObject.
            //    GetComponentInChildren<Animator>().SetFloat("Horizontal", -1 * sign);
            //this.GetFighter(this.PriorityOrder[this.indexPriority]).CharacterCombatObject.
            //    GetComponentInChildren<Animator>().SetFloat("Magnitude", 1);

            //Movimiento del personaje
            //yield return StartCoroutine(MMMovement.MoveFromTo(
            //    this.GetFighter(this.PriorityOrder[this.indexPriority]).CharacterCombatObject.gameObject,
            //    this.GetFighter(this.PriorityOrder[this.indexPriority]).CharacterCombatObject.transform.position,
            //    this.GetFighter(this.PriorityOrder[this.indexPriority]).CharacterCombatObject.transform.position + new Vector3(-2.5f * sign, 0, 0),
            //    1, 0.01f));

            yield return new WaitForSeconds(0);

            //Animacion del personaje de aproximamiento
            //this.GetFighter(this.PriorityOrder[this.indexPriority]).CharacterCombatObject.
            //    GetComponentInChildren<Animator>().SetFloat("Horizontal", 0);
            //this.GetFighter(this.PriorityOrder[this.indexPriority]).CharacterCombatObject.
            //    GetComponentInChildren<Animator>().SetFloat("Magnitude", 0);
        }

        private IEnumerator ExecuteCharacterAnimation(CharacterRPGInfo character, string triggerAnimation)
        {
            Animator anim = character.CharacterCombatObject.GetComponentInChildren<Animator>();
            anim.SetTrigger(triggerAnimation);

            float delay = 1;
            if (anim.runtimeAnimatorController.animationClips.Any(a => a.name.Contains(triggerAnimation)))
                delay = anim.runtimeAnimatorController.animationClips.First(a => a.name.Contains(triggerAnimation)).length;

            yield return new WaitForSeconds(delay);
        }

        #endregion

        #region VFX

        private void ExecuteDamageVFX(GameObject vfxPrefab, Vector3 position, float damage, Color color)
        {
            //if (damage == 0)
            //    return;

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = position + this.VFXSawOffset;

            DamageNumber dn = vfx.GetComponent<DamageNumber>();
            dn.number = damage;
            dn.SetColor(color);
        }

        private void ExecuteAttackStateVFX(COMBAT_RPGATTACKSTATE state, Vector3 position, Color color)
        {
            if (state == COMBAT_RPGATTACKSTATE.MISS)
            {
                this.ExecuteTextVFX(this.VFXNeonTextMissPrefab, position, "Combat_VFX_Miss", color);
            }
            else if (state == COMBAT_RPGATTACKSTATE.CRITICAL)
            {
                this.ExecuteTextVFX(this.VFXNeonTextCriticalPrefab, position, "Combat_VFX_Critical", color);
            }
            else if (state == COMBAT_RPGATTACKSTATE.BLOCKED)
            {
                this.ExecuteTextVFX(this.VFXNeonTextBlockedPrefab, position, "Combat_VFX_Blocked", color);
            }

            this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.NONE;
        }

        private void ExecuteTextVFX(GameObject vfxPrefab, Vector3 position, string key, Color color)
        {
            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = position + this.VFXNeonOffset;

            DamageNumber dn = vfx.GetComponent<DamageNumber>();
            dn.SetColor(color);
        }

        private void ExecuteHitVFX(GameObject vfxPrefab, Vector3 position)
        {
            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = position + this.VFXHitOffset;
        }

        private void ExecuteHealVFX(GameObject vfxPrefab, Vector3 position)
        {
            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = position + this.VFXHealOffset;
        }

        #endregion

        #region LOG

        private void ShowLog(CombatRPGAttackInfo info)
        {
            this.LogText.text = info.AttackIntro + "\n";
            this.LogText.text += "Result " + info.AttackDice + ": "  + info.AttackResult + "\n";
            this.LogText.text += "Armour: " + info.AttackArmour;
        }

        #endregion

        #region Formulas

        private float AttackFormula(CharacterRPGInfo attacker, CharacterRPGInfo receiver)
        {
            Dice dice = attacker.CharacterWeapon.WeaponAttack;
            int diceRoll = dice.DiceRoll();

            //LOG
            this.AttackLogInfo.AttackDice = dice;
            this.AttackLogInfo.AttackResult = diceRoll;
            this.AttackLogInfo.AttackArmour = receiver.CharacterStats.CharacterArmor;

            //Blindness
            if (attacker.CharacterStates.IsBlind)
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }

            float damage = 0;
            if (diceRoll == dice.Value.x) //Fallo 
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }
            else if (diceRoll == dice.Value.y) //Critico
            {
                damage = diceRoll * 2;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE CRITICO. Da�o x2");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
            }
            else //Normal
            {
                damage = diceRoll;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
            }

            //Evasion
            if (receiver.CharacterStates.IsDefend)
            {
                int evasion = receiver.CharacterStates.DefendValue;
                if (evasion == 1) //Pifia
                {
                    damage *= 2;
                    Debug.Log(receiver.CharacterName + " ha evadido pero ha hecho una PIFIA. Da�o x2");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
                }
                else if (evasion == 2 || evasion == 3) //Fracaso
                {
                    //damage = damage;
                    Debug.Log(receiver.CharacterName + " ha evadido pero ha hecho un FRACASO. Da�o x1");
                }
                else if (evasion == 4 || evasion == 5) //Exito
                {
                    damage /= 2;
                    Debug.Log(receiver.CharacterName + " ha evadido y ha hecho un EXITO. Da�o /2");
                }
                else if (evasion == 6) //Critico
                {
                    damage = 0;
                    Debug.Log(receiver.CharacterName + " ha evadido y ha hecho un CRITICO. Da�o x0");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.BLOCKED;
                }
            }

            //Armadura
            damage = Mathf.Clamp(damage - receiver.CharacterStats.CharacterArmor, 0, receiver.CharacterStats.CharacterTotalHealth);

            return damage;
        }

        private float AttackWeaponFormula(CharacterRPGInfo attacker, int indexWeapon, CharacterRPGInfo receiver,
            Dice overrideDice = null, float flatDamage = 0)
        {
            Dice dice = (overrideDice == null) ? attacker.CharacterSpecialWeapons[indexWeapon].WeaponAttack : overrideDice;
            int diceRoll = dice.DiceRoll();

            //Blindness
            if (attacker.CharacterStates.IsBlind)
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }

            float damage = 0;
            if (diceRoll == dice.Value.x) //Fallo 
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }
            else if (diceRoll == dice.Value.y) //Critico
            {
                damage = diceRoll * 2;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE CRITICO. Da�o x2");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
            }
            else //Normal
            {
                damage = diceRoll;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
            }

            //Flat Damage
            damage = (flatDamage != 0) ? flatDamage : damage;

            //Evasion
            if (receiver.CharacterStates.IsDefend)
            {
                int evasion = receiver.CharacterStates.DefendValue;
                if (evasion == 1) //Pifia
                {
                    damage *= 2;
                    Debug.Log(receiver.CharacterName + " ha evadido pero ha hecho una PIFIA. Da�o x2");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
                }
                else if (evasion == 2 || evasion == 3) //Fracaso
                {
                    //damage = damage;
                    Debug.Log(receiver.CharacterName + " ha evadido pero ha hecho un FRACASO. Da�o x1");
                }
                else if (evasion == 4 || evasion == 5) // Exito
                {
                    damage /= 2;
                    Debug.Log(receiver.CharacterName + " ha evadido y ha hecho un EXITO. Da�o /2");
                }
                else if (evasion == 6) //Critico
                {
                    damage = 0;
                    Debug.Log(receiver.CharacterName + " ha evadido y ha hecho un CRITICO. Da�o x0");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.BLOCKED;
                }
            }

            //Armadura
            damage = Mathf.Clamp(damage - receiver.CharacterStats.CharacterArmor, 0, receiver.CharacterStats.CharacterTotalHealth);

            return damage;
        }

        private float AttackWeaponMinimalFormula(CharacterRPGInfo attacker, int indexWeapon)
        {
            Dice dice = attacker.CharacterSpecialWeapons[indexWeapon].WeaponAttack;
            int diceRoll = dice.DiceRoll();

            float damage = 0;
            if (diceRoll == dice.Value.x) //Fallo 
            {
                damage = 0;
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;
            }
            else if (diceRoll == dice.Value.y) //Critico
            {
                damage = diceRoll * 2;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE CRITICO. Da�o x2");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
            }
            else //Normal
            {
                damage = diceRoll;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
            }

            return damage;
        }

        private float AttackAbilityFormula(CharacterRPGInfo attacker, int indexAbility, CharacterRPGInfo receiver)
        {
            Dice dice = attacker.CharacterWeapon.WeaponAttack;
            int diceRoll = dice.DiceRoll();

            //Blindness
            if (attacker.CharacterStates.IsBlind)
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }

            float damage = 0;

            //Si la habilidad es Preparation -> CRITICO es dice.Y ... dice.Y - 2
            if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Preparation")
            {
                damage = this.GetPreparationDamage(dice, diceRoll, attacker, receiver);
            }
            //Si la habilidad es Initiative -> No hay CRITICO
            else if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Initiative")
            {
                damage = this.GetInitiativeDamage(dice, diceRoll, attacker, receiver);
            }
            //Si la habilidad es Anticipation -> PIFIA es 1, 2, 3
            else if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Anticipation")
            {
                damage = this.GetAnticipationDamage(dice, diceRoll, attacker, receiver);
            }
            //Si la habilidad es Speed -> Damage x2
            else if (this.GetAxel().CharacterAbilities[indexAbility].AbilityID == "Speed")
            {
                damage = this.GetSpeedDamage(dice, diceRoll, attacker, receiver);
            }
            //CRITICO NORMAL
            else
            {
                if (diceRoll == dice.Value.x) //Fallo 
                {
                    Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                    return 0;
                }
                else if (diceRoll == dice.Value.y) //Critico
                {
                    damage = diceRoll * 2;
                    Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE CRITICO. Da�o x2");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
                }
                else //Normal
                {
                    damage = diceRoll;
                    Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
                }
            }

            //Evasion / Si la habilidad es Anticipation -> No Evasion
            if (receiver.CharacterStates.IsDefend && this.GetAxel().CharacterAbilities[indexAbility].AbilityID != "Anticipation")
            {
                int evasion = receiver.CharacterStates.DefendValue;
                if (evasion == 1) //Pifia
                {
                    damage *= 2;
                    Debug.Log(receiver.CharacterName + " ha evadido pero ha hecho una PIFIA. Da�o x2");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
                }
                else if (evasion == 2 || evasion == 3) //Fracaso
                {
                    //damage = damage;
                    Debug.Log(receiver.CharacterName + " ha evadido pero ha hecho un FRACASO. Da�o x1");
                }
                else if (evasion == 4 || evasion == 5) //Exito
                {
                    damage /= 2;
                    Debug.Log(receiver.CharacterName + " ha evadido y ha hecho un EXITO. Da�o /2");
                }
                else if (evasion == 6) //Critico
                {
                    damage = 0;
                    Debug.Log(receiver.CharacterName + " ha evadido y ha hecho un CRITICO. Da�o x0");
                    this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.BLOCKED;
                }
            }

            //Armadura
            damage = Mathf.Clamp(damage - receiver.CharacterStats.CharacterArmor, 0, receiver.CharacterStats.CharacterTotalHealth);

            return damage;
        }

        #endregion

        #region Save

        public void SetCheckPoint(string nameScene_CheckPointIndex)
        {
            SaveDataLevel sdl = DataManager.Instance.GetCheckPoint(nameScene_CheckPointIndex.Split(',')[0]);
            int currentIndex = int.Parse(nameScene_CheckPointIndex.Split(',')[1]);

            //Si el checkPoint es anterior al que asocio, lo asocio
            if (sdl.CurrentCheckPointIndex <= currentIndex)
                sdl.CurrentCheckPointIndex = currentIndex;
        }

        public void SetNPCState(string nameScene_nameNPC_StateIndex)
        {
            SaveDataLevel sdl = DataManager.Instance.GetCheckPoint(nameScene_nameNPC_StateIndex.Split(',')[0]);
            string nameNPC = nameScene_nameNPC_StateIndex.Split(',')[1];
            int stateNPC = int.Parse(nameScene_nameNPC_StateIndex.Split(',')[2]);

            NPCInfo npci = sdl.CurrentPlayerNPCs.Single(x => x.NameNPC == nameNPC);
            npci.StateNPC = stateNPC;

            //Si el estado es anterior al que asocio, lo asocio
            if (npci.StateNPC <= stateNPC)
                npci.StateNPC = stateNPC;
        }

        #endregion

        #region Special Weapons

        private void AddWeaponEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, int indexWeapon)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener((functionIWant) => { this.ShowWeaponInfoPanel(indexWeapon); });
            trigger.triggers.Add(entry);
        }

        private void ShowWeaponInfoPanel(int indexWeapon)
        {
            //Nombre
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[0].text = this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponName;
            //Description
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[1].text = this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponName;
            //Attack
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[2].text = "ATTACK: " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponAttack.ToString();
            //Recharge
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[3].text = "RECHARGE: " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponActualRecharge + "/" + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponRecharge;
            //Uses
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[4].text = "USES: " + this.GetAxel().CharacterSpecialWeapons[indexWeapon].WeaponUses;
        }        

        private Dice StunnerDice(WeaponRPGInfo weapon)
        {
            Dice dice = weapon.WeaponAttack;
            int diceRoll = dice.DiceRoll();

            if (diceRoll == 1)
                return new Dice(1, 8);
            else if (diceRoll == 2)
                return new Dice(1, 10);
            else if (diceRoll == 3)
                return new Dice(1, 12);
            else if (diceRoll == 4)
                return new Dice(1, 20);

            return new Dice(0, 0);
        }

        private List<CharacterRPGInfo> GrenadeArea()
        {
            List<CharacterRPGInfo> fighters = new List<CharacterRPGInfo>();
            fighters.AddRange(this.TeamEnemy);

            fighters = fighters.OrderByDescending(x => x.CharacterStats.CharacterInitiative).ToList();
            fighters.RemoveAll(x => x.CharacterStats.CharacterHealth <= 0);

            return fighters;
        }

        #endregion

        #region Abilities

        private void AddAbilityEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, int indexAbility)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener((functionIWant) => { this.ShowAbilityInfoPanel(indexAbility); });
            trigger.triggers.Add(entry);
        }

        private void ShowAbilityInfoPanel(int indexAbility)
        {
            //Nombre
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[0].text = this.GetAxel().CharacterAbilities[indexAbility].AbilityName;
            //Description
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[1].text = this.GetAxel().CharacterAbilities[indexAbility].AbilityName;
            //Other
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[2].text = "";
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[3].text = "";
            this.InfoPanel.GetComponentsInChildren<TMP_Text>()[4].text = "";
        }

        private float GetPreparationDamage(Dice dice, int diceRoll, CharacterRPGInfo attacker, CharacterRPGInfo receiver)
        {
            float damage = 0;

            if (diceRoll == dice.Value.x) //Fallo 
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }
            else if (diceRoll == dice.Value.y || diceRoll == dice.Value.y - 1 || diceRoll == dice.Value.y - 2) //Critico
            {
                damage = diceRoll * 2;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE CRITICO. Da�o x2");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
            }
            else //Normal
            {
                damage = diceRoll;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
            }

            return damage;
        }

        private float GetInitiativeDamage(Dice dice, int diceRoll, CharacterRPGInfo attacker, CharacterRPGInfo receiver)
        {
            float damage = 0;

            if (diceRoll == dice.Value.x) //Fallo 
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }
            else //Normal
            {
                damage = diceRoll;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
            }

            return damage;
        }

        private float GetAnticipationDamage(Dice dice, int diceRoll, CharacterRPGInfo attacker, CharacterRPGInfo receiver)
        {
            float damage = 0;

            if (diceRoll == dice.Value.x || diceRoll == dice.Value.x + 1 || diceRoll == dice.Value.x + 2) //Fallo 
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }
            else if (diceRoll == dice.Value.y) //Critico
            {
                damage = diceRoll * 2;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE CRITICO. Da�o x2");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
            }
            else //Normal
            {
                damage = diceRoll;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
            }

            return damage;
        }

        private float GetSpeedDamage(Dice dice, int diceRoll, CharacterRPGInfo attacker, CharacterRPGInfo receiver)
        {
            float damage = 0;

            if (diceRoll == dice.Value.x) //Fallo 
            {
                Debug.Log(attacker.CharacterName + " ha FALLADO. Da�o 0");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.MISS;

                return 0;
            }
            else //Normal
            {
                damage = diceRoll;
                Debug.Log(attacker.CharacterName + " ha hecho un ATAQUE NORMAL. Da�o x1");
                this.AttackLogInfo.AttackState = COMBAT_RPGATTACKSTATE.CRITICAL;
            }

            return damage * 2;
        }

        #endregion
    }
}