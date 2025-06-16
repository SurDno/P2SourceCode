using System;
using System.Collections.Generic;
using SRDebugger.UI.Controls;
using SRF;

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
      get => _activeTab;
      set => MakeActive(value);
    }

    public IList<SRTab> Tabs => _tabs.AsReadOnly();

    public event Action<SRTabController, SRTab> ActiveTabChanged;

    public void AddTab(SRTab tab, bool visibleInSidebar = true)
    {
      tab.CachedTransform.SetParent((Transform) TabContentsContainer, false);
      tab.CachedGameObject.SetActive(false);
      if (visibleInSidebar)
      {
        SRTabButton srTabButton = SRInstantiate.Instantiate<SRTabButton>(TabButtonPrefab);
        srTabButton.CachedTransform.SetParent((Transform) TabButtonContainer, false);
        srTabButton.TitleText.text = tab.Title;
        srTabButton.IsActive = false;
        srTabButton.Button.onClick.AddListener((UnityAction) (() => MakeActive(tab)));
        tab.TabButton = srTabButton;
      }
      _tabs.Add(tab);
      SortTabs();
      if (_tabs.Count != 1)
        return;
      ActiveTab = tab;
    }

    private void MakeActive(SRTab tab)
    {
      if (!_tabs.Contains(tab))
        throw new ArgumentException("tab is not a member of this tab controller", nameof (tab));
      if ((UnityEngine.Object) _activeTab != (UnityEngine.Object) null)
      {
        _activeTab.CachedGameObject.SetActive(false);
        if ((UnityEngine.Object) _activeTab.TabButton != (UnityEngine.Object) null)
          _activeTab.TabButton.IsActive = false;
        if ((UnityEngine.Object) _activeTab.HeaderExtraContent != (UnityEngine.Object) null)
          _activeTab.HeaderExtraContent.gameObject.SetActive(false);
      }
      _activeTab = tab;
      if ((UnityEngine.Object) _activeTab != (UnityEngine.Object) null)
      {
        _activeTab.CachedGameObject.SetActive(true);
        TabHeaderText.text = _activeTab.Title;
        if ((UnityEngine.Object) _activeTab.TabButton != (UnityEngine.Object) null)
          _activeTab.TabButton.IsActive = true;
        if ((UnityEngine.Object) _activeTab.HeaderExtraContent != (UnityEngine.Object) null)
        {
          _activeTab.HeaderExtraContent.SetParent((Transform) TabHeaderContentContainer, false);
          _activeTab.HeaderExtraContent.gameObject.SetActive(true);
        }
      }
      Action<SRTabController, SRTab> activeTabChanged = ActiveTabChanged;
      if (activeTabChanged == null)
        return;
      activeTabChanged(this, _activeTab);
    }

    private void SortTabs()
    {
      _tabs.Sort((t1, t2) => t1.SortIndex.CompareTo(t2.SortIndex));
      for (int index = 0; index < _tabs.Count; ++index)
      {
        if ((UnityEngine.Object) _tabs[index].TabButton != (UnityEngine.Object) null)
          _tabs[index].TabButton.CachedTransform.SetSiblingIndex(index);
      }
    }
  }
}
