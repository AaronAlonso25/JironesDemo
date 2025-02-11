using UnityEngine;
using MoreMountains.Tools;

namespace NeonBlood
{
    public class GameplayCanvas : MMSingleton<GameplayCanvas>
    {
        [Header("Main Panels")]
        public GameObject MainPanel;

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            
        }

        public void ToogleGameplayPanel()
        {
            this.MainPanel.SetActive(!this.MainPanel.activeSelf);
        }
    }
}