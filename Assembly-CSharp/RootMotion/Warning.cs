using UnityEngine;

namespace RootMotion
{
  public static class Warning
  {
    public static bool logged;

    public static void Log(string message, Warning.Logger logger, bool logInEditMode = false)
    {
      if (!logInEditMode && !Application.isPlaying || Warning.logged)
        return;
      if (logger != null)
        logger(message);
      Warning.logged = true;
    }

    public static void Log(string message, Transform context, bool logInEditMode = false)
    {
      if (!logInEditMode && !Application.isPlaying || Warning.logged)
        return;
      Debug.LogWarning((object) message, (Object) context);
      Warning.logged = true;
    }

    public delegate void Logger(string message);
  }
}
