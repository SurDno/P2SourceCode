// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.Implementation.PinnedUIServiceImpl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRDebugger.UI.Other;
using SRF;
using SRF.Service;
using UnityEngine;

#nullable disable
namespace SRDebugger.Services.Implementation
{
  [SRF.Service.Service(typeof (IPinnedUIService))]
  public class PinnedUIServiceImpl : SRServiceBase<IPinnedUIService>, IPinnedUIService
  {
    private PinnedUIRoot _uiRoot;

    public bool IsProfilerPinned
    {
      get => !((Object) this._uiRoot == (Object) null) && this._uiRoot.Profiler.activeSelf;
      set
      {
        if ((Object) this._uiRoot == (Object) null)
          this.Load();
        this._uiRoot.Profiler.SetActive(value);
      }
    }

    protected override void Awake()
    {
      base.Awake();
      this.CachedTransform.SetParent(Hierarchy.Get("SRDebugger/UI"));
    }

    private void Load()
    {
      PinnedUIRoot prefab = Resources.Load<PinnedUIRoot>("SRDebugger/UI/Prefabs/PinnedUI");
      if ((Object) prefab == (Object) null)
      {
        Debug.LogError((object) "[SRDebugger.PinnedUI] Error loading ui prefab");
      }
      else
      {
        PinnedUIRoot pinnedUiRoot = SRInstantiate.Instantiate<PinnedUIRoot>(prefab);
        pinnedUiRoot.CachedTransform.SetParent(this.CachedTransform, false);
        this._uiRoot = pinnedUiRoot;
        this.UpdateAnchors();
      }
    }

    private void UpdateAnchors()
    {
      switch (Settings.Instance.ProfilerAlignment)
      {
        case PinAlignment.TopLeft:
        case PinAlignment.BottomLeft:
        case PinAlignment.CenterLeft:
          this._uiRoot.Profiler.transform.SetSiblingIndex(0);
          break;
        case PinAlignment.TopRight:
        case PinAlignment.BottomRight:
        case PinAlignment.CenterRight:
          this._uiRoot.Profiler.transform.SetSiblingIndex(1);
          break;
      }
      switch (Settings.Instance.ProfilerAlignment)
      {
        case PinAlignment.TopLeft:
        case PinAlignment.TopRight:
          this._uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
          break;
        case PinAlignment.BottomLeft:
        case PinAlignment.BottomRight:
          this._uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
          break;
        case PinAlignment.CenterLeft:
        case PinAlignment.CenterRight:
          this._uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
          break;
      }
      this._uiRoot.ProfilerHandleManager.SetAlignment(Settings.Instance.ProfilerAlignment);
    }
  }
}
