using Engine.Source.Commons;
using Engine.Source.Settings;
using System;
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
      this.graphicsGameSettings = InstanceByRequest<GraphicsGameSettings>.Instance;
      GraphicSettingsData instance = ScriptableObjectInstance<GraphicSettingsData>.Instance;
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.vSyncView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.vSyncView.SetName("{UI.Menu.Main.Settings.Graphics.VSync}");
      this.vSyncView.SetValueNames(new string[5]
      {
        "{UI.Menu.Main.Settings.Graphics.VSync.0}",
        "{UI.Menu.Main.Settings.Graphics.VSync.1}",
        "{UI.Menu.Main.Settings.Graphics.VSync.2}",
        "{UI.Menu.Main.Settings.Graphics.VSync.3}",
        "{UI.Menu.Main.Settings.Graphics.VSync.4}"
      });
      this.vSyncView.SetSetting(this.graphicsGameSettings.VSync);
      NamedIntSettingsValueView vSyncView = this.vSyncView;
      vSyncView.VisibleValueChangeEvent = vSyncView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(GraphicSettingsHelper.OnAutoValueChange<int>);
      this.gammaView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.gammaView.SetName("{UI.Menu.Main.Settings.Graphics.Gamma}");
      this.gammaView.SetMinValue(this.graphicsGameSettings.Gamma.MinValue);
      this.gammaView.SetMaxValue(this.graphicsGameSettings.Gamma.MaxValue);
      this.gammaView.SetValueNameFunction(new Func<float, string>(SettingsViewUtility.GammaValueName));
      this.gammaView.SetSetting(this.graphicsGameSettings.Gamma);
      this.gammaView.SetValueValidationFunction(new Func<float, float>(SettingsViewUtility.GammaValueValidation), 0.025f);
      FloatSettingsValueView gammaView = this.gammaView;
      gammaView.VisibleValueChangeEvent = gammaView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(GraphicSettingsHelper.OnAutoValueChange<float>);
      this.fieldOfViewView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.fieldOfViewView.SetName("{UI.Menu.Main.Settings.Graphics.FieldOfView}");
      this.fieldOfViewView.SetMinValue(this.graphicsGameSettings.FieldOfView.MinValue);
      this.fieldOfViewView.SetMaxValue(this.graphicsGameSettings.FieldOfView.MaxValue);
      this.fieldOfViewView.SetValueNameFunction(new Func<float, string>(Convert.ToString));
      this.fieldOfViewView.SetSetting(this.graphicsGameSettings.FieldOfView);
      this.fieldOfViewView.SetValueValidationFunction(new Func<float, float>(SettingsViewUtility.RoundValueTo5), 5f);
      FloatSettingsValueView fieldOfViewView = this.fieldOfViewView;
      fieldOfViewView.VisibleValueChangeEvent = fieldOfViewView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(GraphicSettingsHelper.OnAutoValueChange<float>);
      int length1 = instance.TextureSettings.Length;
      string[] names1 = new string[length1];
      int num = instance.MaxSupportedTextureSettings();
      for (int index = 0; index < length1; ++index)
      {
        string str = "{UI.Menu.Main.Settings.Graphics.TextureQuality." + (object) index + "}";
        if (index > num)
          str = "<color=" + this.errorColor.ToRGBHex() + ">" + str + "</color>";
        names1[index] = str;
      }
      this.textureQualityView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.textureQualityView.SetName("{UI.Menu.Main.Settings.Graphics.TextureQuality}");
      this.textureQualityView.SetValueNames(names1);
      this.textureQualityView.SetSetting(this.graphicsGameSettings.TextureQuality);
      NamedIntSettingsValueView textureQualityView = this.textureQualityView;
      textureQualityView.VisibleValueChangeEvent = textureQualityView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(GraphicSettingsHelper.OnAutoValueChange<int>);
      UnityEngine.Object.Instantiate<GameObject>(this.separatorPrefab, (Transform) this.layout.Content, false);
      int length2 = instance.Presets.Length;
      string[] names2 = new string[length2 + 1];
      for (int index = 0; index < length2; ++index)
        names2[index] = "{UI.Menu.Main.Settings.Graphics.Quality." + (object) index + "}";
      names2[length2] = "{UI.Menu.Main.Settings.Graphics.Quality.Custom}";
      this.qualityView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.qualityView.SetName("{UI.Menu.Main.Settings.Graphics.Quality}");
      this.qualityView.SetValueNames(names2);
      NamedIntSettingsValueView qualityView = this.qualityView;
      qualityView.VisibleValueChangeEvent = qualityView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(this.OnQualityChange);
      UnityEngine.Object.Instantiate<GameObject>(this.separatorPrefab, (Transform) this.layout.Content, false);
      this.lodView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.lodView.SetName("{UI.Menu.Main.Settings.Graphics.LevelOfDetail}");
      this.lodView.SetMinValue(this.graphicsGameSettings.LevelOfDetails.MinValue);
      this.lodView.SetMaxValue(this.graphicsGameSettings.LevelOfDetails.MaxValue);
      this.lodView.SetValueNameFunction(new Func<float, string>(this.LodValueName));
      this.lodView.SetSetting(this.graphicsGameSettings.LevelOfDetails);
      this.lodView.SetValueValidationFunction(new Func<float, float>(this.LodValueValidation), 0.1f);
      FloatSettingsValueView lodView = this.lodView;
      lodView.VisibleValueChangeEvent = lodView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(GraphicSettingsHelper.OnAutoValueChange<float>);
      this.aiLodDistanceView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.aiLodDistanceView.SetName("{UI.Menu.Main.Settings.Graphics.AILodDistance}");
      this.aiLodDistanceView.SetMinValue(this.graphicsGameSettings.AILodDistance.MinValue);
      this.aiLodDistanceView.SetMaxValue(this.graphicsGameSettings.AILodDistance.MaxValue);
      this.aiLodDistanceView.SetValueNameFunction(new Func<float, string>(Convert.ToString));
      this.aiLodDistanceView.SetSetting(this.graphicsGameSettings.AILodDistance);
      this.aiLodDistanceView.SetValueValidationFunction(new Func<float, float>(SettingsViewUtility.RoundValueTo5), 5f);
      FloatSettingsValueView aiLodDistanceView = this.aiLodDistanceView;
      aiLodDistanceView.VisibleValueChangeEvent = aiLodDistanceView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(GraphicSettingsHelper.OnAutoValueChange<float>);
      this.anisotropicFilteringView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.anisotropicFilteringView.SetName("{UI.Menu.Main.Settings.Graphics.AnisotropicFiltering}");
      this.anisotropicFilteringView.SetSetting(this.graphicsGameSettings.AnisotropicFiltering);
      BoolSettingsValueView anisotropicFilteringView = this.anisotropicFilteringView;
      anisotropicFilteringView.VisibleValueChangeEvent = anisotropicFilteringView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      int length3 = instance.ShadowSettings.Length;
      string[] names3 = new string[length3];
      for (int index = 0; index < length3; ++index)
        names3[index] = "{UI.Menu.Main.Settings.Graphics.ShadowQuality." + (object) index + "}";
      this.shadowQualityView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.shadowQualityView.SetName("{UI.Menu.Main.Settings.Graphics.ShadowQuality}");
      this.shadowQualityView.SetValueNames(names3);
      this.shadowQualityView.SetSetting(this.graphicsGameSettings.ShadowQuality);
      NamedIntSettingsValueView shadowQualityView = this.shadowQualityView;
      shadowQualityView.VisibleValueChangeEvent = shadowQualityView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(GraphicSettingsHelper.OnAutoValueChange<int>);
      this.shadowDistanceView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.shadowDistanceView.SetName("{UI.Menu.Main.Settings.Graphics.ShadowDistance}");
      this.shadowDistanceView.SetMinValue(this.graphicsGameSettings.ShadowDistance.MinValue);
      this.shadowDistanceView.SetMaxValue(this.graphicsGameSettings.ShadowDistance.MaxValue);
      this.shadowDistanceView.SetValueNameFunction(new Func<float, string>(Convert.ToString));
      this.shadowDistanceView.SetSetting(this.graphicsGameSettings.ShadowDistance);
      this.shadowDistanceView.SetValueValidationFunction(new Func<float, float>(SettingsViewUtility.RoundValueTo5), 5f);
      FloatSettingsValueView shadowDistanceView = this.shadowDistanceView;
      shadowDistanceView.VisibleValueChangeEvent = shadowDistanceView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(GraphicSettingsHelper.OnAutoValueChange<float>);
      this.contactShadowsView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.contactShadowsView.SetName("{UI.Menu.Main.Settings.Graphics.ContactShadows}");
      this.contactShadowsView.SetSetting(this.graphicsGameSettings.ContactShadows);
      BoolSettingsValueView contactShadowsView = this.contactShadowsView;
      contactShadowsView.VisibleValueChangeEvent = contactShadowsView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.aoFieldsView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.aoFieldsView.SetName("{UI.Menu.Main.Settings.Graphics.AOFields}");
      this.aoFieldsView.SetSetting(this.graphicsGameSettings.AOFields);
      BoolSettingsValueView aoFieldsView = this.aoFieldsView;
      aoFieldsView.VisibleValueChangeEvent = aoFieldsView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.ssaoView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.ssaoView.SetName("{UI.Menu.Main.Settings.Graphics.SSAO}");
      this.ssaoView.SetSetting(this.graphicsGameSettings.SSAO);
      BoolSettingsValueView ssaoView = this.ssaoView;
      ssaoView.VisibleValueChangeEvent = ssaoView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.antialiasingView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.antialiasingView.SetName("{UI.Menu.Main.Settings.Graphics.Antialiasing}");
      this.antialiasingView.SetSetting(this.graphicsGameSettings.Antialiasing);
      BoolSettingsValueView antialiasingView = this.antialiasingView;
      antialiasingView.VisibleValueChangeEvent = antialiasingView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.additionalReflectionsView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.additionalReflectionsView.SetName("{UI.Menu.Main.Settings.Graphics.AdditionalReflections}");
      this.additionalReflectionsView.SetSetting(this.graphicsGameSettings.AdditionalReflections);
      BoolSettingsValueView additionalReflectionsView = this.additionalReflectionsView;
      additionalReflectionsView.VisibleValueChangeEvent = additionalReflectionsView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.ssrView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.ssrView.SetName("{UI.Menu.Main.Settings.Graphics.SSR}");
      this.ssrView.SetSetting(this.graphicsGameSettings.SSR);
      BoolSettingsValueView ssrView = this.ssrView;
      ssrView.VisibleValueChangeEvent = ssrView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.dofView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.dofView.SetName("{UI.Menu.Main.Settings.Graphics.DepthOfField}");
      this.dofView.SetSetting(this.graphicsGameSettings.DOF);
      BoolSettingsValueView dofView = this.dofView;
      dofView.VisibleValueChangeEvent = dofView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.motionBlurView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.motionBlurView.SetName("{UI.Menu.Main.Settings.Graphics.MotionBlur}");
      this.motionBlurView.SetSetting(this.graphicsGameSettings.MotionBlur);
      BoolSettingsValueView motionBlurView = this.motionBlurView;
      motionBlurView.VisibleValueChangeEvent = motionBlurView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.softParticlesView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.softParticlesView.SetName("{UI.Menu.Main.Settings.Graphics.SoftParticles}");
      this.softParticlesView.SetSetting(this.graphicsGameSettings.SoftParticles);
      BoolSettingsValueView softParticlesView = this.softParticlesView;
      softParticlesView.VisibleValueChangeEvent = softParticlesView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(GraphicSettingsHelper.OnAutoValueChange<bool>);
      this.volumetricLightingView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.volumetricLightingView.SetName("{UI.Menu.Main.Settings.Graphics.VolumetricLighting}");
      this.volumetricLightingView.SetValueNames(new string[4]
      {
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Off}",
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Quarter}",
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Half}",
        "{UI.Menu.Main.Settings.Graphics.VolumetricLighting.Full}"
      });
      this.volumetricLightingView.SetSetting(this.graphicsGameSettings.VolumetricLighting);
      NamedIntSettingsValueView volumetricLightingView = this.volumetricLightingView;
      volumetricLightingView.VisibleValueChangeEvent = volumetricLightingView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(GraphicSettingsHelper.OnAutoValueChange<int>);
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
      this.vSyncView.RevertVisibleValue();
      this.gammaView.RevertVisibleValue();
      this.fieldOfViewView.RevertVisibleValue();
      this.textureQualityView.RevertVisibleValue();
      GraphicSettingsPreset[] presets = ScriptableObjectInstance<GraphicSettingsData>.Instance.Presets;
      bool flag = true;
      for (int index = 0; index < presets.Length; ++index)
      {
        if (presets[index].IsCurrent())
        {
          this.qualityView.VisibleValue = index;
          flag = false;
          break;
        }
      }
      if (flag)
        this.qualityView.VisibleValue = presets.Length;
      this.lodView.RevertVisibleValue();
      this.aiLodDistanceView.RevertVisibleValue();
      this.anisotropicFilteringView.RevertVisibleValue();
      this.shadowQualityView.RevertVisibleValue();
      this.shadowDistanceView.RevertVisibleValue();
      this.shadowDistanceView.Interactable = this.shadowQualityView.VisibleValue != 0;
      this.contactShadowsView.RevertVisibleValue();
      this.aoFieldsView.RevertVisibleValue();
      this.ssaoView.RevertVisibleValue();
      this.antialiasingView.RevertVisibleValue();
      this.additionalReflectionsView.RevertVisibleValue();
      this.ssrView.RevertVisibleValue();
      this.dofView.RevertVisibleValue();
      this.motionBlurView.RevertVisibleValue();
      this.softParticlesView.RevertVisibleValue();
      this.volumetricLightingView.RevertVisibleValue();
    }

    protected override void OnButtonReset()
    {
      this.vSyncView.ResetValue();
      this.gammaView.ResetValue();
      this.fieldOfViewView.ResetValue();
      this.textureQualityView.ResetValue();
      this.lodView.ResetValue();
      this.aiLodDistanceView.ResetValue();
      this.anisotropicFilteringView.ResetValue();
      this.shadowQualityView.ResetValue();
      this.shadowDistanceView.ResetValue();
      this.contactShadowsView.ResetValue();
      this.aoFieldsView.ResetValue();
      this.ssaoView.ResetValue();
      this.antialiasingView.ResetValue();
      this.additionalReflectionsView.ResetValue();
      this.ssrView.ResetValue();
      this.dofView.ResetValue();
      this.motionBlurView.ResetValue();
      this.softParticlesView.ResetValue();
      this.volumetricLightingView.ResetValue();
      this.graphicsGameSettings.Apply();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.UpdateViews();
      this.graphicsGameSettings.OnApply += new Action(this.UpdateViews);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      this.graphicsGameSettings.OnApply -= new Action(this.UpdateViews);
    }
  }
}
