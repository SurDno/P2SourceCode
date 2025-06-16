// Decompiled with JetBrains decompiler
// Type: EnableOnEngineInitialized
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;

#nullable disable
public class EnableOnEngineInitialized : EngineDependent
{
  [SerializeField]
  private HideableView view;

  protected override void OnConnectToEngine()
  {
    if (!((Object) this.view != (Object) null))
      return;
    this.view.Visible = true;
  }

  protected override void OnDisconnectFromEngine()
  {
  }
}
