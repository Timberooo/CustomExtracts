using Comfort.Common;
using CustomExtracts;
using EFT.UI;
using UnityEngine;

public class ExtractEditor : MonoBehaviour
{
	internal static bool ShowEditor { get; private set; } = false;



	private GameObject input;

	private Rect editorWindowRect         = new Rect(50, 60, 735, 333);
	private Rect unsavedWarningWindowRect = new Rect(850, 500, 350, 130);
	private Rect deleteWarningWindowRect  = new Rect(850, 500, 300, 130);

	private bool unsavedData = false;

	private bool showUnsavedWarning = false;
	private bool showDeleteWarning  = false;

	private string nameText      = null;
	private string positionXText = null;
	private string positionYText = null;
	private string positionZText = null;
	private string sizeXText     = null;
	private string sizeYText     = null;
	private string sizeZText     = null;
	private string rotationXText = null;
	private string rotationYText = null;
	private string rotationZText = null;
	private string timeText      = null;



	private bool LockEditor { get { return showUnsavedWarning || showDeleteWarning; } }



	private void Update()
	{
		if (!Input.GetKeyDown(Plugin.toggleExtractEditorPanel.Value.MainKey))
			return;

		// TODO: Check if player is in hideout then display error message.

		if (input == null)
			input = GameObject.Find("___Input");

		ShowEditor = !ShowEditor;
		Cursor.visible = ShowEditor;
		CustomExtractsManager.ShowExtracts(ShowEditor);

		if (ShowEditor)
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

		input.SetActive(!ShowEditor);
	}



	private void OnDestroy()
	{
		ShowEditor = false;
		showDeleteWarning = false;
		showUnsavedWarning = false;
		CustomExtractsManager.ShowExtracts(false);
	}



	private void OnGUI()
	{
		if (!ShowEditor)
			return;

		editorWindowRect = GUI.Window(420, editorWindowRect, EditorWindowFunc, "Custom Extract Editor");

		if (showUnsavedWarning)
			unsavedWarningWindowRect = GUI.Window(421, unsavedWarningWindowRect, UnsavedWarningWindowFunc, "Warning!");
		if (showDeleteWarning)
			deleteWarningWindowRect = GUI.Window(422, deleteWarningWindowRect, DeleteWarningWindowFunc, "Warning!");
    }



	private void EditorWindowFunc(int windowId)
	{
		GameObject currentExtract = CustomExtractsManager.CurrentExtract;
		bool noExtracts = currentExtract == null;

		GUI.DragWindow(new Rect(0, 0, editorWindowRect.width, 20));

		if (unsavedData)
		{
			Color originalTextColor = GUI.contentColor;

			GUI.contentColor = Color.yellow;
			GUI.Label(new Rect(316, editorWindowRect.height - 35, 110, 25), "unsaved changes");

			GUI.contentColor = originalTextColor;
		}

		NameControls(10, 25, ref currentExtract, noExtracts);

		if (!noExtracts)
		{
			currentExtract.transform.position = TransformControls(10, 120, "Position", ref positionXText, ref positionYText, ref positionZText, currentExtract.transform.position);
			currentExtract.transform.localScale = TransformControls(250, 120, "Size", ref sizeXText, ref sizeYText, ref sizeZText, currentExtract.transform.localScale);
			currentExtract.transform.eulerAngles = TransformControls(490, 120, "Rotation", ref rotationXText, ref rotationYText, ref rotationZText, currentExtract.transform.eulerAngles);
			PropertiesControls(10, 225, ref currentExtract);

			NavigationControls(20, 45);
		}

		SaveLoadControls();
		NewDeleteControls(noExtracts);
	}



	private void UnsavedWarningWindowFunc(int windowId)
	{
		GUI.DragWindow(new Rect(0, 0, unsavedWarningWindowRect.width, 20));

		GUI.Box(new Rect(1, 17, 348, unsavedWarningWindowRect.height - 18), "");
		GUI.Label(new Rect(30, 38, 300, 200), "Do you want to save your changes before loading?");

		if (GUI.Button(new Rect(unsavedWarningWindowRect.width / 2 + 50, 80, 80, 30), "Cancel"))
			showUnsavedWarning = false;

		GUI.backgroundColor = Color.cyan;
		if (GUI.Button(new Rect(unsavedWarningWindowRect.width / 2 - 130, 80, 80, 30), "Save All"))
		{
			// TODO: Save all custom extracts to a json file, then load custom extracts from json file
			ResetAllFieldText();

			unsavedData = false;
			showUnsavedWarning = false;
		}

		GUI.backgroundColor = Color.red;
		if (GUI.Button(new Rect(unsavedWarningWindowRect.width / 2 - 40, 80, 80, 30), "Don't Save"))
		{
			// TODO: Load custom extracts from json file
			ResetAllFieldText();

			unsavedData = false;
			showUnsavedWarning = false;
		}
	}



