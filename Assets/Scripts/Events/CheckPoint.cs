using UnityEngine;
using UnityEngine.Events;

namespace NeonBlood
{
    /// <summary>
    /// Clase encargada del spawn del player una vez muerto en el lugar indicado
    /// </summary>
    public class CheckPoint : MonoBehaviour
    {
        [TextArea]
        public string Description;

        public Transform SpawnPoint;

        public UnityEvent CheckPointEvent;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.ForceCheckPoint();
            }
        }

        public void ForceCheckPoint()
        {
            //Guardo el Check Point en el DataManager
            //if (DataManager.Instance.GetCheckPoint().CurrentCheckPointIndex < int.Parse(this.name.Split('_')[1]))
            //    DataManager.Instance.GetCheckPoint().CurrentCheckPointIndex = int.Parse(this.name.Split('_')[1]);

            if (DataManager.Instance.CanLoadSave)
                DataManager.Instance.SaveData();
        }
    }
}