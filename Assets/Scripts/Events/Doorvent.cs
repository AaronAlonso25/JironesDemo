using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;


public class DoorEvent2 : MonoBehaviour
{
    public string SceneTarget;
    public string DoorTarget;
    public Vector3 DoorOffset;
    public bool IsLoading = false;

    [Header("UI")]
    public GameObject ActionButton;

    private bool isActive;
     private GameObject player;

    public GameObject doorblocked; 
    public GameObject botonblocked;

    public DialogueRunner dialogueRunner;

    public void Awake() 
    {
        dialogueRunner.AddCommandHandler("OpenDoorTP",OpenDoor); 
        dialogueRunner.AddCommandHandler("OpenDoorCocina",OpenDoorCocina);
        dialogueRunner.AddCommandHandler("OpenBoton",OpenBoton);
    }

    private void Start()
    {
        
        doorblocked.SetActive(false);
        botonblocked.SetActive(false);
    }

    

    
   
     void Update()
        
        {
            //Si el player ha entrado en el trigger y pulso Triangle, realizo la accion
            if (this.player && Input.GetButtonDown("Triangle"))
            {
                this.ActionButton.SetActive(false);

                //Viajamos a la siguiente escena
                TpNuevaEscena();
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

        public void TpNuevaEscena()
        {
            SceneManager.LoadScene(SceneTarget);
        }
        public void OpenDoor()
        {
            doorblocked.SetActive(true); 
            Debug.Log("La puerta ahora está abierta.");
        }

        public void OpenDoorCocina()
        {
            doorblocked.SetActive(true); 
            Debug.Log("La puerta ahora está abierta.");
        }
        public void OpenBoton()
        {
            botonblocked.SetActive(true); 
            Debug.Log("El boton esta disponible.");
        }

       
}
