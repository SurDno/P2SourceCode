using UnityEngine;

namespace RootMotion;

public static class Warning {
	public static bool logged;

	public static void Log(string message, Logger logger, bool logInEditMode = false) {
		if ((!logInEditMode && !Application.isPlaying) || logged)
			return;
		if (logger != null)
			logger(message);
		logged = true;
	}

	public static void Log(string message, Transform context, bool logInEditMode = false) {
		if ((!logInEditMode && !Application.isPlaying) || logged)
			return;
		Debug.LogWarning(message, context);
		logged = true;
	}

	public delegate void Logger(string message);
}