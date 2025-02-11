using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    public class DoorEvent : MonoBehaviour
    {
        public string SceneTarget;
        public string DoorTarget;
        public Vector3 DoorOffset;
        public bool IsLoading = false;

        [Header("UI")]
        public GameObject ActionButton;

        private bool isActive;
        private GameObject player;        

        void Start()
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }

        void Update()
        {
            //Si el player ha entrado en el trigger y pulso Triangle, realizo la accion
            if (this.player && Input.GetButtonDown("Triangle"))
            {
                this.ActionButton.SetActive(false);

                //Viajamos a la siguiente escena
                this.GoToScene();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !this.isActive)
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

        private void GoToScene()
        {
            this.isActive = true;

            //Asociar la puerta destino al DataManager
            DataManager.Instance.CurrentLevelDoor = this.DoorTarget;

            //Viajar a la escena de combate
            MenuCanvas.Instance.PlayLevelOptions(this.SceneTarget, IsLoading);
        }
    }
}