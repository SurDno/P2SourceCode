// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.SpreadingComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

#nullable disable
namespace Engine.Source.Components
{
  [Required(typeof (LocationItemComponent))]
  [Factory(typeof (SpreadingComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SpreadingComponent : EngineComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected DiseasedStateEnum diseasedState;
    [FromLocator]
    private SpreadingService spreadingService;

    public DiseasedStateEnum DiseasedState => this.diseasedState;

    public override void OnAdded()
    {
      base.OnAdded();
      this.spreadingService.AddSpreading(this);
    }

    public override void OnRemoved()
    {
      this.spreadingService.RemoveSpreading(this);
      base.OnRemoved();
    }
  }
}
