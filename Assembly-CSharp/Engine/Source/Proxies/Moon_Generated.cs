using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Moon))]
  public class Moon_Generated : Moon, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead
  {
    public object Clone()
    {
      Moon_Generated instance = Activator.CreateInstance<Moon_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Moon_Generated moonGenerated = (Moon_Generated) target2;
      moonGenerated.brightness = this.brightness;
      moonGenerated.contrast = this.contrast;
      moonGenerated.size = this.size;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Brightness", this.brightness);
      DefaultDataWriteUtility.Write(writer, "Contrast", this.contrast);
      DefaultDataWriteUtility.Write(writer, "Size", this.size);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.brightness = DefaultDataReadUtility.Read(reader, "Brightness", this.brightness);
      this.contrast = DefaultDataReadUtility.Read(reader, "Contrast", this.contrast);
      this.size = DefaultDataReadUtility.Read(reader, "Size", this.size);
    }
  }
}
