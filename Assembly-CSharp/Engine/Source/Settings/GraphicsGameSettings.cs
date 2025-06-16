// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.GraphicsGameSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Settings.External;
using Inspectors;

#nullable disable
namespace Engine.Source.Settings
{
  public class GraphicsGameSettings : SettingsInstanceByRequest<GraphicsGameSettings>
  {
    [Inspected]
    public IValue<bool> AdditionalReflections = (IValue<bool>) new BoolValue(nameof (AdditionalReflections), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AdditionalReflections);
    [Inspected]
    public IValue<float> AILodDistance = (IValue<float>) new FloatValue(nameof (AILodDistance), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AILodDistance, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.AiLodsMinDistance, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.AiLodsDistance);
    [Inspected]
    public IValue<bool> AnisotropicFiltering = (IValue<bool>) new BoolValue(nameof (AnisotropicFiltering), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AnisotropicFiltering);
    [Inspected]
    public IValue<bool> Antialiasing = (IValue<bool>) new BoolValue(nameof (Antialiasing), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.Antialiasing);
    [Inspected]
    public IValue<bool> AOFields = (IValue<bool>) new BoolValue(nameof (AOFields), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.AOFields);
    [Inspected]
    public IValue<bool> ContactShadows = (IValue<bool>) new BoolValue(nameof (ContactShadows), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.ContactShadows);
    [Inspected]
    public IValue<bool> DOF = (IValue<bool>) new BoolValue(nameof (DOF), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.DOF);
    [Inspected]
    public IValue<float> Gamma = (IValue<float>) new FloatValue(nameof (Gamma), 0.5f, 0.0f, 1f);
    [Inspected]
    public IValue<float> FieldOfView = (IValue<float>) new FloatValue(nameof (FieldOfView), ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.Fov, ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MinFov, ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MaxFov);
    [Inspected]
    public IValue<float> LevelOfDetails = (IValue<float>) new FloatValue(nameof (LevelOfDetails), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.LevelOfDetails, ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MinLevelOfDetails, ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MaxLevelOfDetails);
    [Inspected]
    public IValue<bool> MotionBlur = (IValue<bool>) new BoolValue(nameof (MotionBlur), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.MotionBlur);
    [Inspected]
    public IValue<float> RenderScale = (IValue<float>) new FloatValue(nameof (RenderScale), 1f, 0.25f, 2f);
    [Inspected]
    public IValue<float> ShadowDistance = (IValue<float>) new FloatValue(nameof (ShadowDistance), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.ShadowDistance, ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MinShadowDistance, ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MaxShadowDistance);
    [Inspected]
    public IValue<int> ShadowQuality = (IValue<int>) new IntValue(nameof (ShadowQuality), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.ShadowQuality, 0, ScriptableObjectInstance<GraphicSettingsData>.Instance.ShadowSettings.Length - 1);
    [Inspected]
    public IValue<bool> SoftParticles = (IValue<bool>) new BoolValue(nameof (SoftParticles), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.SoftParticles);
    [Inspected]
    public IValue<bool> SSAO = (IValue<bool>) new BoolValue(nameof (SSAO), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.SSAO);
    [Inspected]
    public IValue<bool> SSR = (IValue<bool>) new BoolValue(nameof (SSR), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.SSR);
    [Inspected]
    public IValue<int> TextureQuality = (IValue<int>) new IntValue(nameof (TextureQuality), ScriptableObjectInstance<GraphicSettingsData>.Instance.MaxSupportedTextureSettings(), 0, ScriptableObjectInstance<GraphicSettingsData>.Instance.TextureSettings.Length - 1);
    [Inspected]
    public IValue<int> VolumetricLighting = (IValue<int>) new IntValue(nameof (VolumetricLighting), ScriptableObjectInstance<GraphicSettingsData>.Instance.DefaultPreset.VolumetricLighting, 0, 3);
    [Inspected]
    public IValue<int> VSync = (IValue<int>) new IntValue(nameof (VSync), minValue: 0, maxValue: 4);
  }
}
