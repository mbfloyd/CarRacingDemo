//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using UnityEngine;
using System.Collections;
using ALIyerEdon;

namespace ALIyerEdon
{
	public class CameraSwitch : MonoBehaviour
	{

		// MainCamera
		GameObject mainCamera;

		[Header("The first slot must be empty")]
		[Header("Because is used for Main camera")]
		[Space(3)]

		[Header("Camera List :")]
		// List of the camera's gameObjects
		public GameObject[] cameras;

		// Hold curent active camera id
		int currentCamera = 0;

		void Start()
		{
			mainCamera = GameObject.Find("Main Camera");

			cameras[0] = mainCamera;
		}


		// Switch to next camera based total camera counts
		public void NextCamera()
		{
			if (currentCamera < cameras.Length - 1)
				currentCamera++;
			else
				currentCamera = 0;

			SelectCamera(currentCamera);
		}

		// Diactivate all cameras and activate current selected
		public void SelectCamera(int id)
		{

			for (int a = 0; a < cameras.Length; a++)
			{
				cameras[a].SetActive(false);
				cameras[a].GetComponent<AudioListener>().enabled = false;
				cameras[a].GetComponent<Camera>().enabled = false;
			}

			cameras[id].SetActive(true);
			cameras[id].GetComponent<AudioListener>().enabled = true;
			cameras[id].GetComponent<Camera>().enabled = true;

            #region Reflection
            if (PlayerPrefs.GetInt("Reflection") == 0)
			{
				cameras[id].

				GetComponent<Camera>().renderingPath = RenderingPath.Forward;

				cameras[id].GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
			}
			if (PlayerPrefs.GetInt("Reflection") == 1)
			{
				cameras[id].
				GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;
			}
			if (PlayerPrefs.GetInt("Reflection") == 2)
			{
				cameras[id].
				GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;
			}
			#endregion

			#region Anti Aliasing            

			if (PlayerPrefs.GetInt("Anti Aliasing") == 0)
			{
				cameras[id].GetComponent<
					UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
					.antialiasingMode =
					UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.None;
			}
			if (PlayerPrefs.GetInt("Anti Aliasing") == 1)
			{
				cameras[id].GetComponent<
					UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
					.antialiasingMode =
					UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
			}
			if (PlayerPrefs.GetInt("Anti Aliasing") == 2)
			{
				cameras[id].GetComponent<
					UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
					.antialiasingMode =
					UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
			}
			if (PlayerPrefs.GetInt("Anti Aliasing") == 3)
			{
				cameras[id].GetComponent<
					UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
					.antialiasingMode =
					UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.TemporalAntialiasing;
			}
			#endregion

			#region Sun Shaft           

			if (PlayerPrefs.GetInt("Sun Shaft") == 0)
			{
				cameras[id].GetComponent<LightingBox.Effects.SunShafts>()
					.enabled = false;
			}
			if (PlayerPrefs.GetInt("Sun Shaft") == 1)
			{
				if (FindFirstObjectByType<RainLights>())
				{
					if (cameras[id].GetComponent<LightingBox.Effects.SunShafts>())
						cameras[id].GetComponent<LightingBox.Effects.SunShafts>().enabled = false;
				}
				else
				{
					if (cameras[id].GetComponent<LightingBox.Effects.SunShafts>())
					{
						cameras[id].GetComponent<LightingBox.Effects.SunShafts>()
							.enabled = true;
					}
				}
			}

			#endregion
		}
	}
}