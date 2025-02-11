using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    public class DetectiveEvent : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent OnDetectiveActive;
        public UnityEvent OnDetectiveDeActive;

        void Start()
        {
            
        }

        void OnTriggerEnter(Collider other)
        {
            //Si detecto la esfera del modo Detective
            if(other.name == "DetectiveVFX")
            {
                this.ActivateEvent(true);
                this.SetLayer("DetectiveMode");
            }
        }

        void OnTriggerExit(Collider other)
        {
            //Si detecto la esfera del modo Detective
            if (other.name == "DetectiveVFX")
            {
                this.SetLayer("Default");
            }
        }

        public void ActivateEvent(bool active)
        {
            if (active)
                this.OnDetectiveActive.Invoke();
            else
                this.OnDetectiveDeActive.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameLayer">"Default", "DetectiveMode"</param>
        public void SetLayer(string nameLayer)
        {
            this.gameObject.layer = LayerMask.NameToLayer(nameLayer);
            this.ChangeLayersRecursively(this.transform, nameLayer);
        }

        private void ChangeLayersRecursively(Transform trans, string name)
        {
            foreach (Transform child in trans)
            {
                child.gameObject.layer = LayerMask.NameToLayer(name);
                ChangeLayersRecursively(child, name);
            }
        }
    }
}