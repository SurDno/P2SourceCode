using System;
using System.Linq;
using SRDebugger.UI.Other;
using SRF;
using UnityEngine;

namespace SRDebugger.Scripts
{
  public class DebuggerTabController : SRMonoBehaviour
  {
    private DefaultTabs? _activeTab;
    private bool _hasStarted;
    public SRTabController TabController;

    public DefaultTabs? ActiveTab
    {
      get
      {
        DefaultTabs tab = TabController.ActiveTab.Tab;
        return !Enum.IsDefined(typeof (DefaultTabs), tab) ? new DefaultTabs?() : tab;
      }
    }

    protected void Start()
    {
      _hasStarted = true;
      SRTab[] srTabArray = Resources.LoadAll<SRTab>("SRDebugger/UI/Prefabs");
      Enum.GetNames(typeof (DefaultTabs));
      foreach (SRTab prefab in srTabArray)
      {
        if (!(prefab.GetComponent(typeof (IEnableTab)) is IEnableTab component) || component.IsEnabled)
          TabController.AddTab(SRInstantiate.Instantiate(prefab));
      }
      if (OpenTab((DefaultTabs) ((int) _activeTab ?? (int) Settings.Instance.DefaultTab)))
        return;
      TabController.ActiveTab = TabController.Tabs.FirstOrDefault();
    }

    public bool OpenTab(DefaultTabs tab)
    {
      if (!_hasStarted)
      {
        _activeTab = tab;
        return true;
      }
      foreach (SRTab tab1 in TabController.Tabs)
      {
        if (tab1.Tab == tab)
        {
          TabController.ActiveTab = tab1;
          return true;
        }
      }
      return false;
    }
  }
}
