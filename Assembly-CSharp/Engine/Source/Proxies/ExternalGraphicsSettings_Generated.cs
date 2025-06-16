using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ExternalGraphicsSettings))]
  public class ExternalGraphicsSettings_Generated : 
    ExternalGraphicsSettings,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ExternalGraphicsSettings_Generated instance = Activator.CreateInstance<ExternalGraphicsSettings_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ExternalGraphicsSettings_Generated settingsGenerated = (ExternalGraphicsSettings_Generated) target2;
      settingsGenerated.Version = Version;
      settingsGenerated.MindMapIconSize = MindMapIconSize;
      settingsGenerated.Fov = Fov;
      settingsGenerated.MinFov = MinFov;
      settingsGenerated.MaxFov = MaxFov;
      settingsGenerated.MinLevelOfDetails = MinLevelOfDetails;
      settingsGenerated.MaxLevelOfDetails = MaxLevelOfDetails;
      settingsGenerated.MinShadowDistance = MinShadowDistance;
      settingsGenerated.MaxShadowDistance = MaxShadowDistance;
      settingsGenerated.StreamingMipmapsActive = StreamingMipmapsActive;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", Version);
      DefaultDataWriteUtility.Write(writer, "MindMapIconSize", MindMapIconSize);
      DefaultDataWriteUtility.Write(writer, "Fov", Fov);
      DefaultDataWriteUtility.Write(writer, "MinFov", MinFov);
      DefaultDataWriteUtility.Write(writer, "MaxFov", MaxFov);
      DefaultDataWriteUtility.Write(writer, "MinLevelOfDetails", MinLevelOfDetails);
      DefaultDataWriteUtility.Write(writer, "MaxLevelOfDetails", MaxLevelOfDetails);
      DefaultDataWriteUtility.Write(writer, "MinShadowDistance", MinShadowDistance);
      DefaultDataWriteUtility.Write(writer, "MaxShadowDistance", MaxShadowDistance);
      DefaultDataWriteUtility.Write(writer, "StreamingMipmapsActive", StreamingMipmapsActive);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Version = DefaultDataReadUtility.Read(reader, "Version", Version);
      MindMapIconSize = DefaultDataReadUtility.Read(reader, "MindMapIconSize", MindMapIconSize);
      Fov = DefaultDataReadUtility.Read(reader, "Fov", Fov);
      MinFov = DefaultDataReadUtility.Read(reader, "MinFov", MinFov);
      MaxFov = DefaultDataReadUtility.Read(reader, "MaxFov", MaxFov);
      MinLevelOfDetails = DefaultDataReadUtility.Read(reader, "MinLevelOfDetails", MinLevelOfDetails);
      MaxLevelOfDetails = DefaultDataReadUtility.Read(reader, "MaxLevelOfDetails", MaxLevelOfDetails);
      MinShadowDistance = DefaultDataReadUtility.Read(reader, "MinShadowDistance", MinShadowDistance);
      MaxShadowDistance = DefaultDataReadUtility.Read(reader, "MaxShadowDistance", MaxShadowDistance);
      StreamingMipmapsActive = DefaultDataReadUtility.Read(reader, "StreamingMipmapsActive", StreamingMipmapsActive);
    }
  }
}
