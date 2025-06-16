// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.LoadGameWindow
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
  public abstract class LoadGameWindow : UIWindow
  {
    private CameraKindEnum lastCameraKind;
    [Header("Sounds")]
    [SerializeField]
    [FormerlySerializedAs("ClickSound")]
    private AudioClip clickSound;
    [SerializeField]
    private SaveFileItem itemPrefab;
    [SerializeField]
    private RectTransform keyView;
    private List<SaveFileItem> items = new List<SaveFileItem>();

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
      ProfileData current = ServiceLocator.GetService<ProfilesService>().Current;
      if (current == null)
        return;
      List<string> saveNames = ProfilesUtility.GetSaveNames(current.Name);
      string lastSave = current.LastSave;
      foreach (string saveName in saveNames)
      {
        string text = ProfilesUtility.ConvertSaveName(saveName);
        if (saveName == lastSave)
          text = text + "  " + ServiceLocator.GetService<LocalizationService>().GetText("{UI.CurrentSave}");
        SaveFileItem saveFileItem = UnityEngine.Object.Instantiate<SaveFileItem>(this.itemPrefab);
        saveFileItem.transform.SetParent((Transform) this.keyView, false);
        SaveFileItem component = saveFileItem.GetComponent<SaveFileItem>();
        this.items.Add(component);
        component.SetText(text);
        component.File = saveName;
        component.OnPressed += new Action<SaveFileItem>(this.OnKeyItemPressed);
      }
    }

    private void Clear()
    {
      foreach (SaveFileItem saveFileItem in this.items)
      {
        saveFileItem.OnPressed -= new Action<SaveFileItem>(this.OnKeyItemPressed);
        UnityEngine.Object.Destroy((UnityEngine.Object) saveFileItem.gameObject);
      }
      this.items.Clear();
    }

    private void OnKeyItemPressed(SaveFileItem item)
    {
      ProfileData current = ServiceLocator.GetService<ProfilesService>().Current;
      if (current == null)
      {
        Debug.LogError((object) "Profile not found");
      }
      else
      {
        string str = ProfilesUtility.SavePath(current.Name, item.File);
        if (!ProfilesUtility.IsSaveExist(str))
          Debug.LogError((object) ("Save name not found : " + item.File));
        else
          CoroutineService.Instance.Route(LoadGameUtility.RestartGameWithSave(str));
      }
    }

    public void Button_Click_Handler()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
    }

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

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
  }
}
