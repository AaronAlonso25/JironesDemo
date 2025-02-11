using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


namespace NeonBlood{
    public class CombatManager : MonoBehaviour
    {

        public DialogueRunner dialogueRunner;
        public List<CombatEvent> combats;
            public void StartCombat (string combatEvent){
                foreach(CombatEvent ce in combats){
                    if(ce.gameObject.name == combatEvent){
                        ce.InitializeCombat();
                        break;
                    }

                }
                
            }

        public void Awake() 
        {
            dialogueRunner.AddCommandHandler<string>("StartCombat",StartCombat); 
        }
    }
}