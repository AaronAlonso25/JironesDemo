using UnityEngine;
using System.Collections;

namespace NeonBlood
{
    /// <summary>
    /// Clase que espera un retardo para cargar la siguiente escena
    /// </summary>
    public class DelayScene : MonoBehaviour
    {
        //Retardo y nombre de la escena a cargar a continuacion
        public float DelayTime;
        public string NameScene;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(this.DelayTime);
            MenuCanvas.Instance.PlayLevelDirectly(this.NameScene);
        }
    }
}