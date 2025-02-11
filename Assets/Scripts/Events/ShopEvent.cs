using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    public class ShopEvent : MonoBehaviour
    {
        [Header("Shop")]
        public List<ItemRPGInfo> ShopItems;

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
            if (Input.GetButtonDown("Triangle") && this.CanShowShopUI())
            {
                this.ActionButton.SetActive(false);

                //Mostramos el panel de viaje rapido
                this.ShowShopUI();
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

        private void ShowShopUI()
        {
            this.isActive = true;
            StartCoroutine(ShopCanvas.Instance.ShowShopPanel(this));
        }

        public void ReactiveEvent()
        {
            this.isActive = false;
            //Activamos el boton
            this.ActionButton.SetActive(true);
        }

        private bool CanShowShopUI()
        {
            return !this.isActive && this.player;
        }
    }
}