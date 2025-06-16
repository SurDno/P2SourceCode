using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      Clouds_Generated cloudsGenerated = (Clouds_Generated) target2;
      cloudsGenerated.attenuation = attenuation;
      cloudsGenerated.brightness = brightness;
      cloudsGenerated.coverage = coverage;
      cloudsGenerated.opacity = opacity;
      cloudsGenerated.saturation = saturation;
      cloudsGenerated.scattering = scattering;
      cloudsGenerated.sharpness = sharpness;
      cloudsGenerated.size = size;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Attenuation", attenuation);
      DefaultDataWriteUtility.Write(writer, "Brightness", brightness);
      DefaultDataWriteUtility.Write(writer, "Coverage", coverage);
      DefaultDataWriteUtility.Write(writer, "Opacity", opacity);
      DefaultDataWriteUtility.Write(writer, "Saturation", saturation);
      DefaultDataWriteUtility.Write(writer, "Scattering", scattering);
      DefaultDataWriteUtility.Write(writer, "Sharpness", sharpness);
      DefaultDataWriteUtility.Write(writer, "Size", size);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      attenuation = DefaultDataReadUtility.Read(reader, "Attenuation", attenuation);
      brightness = DefaultDataReadUtility.Read(reader, "Brightness", brightness);
      coverage = DefaultDataReadUtility.Read(reader, "Coverage", coverage);
      opacity = DefaultDataReadUtility.Read(reader, "Opacity", opacity);
      saturation = DefaultDataReadUtility.Read(reader, "Saturation", saturation);
      scattering = DefaultDataReadUtility.Read(reader, "Scattering", scattering);
      sharpness = DefaultDataReadUtility.Read(reader, "Sharpness", sharpness);
      size = DefaultDataReadUtility.Read(reader, "Size", size);
    }
  }
}
