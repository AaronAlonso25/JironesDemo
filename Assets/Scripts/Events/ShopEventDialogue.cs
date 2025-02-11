using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ShopEventDialogue : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    public float Monedas=0;
    public int incremento=100;

    public void Awake() 
    {
        dialogueRunner.AddCommandHandler("QuitarBasura",DarMonedas); 
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void DarMonedas()
    {
        Monedas=Monedas+incremento;
        Debug.Log("Has recibido 100 monedas");
    }
}
