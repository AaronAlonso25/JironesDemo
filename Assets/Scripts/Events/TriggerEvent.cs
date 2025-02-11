using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace NeonBlood
{
    /// <summary>
    /// Clase que representa un evento producido por un Trigger
    /// </summary>
    [ExecuteInEditMode]
    public class TriggerEvent : MonoBehaviour
    {
        public enum TriggerMode { ENTER, EXIT };
        public enum TriggerNameOrTag { NAME, TAG };
        public enum TriggerResult { DESTROY_OBJECT, NONE };
        public enum TriggerResultEvent { DESTROY_THIS, NONE };

        [Tooltip("If it is empty, detects everything")]
        public string NameOrTagObject;

        public TriggerMode Mode;
        public TriggerNameOrTag NameOrTag;
        public TriggerResult Result;
        public TriggerResultEvent ResultEvent = TriggerResultEvent.DESTROY_THIS;

        public UnityEvent Events;

        void Awake()
        {
            this.GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider col)
        {
            //Si es el modo contrario, me salgo
            if (this.Mode == TriggerMode.EXIT)
                return;

            if (this.NameOrTag == TriggerNameOrTag.NAME)
            {
                if (col.name == this.NameOrTagObject)
                    StartCoroutine(this.ActivateEvent(col));
            }
            else if (this.NameOrTag == TriggerNameOrTag.TAG)
            {
                if(col.CompareTag(this.NameOrTagObject))
                    StartCoroutine(this.ActivateEvent(col));
            }

            //En otro caso
            if (NameOrTagObject == "")
                StartCoroutine(this.ActivateEvent(col));
        }

        void OnTriggerExit(Collider col)
        {
            //Si es el modo contrario, me salgo
            if (this.Mode == TriggerMode.ENTER)
                return;

            if (this.NameOrTag == TriggerNameOrTag.NAME)
            {
                if (col.name == this.NameOrTagObject)
                    StartCoroutine(this.ActivateEvent(col));
            }
            else if (this.NameOrTag == TriggerNameOrTag.TAG)
            {
                if (col.CompareTag(this.NameOrTagObject))
                    StartCoroutine(this.ActivateEvent(col));
            }

            //En otro caso
            if (NameOrTagObject == "")
                StartCoroutine(this.ActivateEvent(col));
        }

        /// <summary>
        /// Metodo para activar el evento del trigger
        /// </summary>
        /// <param name="col"></param>
        private IEnumerator ActivateEvent(Collider col)
        {
            this.Events.Invoke();
            //Destruyo el objeto que provoca el evento
            if (this.Result == TriggerResult.DESTROY_OBJECT)
                Destroy(col.gameObject);

            yield return new WaitForSeconds(0.5f);

            //Destruyo el evento
            if (this.ResultEvent == TriggerResultEvent.DESTROY_THIS)
                Destroy(this);
        }

        /// <summary>
        /// Metodo para forzar el evento
        /// </summary>
        public void ForceEvent()
        {
            StartCoroutine(this.ActivateEvent(new Collider()));
        }
    }
}