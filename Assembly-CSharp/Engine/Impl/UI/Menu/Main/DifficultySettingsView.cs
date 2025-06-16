using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Difficulties;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main
{
  public class DifficultySettingsView : SettingsView
  {
    [SerializeField]
    private StringView headerViewPrefab;
    [SerializeField]
    private GameObject separatorPrefab;
    [SerializeField]
    private AudioClip originalExperienceEnableSound;
    [SerializeField]
    private StringView tooltipView;
    private BoolSettingsValueView originalExperienceView;
    private NamedIntSettingsValueView presetsView;
    private List<GameObject> headerViews;
    private List<FloatSettingsValueView> floatViews;
    private ConfirmationWindow confirmationInstance;
    private ISelectable selectedView;
    private DifficultySettings difficultySettings;
    private ExternalDifficultySettings externalDifficultySettings;

    protected override void Awake()
    {
      this.headerViews = new List<GameObject>();
      this.floatViews = new List<FloatSettingsValueView>();
      this.difficultySettings = InstanceByRequest<DifficultySettings>.Instance;
      this.externalDifficultySettings = ExternalSettingsInstance<ExternalDifficultySettings>.Instance;
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.originalExperienceView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.originalExperienceView.name = "OriginalExperience";
      this.originalExperienceView.SetName("{UI.Menu.Main.Settings.Difficulty.OriginalExperience}");
      this.originalExperienceView.SetSetting(this.difficultySettings.OriginalExperience);
      BoolSettingsValueView originalExperienceView1 = this.originalExperienceView;
      originalExperienceView1.VisibleValueChangeEvent = originalExperienceView1.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(this.OnOriginalExperienceValueChange);
      BoolSettingsValueView originalExperienceView2 = this.originalExperienceView;
      originalExperienceView2.PointerEnterEvent = originalExperienceView2.PointerEnterEvent + new Action<SettingsValueView<bool>>(this.OnSelect<bool>);
      BoolSettingsValueView originalExperienceView3 = this.originalExperienceView;
      originalExperienceView3.PointerExitEvent = originalExperienceView3.PointerExitEvent + new Action<SettingsValueView<bool>>(this.OnDeselect<bool>);
      this.headerViews.Add(UnityEngine.Object.Instantiate<GameObject>(this.separatorPrefab, (Transform) this.layout.Content, false));
      int count = this.externalDifficultySettings.Presets.Count;
      string[] names = new string[count + 1];
      for (int index = 0; index < count; ++index)
        names[index] = "{UI.Menu.Main.Settings.Difficulty.Preset." + this.externalDifficultySettings.Presets[index].Name + "}";
      names[count] = "{UI.Menu.Main.Settings.Difficulty.Preset.Custom}";
      this.presetsView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.presetsView.name = "Preset";
      this.presetsView.SetName("{UI.Menu.Main.Settings.Difficulty.Preset}");
      this.presetsView.SetValueNames(names);
      NamedIntSettingsValueView presetsView1 = this.presetsView;
      presetsView1.VisibleValueChangeEvent = presetsView1.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(this.OnPresetValueChange);
      NamedIntSettingsValueView presetsView2 = this.presetsView;
      presetsView2.PointerEnterEvent = presetsView2.PointerEnterEvent + new Action<SettingsValueView<int>>(this.OnSelect<int>);
      NamedIntSettingsValueView presetsView3 = this.presetsView;
      presetsView3.PointerExitEvent = presetsView3.PointerExitEvent + new Action<SettingsValueView<int>>(this.OnDeselect<int>);
      this.headerViews.Add(UnityEngine.Object.Instantiate<GameObject>(this.separatorPrefab, (Transform) this.layout.Content, false));
      foreach (DifficultyGroupData group in this.externalDifficultySettings.Groups)
      {
        StringView stringView = UnityEngine.Object.Instantiate<StringView>(this.headerViewPrefab, (Transform) this.layout.Content, false);
        stringView.StringValue = "{UI.Menu.Main.Settings.Difficulty.Group." + group.Name + "}";
        this.headerViews.Add(stringView.gameObject);
        foreach (DifficultyGroupItemData difficultyGroupItemData in group.Items)
        {
          DifficultyItemData difficultyItemData1 = (DifficultyItemData) null;
          foreach (DifficultyItemData difficultyItemData2 in this.externalDifficultySettings.Items)
          {
            if (difficultyItemData2.Name == difficultyGroupItemData.Name)
            {
              difficultyItemData1 = difficultyItemData2;
              break;
            }
          }
          if (difficultyItemData1 != null && this.difficultySettings.GetValueItem(difficultyGroupItemData.Name) != null)
          {
            FloatSettingsValueView settingsValueView1 = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
            settingsValueView1.name = difficultyGroupItemData.Name;
            settingsValueView1.SetName("{UI.Menu.Main.Settings.Difficulty.Item." + difficultyGroupItemData.Name + "}");
            settingsValueView1.SetMinValue(difficultyItemData1.Min);
            settingsValueView1.SetMaxValue(difficultyItemData1.Max);
            settingsValueView1.SetValueNameFunction(new Func<float, string>(SettingsViewUtility.PercentValueName));
            settingsValueView1.SetSetting(this.difficultySettings.GetValueItem(difficultyGroupItemData.Name));
            settingsValueView1.SetValueValidationFunction(new Func<float, float>(this.ValueValidation), 0.1f);
            FloatSettingsValueView settingsValueView2 = settingsValueView1;
            settingsValueView2.VisibleValueChangeEvent = settingsValueView2.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnAutoValueChange<float>);
            FloatSettingsValueView settingsValueView3 = settingsValueView1;
            settingsValueView3.PointerEnterEvent = settingsValueView3.PointerEnterEvent + new Action<SettingsValueView<float>>(this.OnSelect<float>);
            FloatSettingsValueView settingsValueView4 = settingsValueView1;
            settingsValueView4.PointerExitEvent = settingsValueView4.PointerExitEvent + new Action<SettingsValueView<float>>(this.OnDeselect<float>);
            this.floatViews.Add(settingsValueView1);
          }
        }
      }
      base.Awake();
    }

    private void DisableOriginalExperience()
    {
      this.difficultySettings.OriginalExperience.Value = false;
      this.difficultySettings.Apply();
    }

    protected override void OnButtonReset()
    {
      MonoBehaviourInstance<UISounds>.Instance.PlaySound(this.originalExperienceEnableSound);
      this.difficultySettings.OriginalExperience.Value = true;
      foreach (DifficultyPresetData preset in this.externalDifficultySettings.Presets)
      {
        if (preset.Name == "Default")
        {
          this.SetPresetValues(preset);
          break;
        }
      }
      this.difficultySettings.Apply();
      this.SelectItem(0);
    }

    protected override bool OnResetGameAction(GameActionType type, bool down) => false;

    private bool IsCurrentPreset(DifficultyPresetData preset)
    {
      foreach (DifficultyPresetItemData difficultyPresetItemData in preset.Items)
      {
        if ((double) this.difficultySettings.GetValue(difficultyPresetItemData.Name) != (double) difficultyPresetItemData.Value)
          return false;
      }
      return true;
    }

    private void OnAutoValueChange<T>(SettingsValueView<T> view)
    {
      view.ApplyVisibleValue();
      this.difficultySettings.Apply();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.UpdateViews();
      this.difficultySettings.OnApply += new Action(this.UpdateViews);
    }

    private void OnDeselect<T>(SettingsValueView<T> view)
    {
      if (this.selectedView != view)
        return;
      this.SelectView<SettingsValueView<float>>((SettingsValueView<float>) null);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      this.difficultySettings.OnApply -= new Action(this.UpdateViews);
      this.SelectView<SettingsValueView<float>>((SettingsValueView<float>) null);
      if (!((UnityEngine.Object) this.confirmationInstance != (UnityEngine.Object) null))
        return;
      this.confirmationInstance.Hide();
    }

    private void OnOriginalExperienceValueChange(SettingsValueView<bool> view)
    {
      if (view.VisibleValue)
      {
        this.OnButtonReset();
      }
      else
      {
        this.SelectView<SettingsValueView<float>>((SettingsValueView<float>) null);
        view.RevertVisibleValue();
        if ((UnityEngine.Object) this.confirmationInstance == (UnityEngine.Object) null)
          this.confirmationInstance = UnityEngine.Object.Instantiate<ConfirmationWindow>(this.confirmationPrefab, this.transform, false);
        this.confirmationInstance.Show("{UI.Menu.Main.Settings.Difficulty.Confirmation}", new Action(this.DisableOriginalExperience), (Action) null);
      }
    }

    private void OnPresetValueChange(SettingsValueView<int> view)
    {
      List<DifficultyPresetData> presets = this.externalDifficultySettings.Presets;
      if (view.VisibleValue >= presets.Count)
        return;
      this.SetPresetValues(presets[view.VisibleValue]);
      this.difficultySettings.Apply();
    }

    private void OnSelect<T>(SettingsValueView<T> view)
    {
      this.SelectView<SettingsValueView<T>>(view);
    }

    private void SelectView<T>(T view) where T : MonoBehaviour, ISelectable
    {
      if (this.selectedView == (object) view)
        return;
      if (this.selectedView != null)
        this.selectedView.Selected = false;
      this.selectedView = (ISelectable) view;
      if ((UnityEngine.Object) view != (UnityEngine.Object) null)
      {
        view.Selected = true;
        this.tooltipView.StringValue = "{UI.Menu.Main.Settings.Difficulty.Tooltip." + view.name + "}";
        this.tooltipView.gameObject.SetActive(true);
      }
      else
      {
        this.tooltipView.StringValue = (string) null;
        this.tooltipView.gameObject.SetActive(false);
      }
    }

    private void SetPresetValues(DifficultyPresetData preset)
    {
      foreach (DifficultyPresetItemData difficultyPresetItemData in preset.Items)
      {
        IValue<float> valueItem = this.difficultySettings.GetValueItem(difficultyPresetItemData.Name);
        if (valueItem != null)
          valueItem.Value = difficultyPresetItemData.Value;
      }
    }

    private float ValueValidation(float value) => SettingsViewUtility.RoundValue(value, 0.1f);

    private void UpdateViews()
    {
      this.originalExperienceView.RevertVisibleValue();
      bool flag1 = !this.originalExperienceView.VisibleValue;
      this.presetsView.gameObject.SetActive(flag1);
      if (flag1)
      {
        List<DifficultyPresetData> presets = this.externalDifficultySettings.Presets;
        bool flag2 = true;
        for (int index = 0; index < presets.Count; ++index)
        {
          if (this.IsCurrentPreset(presets[index]))
          {
            this.presetsView.VisibleValue = index;
            flag2 = false;
            break;
          }
        }
        if (flag2)
          this.presetsView.VisibleValue = presets.Count;
      }
      foreach (GameObject headerView in this.headerViews)
        headerView.SetActive(flag1);
      foreach (FloatSettingsValueView floatView in this.floatViews)
      {
        floatView.gameObject.SetActive(flag1);
        if (flag1)
          floatView.RevertVisibleValue();
      }
    }
  }
}
