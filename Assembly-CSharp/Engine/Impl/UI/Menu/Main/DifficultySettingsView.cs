using System.Collections.Generic;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Difficulties;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
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
      headerViews = [];
      floatViews = [];
      difficultySettings = InstanceByRequest<DifficultySettings>.Instance;
      externalDifficultySettings = ExternalSettingsInstance<ExternalDifficultySettings>.Instance;
      layout = Instantiate(listLayoutPrefab, transform, false);
      originalExperienceView = Instantiate(boolValueViewPrefab, layout.Content, false);
      originalExperienceView.name = "OriginalExperience";
      originalExperienceView.SetName("{UI.Menu.Main.Settings.Difficulty.OriginalExperience}");
      originalExperienceView.SetSetting(difficultySettings.OriginalExperience);
      BoolSettingsValueView originalExperienceView1 = originalExperienceView;
      originalExperienceView1.VisibleValueChangeEvent = originalExperienceView1.VisibleValueChangeEvent + OnOriginalExperienceValueChange;
      BoolSettingsValueView originalExperienceView2 = originalExperienceView;
      originalExperienceView2.PointerEnterEvent = originalExperienceView2.PointerEnterEvent + OnSelect;
      BoolSettingsValueView originalExperienceView3 = originalExperienceView;
      originalExperienceView3.PointerExitEvent = originalExperienceView3.PointerExitEvent + OnDeselect;
      headerViews.Add(Instantiate(separatorPrefab, layout.Content, false));
      int count = externalDifficultySettings.Presets.Count;
      string[] names = new string[count + 1];
      for (int index = 0; index < count; ++index)
        names[index] = "{UI.Menu.Main.Settings.Difficulty.Preset." + externalDifficultySettings.Presets[index].Name + "}";
      names[count] = "{UI.Menu.Main.Settings.Difficulty.Preset.Custom}";
      presetsView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      presetsView.name = "Preset";
      presetsView.SetName("{UI.Menu.Main.Settings.Difficulty.Preset}");
      presetsView.SetValueNames(names);
      NamedIntSettingsValueView presetsView1 = presetsView;
      presetsView1.VisibleValueChangeEvent = presetsView1.VisibleValueChangeEvent + OnPresetValueChange;
      NamedIntSettingsValueView presetsView2 = presetsView;
      presetsView2.PointerEnterEvent = presetsView2.PointerEnterEvent + OnSelect;
      NamedIntSettingsValueView presetsView3 = presetsView;
      presetsView3.PointerExitEvent = presetsView3.PointerExitEvent + OnDeselect;
      headerViews.Add(Instantiate(separatorPrefab, layout.Content, false));
      foreach (DifficultyGroupData group in externalDifficultySettings.Groups)
      {
        StringView stringView = Instantiate(headerViewPrefab, layout.Content, false);
        stringView.StringValue = "{UI.Menu.Main.Settings.Difficulty.Group." + group.Name + "}";
        headerViews.Add(stringView.gameObject);
        foreach (DifficultyGroupItemData difficultyGroupItemData in group.Items)
        {
          DifficultyItemData difficultyItemData1 = null;
          foreach (DifficultyItemData difficultyItemData2 in externalDifficultySettings.Items)
          {
            if (difficultyItemData2.Name == difficultyGroupItemData.Name)
            {
              difficultyItemData1 = difficultyItemData2;
              break;
            }
          }
          if (difficultyItemData1 != null && difficultySettings.GetValueItem(difficultyGroupItemData.Name) != null)
          {
            FloatSettingsValueView settingsValueView1 = Instantiate(floatValueViewPrefab, layout.Content, false);
            settingsValueView1.name = difficultyGroupItemData.Name;
            settingsValueView1.SetName("{UI.Menu.Main.Settings.Difficulty.Item." + difficultyGroupItemData.Name + "}");
            settingsValueView1.SetMinValue(difficultyItemData1.Min);
            settingsValueView1.SetMaxValue(difficultyItemData1.Max);
            settingsValueView1.SetValueNameFunction(SettingsViewUtility.PercentValueName);
            settingsValueView1.SetSetting(difficultySettings.GetValueItem(difficultyGroupItemData.Name));
            settingsValueView1.SetValueValidationFunction(ValueValidation, 0.1f);
            FloatSettingsValueView settingsValueView2 = settingsValueView1;
            settingsValueView2.VisibleValueChangeEvent = settingsValueView2.VisibleValueChangeEvent + OnAutoValueChange;
            FloatSettingsValueView settingsValueView3 = settingsValueView1;
            settingsValueView3.PointerEnterEvent = settingsValueView3.PointerEnterEvent + OnSelect;
            FloatSettingsValueView settingsValueView4 = settingsValueView1;
            settingsValueView4.PointerExitEvent = settingsValueView4.PointerExitEvent + OnDeselect;
            floatViews.Add(settingsValueView1);
          }
        }
      }
      base.Awake();
    }

    private void DisableOriginalExperience()
    {
      difficultySettings.OriginalExperience.Value = false;
      difficultySettings.Apply();
    }

    protected override void OnButtonReset()
    {
      MonoBehaviourInstance<UISounds>.Instance.PlaySound(originalExperienceEnableSound);
      difficultySettings.OriginalExperience.Value = true;
      foreach (DifficultyPresetData preset in externalDifficultySettings.Presets)
      {
        if (preset.Name == "Default")
        {
          SetPresetValues(preset);
          break;
        }
      }
      difficultySettings.Apply();
      SelectItem(0);
    }

    protected override bool OnResetGameAction(GameActionType type, bool down) => false;

    private bool IsCurrentPreset(DifficultyPresetData preset)
    {
      foreach (DifficultyPresetItemData difficultyPresetItemData in preset.Items)
      {
        if (difficultySettings.GetValue(difficultyPresetItemData.Name) != (double) difficultyPresetItemData.Value)
          return false;
      }
      return true;
    }

    private void OnAutoValueChange<T>(SettingsValueView<T> view)
    {
      view.ApplyVisibleValue();
      difficultySettings.Apply();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      UpdateViews();
      difficultySettings.OnApply += UpdateViews;
    }

    private void OnDeselect<T>(SettingsValueView<T> view)
    {
      if (selectedView != view)
        return;
      SelectView<SettingsValueView<float>>(null);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      difficultySettings.OnApply -= UpdateViews;
      SelectView<SettingsValueView<float>>(null);
      if (!(confirmationInstance != null))
        return;
      confirmationInstance.Hide();
    }

    private void OnOriginalExperienceValueChange(SettingsValueView<bool> view)
    {
      if (view.VisibleValue)
      {
        OnButtonReset();
      }
      else
      {
        SelectView<SettingsValueView<float>>(null);
        view.RevertVisibleValue();
        if (confirmationInstance == null)
          confirmationInstance = Instantiate(confirmationPrefab, transform, false);
        confirmationInstance.Show("{UI.Menu.Main.Settings.Difficulty.Confirmation}", DisableOriginalExperience, null);
      }
    }

    private void OnPresetValueChange(SettingsValueView<int> view)
    {
      List<DifficultyPresetData> presets = externalDifficultySettings.Presets;
      if (view.VisibleValue >= presets.Count)
        return;
      SetPresetValues(presets[view.VisibleValue]);
      difficultySettings.Apply();
    }

    private void OnSelect<T>(SettingsValueView<T> view)
    {
      SelectView(view);
    }

    private void SelectView<T>(T view) where T : MonoBehaviour, ISelectable
    {
      if (selectedView == (object) view)
        return;
      if (selectedView != null)
        selectedView.Selected = false;
      selectedView = view;
      if (view != null)
      {
        view.Selected = true;
        tooltipView.StringValue = "{UI.Menu.Main.Settings.Difficulty.Tooltip." + view.name + "}";
        tooltipView.gameObject.SetActive(true);
      }
      else
      {
        tooltipView.StringValue = null;
        tooltipView.gameObject.SetActive(false);
      }
    }

    private void SetPresetValues(DifficultyPresetData preset)
    {
      foreach (DifficultyPresetItemData difficultyPresetItemData in preset.Items)
      {
        IValue<float> valueItem = difficultySettings.GetValueItem(difficultyPresetItemData.Name);
        if (valueItem != null)
          valueItem.Value = difficultyPresetItemData.Value;
      }
    }

    private float ValueValidation(float value) => SettingsViewUtility.RoundValue(value, 0.1f);

    private void UpdateViews()
    {
      originalExperienceView.RevertVisibleValue();
      bool flag1 = !originalExperienceView.VisibleValue;
      presetsView.gameObject.SetActive(flag1);
      if (flag1)
      {
        List<DifficultyPresetData> presets = externalDifficultySettings.Presets;
        bool flag2 = true;
        for (int index = 0; index < presets.Count; ++index)
        {
          if (IsCurrentPreset(presets[index]))
          {
            presetsView.VisibleValue = index;
            flag2 = false;
            break;
          }
        }
        if (flag2)
          presetsView.VisibleValue = presets.Count;
      }
      foreach (GameObject headerView in headerViews)
        headerView.SetActive(flag1);
      foreach (FloatSettingsValueView floatView in floatViews)
      {
        floatView.gameObject.SetActive(flag1);
        if (flag1)
          floatView.RevertVisibleValue();
      }
    }
  }
}
