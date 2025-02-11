using UnityEngine;

namespace NeonBlood
{
    public class QuestEvent : MonoBehaviour
    {
        public string QuestKey;
        
        void Start()
        {
            this.SetQuest();
        }

        public void SetQuest()
        {
            DetectiveCanvas.Instance.UpdateQuestText(
                TextsManager.Instance.QuestsInfo[this.QuestKey].Title,
                TextsManager.Instance.QuestsInfo[this.QuestKey].Text);
        }
    }
}