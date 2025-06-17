using System;
using InputServices;
using SRDebugger.Internal;
using SRDebugger.UI;
using SRF;
using SRF.Service;
using UnityEngine;

namespace SRDebugger.Services.Implementation
{
  [Service(typeof (IDebugPanelService))]
  public class DebugPanelServiceImpl : ScriptableObject, IDebugPanelService
  {
    private bool _cursorWasVisible;
    private DebugPanelRoot _debugPanelRootObject;
    private bool _isVisible;

    public event Action<IDebugPanelService, bool> VisibilityChanged;

    public bool IsLoaded => _debugPanelRootObject != null;

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

    public DefaultTabs? ActiveTab => _debugPanelRootObject == null ? new DefaultTabs?() : _debugPanelRootObject.TabController.ActiveTab;

    public void OpenTab(DefaultTabs tab)
    {
      if (!IsVisible)
        IsVisible = true;
      _debugPanelRootObject.TabController.OpenTab(tab);
    }

    public void Unload()
    {
      if (_debugPanelRootObject == null)
        return;
      IsVisible = false;
      _debugPanelRootObject.CachedGameObject.SetActive(false);
      Destroy(_debugPanelRootObject.CachedGameObject);
      _debugPanelRootObject = null;
    }

    private void Load()
    {
      DebugPanelRoot prefab = Resources.Load<DebugPanelRoot>("SRDebugger/UI/Prefabs/DebugPanel");
      if (prefab == null)
      {
        Debug.LogError("[SRDebugger] Error loading debug panel prefab");
      }
      else
      {
        _debugPanelRootObject = SRInstantiate.Instantiate(prefab);
        _debugPanelRootObject.name = "Panel";
        DontDestroyOnLoad(_debugPanelRootObject);
        _debugPanelRootObject.CachedTransform.SetParent(Hierarchy.Get("SRDebugger/UI"), true);
        SRDebuggerUtil.EnsureEventSystemExists();
      }
    }
  }
}
