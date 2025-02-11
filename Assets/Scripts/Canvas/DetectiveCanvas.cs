using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;

namespace NeonBlood
{
    public class DetectiveCanvas : MMSingleton<DetectiveCanvas>
    {
        [Header("Container")]
        public GameObject DetectiveContainer;

        [Header("Location")]
        public TMP_Text LocationTitleText;
        public TMP_Text LocationDescriptionText;

        [Header("Quest")]
        public TMP_Text QuestTitleText;
        public TMP_Text QuestDescriptionText;

        void Start()
        {

        }

        public void ActivateDetectiveContainer(bool active)
        {
            this.DetectiveContainer.SetActive(active);
        }

        public void ToogleDetectiveContainer()
        {
            this.DetectiveContainer.SetActive(!this.DetectiveContainer.activeSelf);
        }

        public void UpdateLocationText(string title, string description)
        {
            this.LocationTitleText.text = title;
            this.LocationDescriptionText.text = description;
        }

        public void UpdateQuestText(string title, string description)
        {
            this.QuestTitleText.text = title;
            this.QuestDescriptionText.text = description;
        }
    }
}