using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Stars))]
  public class Stars_Generated : 
    Stars,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      Stars_Generated instance = Activator.CreateInstance<Stars_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Stars_Generated starsGenerated = (Stars_Generated) target2;
      starsGenerated.brightness = this.brightness;
      starsGenerated.size = this.size;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Brightness", this.brightness);
      DefaultDataWriteUtility.Write(writer, "Size", this.size);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.brightness = DefaultDataReadUtility.Read(reader, "Brightness", this.brightness);
      this.size = DefaultDataReadUtility.Read(reader, "Size", this.size);
    }
  }
}
