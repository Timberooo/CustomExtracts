using Comfort.Common;
using EFT;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExtracts
{
	internal static class CustomExtractsManager
	{
		internal enum ChangeColorForExtractState
		{
			General,
			Current,
			DisabledGeneral,
			DisabledCurrent
		}



		private static List<GameObject> _extracts = new();
		private static int              _currentExtractIndex = -1;



		internal static bool NoExtracts
		{
			get { return _extracts.Count <= 0; }
		}

		internal static string CurrentExtractName
		{
			get { return _extracts[_currentExtractIndex].name; }
			set { _extracts[_currentExtractIndex].name = value; }
		}

		internal static Vector3 CurrentExtractPosition
		{
			get { return _extracts[_currentExtractIndex].transform.position; }
			set { _extracts[_currentExtractIndex].transform.position = value; }
		}

		internal static Vector3 CurrentExtractSize
		{
			get { return _extracts[_currentExtractIndex].transform.localScale; }
			set { _extracts[_currentExtractIndex].transform.localScale = value; }
		}

		internal static Vector3 CurrentExtractEulerAngles
		{
			get { return _extracts[_currentExtractIndex].transform.eulerAngles; }
			set { _extracts[_currentExtractIndex].transform.eulerAngles = value; }
		}

		internal static bool CurrentExtractEnabled
		{
			get { return _extracts[_currentExtractIndex].GetComponent<Collider>().enabled; }
			set { _extracts[_currentExtractIndex].GetComponent<Collider>().enabled = value; }
		}

		internal static float CurrentExtractTime
		{
			get { return _extracts[_currentExtractIndex].GetComponent<ExtractTestComponent>().Duration; }
			set { _extracts[_currentExtractIndex].GetComponent<ExtractTestComponent>().Duration = value; }
		}



		internal static void CreateExtract(string name, Vector3 position, Vector3 size, Vector3 eulerAngles, float time = 7f, bool enabled = true)
		{
			if (Singleton<GameWorld>.Instance == null)
			{
				Plugin.CustomExtractsLogger.LogWarning($"{System.Reflection.MethodBase.GetCurrentMethod().Name} was called before {typeof(GameWorld).FullName} was instantiated");
				return;
			}

			GameObject extract = GameObject.CreatePrimitive(PrimitiveType.Cube);

			ExtractTestComponent exfil = extract.AddComponent<ExtractTestComponent>(); // TODO: Look into replacing this with EFT's ExfiltrationPoint type
			exfil.Duration = time;

			extract.GetComponent<Collider>().isTrigger = true;
			extract.GetComponent<Collider>().enabled = enabled;

			// All this crap is to make the debug meshes support transparency
			// From berkhulagu: https://forum.unity.com/threads/change-rendering-mode-via-script.476437/
			Renderer renderer = extract.GetComponent<Renderer>();
			renderer.material.SetOverrideTag("RenderType", "Transparent");
			renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			renderer.material.SetInt("_ZWrite", 0);
			renderer.material.DisableKeyword("_ALPHATEST_ON");
			renderer.material.EnableKeyword("_ALPHABLEND_ON");
			renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			renderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			renderer.material.color = enabled ? Plugin.CurrentExtractColor.Value : Plugin.DisabledCurrentExtractColor.Value;
			renderer.enabled = Plugin.AlwaysShowExtracts.Value || ExtractEditor.ShowEditor;

			extract.name = name;
			extract.layer = 13; // Copying SamSWAT's Fire Support extract implementation. No idea why this has to be set to this value
			extract.transform.position = position;
			extract.transform.eulerAngles = eulerAngles;
			extract.transform.localScale = size;

			// If there are extracts already, update the color of the current extract
			// before adding the new extract and switching the current index to it
			if (!NoExtracts)
				_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = _extracts[_currentExtractIndex].GetComponent<Collider>().enabled ? Plugin.ExtractColor.Value : Plugin.DisabledExtractColor.Value;

			// Extracts are always added at the end of the list,
			// so the index should point to the last one
			_extracts.Add(extract);
			_currentExtractIndex = _extracts.Count - 1;
		}



		internal static void DestroyAllExtracts()
		{
			_extracts.ForEach(GameObject.Destroy);
			_extracts.Clear();

			_currentExtractIndex = -1;
		}



		internal static void DestroyCurrentExtract()
		{
			GameObject.Destroy(_extracts[_currentExtractIndex]);
			_extracts.RemoveAt(_currentExtractIndex);

			if (NoExtracts)
				_currentExtractIndex = -1;
			else
			{
				// Only need to update index when the last object was the current extract;
				// otherwise the next extract gets shifted to the index during removal

				if (_currentExtractIndex >= _extracts.Count)
					_currentExtractIndex = _extracts.Count - 1;

				_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.CurrentExtractColor.Value;
			}
		}



		internal static void IncrementCurrentExtract()
		{
			if (NoExtracts)
			{
				Plugin.CustomExtractsLogger.LogWarning("Cannot increment current extract; no custom extracts exist");
				return;
			}

			_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.ExtractColor.Value;

			// Checks if there is a next extract or if the
			// index should loop back to the first extract
			if (_currentExtractIndex + 1 < _extracts.Count)
				_currentExtractIndex++;
			else
				_currentExtractIndex = 0;

			_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.CurrentExtractColor.Value;
		}



		internal static void DecrementCurrentExtract()
		{
			if (NoExtracts)
			{
				Plugin.CustomExtractsLogger.LogWarning("Cannot decrement current extract; no custom extracts exist");
				return;
			}

			_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.ExtractColor.Value;

			// Checks if there is a previous extract or if
			// the index should loop back to the last extract
			if (_currentExtractIndex - 1 >= 0)
				_currentExtractIndex--;
			else
				_currentExtractIndex = _extracts.Count - 1;

			_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = Plugin.CurrentExtractColor.Value;
		}



		internal static void ShowExtracts(bool show)
		{
			_extracts.ForEach(extract => extract.GetComponent<Renderer>().enabled = show);
		}



		internal static void ChangeColor(ChangeColorForExtractState state, Color color)
		{
			switch (state)
			{
				case ChangeColorForExtractState.General:
				{
					for (int i = 0; i < _extracts.Count; i++)
						if (i != _currentExtractIndex && _extracts[i].GetComponent<Collider>().enabled)
							_extracts[i].GetComponent<Renderer>().material.color = color;

					break;
				}

				case ChangeColorForExtractState.Current:
				{
					if (!NoExtracts && _extracts[_currentExtractIndex].GetComponent<Collider>().enabled)
						_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = color;

					break;
				}

				case ChangeColorForExtractState.DisabledGeneral:
				{
					for (int i = 0; i < _extracts.Count; i++)
						if (i != _currentExtractIndex && _extracts[i].GetComponent<Collider>().enabled == false)
							_extracts[i].GetComponent<Renderer>().material.color = color;

					break;
				}

				case ChangeColorForExtractState.DisabledCurrent:
				{
					if (!NoExtracts && _extracts[_currentExtractIndex].GetComponent<Collider>().enabled == false)
						_extracts[_currentExtractIndex].GetComponent<Renderer>().material.color = color;

					break;
				}
			}
		}
	}
}
