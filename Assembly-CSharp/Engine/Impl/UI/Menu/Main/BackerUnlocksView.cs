using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private List<SelectableSettingsItemView> items = [];
    [SerializeField]
    private GameObject xboxKeyboard;
    private bool isBoxKeyboardEnabled;

    protected override void Awake()
    {
      backerUnlocksService = ServiceLocator.GetService<BackerUnlocksService>();
      layout = Instantiate(listLayoutPrefab, transform, false);
      inputField = Instantiate(inputFieldPrefab, layout.Content, false);
      inputField.SetPlaceholder("{UI.Menu.Main.Settings.BackerUnlocks.EnterCode}");
      inputField.SendEvent += OnInputEnded;
      base.Awake();
    }

    private bool OpenXboxInputField(GameActionType type, bool down)
    {
      EventSystem.current.SetSelectedGameObject(inputField.gameObject);
      return false;
    }

    public void Clear()
    {
      for (int index = 0; index < items.Count; ++index)
        Destroy(items[index].gameObject);
      items.Clear();
    }

    public void Fill()
    {
      if (backerUnlocksService.ItemUnlocked)
      {
        SelectableSettingsItemView settingsItemView = Instantiate(selectableViewPrefab, layout.Content, false);
        items.Add(settingsItemView);
        settingsItemView.SetName("{UI.Menu.Main.Settings.BackerUnlocks.Item}");
      }
      if (backerUnlocksService.QuestUnlocked)
      {
        SelectableSettingsItemView settingsItemView = Instantiate(selectableViewPrefab, layout.Content, false);
        items.Add(settingsItemView);
        settingsItemView.SetName("{UI.Menu.Main.Settings.BackerUnlocks.Quest}");
      }
      if (!backerUnlocksService.PolyhedralRoomUnlocked)
        return;
      SelectableSettingsItemView settingsItemView1 = Instantiate(selectableViewPrefab, layout.Content, false);
      items.Add(settingsItemView1);
      settingsItemView1.SetName("{UI.Menu.Main.Settings.BackerUnlocks.PolyhedralRoom}");
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, OpenXboxInputField);
      backerUnlocksService.AnyChangeEvent -= OnUnlocksChange;
      Clear();
      isBoxKeyboardEnabled = false;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      Fill();
      backerUnlocksService.AnyChangeEvent += OnUnlocksChange;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, OpenXboxInputField);
      xboxKeyboard.SetActive(false);
    }

    public void OnInputEnded(string value)
    {
      inputField.SetMessage("{UI.Menu.Main.Settings.BackerUnlocks." + backerUnlocksService.TryAddCode(value) + "}");
    }

    public void OnUnlocksChange()
    {
      Clear();
      Fill();
    }
  }
}
