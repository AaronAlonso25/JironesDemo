using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

namespace NeonBlood
{
    public class DialogueAdvanceInputRewired : MonoBehaviour
    {
        private DialogueAdvanceInput input;
        
        void Start()
        {
            this.input = this.GetComponent<DialogueAdvanceInput>();
        }

        void Update()
        {
            if(Input.GetButtonDown("Cross_UI"))
            {
                this.input.dialogueView.UserRequestedViewAdvancement();
            }
        }
    }
}