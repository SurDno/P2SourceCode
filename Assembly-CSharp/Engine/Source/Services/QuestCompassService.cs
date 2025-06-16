// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.QuestCompassService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (QuestCompassService)})]
  public class QuestCompassService
  {
    private bool enabled;
    public Action<bool> OnEnableChanged;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.enabled;
      set
      {
        if (this.enabled == value)
          return;
        this.enabled = value;
        Action<bool> onEnableChanged = this.OnEnableChanged;
        if (onEnableChanged == null)
          return;
        onEnableChanged(value);
      }
    }
  }
}
