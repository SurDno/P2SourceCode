// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.ProfileWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
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
      this.RegisterLayer();
      Button[] componentsInChildren = this.GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((UnityAction<BaseEventData>) (eventData => this.Button_Click_Handler()));
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
        ProfileItem profileItem = UnityEngine.Object.Instantiate<ProfileItem>(this.itemPrefab);
        profileItem.transform.SetParent((Transform) this.keyView, false);
        ProfileItem component = profileItem.GetComponent<ProfileItem>();
        this.items.Add(component);
        string text2 = profile.Name + "  [" + (object) ProfilesUtility.GetSaveCount(profile.Name) + "]  ";
        if (profile == current)
          text2 += text1;
        component.Name = profile.Name;
        component.SetText(text2);
        component.OnPressed += new Action<ProfileItem>(this.OnKeyItemPressed);
      }
      this.UpdateButtons();
    }

    private void Clear()
    {
      this.selection = (ProfileItem) null;
      foreach (ProfileItem profileItem in this.items)
      {
        profileItem.OnPressed -= new Action<ProfileItem>(this.OnKeyItemPressed);
        UnityEngine.Object.Destroy((UnityEngine.Object) profileItem.gameObject);
      }
      this.items.Clear();
    }

    private void OnKeyItemPressed(ProfileItem item)
    {
      if ((UnityEngine.Object) this.selection != (UnityEngine.Object) null)
        this.selection.Slection = false;
      this.selection = item;
      this.selection.Slection = true;
      this.UpdateButtons();
    }

    public void Button_Click_Handler()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
    }

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Current_Click_Handler()
    {
      if ((UnityEngine.Object) this.selection == (UnityEngine.Object) null)
        return;
      ServiceLocator.GetService<ProfilesService>().SetCurrent(this.selection.Name);
      this.Clear();
      this.Fill();
    }

    public void Button_Delete_Click_Handler()
    {
      if ((UnityEngine.Object) this.selection == (UnityEngine.Object) null)
        return;
      ServiceLocator.GetService<ProfilesService>().DeleteProfile(this.selection.Name);
      this.Clear();
      this.Fill();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.Fill();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.Clear();
      base.OnDisable();
    }

    private void UpdateButtons()
    {
      if ((UnityEngine.Object) this.currentButton != (UnityEngine.Object) null)
        this.currentButton.interactable = (UnityEngine.Object) this.selection != (UnityEngine.Object) null;
      if (!((UnityEngine.Object) this.deleteButton != (UnityEngine.Object) null))
        return;
      this.deleteButton.interactable = (UnityEngine.Object) this.selection != (UnityEngine.Object) null;
    }
  }
}
