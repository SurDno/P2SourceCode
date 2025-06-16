using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class ControlTipsController : MonoBehaviour
  {
    public const string Main = "Main";
    public const string Move = "Move";
    [SerializeField]
    private List<TipsPanel> _panels;
    private TipsPanel _currentPanel;

    public void HidePannel(string name)
    {
      TipsPanel tipsPanel = this._panels.Find((Predicate<TipsPanel>) (p => p.Name.Equals(name)));
      if (tipsPanel == null)
        return;
      if (this._currentPanel != null)
        this._currentPanel.Panel.SetActive(false);
      this._currentPanel = tipsPanel;
      tipsPanel.Panel.SetActive(false);
    }

    public void ShowPannel(string name)
    {
      TipsPanel tipsPanel = this._panels.Find((Predicate<TipsPanel>) (p => p.Name.Equals(name)));
      if (tipsPanel == null)
        return;
      if (this._currentPanel != null)
        this._currentPanel.Panel.SetActive(false);
      this._currentPanel = tipsPanel;
      tipsPanel.Panel.SetActive(true);
    }

    public void HideAllPanel()
    {
      this._panels.ForEach((Action<TipsPanel>) (p => p.Panel.SetActive(false)));
    }
  }
}
