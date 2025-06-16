using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Fog))]
  public class Fog_Generated : Fog, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead
  {
    public object Clone()
    {
      Fog_Generated instance = Activator.CreateInstance<Fog_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Fog_Generated fogGenerated = (Fog_Generated) target2;
      fogGenerated.density = this.density;
      fogGenerated.height = this.height;
      fogGenerated.startDistance = this.startDistance;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Density", this.density);
      DefaultDataWriteUtility.Write(writer, "Height", this.height);
      DefaultDataWriteUtility.Write(writer, "StartDistance", this.startDistance);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.density = DefaultDataReadUtility.Read(reader, "Density", this.density);
      this.height = DefaultDataReadUtility.Read(reader, "Height", this.height);
      this.startDistance = DefaultDataReadUtility.Read(reader, "StartDistance", this.startDistance);
    }
  }
}
