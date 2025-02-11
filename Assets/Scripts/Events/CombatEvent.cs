using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace NeonBlood
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class CombatEvent : MonoBehaviour
    {
        [Header("Combat")]
        public string CombatName;
        public string CombatScene = "PlaygroundCombat";
        public Vector3 CombatOffset;
        public bool OnDestroyEnd = false;        

        [Header("UI")]
        public GameObject ActionButton;

        [Header("Teams")]
        public List<string> CombatRPGTeamAxel;
        public List<string> CombatRPGTeamEnemy;

        [Header("Events")]
        public UnityEvent VictoryEvent;
        public UnityEvent DefeatEvent;

        [Header("Victory Commands")]
        public List<string> VictoryCheckPointCommands;
        public List<string> VictoryNPCCommands;

        private GameObject player;

        void Start()
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }

        void Update()
        {
            //Si el player ha entrado en el trigger y pulso Triangle, realizo la accion
            if (Input.GetButtonDown("Triangle") && this.player)
            {
                this.InitializeCombat();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //Activamos el boton
                this.ActionButton.SetActive(true);
                this.player = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //Desactivamos el boton
                this.ActionButton.SetActive(false);
                this.player = null;
            }
        }

        public void InitializeCombat()
        {
            //Asociar la informacion del CombatEvent a nuestro DataManager
            DataManager.Instance.CombatName = this.CombatName;
            DataManager.Instance.CombatScene = this.CombatScene;
            DataManager.Instance.CombatRPGTeamAxel = this.CombatRPGTeamAxel;
            DataManager.Instance.CombatRPGTeamEnemy = this.CombatRPGTeamEnemy;
            DataManager.Instance.CombatPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

            DataManager.Instance.CombatVictoryCheckPointCommands = this.VictoryCheckPointCommands;
            DataManager.Instance.CombatVictoryNPCCommands = this.VictoryNPCCommands;

            //Asociamos escena actual al DataManager
            DataManager.Instance.CurrentLevel = SceneManager.GetActiveScene().name;

            //Viajar a la escena de combate
#if UNITY_PS4
            MenuCanvas.Instance.PlayLevel(this.CombatScene);
#else
            MenuCanvas.Instance.PlayLevelDirectly(this.CombatScene);
#endif
        }

        public void EndCombat(bool result)
        {
            if (result)
                this.VictoryEvent?.Invoke();
            else
                this.DefeatEvent?.Invoke();

            //Limpiar la informacion del combate actual
            DataManager.Instance.CombatName = "";
            DataManager.Instance.CombatResult = false;
            DataManager.Instance.CombatPosition = Vector3.zero;

            DataManager.Instance.CombatVictoryCheckPointCommands = new List<string>();
            DataManager.Instance.CombatVictoryNPCCommands = new List<string>();

            if (this.OnDestroyEnd)
                Destroy(this.gameObject);
        }
    }
}