	private void DeleteWarningWindowFunc(int windowId)
	{
		GUI.DragWindow(new Rect(0, 0, deleteWarningWindowRect.width, 20));

		GUI.Box(new Rect(1, 17, 298, deleteWarningWindowRect.height - 18), "");
		GUI.Label(new Rect(19, 38, 300, 200), "Are you sure you want to delete this extract?");

		if (GUI.Button(new Rect(deleteWarningWindowRect.width / 2 + 10, 80, 80, 30), "Cancel"))
			showDeleteWarning = false;

		GUI.backgroundColor = Color.red;
		if (GUI.Button(new Rect(deleteWarningWindowRect.width / 2 - 80, 80, 80, 30), "Delete"))
		{
			CustomExtractsManager.DeleteCurrentExtract();
			ResetAllFieldText();

			showDeleteWarning = false;
			unsavedData = true;
		}
	}



	private void NameControls(float x, float y, ref GameObject currentExtract, bool noExtracts)
	{
		GUI.Box(new Rect(x, y, 715, 90), "");

		if (!noExtracts)
			currentExtract.name = TextSettingField(new Rect(x + 100, y + 20, 515, 22), ref nameText, currentExtract.name);
		else
		{
			Color originalTextColor = GUI.contentColor;

			GUI.contentColor = Color.red;
			GUI.TextField(new Rect(x + 100, y + 20, 515, 22), "no extracts to edit");
			GUI.contentColor = originalTextColor;
			return;
		}
	}



	private Vector3 TransformControls(float x, float y, string name, ref string xText, ref string yText, ref string zText, Vector3 vector)
	{
		float vecX, vecY, vecZ;

		GUI.Box(new Rect(x, y, 235, 100), name);
		GUI.Label(new Rect(x + 5, y + 22, 25, 25), "X");
		GUI.Label(new Rect(x + 5, y + 47, 25, 25), "Y");
		GUI.Label(new Rect(x + 5, y + 72, 25, 25), "Z");

		vecX = FloatSettingField(new Rect(x + 20, y + 22, 60, 22), ref xText, vector.x, true);
		vecY = FloatSettingField(new Rect(x + 20, y + 47, 60, 22), ref yText, vector.y, true);
		vecZ = FloatSettingField(new Rect(x + 20, y + 72, 60, 22), ref zText, vector.z, true);

		return new Vector3(vecX, vecY, vecZ);
	}



	private void PropertiesControls(float x, float y, ref GameObject currentExtract)
	{
		GUI.Box(new Rect(x, y, 715, 60), "");

		// ----- Begin enabled -----
		GUI.Label(new Rect(x + 10, y + 5, 100, 100), "Enabled");

		if (!LockEditor)
		{
			bool enabled = currentExtract.GetComponent<Collider>().enabled;

			currentExtract.GetComponent<Collider>().enabled = GUI.Toggle(new Rect(x + 105, y + 4, 10, 22), enabled, "");

			if (enabled != currentExtract.GetComponent<Collider>().enabled)
				unsavedData = true;
		}
		else
			GUI.Toggle(new Rect(x + 105, y + 4, 10, 22), currentExtract.GetComponent<Collider>().enabled, "");
		// ----- End enabled -----

		// ----- Begin time -----
		GUI.Label(new Rect(x + 10, y + 30, 100, 100), "Time (seconds)");
		currentExtract.GetComponent<ExtractTestComponent>().Duration = FloatSettingField(new Rect(x + 105, y + 30, 35, 22), ref timeText, currentExtract.GetComponent<ExtractTestComponent>().Duration, lowerLimit: 0);
		// ----- End time -----
	}



	private void NavigationControls(float x, float y)
	{
		if (GUI.Button(new Rect(x, y, 80, 22), "Previous") && !LockEditor)
		{
			CustomExtractsManager.DecrementCurrentExtract();
			ResetAllFieldText();
		}

		if (GUI.Button(new Rect(x + 615, y, 80, 22), "Next") && !LockEditor)
		{
			CustomExtractsManager.IncrementCurrentExtract();
			ResetAllFieldText();
		}
	}



	private void SaveLoadControls()
	{
		Color originalBackgroundColor = GUI.backgroundColor;

		GUI.backgroundColor = Color.cyan;
		if (GUI.Button(new Rect(10, editorWindowRect.height - 40, 70, 30), "Save All") && !LockEditor)
		{
			// TODO: Save all custom extracts to a json file
			ResetAllFieldText();

			unsavedData = false;
		}

		GUI.backgroundColor = Color.yellow;
		if (GUI.Button(new Rect(90, editorWindowRect.height - 40, 70, 30), "Load All") && !LockEditor)
		{
			if (unsavedData)
				showUnsavedWarning = true;
			else
			{
				// TODO: Load custom extracts from json file
				ResetAllFieldText();
			}
		}

		GUI.backgroundColor = originalBackgroundColor;
	}



