using UnityEngine;
using TMPro;

namespace NeonBlood
{
    /// <summary>
    /// Clase para asociar un texto
    /// </summary>
    public class TextEvent : MonoBehaviour
    {
        public string TextKey;

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
            switch (this.TextKey)
            {
                case "NewGame":
                    _text.text = TextsManager.Instance.MenusInfo["MainMenu"].Buttons[0];
                    break;
                case "Continue":
                    _text.text = TextsManager.Instance.MenusInfo["MainMenu"].Buttons[1];
                    break;
                case "Levels":
                    _text.text = TextsManager.Instance.MenusInfo["MainMenu"].Buttons[2];
                    break;
                case "Options":
                    _text.text = TextsManager.Instance.MenusInfo["MainMenu"].Buttons[3];
                    break;                
                case "Quit":
                    _text.text = TextsManager.Instance.MenusInfo["MainMenu"].Buttons[4];
                    break;
                case "Select":
                    _text.text = TextsManager.Instance.MenusInfo["OthersMenu"].Buttons[0];
                    break;
                case "Back":
                    _text.text = TextsManager.Instance.MenusInfo["OthersMenu"].Buttons[1];
                    break;
                case "L1":
                    if(OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.STEAM)
                        _text.text = (OptionsManager.Instance.CurrentInput == OptionsManager.Input.CONTROLLER) ? "LB" : "Q";
                    else
                        _text.text = "L1";
                    break;
                case "R1":
                    if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.STEAM)
                        _text.text = (OptionsManager.Instance.CurrentInput == OptionsManager.Input.CONTROLLER) ? "RB" : "E";
                    else
                        _text.text = "R1";
                    break;
            }
        }
    }
}