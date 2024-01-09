using Aki.Reflection.Utils;
using EFT.UI;
using System.Linq;
using System.Reflection;

// From SamSWAT's Time and Weather Changer: https://github.com/dirtbikercj/timeweatherchanger/blob/main/project/SamSWAT.TimeWeatherChanger/Utils/CursorSettings.cs

namespace CustomExtracts
{
	public static class CursorSettings
	{
		private static readonly MethodInfo setCursorMethod;

		static CursorSettings()
		{
			var cursorType = PatchConstants.EftTypes.Single(x => x.GetMethod("SetCursor") != null);
			setCursorMethod = cursorType.GetMethod("SetCursor");
		}

		public static void SetCursor(ECursorType type)
		{
			setCursorMethod.Invoke(null, new object[] { type });
		}
	}
}
