using UnityEngine;
using System.Collections;

namespace NeonBlood
{
    /// <summary>
    /// Clase encargada del Teleport del player a la puerta correspondiente
    /// </summary>
    public class DoorConnection : MonoBehaviour
    {
        [Header("Door")]
        public DoorConnection ConnectedDoor;
		public Vector3 ConnectedDoorOffset;

        [Header("UI")]
        public GameObject ActionButton;

        private GameObject player;
        private bool IsMoving = false;

        void Start()
        {
            
        }

        void Update()
        {
            //Si el player ha entrado en el trigger y pulso Triangle, realizo la accion
            if(Input.GetButtonDown("Triangle") && this.player && !this.IsMoving)
            {
                StartCoroutine(this.TeleportPlayer(this.player));
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

        public IEnumerator TeleportPlayer(GameObject player)
        {
            this.IsMoving = true;
            MenuCanvas.Instance.FadeInOutFunction(0.25f);

            yield return new WaitForSeconds(1);

            player.transform.position = new Vector3(
				this.ConnectedDoor.transform.position.x + this.ConnectedDoorOffset.x, 
				this.ConnectedDoor.transform.position.y + this.ConnectedDoorOffset.y, 
				this.ConnectedDoor.transform.position.z + this.ConnectedDoorOffset.z);

            yield return new WaitForSeconds(1);
            this.IsMoving = false;
        }
    }
}