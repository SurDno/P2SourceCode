using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Drawing.Gradient;
using Engine.Impl.Weather.Element;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Night))]
  public class Night_Generated : 
    Night,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      Night_Generated instance = Activator.CreateInstance<Night_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Night_Generated nightGenerated = (Night_Generated) target2;
      ((ICopyable) this.ambientColor).CopyTo((object) nightGenerated.ambientColor);
      nightGenerated.ambientMultiplier = this.ambientMultiplier;
      nightGenerated.brightness = this.brightness;
      ((ICopyable) this.cloudColor).CopyTo((object) nightGenerated.cloudColor);
      nightGenerated.contrast = this.contrast;
      nightGenerated.directionality = this.directionality;
      ((ICopyable) this.fogColor).CopyTo((object) nightGenerated.fogColor);
      nightGenerated.fogginess = this.fogginess;
      ((ICopyable) this.lightColor).CopyTo((object) nightGenerated.lightColor);
      nightGenerated.lightIntensity = this.lightIntensity;
      nightGenerated.mieMultiplier = this.mieMultiplier;
      ((ICopyable) this.moonColor).CopyTo((object) nightGenerated.moonColor);
      ((ICopyable) this.rayColor).CopyTo((object) nightGenerated.rayColor);
      nightGenerated.rayleighMultiplier = this.rayleighMultiplier;
      nightGenerated.reflectionMultiplier = this.reflectionMultiplier;
      nightGenerated.shadowStrength = this.shadowStrength;
      ((ICopyable) this.skyColor).CopyTo((object) nightGenerated.skyColor);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<ColorGradient>(writer, "AmbientColor", this.ambientColor);
      DefaultDataWriteUtility.Write(writer, "AmbientMultiplier", this.ambientMultiplier);
      DefaultDataWriteUtility.Write(writer, "Brightness", this.brightness);
      DefaultDataWriteUtility.WriteSerialize<ColorGradient>(writer, "CloudColor", this.cloudColor);
      DefaultDataWriteUtility.Write(writer, "Contrast", this.contrast);
      DefaultDataWriteUtility.Write(writer, "Directionality", this.directionality);
      DefaultDataWriteUtility.WriteSerialize<ColorGradient>(writer, "FogColor", this.fogColor);
      DefaultDataWriteUtility.Write(writer, "Fogginess", this.fogginess);
      DefaultDataWriteUtility.WriteSerialize<ColorGradient>(writer, "LightColor", this.lightColor);
      DefaultDataWriteUtility.Write(writer, "LightIntensity", this.lightIntensity);
      DefaultDataWriteUtility.Write(writer, "MieMultiplier", this.mieMultiplier);
      DefaultDataWriteUtility.WriteSerialize<ColorGradient>(writer, "MoonColor", this.moonColor);
      DefaultDataWriteUtility.WriteSerialize<ColorGradient>(writer, "RayColor", this.rayColor);
      DefaultDataWriteUtility.Write(writer, "RayleighMultiplier", this.rayleighMultiplier);
      DefaultDataWriteUtility.Write(writer, "ReflectionMultiplier", this.reflectionMultiplier);
      DefaultDataWriteUtility.Write(writer, "ShadowStrength", this.shadowStrength);
      DefaultDataWriteUtility.WriteSerialize<ColorGradient>(writer, "SkyColor", this.skyColor);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      IDataReader child1 = reader.GetChild("AmbientColor");
      ColorGradient ambientColor = this.ambientColor;
      if (ambientColor is ISerializeDataRead serializeDataRead1)
        serializeDataRead1.DataRead(child1, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(ambientColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      this.ambientMultiplier = DefaultDataReadUtility.Read(reader, "AmbientMultiplier", this.ambientMultiplier);
      this.brightness = DefaultDataReadUtility.Read(reader, "Brightness", this.brightness);
      IDataReader child2 = reader.GetChild("CloudColor");
      ColorGradient cloudColor = this.cloudColor;
      if (cloudColor is ISerializeDataRead serializeDataRead2)
        serializeDataRead2.DataRead(child2, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(cloudColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      this.contrast = DefaultDataReadUtility.Read(reader, "Contrast", this.contrast);
      this.directionality = DefaultDataReadUtility.Read(reader, "Directionality", this.directionality);
      IDataReader child3 = reader.GetChild("FogColor");
      ColorGradient fogColor = this.fogColor;
      if (fogColor is ISerializeDataRead serializeDataRead3)
        serializeDataRead3.DataRead(child3, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(fogColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      this.fogginess = DefaultDataReadUtility.Read(reader, "Fogginess", this.fogginess);
      IDataReader child4 = reader.GetChild("LightColor");
      ColorGradient lightColor = this.lightColor;
      if (lightColor is ISerializeDataRead serializeDataRead4)
        serializeDataRead4.DataRead(child4, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(lightColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      this.lightIntensity = DefaultDataReadUtility.Read(reader, "LightIntensity", this.lightIntensity);
      this.mieMultiplier = DefaultDataReadUtility.Read(reader, "MieMultiplier", this.mieMultiplier);
      IDataReader child5 = reader.GetChild("MoonColor");
      ColorGradient moonColor = this.moonColor;
      if (moonColor is ISerializeDataRead serializeDataRead5)
        serializeDataRead5.DataRead(child5, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(moonColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child6 = reader.GetChild("RayColor");
      ColorGradient rayColor = this.rayColor;
      if (rayColor is ISerializeDataRead serializeDataRead6)
        serializeDataRead6.DataRead(child6, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(rayColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      this.rayleighMultiplier = DefaultDataReadUtility.Read(reader, "RayleighMultiplier", this.rayleighMultiplier);
      this.reflectionMultiplier = DefaultDataReadUtility.Read(reader, "ReflectionMultiplier", this.reflectionMultiplier);
      this.shadowStrength = DefaultDataReadUtility.Read(reader, "ShadowStrength", this.shadowStrength);
      IDataReader child7 = reader.GetChild("SkyColor");
      ColorGradient skyColor = this.skyColor;
      if (skyColor is ISerializeDataRead serializeDataRead7)
        serializeDataRead7.DataRead(child7, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(skyColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
    }
  }
}
