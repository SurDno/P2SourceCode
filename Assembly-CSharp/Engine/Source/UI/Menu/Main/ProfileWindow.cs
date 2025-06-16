using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using InputServices;

namespace Engine.Source.UI.Menu.Main
{
  public abstract class ProfileWindow : UIWindow
  {
    private CameraKindEnum lastCameraKind;
    [Header("Sounds")]
    [SerializeField]
    [FormerlySerializedAs("ClickSound")]
    private AudioClip clickSound;
    [SerializeField]
    private ProfileItem itemPrefab;
    [SerializeField]
    private RectTransform keyView;
    [SerializeField]
    private Button currentButton;
    [SerializeField]
    private Button deleteButton;
    private ProfileItem selection;
    private List<ProfileItem> items = new List<ProfileItem>();

    protected abstract void RegisterLayer();

    public override void Initialize()
    {
      RegisterLayer();
      Button[] componentsInChildren = this.GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((UnityAction<BaseEventData>) (eventData => Button_Click_Handler()));
        componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
      }
      base.Initialize();
    }

    private void Fill()
    {
      string text1 = ServiceLocator.GetService<LocalizationService>().GetText("{UI.CurrentProfile}");
      ProfilesService service = ServiceLocator.GetService<ProfilesService>();
      ProfileData current = service.Current;
      foreach (ProfileData profile in service.Profiles)
      {
        ProfileItem profileItem = UnityEngine.Object.Instantiate<ProfileItem>(itemPrefab);
        profileItem.transform.SetParent((Transform) keyView, false);
        ProfileItem component = profileItem.GetComponent<ProfileItem>();
        items.Add(component);
        string text2 = profile.Name + "  [" + ProfilesUtility.GetSaveCount(profile.Name) + "]  ";
        if (profile == current)
          text2 += text1;
        component.Name = profile.Name;
        component.SetText(text2);
        component.OnPressed += OnKeyItemPressed;
      }
      UpdateButtons();
    }

    private void Clear()
    {
      selection = null;
      foreach (ProfileItem profileItem in items)
      {
        profileItem.OnPressed -= OnKeyItemPressed;
        UnityEngine.Object.Destroy((UnityEngine.Object) profileItem.gameObject);
      }
      items.Clear();
    }

    private void OnKeyItemPressed(ProfileItem item)
    {
      if ((UnityEngine.Object) selection != (UnityEngine.Object) null)
        selection.Slection = false;
      selection = item;
      selection.Slection = true;
      UpdateButtons();
    }

    public void Button_Click_Handler()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.gameObject.GetComponent<AudioSource>().PlayOneShot(clickSound);
    }

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Current_Click_Handler()
    {
      if ((UnityEngine.Object) selection == (UnityEngine.Object) null)
        return;
      ServiceLocator.GetService<ProfilesService>().SetCurrent(selection.Name);
      Clear();
      Fill();
    }

    public void Button_Delete_Click_Handler()
    {
      if ((UnityEngine.Object) selection == (UnityEngine.Object) null)
        return;
      ServiceLocator.GetService<ProfilesService>().DeleteProfile(selection.Name);
      Clear();
      Fill();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      Fill();
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      Clear();
      base.OnDisable();
    }

    private void UpdateButtons()
    {
      if ((UnityEngine.Object) currentButton != (UnityEngine.Object) null)
        currentButton.interactable = (UnityEngine.Object) selection != (UnityEngine.Object) null;
      if (!((UnityEngine.Object) deleteButton != (UnityEngine.Object) null))
        return;
      deleteButton.interactable = (UnityEngine.Object) selection != (UnityEngine.Object) null;
    }
  }
}
