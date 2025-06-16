// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.CombatServiceCombatInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services
{
  public class CombatServiceCombatInfo
  {
    private static int lastId;
    public List<CombatServiceCharacterInfo> Characters;
    public List<CombatServiceCombatFractionInfo> Fractions;
    public CombatServiceCharacterStateEnum State;
    public List<CombatServiceEscapedCharacterInfo> EscapedCharacters;
    public int Id;

    public CombatServiceCombatInfo() => this.Id = CombatServiceCombatInfo.lastId++;
  }
}
