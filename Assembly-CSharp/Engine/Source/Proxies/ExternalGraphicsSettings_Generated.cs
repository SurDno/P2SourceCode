using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ExternalGraphicsSettings_Generated settingsGenerated = (ExternalGraphicsSettings_Generated) target2;
      settingsGenerated.Version = this.Version;
      settingsGenerated.MindMapIconSize = this.MindMapIconSize;
      settingsGenerated.Fov = this.Fov;
      settingsGenerated.MinFov = this.MinFov;
      settingsGenerated.MaxFov = this.MaxFov;
      settingsGenerated.MinLevelOfDetails = this.MinLevelOfDetails;
      settingsGenerated.MaxLevelOfDetails = this.MaxLevelOfDetails;
      settingsGenerated.MinShadowDistance = this.MinShadowDistance;
      settingsGenerated.MaxShadowDistance = this.MaxShadowDistance;
      settingsGenerated.StreamingMipmapsActive = this.StreamingMipmapsActive;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", this.Version);
      DefaultDataWriteUtility.Write(writer, "MindMapIconSize", this.MindMapIconSize);
      DefaultDataWriteUtility.Write(writer, "Fov", this.Fov);
      DefaultDataWriteUtility.Write(writer, "MinFov", this.MinFov);
      DefaultDataWriteUtility.Write(writer, "MaxFov", this.MaxFov);
      DefaultDataWriteUtility.Write(writer, "MinLevelOfDetails", this.MinLevelOfDetails);
      DefaultDataWriteUtility.Write(writer, "MaxLevelOfDetails", this.MaxLevelOfDetails);
      DefaultDataWriteUtility.Write(writer, "MinShadowDistance", this.MinShadowDistance);
      DefaultDataWriteUtility.Write(writer, "MaxShadowDistance", this.MaxShadowDistance);
      DefaultDataWriteUtility.Write(writer, "StreamingMipmapsActive", this.StreamingMipmapsActive);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Version = DefaultDataReadUtility.Read(reader, "Version", this.Version);
      this.MindMapIconSize = DefaultDataReadUtility.Read(reader, "MindMapIconSize", this.MindMapIconSize);
      this.Fov = DefaultDataReadUtility.Read(reader, "Fov", this.Fov);
      this.MinFov = DefaultDataReadUtility.Read(reader, "MinFov", this.MinFov);
      this.MaxFov = DefaultDataReadUtility.Read(reader, "MaxFov", this.MaxFov);
      this.MinLevelOfDetails = DefaultDataReadUtility.Read(reader, "MinLevelOfDetails", this.MinLevelOfDetails);
      this.MaxLevelOfDetails = DefaultDataReadUtility.Read(reader, "MaxLevelOfDetails", this.MaxLevelOfDetails);
      this.MinShadowDistance = DefaultDataReadUtility.Read(reader, "MinShadowDistance", this.MinShadowDistance);
      this.MaxShadowDistance = DefaultDataReadUtility.Read(reader, "MaxShadowDistance", this.MaxShadowDistance);
      this.StreamingMipmapsActive = DefaultDataReadUtility.Read(reader, "StreamingMipmapsActive", this.StreamingMipmapsActive);
    }
  }
}
