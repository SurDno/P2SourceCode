using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using Engine.Source.UI.Menu.Main;
using InputServices;

namespace Engine.Impl.UI.Menu.Main
{
  public class LoadGameMenu : MonoBehaviour
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
    private EventView viewChangeProfile;
    [SerializeField]
    private EventView viewBack;
    [SerializeField]
    private GameObject removeTipObject;
    [SerializeField]
    private GameObject back;
    [SerializeField]
    private GameObject changeProfile;
    private ScrollRect scroll;
    private ProfilesService profilesService;
    private LayoutContainer layout;
    private List<SelectableSettingsItemView> items = new List<SelectableSettingsItemView>();
    private SelectableSettingsItemView selectedItem;
    private ConfirmationWindow confirmationInstance;
    private int currentSelected;
    private ControlSwitcher _controlSwitcher = new ControlSwitcher();
    private bool canLoad;
    private bool canDelete;
    private bool canChangeProfile;
    private Coroutine scrollCoroutine;

    private void Awake()
    {
      profilesService = ServiceLocator.GetService<ProfilesService>();
      layout = UnityEngine.Object.Instantiate<LayoutContainer>(listLayoutPrefab, this.transform, false);
      scroll = layout.transform.GetChild(0).GetComponent<ScrollRect>();
    }

    private void Subscribe()
    {
      _controlSwitcher.SubmitAction(loadButton, GameActionType.Submit, TryLoadSelected);
      _controlSwitcher.SubmitAction(deleteButton, GameActionType.Split, TryDeleteSelected);
    }

    private bool SelectPrevious(GameActionType type, bool down)
    {
      if (down)
      {
        if (scrollCoroutine != null)
          this.StopCoroutine(scrollCoroutine);
        SelectItem(items[currentSelected > 0 ? currentSelected - 1 : items.Count - 1]);
        scrollCoroutine = this.StartCoroutine(ScrollCoroutine(true));
      }
      else if (scrollCoroutine != null)
        this.StopCoroutine(scrollCoroutine);
      return true;
    }

    private bool SelectNext(GameActionType type, bool down)
    {
      if (down)
      {
        if (scrollCoroutine != null)
          this.StopCoroutine(scrollCoroutine);
        SelectItem(items[currentSelected < items.Count - 1 ? currentSelected + 1 : 0]);
        scrollCoroutine = this.StartCoroutine(ScrollCoroutine(false));
      }
      else if (scrollCoroutine != null)
        this.StopCoroutine(scrollCoroutine);
      return true;
    }

    private IEnumerator ScrollCoroutine(bool isUp)
    {
      yield return (object) new WaitForSeconds(0.5f);
      while (true)
      {
        int sellected = !isUp ? (currentSelected < items.Count - 1 ? currentSelected + 1 : 0) : (currentSelected > 0 ? currentSelected - 1 : items.Count - 1);
        SelectItem(items[sellected]);
        yield return (object) new WaitForSeconds(0.05f);
      }
    }

