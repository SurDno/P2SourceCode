// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.PlayerJumpAbilityController_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerJumpAbilityController))]
  public class PlayerJumpAbilityController_Generated : 
    PlayerJumpAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerJumpAbilityController_Generated instance = Activator.CreateInstance<PlayerJumpAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((PlayerJumpAbilityController) target2).jump = this.jump;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "jump", this.jump);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.jump = DefaultDataReadUtility.Read(reader, "jump", this.jump);
    }
  }
}
