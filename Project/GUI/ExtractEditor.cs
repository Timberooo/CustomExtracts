using Comfort.Common;
using EFT;
using EFT.UI;
using UnityEngine;

namespace CustomExtracts
{
	internal class ExtractEditor : MonoBehaviour
	{
		internal static bool ShowEditor { get; private set; } = false;



		private CustomExtractsManager _customExtractsManager = null;
		private GameObject _input;

		private Rect _editorWindowRect         = new Rect(50, 60, 785, 333);
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

			if (_customExtractsManager == null)
				if (Singleton<GameWorld>.Instantiated)
					_customExtractsManager = Singleton<GameWorld>.Instance.gameObject.GetComponent<CustomExtractsManager>();

			if (_input == null)
				_input = GameObject.Find("___Input");

			ShowEditor = !ShowEditor;
			Cursor.visible = ShowEditor;

			if (ShowEditor)
			{
				CursorSettings.SetCursor(ECursorType.Idle);
				Cursor.lockState = CursorLockMode.None;
				Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuContextMenu);

				_customExtractsManager.ShowExtracts(true);
			}
			else
			{
				CursorSettings.SetCursor(ECursorType.Invisible);
				Cursor.lockState = CursorLockMode.Locked;
				Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuDropdown);

				if (Plugin.AlwaysShowExtracts.Value == false)
					_customExtractsManager.ShowExtracts(false);
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
				GUI.Label(new Rect(341, _editorWindowRect.height - 35, 110, 25), "unsaved changes");

