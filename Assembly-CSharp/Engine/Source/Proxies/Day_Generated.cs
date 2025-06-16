using System;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Drawing.Gradient;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Day))]
  public class Day_Generated : Day, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead
  {
    public object Clone()
    {
      Day_Generated instance = Activator.CreateInstance<Day_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      Day_Generated dayGenerated = (Day_Generated) target2;
      ((ICopyable) ambientColor).CopyTo(dayGenerated.ambientColor);
      dayGenerated.ambientMultiplier = ambientMultiplier;
      dayGenerated.brightness = brightness;
      ((ICopyable) cloudColor).CopyTo(dayGenerated.cloudColor);
      dayGenerated.contrast = contrast;
      dayGenerated.directionality = directionality;
      ((ICopyable) fogColor).CopyTo(dayGenerated.fogColor);
      dayGenerated.fogginess = fogginess;
      ((ICopyable) lightColor).CopyTo(dayGenerated.lightColor);
      dayGenerated.lightIntensity = lightIntensity;
      dayGenerated.mieMultiplier = mieMultiplier;
      ((ICopyable) rayColor).CopyTo(dayGenerated.rayColor);
      dayGenerated.rayleighMultiplier = rayleighMultiplier;
      dayGenerated.reflectionMultiplier = reflectionMultiplier;
      dayGenerated.shadowStrength = shadowStrength;
      ((ICopyable) skyColor).CopyTo(dayGenerated.skyColor);
      ((ICopyable) sunColor).CopyTo(dayGenerated.sunColor);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "AmbientColor", ambientColor);
      DefaultDataWriteUtility.Write(writer, "AmbientMultiplier", ambientMultiplier);
      DefaultDataWriteUtility.Write(writer, "Brightness", brightness);
      DefaultDataWriteUtility.WriteSerialize(writer, "CloudColor", cloudColor);
      DefaultDataWriteUtility.Write(writer, "Contrast", contrast);
      DefaultDataWriteUtility.Write(writer, "Directionality", directionality);
      DefaultDataWriteUtility.WriteSerialize(writer, "FogColor", fogColor);
      DefaultDataWriteUtility.Write(writer, "Fogginess", fogginess);
      DefaultDataWriteUtility.WriteSerialize(writer, "LightColor", lightColor);
      DefaultDataWriteUtility.Write(writer, "LightIntensity", lightIntensity);
      DefaultDataWriteUtility.Write(writer, "MieMultiplier", mieMultiplier);
      DefaultDataWriteUtility.WriteSerialize(writer, "RayColor", rayColor);
      DefaultDataWriteUtility.Write(writer, "RayleighMultiplier", rayleighMultiplier);
      DefaultDataWriteUtility.Write(writer, "ReflectionMultiplier", reflectionMultiplier);
      DefaultDataWriteUtility.Write(writer, "ShadowStrength", shadowStrength);
      DefaultDataWriteUtility.WriteSerialize(writer, "SkyColor", skyColor);
      DefaultDataWriteUtility.WriteSerialize(writer, "SunColor", sunColor);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      IDataReader child1 = reader.GetChild("AmbientColor");
      ColorGradient ambientColor = this.ambientColor;
      if (ambientColor is ISerializeDataRead serializeDataRead1)
        serializeDataRead1.DataRead(child1, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(ambientColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      ambientMultiplier = DefaultDataReadUtility.Read(reader, "AmbientMultiplier", ambientMultiplier);
      brightness = DefaultDataReadUtility.Read(reader, "Brightness", brightness);
      IDataReader child2 = reader.GetChild("CloudColor");
      ColorGradient cloudColor = this.cloudColor;
      if (cloudColor is ISerializeDataRead serializeDataRead2)
        serializeDataRead2.DataRead(child2, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(cloudColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      contrast = DefaultDataReadUtility.Read(reader, "Contrast", contrast);
      directionality = DefaultDataReadUtility.Read(reader, "Directionality", directionality);
      IDataReader child3 = reader.GetChild("FogColor");
      ColorGradient fogColor = this.fogColor;
      if (fogColor is ISerializeDataRead serializeDataRead3)
        serializeDataRead3.DataRead(child3, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(fogColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      fogginess = DefaultDataReadUtility.Read(reader, "Fogginess", fogginess);
      IDataReader child4 = reader.GetChild("LightColor");
      ColorGradient lightColor = this.lightColor;
      if (lightColor is ISerializeDataRead serializeDataRead4)
        serializeDataRead4.DataRead(child4, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(lightColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      lightIntensity = DefaultDataReadUtility.Read(reader, "LightIntensity", lightIntensity);
      mieMultiplier = DefaultDataReadUtility.Read(reader, "MieMultiplier", mieMultiplier);
      IDataReader child5 = reader.GetChild("RayColor");
      ColorGradient rayColor = this.rayColor;
      if (rayColor is ISerializeDataRead serializeDataRead5)
        serializeDataRead5.DataRead(child5, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(rayColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      rayleighMultiplier = DefaultDataReadUtility.Read(reader, "RayleighMultiplier", rayleighMultiplier);
      reflectionMultiplier = DefaultDataReadUtility.Read(reader, "ReflectionMultiplier", reflectionMultiplier);
      shadowStrength = DefaultDataReadUtility.Read(reader, "ShadowStrength", shadowStrength);
      IDataReader child6 = reader.GetChild("SkyColor");
      ColorGradient skyColor = this.skyColor;
      if (skyColor is ISerializeDataRead serializeDataRead6)
        serializeDataRead6.DataRead(child6, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(skyColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child7 = reader.GetChild("SunColor");
      ColorGradient sunColor = this.sunColor;
      if (sunColor is ISerializeDataRead serializeDataRead7)
        serializeDataRead7.DataRead(child7, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(sunColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
    }
  }
}
