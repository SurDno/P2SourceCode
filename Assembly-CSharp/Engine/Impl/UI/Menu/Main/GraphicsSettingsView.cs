using System;
using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main
{
  public class GraphicsSettingsView : SettingsView
  {
    [SerializeField]
    private GameObject separatorPrefab;
    [SerializeField]
    private Color errorColor;
    private NamedIntSettingsValueView vSyncView;
    private FloatSettingsValueView gammaView;
    private FloatSettingsValueView fieldOfViewView;
    private NamedIntSettingsValueView textureQualityView;
    private NamedIntSettingsValueView qualityView;
    private FloatSettingsValueView lodView;
    private FloatSettingsValueView aiLodDistanceView;
    private BoolSettingsValueView anisotropicFilteringView;
    private NamedIntSettingsValueView shadowQualityView;
    private FloatSettingsValueView shadowDistanceView;
    private BoolSettingsValueView contactShadowsView;
    private BoolSettingsValueView aoFieldsView;
    private BoolSettingsValueView ssaoView;
    private BoolSettingsValueView antialiasingView;
    private BoolSettingsValueView additionalReflectionsView;
    private BoolSettingsValueView ssrView;
    private BoolSettingsValueView dofView;
    private BoolSettingsValueView motionBlurView;
    private BoolSettingsValueView softParticlesView;
    private NamedIntSettingsValueView volumetricLightingView;
    private GraphicsGameSettings graphicsGameSettings;

    protected override void Awake()
    {
      graphicsGameSettings = InstanceByRequest<GraphicsGameSettings>.Instance;
      GraphicSettingsData instance = ScriptableObjectInstance<GraphicSettingsData>.Instance;
      layout = Instantiate(listLayoutPrefab, transform, false);
      this.vSyncView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      this.vSyncView.SetName("{UI.Menu.Main.Settings.Graphics.VSync}");
      this.vSyncView.SetValueNames(new string[5]
      {
        "{UI.Menu.Main.Settings.Graphics.VSync.0}",
        "{UI.Menu.Main.Settings.Graphics.VSync.1}",
        "{UI.Menu.Main.Settings.Graphics.VSync.2}",
        "{UI.Menu.Main.Settings.Graphics.VSync.3}",
        "{UI.Menu.Main.Settings.Graphics.VSync.4}"
      });
      this.vSyncView.SetSetting(graphicsGameSettings.VSync);
      NamedIntSettingsValueView vSyncView = this.vSyncView;
      vSyncView.VisibleValueChangeEvent = vSyncView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.gammaView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.gammaView.SetName("{UI.Menu.Main.Settings.Graphics.Gamma}");
      this.gammaView.SetMinValue(graphicsGameSettings.Gamma.MinValue);
      this.gammaView.SetMaxValue(graphicsGameSettings.Gamma.MaxValue);
      this.gammaView.SetValueNameFunction(SettingsViewUtility.GammaValueName);
      this.gammaView.SetSetting(graphicsGameSettings.Gamma);
      this.gammaView.SetValueValidationFunction(SettingsViewUtility.GammaValueValidation, 0.025f);
      FloatSettingsValueView gammaView = this.gammaView;
      gammaView.VisibleValueChangeEvent = gammaView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.fieldOfViewView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.fieldOfViewView.SetName("{UI.Menu.Main.Settings.Graphics.FieldOfView}");
      this.fieldOfViewView.SetMinValue(graphicsGameSettings.FieldOfView.MinValue);
      this.fieldOfViewView.SetMaxValue(graphicsGameSettings.FieldOfView.MaxValue);
      this.fieldOfViewView.SetValueNameFunction(Convert.ToString);
      this.fieldOfViewView.SetSetting(graphicsGameSettings.FieldOfView);
      this.fieldOfViewView.SetValueValidationFunction(SettingsViewUtility.RoundValueTo5, 5f);
      FloatSettingsValueView fieldOfViewView = this.fieldOfViewView;
      fieldOfViewView.VisibleValueChangeEvent = fieldOfViewView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      int length1 = instance.TextureSettings.Length;
      string[] names1 = new string[length1];
      int num = instance.MaxSupportedTextureSettings();
      for (int index = 0; index < length1; ++index)
      {
        string str = "{UI.Menu.Main.Settings.Graphics.TextureQuality." + index + "}";
        if (index > num)
          str = "<color=" + errorColor.ToRGBHex() + ">" + str + "</color>";
        names1[index] = str;
      }
      this.textureQualityView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      this.textureQualityView.SetName("{UI.Menu.Main.Settings.Graphics.TextureQuality}");
      this.textureQualityView.SetValueNames(names1);
      this.textureQualityView.SetSetting(graphicsGameSettings.TextureQuality);
      NamedIntSettingsValueView textureQualityView = this.textureQualityView;
      textureQualityView.VisibleValueChangeEvent = textureQualityView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      Instantiate(separatorPrefab, layout.Content, false);
      int length2 = instance.Presets.Length;
      string[] names2 = new string[length2 + 1];
      for (int index = 0; index < length2; ++index)
        names2[index] = "{UI.Menu.Main.Settings.Graphics.Quality." + index + "}";
      names2[length2] = "{UI.Menu.Main.Settings.Graphics.Quality.Custom}";
      this.qualityView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      this.qualityView.SetName("{UI.Menu.Main.Settings.Graphics.Quality}");
      this.qualityView.SetValueNames(names2);
      NamedIntSettingsValueView qualityView = this.qualityView;
      qualityView.VisibleValueChangeEvent = qualityView.VisibleValueChangeEvent + OnQualityChange;
      Instantiate(separatorPrefab, layout.Content, false);
      this.lodView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.lodView.SetName("{UI.Menu.Main.Settings.Graphics.LevelOfDetail}");
      this.lodView.SetMinValue(graphicsGameSettings.LevelOfDetails.MinValue);
      this.lodView.SetMaxValue(graphicsGameSettings.LevelOfDetails.MaxValue);
      this.lodView.SetValueNameFunction(LodValueName);
      this.lodView.SetSetting(graphicsGameSettings.LevelOfDetails);
      this.lodView.SetValueValidationFunction(LodValueValidation, 0.1f);
      FloatSettingsValueView lodView = this.lodView;
      lodView.VisibleValueChangeEvent = lodView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.aiLodDistanceView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.aiLodDistanceView.SetName("{UI.Menu.Main.Settings.Graphics.AILodDistance}");
      this.aiLodDistanceView.SetMinValue(graphicsGameSettings.AILodDistance.MinValue);
      this.aiLodDistanceView.SetMaxValue(graphicsGameSettings.AILodDistance.MaxValue);
      this.aiLodDistanceView.SetValueNameFunction(Convert.ToString);
      this.aiLodDistanceView.SetSetting(graphicsGameSettings.AILodDistance);
      this.aiLodDistanceView.SetValueValidationFunction(SettingsViewUtility.RoundValueTo5, 5f);
      FloatSettingsValueView aiLodDistanceView = this.aiLodDistanceView;
      aiLodDistanceView.VisibleValueChangeEvent = aiLodDistanceView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.anisotropicFilteringView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.anisotropicFilteringView.SetName("{UI.Menu.Main.Settings.Graphics.AnisotropicFiltering}");
      this.anisotropicFilteringView.SetSetting(graphicsGameSettings.AnisotropicFiltering);
      BoolSettingsValueView anisotropicFilteringView = this.anisotropicFilteringView;
      anisotropicFilteringView.VisibleValueChangeEvent = anisotropicFilteringView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      int length3 = instance.ShadowSettings.Length;
      string[] names3 = new string[length3];
      for (int index = 0; index < length3; ++index)
        names3[index] = "{UI.Menu.Main.Settings.Graphics.ShadowQuality." + index + "}";
      this.shadowQualityView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      this.shadowQualityView.SetName("{UI.Menu.Main.Settings.Graphics.ShadowQuality}");
      this.shadowQualityView.SetValueNames(names3);
      this.shadowQualityView.SetSetting(graphicsGameSettings.ShadowQuality);
      NamedIntSettingsValueView shadowQualityView = this.shadowQualityView;
      shadowQualityView.VisibleValueChangeEvent = shadowQualityView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.shadowDistanceView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.shadowDistanceView.SetName("{UI.Menu.Main.Settings.Graphics.ShadowDistance}");
      this.shadowDistanceView.SetMinValue(graphicsGameSettings.ShadowDistance.MinValue);
      this.shadowDistanceView.SetMaxValue(graphicsGameSettings.ShadowDistance.MaxValue);
      this.shadowDistanceView.SetValueNameFunction(Convert.ToString);
      this.shadowDistanceView.SetSetting(graphicsGameSettings.ShadowDistance);
      this.shadowDistanceView.SetValueValidationFunction(SettingsViewUtility.RoundValueTo5, 5f);
      FloatSettingsValueView shadowDistanceView = this.shadowDistanceView;
      shadowDistanceView.VisibleValueChangeEvent = shadowDistanceView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.contactShadowsView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.contactShadowsView.SetName("{UI.Menu.Main.Settings.Graphics.ContactShadows}");
      this.contactShadowsView.SetSetting(graphicsGameSettings.ContactShadows);
      BoolSettingsValueView contactShadowsView = this.contactShadowsView;
      contactShadowsView.VisibleValueChangeEvent = contactShadowsView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.aoFieldsView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.aoFieldsView.SetName("{UI.Menu.Main.Settings.Graphics.AOFields}");
      this.aoFieldsView.SetSetting(graphicsGameSettings.AOFields);
      BoolSettingsValueView aoFieldsView = this.aoFieldsView;
      aoFieldsView.VisibleValueChangeEvent = aoFieldsView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.ssaoView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.ssaoView.SetName("{UI.Menu.Main.Settings.Graphics.SSAO}");
      this.ssaoView.SetSetting(graphicsGameSettings.SSAO);
      BoolSettingsValueView ssaoView = this.ssaoView;
      ssaoView.VisibleValueChangeEvent = ssaoView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.antialiasingView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.antialiasingView.SetName("{UI.Menu.Main.Settings.Graphics.Antialiasing}");
      this.antialiasingView.SetSetting(graphicsGameSettings.Antialiasing);
      BoolSettingsValueView antialiasingView = this.antialiasingView;
      antialiasingView.VisibleValueChangeEvent = antialiasingView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.additionalReflectionsView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.additionalReflectionsView.SetName("{UI.Menu.Main.Settings.Graphics.AdditionalReflections}");
      this.additionalReflectionsView.SetSetting(graphicsGameSettings.AdditionalReflections);
      BoolSettingsValueView additionalReflectionsView = this.additionalReflectionsView;
      additionalReflectionsView.VisibleValueChangeEvent = additionalReflectionsView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.ssrView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.ssrView.SetName("{UI.Menu.Main.Settings.Graphics.SSR}");
      this.ssrView.SetSetting(graphicsGameSettings.SSR);
      BoolSettingsValueView ssrView = this.ssrView;
      ssrView.VisibleValueChangeEvent = ssrView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.dofView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.dofView.SetName("{UI.Menu.Main.Settings.Graphics.DepthOfField}");
      this.dofView.SetSetting(graphicsGameSettings.DOF);
      BoolSettingsValueView dofView = this.dofView;
      dofView.VisibleValueChangeEvent = dofView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.motionBlurView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.motionBlurView.SetName("{UI.Menu.Main.Settings.Graphics.MotionBlur}");
      this.motionBlurView.SetSetting(graphicsGameSettings.MotionBlur);
      BoolSettingsValueView motionBlurView = this.motionBlurView;
      motionBlurView.VisibleValueChangeEvent = motionBlurView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.softParticlesView = Instantiate(boolValueViewPrefab, layout.Content, false);
      this.softParticlesView.SetName("{UI.Menu.Main.Settings.Graphics.SoftParticles}");
      this.softParticlesView.SetSetting(graphicsGameSettings.SoftParticles);
      BoolSettingsValueView softParticlesView = this.softParticlesView;
      softParticlesView.VisibleValueChangeEvent = softParticlesView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      this.volumetricLightingView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      this.volumetricLightingView.SetName("{UI.Menu.Main.Settings.Graphics.VolumetricLighting}");
      this.volumetricLightingView.SetValueNames(new string[4]
      {
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Off}",
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Quarter}",
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Half}",
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Full}"
      });
      this.volumetricLightingView.SetSetting(graphicsGameSettings.VolumetricLighting);
      NamedIntSettingsValueView volumetricLightingView = this.volumetricLightingView;
      volumetricLightingView.VisibleValueChangeEvent = volumetricLightingView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      base.Awake();
    }

    private void OnQualityChange(SettingsValueView<int> view)
    {
      GraphicSettingsPreset[] presets = ScriptableObjectInstance<GraphicSettingsData>.Instance.Presets;
      if (view.VisibleValue >= presets.Length)
        return;
      presets[view.VisibleValue].Apply();
    }

    private string LodValueName(float value) => value.ToString("F1");

    private float LodValueValidation(float value) => SettingsViewUtility.RoundValue(value, 0.1f);

    private void UpdateViews()
    {
      vSyncView.RevertVisibleValue();
      gammaView.RevertVisibleValue();
      fieldOfViewView.RevertVisibleValue();
      textureQualityView.RevertVisibleValue();
      GraphicSettingsPreset[] presets = ScriptableObjectInstance<GraphicSettingsData>.Instance.Presets;
      bool flag = true;
      for (int index = 0; index < presets.Length; ++index)
      {
        if (presets[index].IsCurrent())
        {
          qualityView.VisibleValue = index;
          flag = false;
          break;
        }
      }
      if (flag)
        qualityView.VisibleValue = presets.Length;
      lodView.RevertVisibleValue();
      aiLodDistanceView.RevertVisibleValue();
      anisotropicFilteringView.RevertVisibleValue();
      shadowQualityView.RevertVisibleValue();
      shadowDistanceView.RevertVisibleValue();
      shadowDistanceView.Interactable = shadowQualityView.VisibleValue != 0;
      contactShadowsView.RevertVisibleValue();
      aoFieldsView.RevertVisibleValue();
      ssaoView.RevertVisibleValue();
      antialiasingView.RevertVisibleValue();
      additionalReflectionsView.RevertVisibleValue();
      ssrView.RevertVisibleValue();
      dofView.RevertVisibleValue();
      motionBlurView.RevertVisibleValue();
      softParticlesView.RevertVisibleValue();
      volumetricLightingView.RevertVisibleValue();
    }

    protected override void OnButtonReset()
    {
      vSyncView.ResetValue();
      gammaView.ResetValue();
      fieldOfViewView.ResetValue();
      textureQualityView.ResetValue();
      lodView.ResetValue();
      aiLodDistanceView.ResetValue();
      anisotropicFilteringView.ResetValue();
      shadowQualityView.ResetValue();
      shadowDistanceView.ResetValue();
      contactShadowsView.ResetValue();
      aoFieldsView.ResetValue();
      ssaoView.ResetValue();
      antialiasingView.ResetValue();
      additionalReflectionsView.ResetValue();
      ssrView.ResetValue();
      dofView.ResetValue();
      motionBlurView.ResetValue();
      softParticlesView.ResetValue();
      volumetricLightingView.ResetValue();
      graphicsGameSettings.Apply();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      UpdateViews();
      graphicsGameSettings.OnApply += UpdateViews;
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      graphicsGameSettings.OnApply -= UpdateViews;
    }
  }
}
