using Comfort.Common;
using EFT;
using EFT.UI;
using UnityEngine;

namespace CustomExtracts
{
	internal class ExtractEditor : MonoBehaviour
	{
		internal static bool ShowEditor { get; private set; } = false;



		private GameObject _input;

		private Rect _editorWindowRect         = new Rect(50, 60, 735, 333);
		private Rect _unsavedWarningWindowRect = new Rect(850, 500, 350, 130);
		private Rect _deleteWarningWindowRect  = new Rect(850, 500, 300, 130);

		private bool _unsavedData = false;

		private bool _showUnsavedWarning = false;
		private bool _showDeleteWarning  = false;

		// GUI.TextField backing fields
		private string _nameText      = null;
		private string _positionXText = null;
		private string _positionYText = null;
		private string _positionZText = null;
		private string _sizeXText     = null;
		private string _sizeYText     = null;
		private string _sizeZText     = null;
		private string _rotationXText = null;
		private string _rotationYText = null;
		private string _rotationZText = null;
		private string _timeText      = null;



		private bool LockEditor { get { return _showUnsavedWarning || _showDeleteWarning; } }



		private void Update()
		{
			if (!Input.GetKeyDown(Plugin.ToggleExtractEditor.Value.MainKey))
				return;

			// TODO: Check if player is in hideout then display error message.

			if (_input == null)
				_input = GameObject.Find("___Input");

			ShowEditor = !ShowEditor;
			Cursor.visible = ShowEditor;

			if (ShowEditor)
			{
				CursorSettings.SetCursor(ECursorType.Idle);
				Cursor.lockState = CursorLockMode.None;
				Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuContextMenu);

				CustomExtractsManager.ShowExtracts(true);
			}
			else
			{
				CursorSettings.SetCursor(ECursorType.Invisible);
				Cursor.lockState = CursorLockMode.Locked;
				Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuDropdown);

				if (Plugin.AlwaysShowExtracts.Value == false)
					CustomExtractsManager.ShowExtracts(false);
			}

