// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.CrowdAreaInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Inspectors;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CrowdAreaInfo
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public AreaEnum Area;
    [DataReadProxy(MemberEnum.None, Name = "Templates")]
    [DataWriteProxy(MemberEnum.None, Name = "Templates")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<CrowdTemplateInfo> TemplateInfos = new List<CrowdTemplateInfo>();
  }
}
