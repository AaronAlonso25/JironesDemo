using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace NeonBlood
{
    /// <summary>
    /// Clase encarga de cuando un objeto o el player se quedan dentro del propio trigger
    /// </summary>
    public class TriggerStayEvent : MonoBehaviour
    {
        public enum TriggerNameOrTag { NAME, TAG };
        public enum TriggerResult { DESTROY_OBJECT, NONE };
        public enum TriggerResultEvent { DESTROY_THIS, NONE };

        [Tooltip("If it is empty, detects everything")]
        public string NameOrTagObject;

        public TriggerNameOrTag NameOrTag;
        public TriggerResult Result;
        public TriggerResultEvent ResultEvent = TriggerResultEvent.DESTROY_THIS;

        public UnityEvent OntriggerEnter;
        public UnityEvent OntriggerStay;
        public UnityEvent OntriggerExit;

        void Awake()
        {
            this.GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (this.NameOrTag == TriggerNameOrTag.NAME)
            {
                if (other.name == this.NameOrTagObject)
                    StartCoroutine(this.ActivateEventEnter(other));
            }
            else if (this.NameOrTag == TriggerNameOrTag.TAG)
            {
                if (other.CompareTag(this.NameOrTagObject))
                    StartCoroutine(this.ActivateEventEnter(other));
            }

            //En otro caso
            if (NameOrTagObject == "")
                StartCoroutine(this.ActivateEventEnter(other));
        }

        private void OnTriggerStay(Collider other)
        {
            if (this.NameOrTag == TriggerNameOrTag.NAME)
            {
                if (other.name == this.NameOrTagObject)
                    StartCoroutine(this.ActivateEventStay(other));
            }
            else if (this.NameOrTag == TriggerNameOrTag.TAG)
            {
                if (other.CompareTag(this.NameOrTagObject))
                    StartCoroutine(this.ActivateEventStay(other));
            }

            //En otro caso
            if (NameOrTagObject == "")
                StartCoroutine(this.ActivateEventStay(other));
        }

        private void OnTriggerExit(Collider other)
        {
            if (this.NameOrTag == TriggerNameOrTag.NAME)
            {
                if (other.name == this.NameOrTagObject)
                    StartCoroutine(this.ActivateEventExit(other));
            }
            else if (this.NameOrTag == TriggerNameOrTag.TAG)
            {
                if (other.CompareTag(this.NameOrTagObject))
                    StartCoroutine(this.ActivateEventExit(other));
            }

            //En otro caso
            if (NameOrTagObject == "")
                StartCoroutine(this.ActivateEventExit(other));
        }

        private IEnumerator ActivateEventEnter(Collider col)
        {
            this.OntriggerEnter.Invoke();
            yield return StartCoroutine(this.PostActivateEvent(col));
        }

        private IEnumerator ActivateEventStay(Collider col)
        {
            this.OntriggerStay.Invoke();
            yield return StartCoroutine(this.PostActivateEvent(col));
        }

        private IEnumerator ActivateEventExit(Collider col)
        {
            this.OntriggerExit.Invoke();
            yield return StartCoroutine(this.PostActivateEvent(col));
        }

        private IEnumerator PostActivateEvent(Collider col)
        {
            //Destruyo el objeto que provoca el evento
            if (this.Result == TriggerResult.DESTROY_OBJECT)
                Destroy(col.gameObject);

            yield return new WaitForSeconds(0.5f);

            //Destruyo el evento
            if (this.ResultEvent == TriggerResultEvent.DESTROY_THIS)
                Destroy(this);
        }

        public void ForceEventEnter()
        {
            OntriggerEnter.Invoke();
        }

        public void ForceEventStay()
        {
            OntriggerStay.Invoke();
        }

        public void ForceEventExit()
        {
            OntriggerExit.Invoke();
        }
    }
}