			_input.SetActive(!ShowEditor);
		}



		private void OnDestroy()
		{
			ShowEditor = false;
			_showDeleteWarning = false;
			_showUnsavedWarning = false;
		}



		private void OnGUI()
		{
			if (!ShowEditor)
				return;

			_editorWindowRect = GUI.Window(420, _editorWindowRect, EditorWindowFunc, "Custom Extract Editor");

			if (_showUnsavedWarning)
				_unsavedWarningWindowRect = GUI.Window(421, _unsavedWarningWindowRect, UnsavedWarningWindowFunc, "Warning!");
			if (_showDeleteWarning)
				_deleteWarningWindowRect = GUI.Window(422, _deleteWarningWindowRect, DeleteWarningWindowFunc, "Warning!");
		}



		// ===== Window Functions =====

		private void EditorWindowFunc(int windowId)
		{
			GUI.DragWindow(new Rect(0, 0, _editorWindowRect.width, 20));

			if (_unsavedData)
			{
				Color originalTextColor = GUI.contentColor;

				GUI.contentColor = Color.yellow;
				GUI.Label(new Rect(316, _editorWindowRect.height - 35, 110, 25), "unsaved changes");

				GUI.contentColor = originalTextColor;
			}

			NameControls(10, 25);

			if (!CustomExtractsManager.NoExtracts)
			{
				CustomExtractsManager.CurrentExtractPosition = TransformControls(10, 120, "Position", ref _positionXText, ref _positionYText, ref _positionZText, CustomExtractsManager.CurrentExtractPosition);
				CustomExtractsManager.CurrentExtractSize = TransformControls(250, 120, "Size", ref _sizeXText, ref _sizeYText, ref _sizeZText, CustomExtractsManager.CurrentExtractSize);
				CustomExtractsManager.CurrentExtractEulerAngles = TransformControls(490, 120, "Rotation", ref _rotationXText, ref _rotationYText, ref _rotationZText, CustomExtractsManager.CurrentExtractEulerAngles);
				PropertiesControls(10, 225);

				NavigationControls(20, 45);
			}

			SaveLoadControls();
			NewDeleteControls();
		}



		private void UnsavedWarningWindowFunc(int windowId)
		{
			GUI.DragWindow(new Rect(0, 0, _unsavedWarningWindowRect.width, 20));

			GUI.Box(new Rect(1, 17, 348, _unsavedWarningWindowRect.height - 18), "");
			GUI.Label(new Rect(30, 38, 300, 200), "Do you want to save your changes before loading?");

			if (GUI.Button(new Rect(_unsavedWarningWindowRect.width / 2 + 50, 80, 80, 30), "Cancel"))
				_showUnsavedWarning = false;

			GUI.backgroundColor = Color.cyan;
			if (GUI.Button(new Rect(_unsavedWarningWindowRect.width / 2 - 130, 80, 80, 30), "Save All"))
			{
				// TODO: Save all custom extracts to a json file, then load custom extracts from json file
				ResetAllFieldText();

				_unsavedData = false;
				_showUnsavedWarning = false;
			}

			GUI.backgroundColor = Color.red;
			if (GUI.Button(new Rect(_unsavedWarningWindowRect.width / 2 - 40, 80, 80, 30), "Don't Save"))
			{
				// TODO: Load custom extracts from json file
				ResetAllFieldText();

				_unsavedData = false;
				_showUnsavedWarning = false;
			}
		}



		private void DeleteWarningWindowFunc(int windowId)
		{
			GUI.DragWindow(new Rect(0, 0, _deleteWarningWindowRect.width, 20));

			GUI.Box(new Rect(1, 17, 298, _deleteWarningWindowRect.height - 18), "");
			GUI.Label(new Rect(19, 38, 300, 200), "Are you sure you want to delete this extract?");

			if (GUI.Button(new Rect(_deleteWarningWindowRect.width / 2 + 10, 80, 80, 30), "Cancel"))
				_showDeleteWarning = false;

			GUI.backgroundColor = Color.red;
			if (GUI.Button(new Rect(_deleteWarningWindowRect.width / 2 - 80, 80, 80, 30), "Delete"))
			{
				CustomExtractsManager.DestroyCurrentExtract();
				ResetAllFieldText();

				_showDeleteWarning = false;
				_unsavedData = true;
			}
		}



		// ===== Controls =====

		private void NameControls(float x, float y)
		{
			GUI.Box(new Rect(x, y, 715, 90), "");

			if (!CustomExtractsManager.NoExtracts)
				CustomExtractsManager.CurrentExtractName = StringSettingField(new Rect(x + 100, y + 20, 515, 22), ref _nameText, CustomExtractsManager.CurrentExtractName);
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



		private void PropertiesControls(float x, float y)
		{
			GUI.Box(new Rect(x, y, 715, 60), "");

			// ----- Begin enabled -----
			GUI.Label(new Rect(x + 10, y + 5, 100, 100), "Enabled");

			if (!LockEditor)
			{
				bool prevEnabled = CustomExtractsManager.CurrentExtractEnabled;
				CustomExtractsManager.CurrentExtractEnabled = GUI.Toggle(new Rect(x + 105, y + 4, 10, 22), CustomExtractsManager.CurrentExtractEnabled, "");

				if (prevEnabled != CustomExtractsManager.CurrentExtractEnabled)
					_unsavedData = true;
			}
			else
				GUI.Toggle(new Rect(x + 105, y + 4, 10, 22), CustomExtractsManager.CurrentExtractEnabled, "");
			// ----- End enabled -----

			// ----- Begin time -----
			GUI.Label(new Rect(x + 10, y + 30, 100, 100), "Time (seconds)");
			CustomExtractsManager.CurrentExtractTime = FloatSettingField(new Rect(x + 105, y + 30, 35, 22), ref _timeText, CustomExtractsManager.CurrentExtractTime, lowerLimit: 0);
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
			if (GUI.Button(new Rect(10, _editorWindowRect.height - 40, 70, 30), "Save All") && !LockEditor)
			{
				// TODO: Save all custom extracts to a json file
				ResetAllFieldText();

				_unsavedData = false;
			}

			GUI.backgroundColor = Color.yellow;
			if (GUI.Button(new Rect(90, _editorWindowRect.height - 40, 70, 30), "Load All") && !LockEditor)
			{
				if (_unsavedData)
					_showUnsavedWarning = true;
				else
				{
					// TODO: Load custom extracts from json file
					ResetAllFieldText();
				}
			}

			GUI.backgroundColor = originalBackgroundColor;
		}



		private void NewDeleteControls()
		{
			Color originalBackgroundColor = GUI.backgroundColor;

			GUI.backgroundColor = Color.green;
			if (GUI.Button(new Rect(575, _editorWindowRect.height - 40, 70, 30), "New") && !LockEditor)
			{
				// TODO: Create a new disabled extract at player position
				//       with default size, rotation, and time.

				CustomExtractsManager.CreateExtract("NewExtract", Singleton<GameWorld>.Instance.MainPlayer.Transform.position, new Vector3(10f, 10f, 10f), Vector3.zero, enabled: false);
				ResetAllFieldText();
				_unsavedData = true;
			}

			if (CustomExtractsManager.NoExtracts)
				return;

			GUI.backgroundColor = Color.red;
			if (GUI.Button(new Rect(655, _editorWindowRect.height - 40, 70, 30), "Delete") && !LockEditor)
				_showDeleteWarning = true;

			GUI.backgroundColor = originalBackgroundColor;
		}



		// ===== Setting Fields =====

		private string StringSettingField(Rect rect, ref string displayText, string val)
		{
			Color originalTextColor = GUI.contentColor;
			string returnStr = val;

			// Get initial value text
			if (displayText == null)
				displayText = val.ToString();

			// Indicate text has changed
			if (displayText != val)
				GUI.contentColor = Color.yellow;

			if (!LockEditor)
				displayText = GUI.TextField(rect, displayText);
			else
				GUI.TextField(rect, displayText); // TextField can't update if it's input string never changes

			GUI.contentColor = originalTextColor;

			if (GUI.Button(new Rect(rect.x + rect.width / 2 - 50, rect.y + rect.height + 10, 45, 22), "Set") && !LockEditor)
			{
				returnStr = displayText;
				_unsavedData = true;
			}

			if (GUI.Button(new Rect(rect.x + rect.width / 2 + 5, rect.y + rect.height + 10, 45, 22), "Clear") && !LockEditor)
				displayText = returnStr;

			return returnStr;
		}



		private float FloatSettingField(Rect rect, ref string displayText, float val, bool incrementAndDecrementButtons = false, float? lowerLimit = null, float? upperLimit = null)
		{
			Color originalTextColor = GUI.contentColor;
			float returnVal = val;
			bool validFloat;

			// Get initial value text
			if (displayText == null)
				displayText = val.ToString();

			validFloat = float.TryParse(displayText, out float unused);

			if (validFloat && ((lowerLimit != null && float.Parse(displayText) < lowerLimit) || (upperLimit != null && float.Parse(displayText) > upperLimit)))
				validFloat = false;

			// Indicate text has changed or is invalid
			if (displayText != val.ToString())
				GUI.contentColor = Color.yellow;
			if (!validFloat)
				GUI.contentColor = Color.red;

			if (!LockEditor)
				displayText = GUI.TextField(rect, displayText);
			else
				GUI.TextField(rect, displayText); // TextField can't update if it's input string never changes

			GUI.contentColor = originalTextColor;

			if (incrementAndDecrementButtons)
			{
				if (GUI.RepeatButton(new Rect(rect.x + rect.width + 5, rect.y, 22, 22), "-") && validFloat && !LockEditor)
				{
					float temp = float.Parse(displayText) - Time.deltaTime;
					displayText = temp.ToString();
				}

				if (GUI.RepeatButton(new Rect(rect.x + rect.width + 27, rect.y, 22, 22), "+") && validFloat && !LockEditor)
				{
					float temp = float.Parse(displayText) + Time.deltaTime;
					displayText = temp.ToString();
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
					returnVal = float.Parse(displayText);
					displayText = returnVal.ToString(); // text may contain leading or trailing periods that are truncated for the float, so get float.ToString()'s version of the string for consistency

					_unsavedData = true;
				}
			}

			if (GUI.Button(new Rect(rect.x + rect.width + (incrementAndDecrementButtons ? 104 : 55), rect.y, 45, 22), "Clear") && !LockEditor)
				displayText = val.ToString();

			return returnVal;
		}



		private void ResetAllFieldText()
		{
			_nameText = null;
			_positionXText = null;
			_positionYText = null;
			_positionZText = null;
			_sizeXText = null;
			_sizeYText = null;
			_sizeZText = null;
			_rotationXText = null;
			_rotationYText = null;
			_rotationZText = null;
			_timeText = null;
		}
	}
}
