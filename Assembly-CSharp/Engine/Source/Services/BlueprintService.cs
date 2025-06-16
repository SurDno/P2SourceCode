// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.BlueprintService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (IBlueprintService)})]
  public class BlueprintService : IBlueprintService
  {
    public void Start(IBlueprintObject bp, IEntity owner, Action complete)
    {
      BlueprintServiceUtility.Start(bp, owner, complete, (string) null);
    }
  }
}
