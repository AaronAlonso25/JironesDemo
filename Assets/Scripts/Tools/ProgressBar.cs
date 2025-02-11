using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace NeonBlood
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("UI")]
        public TMP_Text ProgressBar_Text_1;
        public TMP_Text ProgressBar_Text_2;
        public Image ProgressBar_Image;

        [Header("Stats")]
        public float ProgressBar_MinValue = 0;
        public float ProgressBar_MaxValue = 0;

        void Start()
        {

        }

        public void SetValue(float value)
        {
            this.ProgressBar_Text_1.text = "" + value;
            this.ProgressBar_Text_2.text = "" + value;

            this.ProgressBar_Image.fillAmount = value / this.ProgressBar_MaxValue;
        }
    }
}