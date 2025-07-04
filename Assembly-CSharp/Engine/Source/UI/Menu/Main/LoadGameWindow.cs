﻿using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private List<SaveFileItem> items = [];

    protected abstract void RegisterLayer();

    public override void Initialize()
    {
      RegisterLayer();
      Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(eventData => Button_Click_Handler());
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
        SaveFileItem saveFileItem = Instantiate(itemPrefab);
        saveFileItem.transform.SetParent(keyView, false);
        SaveFileItem component = saveFileItem.GetComponent<SaveFileItem>();
        items.Add(component);
        component.SetText(text);
        component.File = saveName;
        component.OnPressed += OnKeyItemPressed;
      }
    }

    private void Clear()
    {
      foreach (SaveFileItem saveFileItem in items)
      {
        saveFileItem.OnPressed -= OnKeyItemPressed;
        Destroy(saveFileItem.gameObject);
      }
      items.Clear();
    }

    private void OnKeyItemPressed(SaveFileItem item)
    {
      ProfileData current = ServiceLocator.GetService<ProfilesService>().Current;
      if (current == null)
      {
        Debug.LogError("Profile not found");
      }
      else
      {
        string str = ProfilesUtility.SavePath(current.Name, item.File);
        if (!ProfilesUtility.IsSaveExist(str))
          Debug.LogError("Save name not found : " + item.File);
        else
          CoroutineService.Instance.Route(LoadGameUtility.RestartGameWithSave(str));
      }
    }

    public void Button_Click_Handler()
    {
      if (!gameObject.activeInHierarchy)
        return;
      gameObject.GetComponent<AudioSource>().PlayOneShot(clickSound);
    }

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    protected override void OnEnable()
    {
      base.OnEnable();
      Fill();
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
      Clear();
      base.OnDisable();
    }
  }
}
