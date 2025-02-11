using UnityEngine;
using UnityEngine.UI;

namespace NeonBlood
{
    /// <summary>
    /// Clase para asociar un imagen
    /// </summary>
    public class ImageEvent : MonoBehaviour
    {
        public Sprite ImagePC;
        public Sprite ImagePCKeyboard;
        public Sprite ImagePS4;
        public Sprite ImagePS5;
        public Sprite ImageSwitch;

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
            Image _image = this.GetComponent<Image>();
            if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.STEAM)
            {
                if (OptionsManager.Instance.CurrentInput == OptionsManager.Input.CONTROLLER)
                    _image.sprite = this.ImagePC;
                else
                    _image.sprite = this.ImagePCKeyboard;
            }
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.PS4)
                _image.sprite = this.ImagePS4;
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.PS5)
                _image.sprite = this.ImagePS5;
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.SWITCH)
                _image.sprite = this.ImageSwitch;
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.GAMECORE)
                _image.sprite = this.ImagePC;

            #region PS4 Asia-Japan

#if UNITY_PS4
            bool isCrossEnterButton =
                UnityEngine.PS4.Utility.GetSystemServiceParam(UnityEngine.PS4.Utility.SystemServiceParamId.EnterButtonAssign) == 1;

            //El boton asociado al Enter es Circle
            if (!isCrossEnterButton)
            {
                //Si el boton es X -> Lo cambio a O
                if (ImagePS4.name == "SonyWhite_01_0")
                {
                    Sprite circlePS4Image = Resources.Load<Sprite>("UI/SonyWhite_00");
                    _image.sprite = circlePS4Image;
                }
                //Si el boton es O -> Lo cambio a X
                else if (ImagePS4.name == "SonyWhite_00_0")
                {
                    Sprite circlePS4Image = Resources.Load<Sprite>("UI/SonyWhite_01");
                    _image.sprite = circlePS4Image;
                }
            }
#endif

            #endregion
        }
    }
}