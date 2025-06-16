// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.BackerUnlocksView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace Engine.Impl.UI.Menu.Main
{
  public class BackerUnlocksView : SettingsView
  {
    [SerializeField]
    private SelectableSettingsItemView selectableViewPrefab;
    [SerializeField]
    private MenuInputField inputFieldPrefab;
    private BackerUnlocksService backerUnlocksService;
    private MenuInputField inputField;
    private List<SelectableSettingsItemView> items = new List<SelectableSettingsItemView>();
    [SerializeField]
    private GameObject xboxKeyboard;
    private bool isBoxKeyboardEnabled = false;

    protected override void Awake()
    {
      this.backerUnlocksService = ServiceLocator.GetService<BackerUnlocksService>();
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.inputField = UnityEngine.Object.Instantiate<MenuInputField>(this.inputFieldPrefab, (Transform) this.layout.Content, false);
      this.inputField.SetPlaceholder("{UI.Menu.Main.Settings.BackerUnlocks.EnterCode}");
      this.inputField.SendEvent += new Action<string>(this.OnInputEnded);
      base.Awake();
    }

    private bool OpenXboxInputField(GameActionType type, bool down)
    {
      EventSystem.current.SetSelectedGameObject(this.inputField.gameObject);
      return false;
    }

    public void Clear()
    {
      for (int index = 0; index < this.items.Count; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.items[index].gameObject);
      this.items.Clear();
    }

    public void Fill()
    {
      if (this.backerUnlocksService.ItemUnlocked)
      {
        SelectableSettingsItemView settingsItemView = UnityEngine.Object.Instantiate<SelectableSettingsItemView>(this.selectableViewPrefab, (Transform) this.layout.Content, false);
        this.items.Add(settingsItemView);
        settingsItemView.SetName("{UI.Menu.Main.Settings.BackerUnlocks.Item}");
      }
      if (this.backerUnlocksService.QuestUnlocked)
      {
        SelectableSettingsItemView settingsItemView = UnityEngine.Object.Instantiate<SelectableSettingsItemView>(this.selectableViewPrefab, (Transform) this.layout.Content, false);
        this.items.Add(settingsItemView);
        settingsItemView.SetName("{UI.Menu.Main.Settings.BackerUnlocks.Quest}");
      }
      if (!this.backerUnlocksService.PolyhedralRoomUnlocked)
        return;
      SelectableSettingsItemView settingsItemView1 = UnityEngine.Object.Instantiate<SelectableSettingsItemView>(this.selectableViewPrefab, (Transform) this.layout.Content, false);
      this.items.Add(settingsItemView1);
      settingsItemView1.SetName("{UI.Menu.Main.Settings.BackerUnlocks.PolyhedralRoom}");
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.OpenXboxInputField));
      this.backerUnlocksService.AnyChangeEvent -= new Action(this.OnUnlocksChange);
      this.Clear();
      this.isBoxKeyboardEnabled = false;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.Fill();
      this.backerUnlocksService.AnyChangeEvent += new Action(this.OnUnlocksChange);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.OpenXboxInputField));
      this.xboxKeyboard.SetActive(false);
    }

    public void OnInputEnded(string value)
    {
      this.inputField.SetMessage("{UI.Menu.Main.Settings.BackerUnlocks." + (object) this.backerUnlocksService.TryAddCode(value) + "}");
    }

    public void OnUnlocksChange()
    {
      this.Clear();
      this.Fill();
    }
  }
}
