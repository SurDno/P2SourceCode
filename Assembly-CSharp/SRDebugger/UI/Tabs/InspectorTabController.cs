using Inspectors;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRF;
using UnityEngine;

namespace SRDebugger.UI.Tabs
{
  public class InspectorTabController : SRMonoBehaviour
  {
    [SerializeField]
    private RectTransform layoutContainer;
    private Rect rect;
    private InspectedProvider inspector = new(RuntimeInspectedDrawer.Instance);
    private Vector2 scrollInspector = Vector2.zero;
    private Vector2 scrollMenu = Vector2.zero;

    private void Start()
    {
      Service.Panel.VisibilityChanged += PanelOnVisibilityChanged;
    }

    private void PanelOnVisibilityChanged(IDebugPanelService debugPanelService, bool visible)
    {
      enabled = visible;
    }

    private void OnGUI()
    {
      if (layoutContainer == null)
        return;
      rect = RectTransformToScreenSpace(layoutContainer);
      GUISkin skin = GUI.skin;
      GUI.skin = RuntimeInspectedDrawer.Instance.Skin;
      GUILayout.BeginArea(rect);
      if (RuntimeInspectedDrawer.Instance.MenuVisible)
      {
        scrollMenu = GUILayout.BeginScrollView(scrollMenu);
        RuntimeInspectedDrawer.Instance.DrawContextMenu();
        GUILayout.EndScrollView();
      }
      else
      {
        scrollInspector = GUILayout.BeginScrollView(scrollInspector);
        inspector.Draw(MonoBehaviourInstance<EngineInspector>.Instance, null);
        GUILayout.EndScrollView();
      }
      GUILayout.EndArea();
      GUI.skin = skin;
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
      Vector2 vector2 = Vector2.Scale(transform.rect.size, transform.lossyScale);
      Rect screenSpace = new Rect(transform.position.x, Screen.height - transform.position.y, vector2.x, vector2.y);
      screenSpace.x -= transform.pivot.x * vector2.x;
      screenSpace.y -= (1f - transform.pivot.y) * vector2.y;
      return screenSpace;
    }
  }
}
