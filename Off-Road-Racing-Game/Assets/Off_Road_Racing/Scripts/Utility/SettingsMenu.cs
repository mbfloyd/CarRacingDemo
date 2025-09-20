//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ALIyerEdon;

namespace ALIyerEdon
{
    public class SettingsMenu : MonoBehaviour
    {
        // UI items in settings menu window
        public Dropdown resolution;
        public Dropdown reflection;
        public Dropdown antiAliasing;
        public Dropdown controlType;
        public Dropdown sunShaft;
        public Dropdown wheelSmoke;
        public Dropdown displayFPS;       
        public Dropdown dynamicCamera;       
        
        public Slider AccelSensibility;
        public Text AccelSensibilityInfo;
        public Slider SteerWheelSensibility;
        public Text SteerWheelInfo;
        public Slider musicVolume;
        public Text musicVolumeInfo;
       
        public Toggle positionUI;
        public Toggle localPosition;

        // Start is called before the first frame update
        void Start()
        {
            // Load initilial settings            
            AccelSensibility.value = PlayerPrefs.GetFloat("accelSensibility");
            AccelSensibilityInfo.text = AccelSensibility.value.ToString();

            SteerWheelSensibility.value = PlayerPrefs.GetFloat("SteeringWheelSens");
            SteerWheelInfo.text = SteerWheelSensibility.value.ToString();

            resolution.value = PlayerPrefs.GetInt("ResQuality");
            reflection.value = PlayerPrefs.GetInt("Reflection");
            antiAliasing.value = PlayerPrefs.GetInt("Anti Aliasing");
            controlType.value = PlayerPrefs.GetInt("ControlType");
            sunShaft.value = PlayerPrefs.GetInt("Sun Shaft");
            displayFPS.value = PlayerPrefs.GetInt("Display FPS");
            dynamicCamera.value = PlayerPrefs.GetInt("Dynamic Camera");
            wheelSmoke.value = PlayerPrefs.GetInt("Wheel Smoke");

            musicVolume.value = PlayerPrefs.GetFloat("Music");

            // 3 => true  0 => false
            if (PlayerPrefs.GetInt("ShowLocalPosition") == 3)
                localPosition.isOn = true;
            else
                localPosition.isOn = false;

            if (PlayerPrefs.GetInt("ShowPositionUI") == 3)
                positionUI.isOn = true;
            else
                positionUI.isOn = false;
        }

        // Accelerometer  Sensibility
        public void Accelometer_Sensibility()
        {
            PlayerPrefs.SetFloat("accelSensibility", AccelSensibility.value);
            AccelSensibilityInfo.text = AccelSensibility.value.ToString();
        }
        public void SteerWheel_Sensibility()
        {
            PlayerPrefs.SetFloat("SteeringWheelSens", SteerWheelSensibility.value);
            SteerWheelInfo.text = SteerWheelSensibility.value.ToString();
        }
        public void Music_Volume()
        {
            PlayerPrefs.SetFloat("Music", musicVolume.value);
            musicVolumeInfo.text = musicVolume.value.ToString();
            FindFirstObjectByType<Load_Settings>().Update_MusicVolume(musicVolume.value);
        }
       
        // Screen resolution quality : best for performance
        public void Set_Resolution()
        {
            PlayerPrefs.SetInt("ResQuality", resolution.value);

            if (PlayerPrefs.GetInt("ResQuality") == 0)
            {
                Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.5f),
                    (int)(PlayerPrefs.GetInt("OriginalY") * 0.5f), true);
            }
            if (PlayerPrefs.GetInt("ResQuality") == 1)
            {
                Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.7f),
                    (int)(PlayerPrefs.GetInt("OriginalY") * 0.7f), true);
            }
            if (PlayerPrefs.GetInt("ResQuality") == 2)
            {
                Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.85f),
                    (int)(PlayerPrefs.GetInt("OriginalY") * 0.85f), true);
            }
        }

        // Screen space reflections
        public void Set_Reflection()
        {
            Trive.Rendering.StochasticReflections ssr2;

            GameObject.FindFirstObjectByType<
                    UnityEngine.Rendering.PostProcessing.PostProcessVolume>()
                    .profile.TryGetSettings(out ssr2);

            if (reflection.value == 0)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.Forward;

                ssr2.enabled.value = false;
            }
            if (reflection.value == 1)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;

                ssr2.enabled.value = true;

                ssr2.resolveDownsample.value = false;
                ssr2.raycastDownsample.value = false;
            }
        }

        public void Toggle_LocalPosition()
        {
            StartCoroutine(Toggle_LocalPosition_Save());
        }

        IEnumerator Toggle_LocalPosition_Save()
        {
            yield return new WaitForEndOfFrame();

            if (localPosition.isOn)
                PlayerPrefs.SetInt("ShowLocalPosition", 3);
            else
                PlayerPrefs.SetInt("ShowLocalPosition", 0);
        }

        // Racing Position UI Display
        public void Toggle_PositionUI()
        {
            StartCoroutine(Toggle_PositionUI_Save());
        }

        IEnumerator Toggle_PositionUI_Save()
        {
            yield return new WaitForEndOfFrame();

            if (positionUI.isOn)
                PlayerPrefs.SetInt("ShowPositionUI", 3);
            else
                PlayerPrefs.SetInt("ShowPositionUI", 0);
        }

        // Control type : accelerometer , steering wheel , arrow keys
        public void Set_ControlType()
        {
            PlayerPrefs.SetInt("ControlType", controlType.value);
        }
        public void Set_AntiAliasing()
        {
            PlayerPrefs.SetInt("Anti Aliasing", antiAliasing.value);

            if(PlayerPrefs.GetInt("Anti Aliasing") == 0)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                    .antialiasingMode = 
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.None;
            }
            if (PlayerPrefs.GetInt("Anti Aliasing") == 1)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                    .antialiasingMode =
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
            }
            if (PlayerPrefs.GetInt("Anti Aliasing") == 2)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                    .antialiasingMode =
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
            }
            if (PlayerPrefs.GetInt("Anti Aliasing") == 3)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                    .antialiasingMode =
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.TemporalAntialiasing;
            }
        }
        public void Set_SunShaft()
        {
            PlayerPrefs.SetInt("Sun Shaft", sunShaft.value);
        }
        public void Set_WheelSmoke()
        {
            PlayerPrefs.SetInt("Wheel Smoke", wheelSmoke.value);
        }
        public void Set_DisplayFPS()
        {
            PlayerPrefs.SetInt("Display FPS", displayFPS.value);
        }
        public void Set_DynamicCamera()
        {
            PlayerPrefs.SetInt("Dynamic Camera", dynamicCamera.value);
        }

        // Save reflection values after close the settings menu to avoid crash on the low-end devices
        public void Close_Save()
        {
            PlayerPrefs.SetInt("Reflection", reflection.value);
        }
    }
}