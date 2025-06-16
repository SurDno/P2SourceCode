using SRDebugger.UI.Controls;
using SRF;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SRDebugger.UI.Other
{
  public class SRTabController : SRMonoBehaviour
  {
    private readonly SRList<SRTab> _tabs = new SRList<SRTab>();
    private SRTab _activeTab;
    public RectTransform TabButtonContainer;
    public SRTabButton TabButtonPrefab;
    public RectTransform TabContentsContainer;
    public RectTransform TabHeaderContentContainer;
    public Text TabHeaderText;

    public SRTab ActiveTab
    {
      get => this._activeTab;
      set => this.MakeActive(value);
    }

    public IList<SRTab> Tabs => (IList<SRTab>) this._tabs.AsReadOnly();

    public event Action<SRTabController, SRTab> ActiveTabChanged;

    public void AddTab(SRTab tab, bool visibleInSidebar = true)
    {
      tab.CachedTransform.SetParent((Transform) this.TabContentsContainer, false);
      tab.CachedGameObject.SetActive(false);
      if (visibleInSidebar)
      {
        SRTabButton srTabButton = SRInstantiate.Instantiate<SRTabButton>(this.TabButtonPrefab);
        srTabButton.CachedTransform.SetParent((Transform) this.TabButtonContainer, false);
        srTabButton.TitleText.text = tab.Title;
        srTabButton.IsActive = false;
        srTabButton.Button.onClick.AddListener((UnityAction) (() => this.MakeActive(tab)));
        tab.TabButton = srTabButton;
      }
      this._tabs.Add(tab);
      this.SortTabs();
      if (this._tabs.Count != 1)
        return;
      this.ActiveTab = tab;
    }

    private void MakeActive(SRTab tab)
    {
      if (!this._tabs.Contains(tab))
        throw new ArgumentException("tab is not a member of this tab controller", nameof (tab));
      if ((UnityEngine.Object) this._activeTab != (UnityEngine.Object) null)
      {
        this._activeTab.CachedGameObject.SetActive(false);
        if ((UnityEngine.Object) this._activeTab.TabButton != (UnityEngine.Object) null)
          this._activeTab.TabButton.IsActive = false;
        if ((UnityEngine.Object) this._activeTab.HeaderExtraContent != (UnityEngine.Object) null)
          this._activeTab.HeaderExtraContent.gameObject.SetActive(false);
      }
      this._activeTab = tab;
      if ((UnityEngine.Object) this._activeTab != (UnityEngine.Object) null)
      {
        this._activeTab.CachedGameObject.SetActive(true);
        this.TabHeaderText.text = this._activeTab.Title;
        if ((UnityEngine.Object) this._activeTab.TabButton != (UnityEngine.Object) null)
          this._activeTab.TabButton.IsActive = true;
        if ((UnityEngine.Object) this._activeTab.HeaderExtraContent != (UnityEngine.Object) null)
        {
          this._activeTab.HeaderExtraContent.SetParent((Transform) this.TabHeaderContentContainer, false);
          this._activeTab.HeaderExtraContent.gameObject.SetActive(true);
        }
      }
      Action<SRTabController, SRTab> activeTabChanged = this.ActiveTabChanged;
      if (activeTabChanged == null)
        return;
      activeTabChanged(this, this._activeTab);
    }

    private void SortTabs()
    {
      this._tabs.Sort((Comparison<SRTab>) ((t1, t2) => t1.SortIndex.CompareTo(t2.SortIndex)));
      for (int index = 0; index < this._tabs.Count; ++index)
      {
        if ((UnityEngine.Object) this._tabs[index].TabButton != (UnityEngine.Object) null)
          this._tabs[index].TabButton.CachedTransform.SetSiblingIndex(index);
      }
    }
  }
}
