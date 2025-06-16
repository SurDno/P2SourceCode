// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.EntityViewBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Inspectors;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class EntityViewBase : EntityView
  {
    [Inspected]
    private IEntity value;

    public override IEntity Value
    {
      get => this.value;
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        this.ApplyValue();
      }
    }

    protected abstract void ApplyValue();
  }
}
