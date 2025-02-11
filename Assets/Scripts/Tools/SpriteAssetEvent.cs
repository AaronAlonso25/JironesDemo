using UnityEngine;
using TMPro;

namespace NeonBlood
{
    /// <summary>
    /// Clase para asociar un atlas de sprites en el TMPro
    /// </summary>
    public class SpriteAssetEvent : MonoBehaviour
    {
        public TMP_SpriteAsset SpritesPC;
        public TMP_SpriteAsset SpritesPCKeyboard;
        public TMP_SpriteAsset SpritesPS4;
        public TMP_SpriteAsset SpritesPS5;
        public TMP_SpriteAsset SpritesSwitch;

        void Start()
        {
            this.ForceEvent();
        }

        void OnEnable()
        {
            this.ForceEvent();
        }

        public void ForceEvent()
        {
            TMP_Text _text = this.GetComponent<TMP_Text>();
            if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.STEAM)
            {
                if (OptionsManager.Instance.CurrentInput == OptionsManager.Input.CONTROLLER)
                    _text.spriteAsset = this.SpritesPC;
                else
                    _text.spriteAsset = this.SpritesPCKeyboard;
            }
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.PS4)
                _text.spriteAsset = this.SpritesPS4;
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.PS5)
                _text.spriteAsset = this.SpritesPS5;
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.SWITCH)
                _text.spriteAsset = this.SpritesSwitch;
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.GAMECORE)
                _text.spriteAsset = this.SpritesPC;
        }
    }
}