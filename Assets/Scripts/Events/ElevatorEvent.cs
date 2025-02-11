using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    public class ElevatorEvent : MonoBehaviour
    {
        [Header("Elevator")]
        public GameObject Elevator;
        public List<Vector3> ElevatorFloors;
        public float ElevatorFloorTime = 1;

        [Header("UI")]
        public GameObject ActionButton;

        private bool isActive;
        private GameObject player;

        public int CurrentFloor { get; set; }

        void Start()
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }

        void Update()
        {
            //Si el player ha entrado en el trigger y pulso Triangle, realizo la accion
            if (Input.GetButtonDown("Triangle") && this.CanShowElevatorUI())
            {
                this.ActionButton.SetActive(false);

                //Mostramos el panel del ascensor
                this.ShowElevatorUI();
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

        private void ShowElevatorUI()
        {
            this.isActive = true;
            StartCoroutine(ElevatorCanvas.Instance.ShowElevatorPanel(this));
        }

        public void ReactiveEvent()
        {
            this.isActive = false;
            //Activamos el boton
            this.ActionButton.SetActive(true);
        }

        private bool CanShowElevatorUI()
        {
            return !this.isActive && this.player;
        }

        #region Floors

        public void GoToFloor(int indexFloor)
        {
            StartCoroutine(this.GoToFloorCo(indexFloor));
        }

        private IEnumerator GoToFloorCo(int indexFloor)
        {
            //Player
            this.player.GetComponent<CharacterController>().CanMove = false;
            this.player.transform.SetParent(this.Elevator.transform);

            yield return StartCoroutine(ProgrammerTools.MoveFromTo(
                this.Elevator,
                this.Elevator.transform.position,
                this.ElevatorFloors[indexFloor],
                this.ElevatorFloorTime * Mathf.Abs(this.CurrentFloor - indexFloor)));

            //Player
            this.player.GetComponent<CharacterController>().CanMove = true;
            this.player.transform.SetParent(null);

            this.CurrentFloor = indexFloor;
            this.ReactiveEvent();
        }

        #endregion
    }
}