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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      Night_Generated nightGenerated = (Night_Generated) target2;
      ((ICopyable) ambientColor).CopyTo(nightGenerated.ambientColor);
      nightGenerated.ambientMultiplier = ambientMultiplier;
      nightGenerated.brightness = brightness;
      ((ICopyable) cloudColor).CopyTo(nightGenerated.cloudColor);
      nightGenerated.contrast = contrast;
      nightGenerated.directionality = directionality;
      ((ICopyable) fogColor).CopyTo(nightGenerated.fogColor);
      nightGenerated.fogginess = fogginess;
      ((ICopyable) lightColor).CopyTo(nightGenerated.lightColor);
      nightGenerated.lightIntensity = lightIntensity;
      nightGenerated.mieMultiplier = mieMultiplier;
      ((ICopyable) moonColor).CopyTo(nightGenerated.moonColor);
      ((ICopyable) rayColor).CopyTo(nightGenerated.rayColor);
      nightGenerated.rayleighMultiplier = rayleighMultiplier;
      nightGenerated.reflectionMultiplier = reflectionMultiplier;
      nightGenerated.shadowStrength = shadowStrength;
      ((ICopyable) skyColor).CopyTo(nightGenerated.skyColor);
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
      DefaultDataWriteUtility.WriteSerialize(writer, "MoonColor", moonColor);
      DefaultDataWriteUtility.WriteSerialize(writer, "RayColor", rayColor);
      DefaultDataWriteUtility.Write(writer, "RayleighMultiplier", rayleighMultiplier);
      DefaultDataWriteUtility.Write(writer, "ReflectionMultiplier", reflectionMultiplier);
      DefaultDataWriteUtility.Write(writer, "ShadowStrength", shadowStrength);
      DefaultDataWriteUtility.WriteSerialize(writer, "SkyColor", skyColor);
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
      rayleighMultiplier = DefaultDataReadUtility.Read(reader, "RayleighMultiplier", rayleighMultiplier);
      reflectionMultiplier = DefaultDataReadUtility.Read(reader, "ReflectionMultiplier", reflectionMultiplier);
      shadowStrength = DefaultDataReadUtility.Read(reader, "ShadowStrength", shadowStrength);
      IDataReader child7 = reader.GetChild("SkyColor");
      ColorGradient skyColor = this.skyColor;
      if (skyColor is ISerializeDataRead serializeDataRead7)
        serializeDataRead7.DataRead(child7, typeof (ColorGradient));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(skyColor.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
    }
  }
}