	private void NewDeleteControls(bool noExtracts)
	{
		Color originalBackgroundColor = GUI.backgroundColor;

		GUI.backgroundColor = Color.green;
		if (GUI.Button(new Rect(575, editorWindowRect.height - 40, 70, 30), "New") && !LockEditor)
		{
			// TODO: Create a new disabled extract at player position
			//       with default size, rotation, and time.

			CustomExtractsManager.CreateExtract(Vector3.zero, new Vector3(10, 10, 10), Vector3.zero, "NewExtract", 7f, false);
			ResetAllFieldText();
			unsavedData = true;
		}

		if (noExtracts)
			return;

		GUI.backgroundColor = Color.red;
		if (GUI.Button(new Rect(655, editorWindowRect.height - 40, 70, 30), "Delete") && !LockEditor)
			showDeleteWarning = true;

		GUI.backgroundColor = originalBackgroundColor;
	}



	private string TextSettingField(Rect rect, ref string text, string val)
	{
		Color originalTextColor = GUI.contentColor;
		string returnStr = val;

		// Get initial value text
		if (text == null)
			text = val.ToString();

		// Indicate value text has changed
		if (text != val)
			GUI.contentColor = Color.yellow;

		if (!LockEditor)
			text = GUI.TextField(rect, text);
		else
			GUI.TextField(rect, text); // TextField can't update if it's input string never changes

		GUI.contentColor = originalTextColor;

		if (GUI.Button(new Rect(rect.x + rect.width / 2 - 50, rect.y + rect.height + 10, 45, 22), "Set") && !LockEditor)
		{
			returnStr = text;
			unsavedData = true;
		}

		if (GUI.Button(new Rect(rect.x + rect.width / 2 + 5, rect.y + rect.height + 10, 45, 22), "Clear") && !LockEditor)
			text = returnStr;

		return returnStr;
	}



	private float FloatSettingField(Rect rect, ref string text, float val, bool incrementAndDecrementButtons = false, float? lowerLimit = null, float? upperLimit = null)
	{
		Color originalTextColor = GUI.contentColor;
		float returnVal = val;
		bool validFloat;

		// Get initial value text
		if (text == null)
			text = val.ToString();

		validFloat = float.TryParse(text, out float unused);

		if (validFloat && ((lowerLimit != null && float.Parse(text) < lowerLimit) || (upperLimit != null && float.Parse(text) > upperLimit)))
			validFloat = false;

		// Indicate value text has changed or is invalid
		if (text != val.ToString())
			GUI.contentColor = Color.yellow;
		if (!validFloat)
			GUI.contentColor = Color.red;

		if (!LockEditor)
			text = GUI.TextField(rect, text);
		else
			GUI.TextField(rect, text); // TextField can't update if it's input string never changes

		GUI.contentColor = originalTextColor;

		if (incrementAndDecrementButtons)
		{
			if (GUI.RepeatButton(new Rect(rect.x + rect.width + 5, rect.y, 22, 22), "-") && validFloat && !LockEditor)
			{
				float temp = float.Parse(text) - Time.deltaTime;
				text = temp.ToString();
			}

			if (GUI.RepeatButton(new Rect(rect.x + rect.width + 27, rect.y, 22, 22), "+") && validFloat && !LockEditor)
			{
				float temp = float.Parse(text) + Time.deltaTime;
				text = temp.ToString();
			}
		}

		if (GUI.Button(new Rect(rect.x + rect.width + (incrementAndDecrementButtons ? 54 : 5), rect.y, 45, 22), "Set") && !LockEditor)
		{
			if (!validFloat)
			{
				// TODO: Display notification indicating invalid float
			}
			else
			{
				returnVal = float.Parse(text);
				text = returnVal.ToString();

				unsavedData = true;
			}
		}

		if (GUI.Button(new Rect(rect.x + rect.width + (incrementAndDecrementButtons ? 104 : 55), rect.y, 45, 22), "Clear") && !LockEditor)
			text = val.ToString();

		return returnVal;
	}



	private void ResetAllFieldText()
	{
		nameText = null;
		positionXText = null;
		positionYText = null;
		positionZText = null;
		sizeXText = null;
		sizeYText = null;
		sizeZText = null;
		rotationXText = null;
		rotationYText = null;
		rotationZText = null;
		timeText = null;
	}
}
