// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.PlatformDependentObjectSwitcher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
