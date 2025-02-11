using UnityEngine;

namespace NeonBlood
{
    public class LocationEvent : MonoBehaviour
    {
        public string LocationKey;
        
        void Start()
        {
            this.SetLocation();
        }

        public void SetLocation()
        {
            DetectiveCanvas.Instance.UpdateLocationText(
                TextsManager.Instance.LocationsInfo[this.LocationKey].Title,
                TextsManager.Instance.LocationsInfo[this.LocationKey].Text);
        }
    }
}