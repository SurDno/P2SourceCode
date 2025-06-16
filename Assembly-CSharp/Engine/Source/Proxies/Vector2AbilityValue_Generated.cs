// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.Vector2AbilityValue_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Effects.Values;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Vector2AbilityValue))]
  public class Vector2AbilityValue_Generated : 
    Vector2AbilityValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      Vector2AbilityValue_Generated instance = Activator.CreateInstance<Vector2AbilityValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((AbilityValue<Vector2>) target2).value = this.value;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.value = UnityDataReadUtility.Read(reader, "Value", this.value);
    }
  }
}
