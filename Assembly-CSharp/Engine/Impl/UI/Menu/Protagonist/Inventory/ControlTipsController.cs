using System.Collections.Generic;

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
      TipsPanel tipsPanel = _panels.Find(p => p.Name.Equals(name));
      if (tipsPanel == null)
        return;
      if (_currentPanel != null)
        _currentPanel.Panel.SetActive(false);
      _currentPanel = tipsPanel;
      tipsPanel.Panel.SetActive(false);
    }

    public void ShowPannel(string name)
    {
      TipsPanel tipsPanel = _panels.Find(p => p.Name.Equals(name));
      if (tipsPanel == null)
        return;
      if (_currentPanel != null)
        _currentPanel.Panel.SetActive(false);
      _currentPanel = tipsPanel;
      tipsPanel.Panel.SetActive(true);
    }

    public void HideAllPanel()
    {
      _panels.ForEach(p => p.Panel.SetActive(false));
    }
  }
}
