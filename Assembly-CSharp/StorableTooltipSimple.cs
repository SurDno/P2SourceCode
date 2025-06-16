// Decompiled with JetBrains decompiler
// Type: StorableTooltipSimple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

#nullable disable
[Factory(typeof (IStorableTooltipComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StorableTooltipSimple : IStorableTooltipComponent
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected bool isEnabled = true;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected StorableTooltipInfo info = new StorableTooltipInfo();

  [Inspected]
  public bool IsEnabled => this.isEnabled;

  public StorableTooltipInfo GetInfo(IEntity owner) => this.info;
}
