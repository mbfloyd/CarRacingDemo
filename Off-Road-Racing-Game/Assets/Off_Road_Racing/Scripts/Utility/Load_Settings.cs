//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using UnityEngine;
using System.Collections;
using ALIyerEdon;

namespace ALIyerEdon
{
	public class Load_Settings : MonoBehaviour
	{


		public AudioSource music;

		void Start()
		{

            music.volume = PlayerPrefs.GetFloat("Music");

			if (FindFirstObjectByType<Race_Manager>())
			{
				if (PlayerPrefs.GetInt("ShowLocalPosition") == 3)
                    FindFirstObjectByType<Race_Manager>().showLocalPosition = true;
				else
                    FindFirstObjectByType<Race_Manager>().showLocalPosition = false;
            }

            #region Reflection          

            Trive.Rendering.StochasticReflections ssr2;

            GameObject.FindFirstObjectByType<
                    UnityEngine.Rendering.PostProcessing.PostProcessVolume>()
                    .profile.TryGetSettings(out ssr2);


            if (PlayerPrefs.GetInt("Reflection") == 0)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.Forward;


                GameObject.FindGameObjectWithTag("MainCamera").
                    GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

                ssr2.enabled.value = false;
            }
            if (PlayerPrefs.GetInt("Reflection") == 1)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;

                ssr2.enabled.value = true;

                ssr2.resolveDownsample.value = false;
                ssr2.raycastDownsample.value = false;
            }

            #endregion

            #region Anti Aliasing            

            if (PlayerPrefs.GetInt("Anti Aliasing") == 0)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                    .antialiasingMode = UnityEngine.Rendering.PostProcessing
                    .PostProcessLayer.Antialiasing.None;
            }
            if (PlayerPrefs.GetInt("Anti Aliasing") == 1)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                     UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                     .antialiasingMode = UnityEngine.Rendering.PostProcessing
                     .PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
            }
            if (PlayerPrefs.GetInt("Anti Aliasing") == 2)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                     UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                     .antialiasingMode = UnityEngine.Rendering.PostProcessing
                     .PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
            }
            if (PlayerPrefs.GetInt("Anti Aliasing") == 3)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<
                    UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
                    .antialiasingMode = UnityEngine.Rendering.PostProcessing
                    .PostProcessLayer.Antialiasing.TemporalAntialiasing;
            }
            #endregion

        }

        public void Update_MusicVolume(float volume)
		{
			music.volume = volume;


		}
	}
}