// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.ForcedDialogCharacterInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Inspectors;

#nullable disable
namespace Engine.Source.Services
{
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class ForcedDialogCharacterInfo
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public float Distance;
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    [Inspected]
    public IEntity Character;
  }
}
