// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.Wind_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Wind))]
  public class Wind_Generated : Wind, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead
  {
    public object Clone()
    {
      Wind_Generated instance = Activator.CreateInstance<Wind_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Wind_Generated windGenerated = (Wind_Generated) target2;
      windGenerated.degrees = this.degrees;
      windGenerated.speed = this.speed;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Degrees", this.degrees);
      DefaultDataWriteUtility.Write(writer, "Speed", this.speed);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.degrees = DefaultDataReadUtility.Read(reader, "Degrees", this.degrees);
      this.speed = DefaultDataReadUtility.Read(reader, "Speed", this.speed);
    }
  }
}
