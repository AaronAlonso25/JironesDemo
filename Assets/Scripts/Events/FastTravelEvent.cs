using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    public class FastTravelEvent : MonoBehaviour
    {
        [Header("UI")]
        public GameObject ActionButton;
        public Vector3 FastTravelOffset;

        private bool isActive;
        private GameObject player;

        void Start()
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }

        void Update()
        {
            //Si el player ha entrado en el trigger y pulso Triangle, realizo la accion
            if (Input.GetButtonDown("Triangle") && this.CanShowFastTravelUI())
            {
                this.ActionButton.SetActive(false);

                //Mostramos el panel de viaje rapido
                this.ShowFastTravelUI();
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

        private void ShowFastTravelUI()
        {
            this.isActive = true;
            StartCoroutine(FastTravelCanvas.Instance.ShowFastTravelPanel(this));
        }

        public void ReactiveEvent()
        {
            this.isActive = false;
            //Activamos el boton
            this.ActionButton.SetActive(true);
        }

        private bool CanShowFastTravelUI()
        {
            return !this.isActive && this.player;
        }
    }
}