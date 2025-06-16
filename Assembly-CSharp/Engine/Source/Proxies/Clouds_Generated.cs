using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Clouds))]
  public class Clouds_Generated : 
    Clouds,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      Clouds_Generated instance = Activator.CreateInstance<Clouds_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Clouds_Generated cloudsGenerated = (Clouds_Generated) target2;
      cloudsGenerated.attenuation = this.attenuation;
      cloudsGenerated.brightness = this.brightness;
      cloudsGenerated.coverage = this.coverage;
      cloudsGenerated.opacity = this.opacity;
      cloudsGenerated.saturation = this.saturation;
      cloudsGenerated.scattering = this.scattering;
      cloudsGenerated.sharpness = this.sharpness;
      cloudsGenerated.size = this.size;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Attenuation", this.attenuation);
      DefaultDataWriteUtility.Write(writer, "Brightness", this.brightness);
      DefaultDataWriteUtility.Write(writer, "Coverage", this.coverage);
      DefaultDataWriteUtility.Write(writer, "Opacity", this.opacity);
      DefaultDataWriteUtility.Write(writer, "Saturation", this.saturation);
      DefaultDataWriteUtility.Write(writer, "Scattering", this.scattering);
      DefaultDataWriteUtility.Write(writer, "Sharpness", this.sharpness);
      DefaultDataWriteUtility.Write(writer, "Size", this.size);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.attenuation = DefaultDataReadUtility.Read(reader, "Attenuation", this.attenuation);
      this.brightness = DefaultDataReadUtility.Read(reader, "Brightness", this.brightness);
      this.coverage = DefaultDataReadUtility.Read(reader, "Coverage", this.coverage);
      this.opacity = DefaultDataReadUtility.Read(reader, "Opacity", this.opacity);
      this.saturation = DefaultDataReadUtility.Read(reader, "Saturation", this.saturation);
      this.scattering = DefaultDataReadUtility.Read(reader, "Scattering", this.scattering);
      this.sharpness = DefaultDataReadUtility.Read(reader, "Sharpness", this.sharpness);
      this.size = DefaultDataReadUtility.Read(reader, "Size", this.size);
    }
  }
}
