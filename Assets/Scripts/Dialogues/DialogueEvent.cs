using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

namespace NeonBlood
{
    /// <summary>
    /// 
    /// Casos Base
    /// 
    /// 1. Tenemos un NPC que sus estados cambian
    ///         Meter su nombre en NPCName
    ///         Meter sus flowcharts en NPCFlowcharts
    ///     
    /// 2. Tenemos un NPC que sus estados no cambian
    ///         Dejamos vacio NPCName
    ///         Metemos 1 flowchart en NPCFlowcharts
    ///         
    /// 3. Tenemos un NPC mudo
    ///         Dejamos vacio NPCName
    ///         Dejamos vacio NPCFlowcharts
    ///         
    /// ----------------------------------------------------
    ///         
    /// Casos Automatizacion
    ///         
    /// 4. Tenemos un NPC que vamos a utilizar muchisimo con un mensaje tipo que nunca va a cambiar
    ///         Metemos una "palabra clave" en NPCName (Busy, Sad...)
    ///         Hacemos un prefab de ese Flowchart y lo metemos en Resources
    ///         Dejamos vacio NPCFlowcharts
    ///         Cargamos desde codigo ese Flowchart y lo asociamos dependiendo de la palabra clave
    ///         
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class DialogueEvent : MonoBehaviour
    {
        [Header("NPC")]
        public string NPCName;

        [Header("Dialogues")]
        public DialogueRunner DialogueRunner;
        public List<string> NPCDialogues;

        [Header("UI")]
        public GameObject ActionButton;
        public GameObject DialogueIcon;

        [Header("Debug")]
        private int npcCurrentState;
        private string npcCurrentNode;

        private GameObject player;
        private bool forceDialogue = false;

        void Start()
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }

        void Update()
        {
            //Si el player ha entrado en el trigger y pulso Triangle, realizo la accion
            if (Input.GetButtonDown("Triangle") && this.CanForceDialogueEvent() && !this.DialogueRunner.IsDialogueRunning)
            {
                this.StartDialogue();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !this.forceDialogue)
            {
                //Activamos el boton
                this.ActionButton.SetActive(true);
                this.player = other.gameObject;

                //Desactivamos el icono del dialogo
                this.DialogueIcon.SetActive(false);
                this.player = other.gameObject;

                //Asociamos el flowchart
                this.SetNPCState();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //Desactivamos el boton
                this.ActionButton.SetActive(false);
                this.player = null;

                //Activamos el icono del dialogo
                this.DialogueIcon.SetActive(true);
                this.player = null;

                //Reiniciamos el force si lo hemos activado
                this.forceDialogue = false;

                if (this.npcCurrentNode != "")
                    this.npcCurrentNode = "";
            }
        }

        private void SetNPCState()
        {
            //Caso NPC
            if (this.NPCName != "")
            {
                //Cogemos el estado actual del NPC
                this.npcCurrentState = DataManager.Instance.GetCheckPoint().CurrentPlayerNPCs.Single(x => x.NameNPC == this.NPCName).StateNPC;
                //Cogemos el flowchart corresponiente al estado del NPC
                this.npcCurrentNode = this.NPCDialogues[this.npcCurrentState];
            }
            //Cualquier otro caso
            else
            {
                //Si hay algun dialogo asociado
                if (this.NPCDialogues.Count != 0)
                {
                    this.npcCurrentState = 0;
                    this.npcCurrentNode = this.NPCDialogues[this.npcCurrentState];
                }
            }
        }

        public void StartDialogue()
        {
            this.forceDialogue = true;
            this.SetNPCState();

            this.DialogueRunner.onDialogueStart.AddListener(this.StartConversation);
            this.DialogueRunner.onDialogueComplete.AddListener(this.EndConversation);

            this.DialogueRunner.StartDialogue(this.npcCurrentNode);
            this.ActionButton.SetActive(false);
        }

        public void StartConversation()
        {
            LevelManager.Instance.Player.CanMove = false;
        }

        public void EndConversation()
        {
            LevelManager.Instance.Player.CanMove = true;
        }

        private bool CanForceDialogueEvent()
        {
            return this.player && !MapCanvas.Instance.ContainerPanel.activeSelf && !PauseCanvas.Instance.MainPanel.activeSelf;
        }
    }
}