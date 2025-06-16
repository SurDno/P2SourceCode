// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.ContainerOpenStateEnumParameter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class ContainerOpenStateEnumParameter : Parameter<ContainerOpenStateEnum>
  {
    protected override bool Compare(ContainerOpenStateEnum a, ContainerOpenStateEnum b) => a == b;
  }
}
