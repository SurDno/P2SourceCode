// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.Implementation.DebugPanelServiceImpl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using InputServices;
using SRDebugger.Internal;
using SRDebugger.UI;
using SRF;
using System;
using UnityEngine;

#nullable disable
namespace SRDebugger.Services.Implementation
{
  [SRF.Service.Service(typeof (IDebugPanelService))]
  public class DebugPanelServiceImpl : ScriptableObject, IDebugPanelService
  {
    private bool _cursorWasVisible;
    private DebugPanelRoot _debugPanelRootObject;
    private bool _isVisible;

    public event Action<IDebugPanelService, bool> VisibilityChanged;

    public bool IsLoaded => (UnityEngine.Object) this._debugPanelRootObject != (UnityEngine.Object) null;

    public bool IsVisible
    {
      get => this.IsLoaded && this._isVisible;
      set
      {
        if (this._isVisible == value)
          return;
        if (value)
        {
          if (!this.IsLoaded)
            this.Load();
          SRDebuggerUtil.EnsureEventSystemExists();
          this._debugPanelRootObject.CanvasGroup.alpha = 1f;
          this._debugPanelRootObject.CanvasGroup.interactable = true;
          this._debugPanelRootObject.CanvasGroup.blocksRaycasts = true;
          this._cursorWasVisible = CursorService.Instance.Visible;
          CursorService.Instance.Free = CursorService.Instance.Visible = true;
        }
        else
        {
          if (this.IsLoaded)
          {
            this._debugPanelRootObject.CanvasGroup.alpha = 0.0f;
            this._debugPanelRootObject.CanvasGroup.interactable = false;
            this._debugPanelRootObject.CanvasGroup.blocksRaycasts = false;
          }
          CursorService.Instance.Free = CursorService.Instance.Visible = this._cursorWasVisible;
        }
        this._isVisible = value;
        Action<IDebugPanelService, bool> visibilityChanged = this.VisibilityChanged;
        if (visibilityChanged == null)
          return;
        visibilityChanged((IDebugPanelService) this, this._isVisible);
      }
    }

    public DefaultTabs? ActiveTab
    {
      get
      {
        return (UnityEngine.Object) this._debugPanelRootObject == (UnityEngine.Object) null ? new DefaultTabs?() : this._debugPanelRootObject.TabController.ActiveTab;
      }
    }

    public void OpenTab(DefaultTabs tab)
    {
      if (!this.IsVisible)
        this.IsVisible = true;
      this._debugPanelRootObject.TabController.OpenTab(tab);
    }

    public void Unload()
    {
      if ((UnityEngine.Object) this._debugPanelRootObject == (UnityEngine.Object) null)
        return;
      this.IsVisible = false;
      this._debugPanelRootObject.CachedGameObject.SetActive(false);
      UnityEngine.Object.Destroy((UnityEngine.Object) this._debugPanelRootObject.CachedGameObject);
      this._debugPanelRootObject = (DebugPanelRoot) null;
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
        this._debugPanelRootObject = SRInstantiate.Instantiate<DebugPanelRoot>(prefab);
        this._debugPanelRootObject.name = "Panel";
        UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this._debugPanelRootObject);
        this._debugPanelRootObject.CachedTransform.SetParent(Hierarchy.Get("SRDebugger/UI"), true);
        SRDebuggerUtil.EnsureEventSystemExists();
      }
    }
  }
}
