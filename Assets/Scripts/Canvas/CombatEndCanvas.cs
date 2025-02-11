using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace NeonBlood
{
    public class CombatEndCanvas : MonoBehaviour
    {
        public GameObject CombatEndPanel;
        
        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);            
            this.CombatEndPanel.SetActive(true);
        }

        public void Continue()
        {
#if UNITY_PS4
            MenuCanvas.Instance.PlayLevel(DataManager.Instance.CombatScene);
#else
            MenuCanvas.Instance.PlayLevelDirectly(DataManager.Instance.CombatScene);
#endif
        }
    }
}