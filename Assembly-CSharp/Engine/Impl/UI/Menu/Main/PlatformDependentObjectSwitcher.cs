using System.Collections.Generic;
using UnityEngine;

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
      PlatformDependentObjectSwitcher.SetActiveAllGameObjects(!flag ? this._pcControls : this._consoleControls, false);
      PlatformDependentObjectSwitcher.SetActiveAllGameObjects(!flag ? this._consoleControls : this._pcControls, true);
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
