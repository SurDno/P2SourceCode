// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.ProfileMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using Engine.Source.UI;
using InputServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Main
{
  public class ProfileMenu : MonoBehaviour
  {
    [SerializeField]
    private LayoutContainer listLayoutPrefab;
    [SerializeField]
    private SelectableSettingsItemView selectableViewPrefab;
    [SerializeField]
    private Button loadButton;
    [SerializeField]
    private Button deleteButton;
    [SerializeField]
    private HideableView hasSelectedView;
    [SerializeField]
    private StringView selectedFileView;
    [SerializeField]
    private ConfirmationWindow confirmationPrefab;
    [SerializeField]
    private List<ProfileData> profiles;
    [SerializeField]
    private GameObject removeTipObject;
    private ScrollRect scroll;
    private ProfilesService profilesService;
    private LayoutContainer layout;
    private List<SelectableSettingsItemView> items = new List<SelectableSettingsItemView>();
    private SelectableSettingsItemView selectedItem = (SelectableSettingsItemView) null;
    private ConfirmationWindow confirmationInstance;
    private int currentSelected = 0;
    private ControlSwitcher _controlSwitcher = new ControlSwitcher();
    private bool canLoad;
    private bool canDelete;
    private Coroutine scrollCoroutine;

    private void Awake()
    {
      this.profilesService = ServiceLocator.GetService<ProfilesService>();
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.scroll = this.layout.transform.GetChild(0).GetComponent<ScrollRect>();
    }

    private void Subscribe()
    {
      this._controlSwitcher.SubmitAction(this.loadButton, GameActionType.Submit, new Action(this.LoadSelected));
      this._controlSwitcher.SubmitAction(this.deleteButton, GameActionType.Split, new Action(this.TryDeleteSelected));
    }

    private bool SelectPrevious(GameActionType type, bool down)
    {
      if (down)
      {
        this.SelectItem(this.items[this.currentSelected > 0 ? this.currentSelected - 1 : this.items.Count - 1]);
        this.scrollCoroutine = this.StartCoroutine(this.ScrollCoroutine(true));
      }
      else if (this.scrollCoroutine != null)
        this.StopCoroutine(this.scrollCoroutine);
      return true;
    }

    private bool SelectNext(GameActionType type, bool down)
    {
      if (down)
      {
        this.SelectItem(this.items[this.currentSelected < this.items.Count - 1 ? this.currentSelected + 1 : 0]);
        this.scrollCoroutine = this.StartCoroutine(this.ScrollCoroutine(false));
      }
      else if (this.scrollCoroutine != null)
        this.StopCoroutine(this.scrollCoroutine);
      return true;
    }

    private IEnumerator ScrollCoroutine(bool isUp)
    {
      yield return (object) new WaitForSeconds(0.5f);
      while (true)
      {
        int sellected = !isUp ? (this.currentSelected < this.items.Count - 1 ? this.currentSelected + 1 : 0) : (this.currentSelected > 0 ? this.currentSelected - 1 : this.items.Count - 1);
        this.SelectItem(this.items[sellected]);
        yield return (object) new WaitForSeconds(0.05f);
      }
    }

    private bool LoadSelected(GameActionType type, bool down)
    {
      if (down && InputService.Instance.JoystickUsed)
        this.LoadSelected();
      return true;
    }

    private void TryDeleteSelected()
    {
      if (!this.canDelete)
        return;
      this.ShowConfirmation("{UI.Menu.Main.Profile.DeleteConfirmation}", new Action(this.DeleteSelected));
    }

    private void DeleteSelected()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, new GameActionHandle(this.SelectPrevious), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, new GameActionHandle(this.SelectNext), true);
      if (!(bool) (UnityEngine.Object) this.selectedItem || !this.canDelete)
        return;
      int selectedIndex = 0;
      for (int index = 0; index < this.items.Count; ++index)
      {
        if ((UnityEngine.Object) this.items[index] == (UnityEngine.Object) this.selectedItem)
        {
          selectedIndex = index;
          break;
        }
      }
      this.profilesService.DeleteProfile(this.selectedItem.name);
      this.Clear();
      this.Fill();
      CoroutineService.Instance.WaitFrame(1, (Action) (() =>
      {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        if (this.items.Count <= 0)
          return;
        selectedIndex = Mathf.Min(selectedIndex, this.items.Count - 1);
        this.SelectItem(this.items[selectedIndex]);
      }));
    }

    public void Clear()
    {
      this.SelectItem((SelectableSettingsItemView) null);
      for (int index = 0; index < this.items.Count; ++index)
      {
        this.items[index].GetComponent<SelectableSettingsItemView>().ClickEvent -= new Action<SelectableSettingsItemView>(this.OnClickItem);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.items[index].gameObject);
      }
      this.items.Clear();
    }

    public void Fill()
    {
      ProfileData current = this.profilesService.Current;
      SelectableSettingsItemView settingsItemView1 = (SelectableSettingsItemView) null;
      foreach (ProfileData profile in this.profilesService.Profiles)
        this.profiles.Add(profile);
      for (int index = this.profiles.Count - 1; index >= 0; --index)
      {
        ProfileData profile = this.profiles[index];
        SelectableSettingsItemView settingsItemView2 = UnityEngine.Object.Instantiate<SelectableSettingsItemView>(this.selectableViewPrefab, (Transform) this.layout.Content, false);
        this.items.Add(settingsItemView2);
        string str = ProfilesUtility.ConvertProfileName(profile.Name, "{UI.Menu.Main.Profile.LongFormat}");
        if (profile == current)
        {
          str = str + " " + ServiceLocator.GetService<LocalizationService>().GetText("{UI.CurrentProfile}");
          settingsItemView1 = settingsItemView2;
        }
        settingsItemView2.SetName(str);
        settingsItemView2.ClickEvent += new Action<SelectableSettingsItemView>(this.OnClickItem);
        settingsItemView2.name = profile.Name;
        string lastSave = ProfilesUtility.GetLastSave(profile.Name);
        if (lastSave != "")
        {
          DateTime saveCreationTime = ProfilesUtility.GetSaveCreationTime(profile.Name, lastSave);
          if (saveCreationTime != DateTime.MinValue)
            settingsItemView2.SetValue(ProfilesUtility.ConvertCreationTime(saveCreationTime, "{SaveDateTimeFormat}"));
          else
            settingsItemView2.SetValue("");
        }
        else
          settingsItemView2.SetValue("");
      }
      this.profiles.Clear();
      this.SelectItem(settingsItemView1);
    }

    private void LoadSelected()
    {
      if (!(bool) (UnityEngine.Object) this.selectedItem || !this.canLoad)
        return;
      this.profilesService.SetCurrent(this.selectedItem.name);
      ServiceLocator.GetService<UIService>().Swap<IStartLoadGameWindow>();
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) this.confirmationInstance != (UnityEngine.Object) null)
        this.confirmationInstance.Hide();
      this.Clear();
      this.profiles = (List<ProfileData>) null;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.SelectPrevious));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.SelectNext));
      this._controlSwitcher.Dispose();
    }

    private void OnEnable()
    {
      this.profiles = new List<ProfileData>();
      this.Fill();
      this.Subscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, new GameActionHandle(this.SelectPrevious));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, new GameActionHandle(this.SelectNext));
    }

    private void OnClickItem(SelectableSettingsItemView item)
    {
      if ((UnityEngine.Object) item == (UnityEngine.Object) this.selectedItem)
        this.LoadSelected();
      else
        this.SelectItem(item);
    }

    private void SelectItem(SelectableSettingsItemView item)
    {
      if ((UnityEngine.Object) item == (UnityEngine.Object) this.selectedItem)
        return;
      if ((bool) (UnityEngine.Object) this.selectedItem)
        this.selectedItem.Selected = false;
      this.selectedItem = item;
      SelectableSettingsItemView selectedItem = this.selectedItem;
      bool flag = false;
      if ((bool) (UnityEngine.Object) selectedItem)
      {
        this.selectedItem.Selected = true;
        this.selectedFileView.StringValue = this.selectedItem.name;
        flag = this.profilesService.Current.Name == this.selectedItem.name;
      }
      this.loadButton.interactable = (bool) (UnityEngine.Object) selectedItem;
      this.canLoad = (bool) (UnityEngine.Object) selectedItem;
      this.deleteButton.interactable = (bool) (UnityEngine.Object) selectedItem && !flag;
      this.canDelete = (bool) (UnityEngine.Object) selectedItem && !flag;
      this.removeTipObject.SetActive(this.canDelete);
      this.hasSelectedView.Visible = (bool) (UnityEngine.Object) selectedItem;
      this.currentSelected = this.items.IndexOf(item);
      this.FillForSelected(this.currentSelected);
    }

    private void ShowConfirmation(string text, Action onAccept)
    {
      if ((UnityEngine.Object) this.confirmationInstance == (UnityEngine.Object) null)
        this.confirmationInstance = UnityEngine.Object.Instantiate<ConfirmationWindow>(this.confirmationPrefab, this.transform, false);
      this.confirmationInstance.Show(text, onAccept, (Action) (() =>
      {
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, new GameActionHandle(this.SelectPrevious), true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, new GameActionHandle(this.SelectNext), true);
      }));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.SelectPrevious));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.SelectNext));
    }

    public void FillForSelected(int index)
    {
      float height = this.layout.Content.parent.parent.GetComponent<RectTransform>().rect.height;
      float num1 = this.items[0].GetComponent<RectTransform>().rect.height + 1f;
      float num2 = (float) (index + 2) * num1;
      RectTransform component = this.layout.Content.parent.GetComponent<RectTransform>();
      Vector2 anchoredPosition = component.anchoredPosition;
      if ((double) num2 - (double) anchoredPosition.y > (double) height)
        anchoredPosition.y = (double) num2 + (double) anchoredPosition.y > (double) height ? num2 - height : 0.0f;
      else if ((double) num2 - (double) num1 * 2.0 - (double) anchoredPosition.y < 0.0)
        anchoredPosition.y = num2 - num1 * 2f;
      component.anchoredPosition = anchoredPosition;
    }
  }
}
