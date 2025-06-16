namespace SRDebugger.Internal
{
  public static class SRDebuggerUtil
  {
    public static bool EnsureEventSystemExists()
    {
      if ((Object) EventSystem.current != (Object) null)
        return false;
      EventSystem objectOfType = Object.FindObjectOfType<EventSystem>();
      if ((Object) objectOfType != (Object) null && objectOfType.gameObject.activeSelf && objectOfType.enabled)
        return false;
      Debug.LogWarning((object) "[SRDebugger] No EventSystem found in scene - creating a default one.");
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
