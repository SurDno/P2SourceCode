// Decompiled with JetBrains decompiler
// Type: ActiveWindowCheck`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using UnityEngine;

#nullable disable
public abstract class ActiveWindowCheck<T> : MonoBehaviour
{
  [SerializeField]
  private HideableView view;

  private void Start()
  {
    if (!((Object) this.view != (Object) null))
      return;
    this.view.Visible = ServiceLocator.GetService<UIService>().Active is T;
  }
}
