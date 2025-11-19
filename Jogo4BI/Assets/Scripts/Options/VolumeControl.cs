    using UnityEngine;
    using UnityEngine.UI; 
    using UnityEngine.Audio; 

    public class VolumeControl : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public string exposedParameterName; 

        public void SetVolume(float sliderValue)
        {
            
            audioMixer.SetFloat(exposedParameterName, Mathf.Log10(sliderValue) * 20);
        }
    }