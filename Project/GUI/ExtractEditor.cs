using Comfort.Common;
using EFT.UI;
using UnityEngine;

namespace CustomExtracts
{
	public class ExtractEditor : MonoBehaviour
	{
		private GameObject input;
		private Rect       windowRect = new Rect(50, 50, 630, 360);

		public static bool   showEditor = false;
		public static string extractName = "";



		void Update()
		{
			if (!Input.GetKeyDown(Plugin.toggleExtractEditorPanel.Value.MainKey))
				return;

			// TODO: Check if player is in hideout then display error message

			if (input == null)
				input = GameObject.Find("___Input");

			showEditor = !showEditor;
			Cursor.visible = showEditor;
			CustomExtractsManager.ShowExtracts(showEditor);

			if (showEditor)
			{
				CursorSettings.SetCursor(ECursorType.Idle);
				Cursor.lockState = CursorLockMode.None;
				Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuContextMenu);
			}
			else
			{
				CursorSettings.SetCursor(ECursorType.Invisible);
				Cursor.lockState = CursorLockMode.Locked;
				Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuDropdown);
			}

			input.SetActive(!showEditor);
		}



		void OnDestroy()
		{
			showEditor = false;
			CustomExtractsManager.ShowExtracts(false);

			extractName = "";
		}



		void OnGUI()
		{
			if (showEditor)
				windowRect = GUI.Window(420, windowRect, WindowFunction, "Extract Editor");
		}



		void WindowFunction(int windowId)
		{
			GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));

			GUI.Box(new Rect(10, 25, 610, 60), "");

			if (GUI.Button(new Rect(20, 45, 80, 22), "Previous"))
				CustomExtractsManager.DecrementCurrentExtract();

			CustomExtractsManager.UpdateCurrentExtractName(GUI.TextField(new Rect(110, 45, 410, 22), extractName));

			if (GUI.Button(new Rect(530, 45, 80, 22), "Next"))
				CustomExtractsManager.IncrementCurrentExtract();


			GUI.Box(new Rect(10, 90, 200, 100), "Position");

			GUI.Label(new Rect(15, 112, 100, 100), "X");
			GUI.TextField(new Rect(30, 112, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(95, 117, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(175, 112, 32, 22), "0.1");

			GUI.Label(new Rect(15, 137, 100, 100), "Y");
			GUI.TextField(new Rect(30, 137, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(95, 142, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(175, 137, 32, 22), "0.1");

			GUI.Label(new Rect(15, 162, 100, 100), "Z");
			GUI.TextField(new Rect(30, 162, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(95, 167, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(175, 162, 32, 22), "0.1");

			GUI.Box(new Rect(215, 90, 200, 100), "Size");

			GUI.Label(new Rect(220, 112, 100, 100), "X");
			GUI.TextField(new Rect(235, 112, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(300, 117, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(380, 112, 32, 22), "0.1");

			GUI.Label(new Rect(220, 137, 100, 100), "Y");
			GUI.TextField(new Rect(235, 137, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(300, 142, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(380, 137, 32, 22), "0.1");

			GUI.Label(new Rect(220, 162, 100, 100), "Z");
			GUI.TextField(new Rect(235, 162, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(300, 167, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(380, 162, 32, 22), "0.1");

			GUI.Box(new Rect(420, 90, 200, 100), "Rotation");

			GUI.Label(new Rect(425, 112, 100, 100), "X");
			GUI.TextField(new Rect(440, 112, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(505, 117, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(585, 112, 32, 22), "0.1");

			GUI.Label(new Rect(425, 137, 100, 100), "Y");
			GUI.TextField(new Rect(440, 137, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(505, 142, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(585, 137, 32, 22), "0.1");

			GUI.Label(new Rect(425, 162, 100, 100), "Z");
			GUI.TextField(new Rect(440, 162, 60, 22), "0");
			GUI.HorizontalSlider(new Rect(505, 167, 75, 22), 0, -1, 1);
			GUI.TextField(new Rect(585, 162, 32, 22), "0.1");
		}
	}
}
