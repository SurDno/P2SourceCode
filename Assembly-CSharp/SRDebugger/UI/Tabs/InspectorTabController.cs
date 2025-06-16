using Inspectors;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRF;
using System;
using UnityEngine;

namespace SRDebugger.UI.Tabs
{
  public class InspectorTabController : SRMonoBehaviour
  {
    [SerializeField]
    private RectTransform layoutContainer;
    private Rect rect;
    private InspectedProvider inspector = new InspectedProvider((IInspectedDrawer) RuntimeInspectedDrawer.Instance);
    private Vector2 scrollInspector = Vector2.zero;
    private Vector2 scrollMenu = Vector2.zero;

    private void Start()
    {
      Service.Panel.VisibilityChanged += new Action<IDebugPanelService, bool>(this.PanelOnVisibilityChanged);
    }

    private void PanelOnVisibilityChanged(IDebugPanelService debugPanelService, bool visible)
    {
      this.enabled = visible;
    }

    private void OnGUI()
    {
      if ((UnityEngine.Object) this.layoutContainer == (UnityEngine.Object) null)
        return;
      this.rect = InspectorTabController.RectTransformToScreenSpace(this.layoutContainer);
      GUISkin skin = GUI.skin;
      GUI.skin = RuntimeInspectedDrawer.Instance.Skin;
      GUILayout.BeginArea(this.rect);
      if (RuntimeInspectedDrawer.Instance.MenuVisible)
      {
        this.scrollMenu = GUILayout.BeginScrollView(this.scrollMenu);
        RuntimeInspectedDrawer.Instance.DrawContextMenu();
        GUILayout.EndScrollView();
      }
      else
      {
        this.scrollInspector = GUILayout.BeginScrollView(this.scrollInspector);
        this.inspector.Draw((object) MonoBehaviourInstance<EngineInspector>.Instance, (Action<object>) null);
        GUILayout.EndScrollView();
      }
      GUILayout.EndArea();
      GUI.skin = skin;
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
      Vector2 vector2 = Vector2.Scale(transform.rect.size, (Vector2) transform.lossyScale);
      Rect screenSpace = new Rect(transform.position.x, (float) Screen.height - transform.position.y, vector2.x, vector2.y);
      screenSpace.x -= transform.pivot.x * vector2.x;
      screenSpace.y -= (1f - transform.pivot.y) * vector2.y;
      return screenSpace;
    }
  }
}
