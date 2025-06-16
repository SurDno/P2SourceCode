using System.Collections.Generic;

namespace Engine.Impl.UI.Menu.Main
{
  public class PlatformDependentObjectSwitcher : MonoBehaviour
  {
    [SerializeField]
    private List<GameObject> _pcControls;
    [SerializeField]
    private List<GameObject> _consoleControls;

    private void Awake()
    {
      bool flag = true;
      SetActiveAllGameObjects(!flag ? _pcControls : _consoleControls, false);
      SetActiveAllGameObjects(!flag ? _consoleControls : _pcControls, true);
    }

    private static void SetActiveAllGameObjects(List<GameObject> list, bool isActive)
    {
      if (list == null)
        return;
      foreach (GameObject gameObject in list)
        gameObject.SetActive(isActive);
    }
  }
}
