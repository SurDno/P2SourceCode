// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.LockStatePriorityContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Gate;
using Engine.Common.Generator;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad, Type = typeof (PriorityContainer<LockState>))]
  public class LockStatePriorityContainer : PriorityContainer<LockState>
  {
    protected override bool IsDefault(LockState value) => value == LockState.Unlocked;
  }
}