				GUI.contentColor = originalTextColor;
			}

			NameControls(10, 25);

			if (!_customExtractsManager.NoExtracts)
			{
				_customExtractsManager.CurrentExtractPosition = TransformControls(10, 120, "Position", "X", "Y", "Z", 15, ref _positionXText, ref _positionYText, ref _positionZText, _customExtractsManager.CurrentExtractPosition);
				_customExtractsManager.CurrentExtractSize = TransformControls(250, 120, "Size", "Width", "Height", "Length", 45, ref _sizeXText, ref _sizeYText, ref _sizeZText, _customExtractsManager.CurrentExtractSize);
				_customExtractsManager.CurrentExtractEulerAngles = TransformControls(520, 120, "Rotation", "Pitch", "Yaw", "Roll", 35, ref _rotationXText, ref _rotationYText, ref _rotationZText, _customExtractsManager.CurrentExtractEulerAngles);
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
				_customExtractsManager.DestroyCurrentExtract();
				ResetAllFieldText();

				_showDeleteWarning = false;
				_unsavedData = true;
			}
		}



		// ===== Controls =====

		private void NameControls(float x, float y)
		{
			GUI.Box(new Rect(x, y, 765, 90), "");

			if (!_customExtractsManager.NoExtracts)
				_customExtractsManager.CurrentExtractName = StringSettingField(new Rect(x + 100, y + 20, 565, 22), ref _nameText, _customExtractsManager.CurrentExtractName);
			else
			{
				Color originalTextColor = GUI.contentColor;

				GUI.contentColor = Color.red;
				GUI.TextField(new Rect(x + 100, y + 20, 565, 22), "no extracts to edit");
				GUI.contentColor = originalTextColor;
				return;
			}
		}



		private Vector3 TransformControls(float x, float y, string name, string param1Name, string param2Name, string param3Name, float paramNameOffset, ref string param1Text, ref string param2Text, ref string param3Text, Vector3 vector)
		{
			float vecX, vecY, vecZ;

			GUI.Box(new Rect(x, y, 220 + paramNameOffset, 100), name);

			vecX = FloatSettingField(x + 5, y + 22, param1Name, paramNameOffset, ref param1Text, vector.x, true);
			vecY = FloatSettingField(x + 5, y + 47, param2Name, paramNameOffset, ref param2Text, vector.y, true);
			vecZ = FloatSettingField(x + 5, y + 72, param3Name, paramNameOffset, ref param3Text, vector.z, true);

			return new Vector3(vecX, vecY, vecZ);
		}



		private void PropertiesControls(float x, float y)
		{
			GUI.Box(new Rect(x, y, 765, 60), "");

			// ----- Begin enabled -----
			GUI.Label(new Rect(x + 10, y + 5, 100, 100), "Enabled");

			if (!LockEditor)
			{
				bool prevEnabled = _customExtractsManager.CurrentExtractEnabled;
				_customExtractsManager.CurrentExtractEnabled = GUI.Toggle(new Rect(x + 105, y + 4, 10, 22), _customExtractsManager.CurrentExtractEnabled, "");

				if (prevEnabled != _customExtractsManager.CurrentExtractEnabled)
					_unsavedData = true;
			}
			else
				GUI.Toggle(new Rect(x + 105, y + 4, 10, 22), _customExtractsManager.CurrentExtractEnabled, "");
			// ----- End enabled -----

			// ----- Begin time -----
			_customExtractsManager.CurrentExtractTime = FloatSettingField(15, 255, "Time (seconds)", 95, ref _timeText, _customExtractsManager.CurrentExtractTime, lowerLimit: 0);
			// ----- End time -----
		}



		private void NavigationControls(float x, float y)
		{
			if (GUI.Button(new Rect(x, y, 80, 22), "Previous") && !LockEditor)
			{
				_customExtractsManager.DecrementCurrentExtract();
				ResetAllFieldText();
			}

			if (GUI.Button(new Rect(x + 665, y, 80, 22), "Next") && !LockEditor)
			{
				_customExtractsManager.IncrementCurrentExtract();
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
			if (GUI.Button(new Rect(625, _editorWindowRect.height - 40, 70, 30), "New") && !LockEditor)
			{
				// TODO: Create a new disabled extract at player position
				//       with default size, rotation, and time.

				_customExtractsManager.CreateExtract("NewExtract", Singleton<GameWorld>.Instance.MainPlayer.Transform.position, new Vector3(10f, 10f, 10f), Vector3.zero, enabled: false);
				ResetAllFieldText();
				_unsavedData = true;
			}

			if (_customExtractsManager.NoExtracts)
				return;

			GUI.backgroundColor = Color.red;
			if (GUI.Button(new Rect(705, _editorWindowRect.height - 40, 70, 30), "Delete") && !LockEditor)
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



		private float FloatSettingField(float x, float y, string name, float nameWidth, ref string displayText, float val, bool incrementAndDecrementButtons = false, float? lowerLimit = null, float? upperLimit = null)
		{
			GUI.Label(new Rect(x, y, nameWidth, 25), name);

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
				displayText = GUI.TextField(new Rect(x + nameWidth, y, 60, 22), displayText);
			else
				GUI.TextField(new Rect(x + nameWidth, y, 60, 22), displayText); // TextField can't update if it's input string never changes

			GUI.contentColor = originalTextColor;

			if (incrementAndDecrementButtons)
			{
				if (GUI.RepeatButton(new Rect(x + nameWidth + 65, y, 22, 22), "-") && validFloat && !LockEditor)
				{
					float temp = float.Parse(displayText) - Time.deltaTime;
					displayText = temp.ToString();
				}

				if (GUI.RepeatButton(new Rect(x + nameWidth + 87, y, 22, 22), "+") && validFloat && !LockEditor)
				{
					float temp = float.Parse(displayText) + Time.deltaTime;
					displayText = temp.ToString();
				}
			}

			if (GUI.Button(new Rect(x + nameWidth + (incrementAndDecrementButtons ? 114 : 65), y, 45, 22), "Set") && !LockEditor)
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

			if (GUI.Button(new Rect(x + nameWidth + (incrementAndDecrementButtons ? 164 : 115), y, 45, 22), "Clear") && !LockEditor)
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
