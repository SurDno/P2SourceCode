using System;
using InputServices;
using SRDebugger.Internal;
using SRDebugger.UI;
using SRF;
using SRF.Service;

namespace SRDebugger.Services.Implementation
{
  [Service(typeof (IDebugPanelService))]
  public class DebugPanelServiceImpl : ScriptableObject, IDebugPanelService
  {
    private bool _cursorWasVisible;
    private DebugPanelRoot _debugPanelRootObject;
    private bool _isVisible;

    public event Action<IDebugPanelService, bool> VisibilityChanged;

    public bool IsLoaded => (UnityEngine.Object) _debugPanelRootObject != (UnityEngine.Object) null;

    public bool IsVisible
    {
      get => IsLoaded && _isVisible;
      set
      {
        if (_isVisible == value)
          return;
        if (value)
        {
          if (!IsLoaded)
            Load();
          SRDebuggerUtil.EnsureEventSystemExists();
          _debugPanelRootObject.CanvasGroup.alpha = 1f;
          _debugPanelRootObject.CanvasGroup.interactable = true;
          _debugPanelRootObject.CanvasGroup.blocksRaycasts = true;
          _cursorWasVisible = CursorService.Instance.Visible;
          CursorService.Instance.Free = CursorService.Instance.Visible = true;
        }
        else
        {
          if (IsLoaded)
          {
            _debugPanelRootObject.CanvasGroup.alpha = 0.0f;
            _debugPanelRootObject.CanvasGroup.interactable = false;
            _debugPanelRootObject.CanvasGroup.blocksRaycasts = false;
          }
          CursorService.Instance.Free = CursorService.Instance.Visible = _cursorWasVisible;
        }
        _isVisible = value;
        Action<IDebugPanelService, bool> visibilityChanged = VisibilityChanged;
        if (visibilityChanged == null)
          return;
        visibilityChanged(this, _isVisible);
      }
    }

    public DefaultTabs? ActiveTab
    {
      get
      {
        return (UnityEngine.Object) _debugPanelRootObject == (UnityEngine.Object) null ? new DefaultTabs?() : _debugPanelRootObject.TabController.ActiveTab;
      }
    }

    public void OpenTab(DefaultTabs tab)
    {
      if (!IsVisible)
        IsVisible = true;
      _debugPanelRootObject.TabController.OpenTab(tab);
    }

    public void Unload()
    {
      if ((UnityEngine.Object) _debugPanelRootObject == (UnityEngine.Object) null)
        return;
      IsVisible = false;
      _debugPanelRootObject.CachedGameObject.SetActive(false);
      UnityEngine.Object.Destroy((UnityEngine.Object) _debugPanelRootObject.CachedGameObject);
      _debugPanelRootObject = null;
    }

    private void Load()
    {
      DebugPanelRoot prefab = Resources.Load<DebugPanelRoot>("SRDebugger/UI/Prefabs/DebugPanel");
      if ((UnityEngine.Object) prefab == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "[SRDebugger] Error loading debug panel prefab");
      }
      else
      {
        _debugPanelRootObject = SRInstantiate.Instantiate<DebugPanelRoot>(prefab);
        _debugPanelRootObject.name = "Panel";
        UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) _debugPanelRootObject);
        _debugPanelRootObject.CachedTransform.SetParent(Hierarchy.Get("SRDebugger/UI"), true);
        SRDebuggerUtil.EnsureEventSystemExists();
      }
    }
  }
}