    private bool LoadSelected(GameActionType type, bool down)
    {
      if (down && InputService.Instance.JoystickUsed)
        LoadSelected();
      return true;
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) confirmationInstance != (UnityEngine.Object) null)
        confirmationInstance.Hide();
      Clear();
      _controlSwitcher.Dispose();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, SelectPrevious);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, SelectNext);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, Back);
    }

    private bool Back(GameActionType type, bool down)
    {
      if (!down)
        return false;
      if (canChangeProfile)
        viewChangeProfile.Invoke();
      else
        viewBack.Invoke();
      return true;
    }

    private void OnEnable()
    {
      Fill();
      Subscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, Back, true);
      canChangeProfile = !GameLoadGameWindow.IsActive;
      changeProfile.SetActive(canChangeProfile);
      back.SetActive(!canChangeProfile);
    }

    public void Fill()
    {
      ProfileData current = profilesService.Current;
      if (current == null)
        return;
      List<string> saveNames = ProfilesUtility.GetSaveNames(current.Name);
      string lastSave = current.LastSave;
      SelectableSettingsItemView settingsItemView1 = null;
      foreach (string str1 in saveNames)
      {
        string str2 = ProfilesUtility.ConvertSaveName(str1);
        SelectableSettingsItemView settingsItemView2 = UnityEngine.Object.Instantiate<SelectableSettingsItemView>(selectableViewPrefab, (Transform) layout.Content, false);
        items.Add(settingsItemView2);
        if (str1 == lastSave)
        {
          str2 = str2 + " " + ServiceLocator.GetService<LocalizationService>().GetText("{UI.CurrentSave}");
          settingsItemView1 = settingsItemView2;
        }
        settingsItemView2.SetName(str2);
        settingsItemView2.ClickEvent += OnClickItem;
        settingsItemView2.name = str1;
        DateTime saveCreationTime = ProfilesUtility.GetSaveCreationTime(current.Name, str1);
        settingsItemView2.SetValue(ProfilesUtility.ConvertCreationTime(saveCreationTime, "{SaveDateTimeFormat}"));
      }
      SelectItem(settingsItemView1);
    }

    public void Clear()
    {
      SelectItem(null);
      for (int index = 0; index < items.Count; ++index)
      {
        items[index].GetComponent<SelectableSettingsItemView>().ClickEvent -= OnClickItem;
        UnityEngine.Object.Destroy((UnityEngine.Object) items[index].gameObject);
      }
      items.Clear();
    }

    private void TryLoadSelected()
    {
      if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
        ShowConfirmation("{UI.Menu.Main.Save.LoadConfirmation}", LoadSelected);
      else
        LoadSelected();
    }

    private void LoadSelected()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext, true);
      if (!(bool) (UnityEngine.Object) selectedItem || !canLoad)
        return;
      ProfileData current = profilesService.Current;
      if (current == null)
      {
        Debug.LogError((object) "Profile not found");
      }
      else
      {
        string name = selectedItem.name;
        string str = ProfilesUtility.SavePath(current.Name, name);
        if (!ProfilesUtility.IsSaveExist(str))
          Debug.LogError((object) ("Save name not found : " + name));
        else
          CoroutineService.Instance.Route(LoadGameUtility.RestartGameWithSave(str));
      }
    }

    private void TryDeleteSelected()
    {
      if (!canDelete)
        return;
      ShowConfirmation("{UI.Menu.Main.Save.DeleteConfirmation}", DeleteSelected);
    }

    private void DeleteSelected()
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext, true);
      if (!(bool) (UnityEngine.Object) selectedItem || !canDelete)
        return;
      int selectedIndex = 0;
      for (int index = 0; index < items.Count; ++index)
      {
        if ((UnityEngine.Object) items[index] == (UnityEngine.Object) selectedItem)
        {
          selectedIndex = index;
          break;
        }
      }
      profilesService.DeleteSave(selectedItem.name);
      Clear();
      Fill();
      CoroutineService.Instance.WaitFrame(1, (Action) (() =>
      {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        if (items.Count <= 0)
          return;
        selectedIndex = Mathf.Min(selectedIndex, items.Count - 1);
        SelectItem(items[selectedIndex]);
      }));
    }

    private void OnClickItem(SelectableSettingsItemView item)
    {
      if ((UnityEngine.Object) item == (UnityEngine.Object) selectedItem)
        TryLoadSelected();
      else
        SelectItem(item);
    }

    private void SelectItem(SelectableSettingsItemView item)
    {
      if ((UnityEngine.Object) item == (UnityEngine.Object) this.selectedItem)
        return;
      if ((bool) (UnityEngine.Object) this.selectedItem)
        this.selectedItem.Selected = false;
      this.selectedItem = item;
      SelectableSettingsItemView selectedItem = this.selectedItem;
      if ((bool) (UnityEngine.Object) selectedItem)
      {
        this.selectedItem.Selected = true;
        selectedFileView.StringValue = this.selectedItem.name;
      }
      loadButton.interactable = (bool) (UnityEngine.Object) selectedItem;
      deleteButton.interactable = (bool) (UnityEngine.Object) selectedItem;
      canLoad = (bool) (UnityEngine.Object) selectedItem;
      canDelete = (bool) (UnityEngine.Object) selectedItem;
      removeTipObject.SetActive(canDelete);
      hasSelectedView.Visible = (bool) (UnityEngine.Object) selectedItem;
      currentSelected = items.IndexOf(item);
      FillForSelected(currentSelected);
    }

    public void FillForSelected(int index)
    {
      float height = layout.Content.parent.parent.GetComponent<RectTransform>().rect.height;
      float num1 = items[0].GetComponent<RectTransform>().rect.height + 1f;
      float num2 = (index + 2) * num1;
      RectTransform component = layout.Content.parent.GetComponent<RectTransform>();
      Vector2 anchoredPosition = component.anchoredPosition;
      if (num2 - (double) anchoredPosition.y > height)
        anchoredPosition.y = num2 + (double) anchoredPosition.y > height ? num2 - height : 0.0f;
      else if (num2 - num1 * 2.0 - (double) anchoredPosition.y < 0.0)
        anchoredPosition.y = num2 - num1 * 2f;
      component.anchoredPosition = anchoredPosition;
    }

    private void ShowConfirmation(string text, Action onAccept)
    {
      if ((UnityEngine.Object) confirmationInstance == (UnityEngine.Object) null)
        confirmationInstance = UnityEngine.Object.Instantiate<ConfirmationWindow>(confirmationPrefab, this.transform, false);
      confirmationInstance.Show(text, onAccept, (Action) (() =>
      {
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious, true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext, true);
      }));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, SelectPrevious);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, SelectNext);
    }
  }
}
