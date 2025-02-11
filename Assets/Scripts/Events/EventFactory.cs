using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    [System.Serializable]
    public class CustomEvent : UnityEvent { }

    /// <summary>
    /// Clase que realiza eventos de los dialogos
    /// </summary>
    public class EventFactory : MonoBehaviour
    {
        [System.Serializable]
        public class CustomEventFactory
        {
            public float TimeToActivate = 0.5f;
            public CustomEvent Event;
        }

        public List<CustomEventFactory> FactoryEvents;

        //Indica si destruyo este objeto al usarse
        public bool DestroyAfterEvent = false;

        /// <summary>
        /// Metodo para ejecutar los eventos
        /// </summary>
        public void ExecuteEvent()
        {
            StartCoroutine(this.ExecuteEventCo());
        }

        /// <summary>
        /// Metodo para ejecutar los eventos
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExecuteEventCo()
        {
            foreach (CustomEventFactory cef in this.FactoryEvents)
            {
                yield return new WaitForSeconds(cef.TimeToActivate);
                cef.Event.Invoke();
            }

            if (this.DestroyAfterEvent)
                Destroy(this.gameObject);
        }
    }
}