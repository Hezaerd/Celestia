using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace MHL.Core.Logs
{
	public static class Logz
	{
		private const string InfoColor = nameof(Color.white);
		private const string WarnColor = nameof(Color.yellow);
		private const string ErrorColor = nameof(Color.red);
		
		
		[Conditional("DEBUG")]
		public static void Info(object tag, object message)
		{
			Debug.Log(FormatMessage(InfoColor, tag, message));
		}

		[Conditional("DEBUG")]
		public static void Warn(object tag, object message)
		{
			Debug.LogWarning(FormatMessage(WarnColor, tag, message));
		}

		[Conditional("DEBUG")]
		public static void Error(object tag, object message)
		{
			Debug.LogError(FormatMessage(ErrorColor, tag, message));
		}
		
		private static string FormatMessage(string color, object tag, object message)
		{
			return $"[{tag}]: <color={color}>{message}</color>";
		}
	}
}