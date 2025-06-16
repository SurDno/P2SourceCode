using UnityEngine;
using UnityEngine.EventSystems;

namespace SRDebugger.Internal
{
  public static class SRDebuggerUtil
  {
    public static bool EnsureEventSystemExists()
    {
      if (EventSystem.current != null)
        return false;
      EventSystem objectOfType = Object.FindObjectOfType<EventSystem>();
      if (objectOfType != null && objectOfType.gameObject.activeSelf && objectOfType.enabled)
        return false;
      Debug.LogWarning("[SRDebugger] No EventSystem found in scene - creating a default one.");
      CreateDefaultEventSystem();
      return true;
    }

    public static void CreateDefaultEventSystem()
    {
      GameObject gameObject = new GameObject("EventSystem");
      gameObject.AddComponent<EventSystem>();
      gameObject.AddComponent<StandaloneInputModule>();
    }
  }
}
