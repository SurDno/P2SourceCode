// Decompiled with JetBrains decompiler
// Type: SRDebugger.Scripts.DebuggerTabController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRDebugger.UI.Other;
using SRF;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
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
        DefaultTabs tab = this.TabController.ActiveTab.Tab;
        return !Enum.IsDefined(typeof (DefaultTabs), (object) tab) ? new DefaultTabs?() : new DefaultTabs?(tab);
      }
    }

    protected void Start()
    {
      this._hasStarted = true;
      SRTab[] srTabArray = Resources.LoadAll<SRTab>("SRDebugger/UI/Prefabs");
      Enum.GetNames(typeof (DefaultTabs));
      foreach (SRTab prefab in srTabArray)
      {
        if (!(prefab.GetComponent(typeof (IEnableTab)) is IEnableTab component) || component.IsEnabled)
          this.TabController.AddTab(SRInstantiate.Instantiate<SRTab>(prefab));
      }
      if (this.OpenTab((DefaultTabs) ((int) this._activeTab ?? (int) Settings.Instance.DefaultTab)))
        return;
      this.TabController.ActiveTab = this.TabController.Tabs.FirstOrDefault<SRTab>();
    }

    public bool OpenTab(DefaultTabs tab)
    {
      if (!this._hasStarted)
      {
        this._activeTab = new DefaultTabs?(tab);
        return true;
      }
      foreach (SRTab tab1 in (IEnumerable<SRTab>) this.TabController.Tabs)
      {
        if (tab1.Tab == tab)
        {
          this.TabController.ActiveTab = tab1;
          return true;
        }
      }
      return false;
    }
  }
}
