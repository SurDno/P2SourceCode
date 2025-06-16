// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.LoadWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using InputServices;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Main
{
  public class LoadWindow : MonoBehaviour
  {
    [SerializeField]
    private SpriteView spriteView;
    [SerializeField]
    private StringView tooltipTagView;
    [SerializeField]
    private HideableView defaultSprite;
    [SerializeField]
    private LoadWindowGameData gameDataSettings;
    [SerializeField]
    private Slider progress;
    [SerializeField]
    private GameObject loadingSpinner;
    private int gameDay = 0;
    private LoadWindowMode mode = LoadWindowMode.Initial;
    private string bufferedTooltip = (string) null;

    public static LoadWindow Instance { get; private set; }

    public int GameDay
    {
      get => this.gameDay;
      set
      {
        if (this.gameDay == value)
          return;
        this.gameDay = value;
        this.Invalidate();
      }
    }

    public LoadWindowMode Mode
    {
      get => this.mode;
      set
      {
        if (this.mode == value)
          return;
        this.mode = value;
        this.Invalidate();
      }
    }

    public bool Show
    {
      get => this.gameObject.activeSelf;
      set => this.gameObject.SetActive(value);
    }

    public bool ShowProgress
    {
      get => this.progress.gameObject.activeSelf;
      set => this.progress.gameObject.SetActive(value);
    }

    public float Progress
    {
      get => this.progress.value;
      set => this.progress.value = value;
    }

    private void Awake() => LoadWindow.Instance = this;

    private void Build()
    {
      this.defaultSprite.Visible = this.mode == LoadWindowMode.Initial;
      Sprite sprite1 = (Sprite) null;
      string str = (string) null;
      if (this.mode == LoadWindowMode.StartGameData || this.mode == LoadWindowMode.LoadSavedGame)
      {
        LoadWindowGameDataItem windowGameDataItem = this.gameDataSettings.GetItem(InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().Name);
        if (!windowGameDataItem.IsNull)
        {
          UnitySubAsset<Sprite> imageInformation;
          if (this.mode == LoadWindowMode.LoadSavedGame)
          {
            IProfilesService service = ServiceLocator.GetService<IProfilesService>();
            int intValue = service != null ? service.GetIntValue("Deaths") : 0;
            if ((UnityEngine.Object) windowGameDataItem.LoadStorables != (UnityEngine.Object) null && InstanceByRequest<EngineApplication>.Instance.IsInitialized)
            {
              IInventoryPlaceholderSerializable placeholderSerializable = this.Evaluate<IInventoryPlaceholderSerializable>((LoadWindowDataItem<IInventoryPlaceholderSerializable>[]) windowGameDataItem.LoadStorables.Items, this.GameDay, intValue);
              if (placeholderSerializable != new IInventoryPlaceholderSerializable())
              {
                Sprite sprite2;
                if (!(placeholderSerializable.Value is InventoryPlaceholder inventoryPlaceholder))
                {
                  sprite2 = (Sprite) null;
                }
                else
                {
                  imageInformation = inventoryPlaceholder.ImageInformation;
                  sprite2 = imageInformation.Value;
                }
                sprite1 = sprite2;
              }
            }
            if ((UnityEngine.Object) windowGameDataItem.LoadTooltips != (UnityEngine.Object) null)
              str = this.Evaluate<string>((LoadWindowDataItem<string>[]) windowGameDataItem.LoadTooltips.Items, this.GameDay, intValue);
          }
          if ((UnityEngine.Object) sprite1 == (UnityEngine.Object) null)
          {
            Sprite sprite3;
            if (!(windowGameDataItem.StartStorable.Value is InventoryPlaceholder inventoryPlaceholder))
            {
              sprite3 = (Sprite) null;
            }
            else
            {
              imageInformation = inventoryPlaceholder.ImageInformation;
              sprite3 = imageInformation.Value;
            }
            sprite1 = sprite3;
          }
          if (str == null)
            str = windowGameDataItem.StartTooltip;
        }
      }
      this.spriteView.SetValue(sprite1, true);
      this.tooltipTagView.StringValue = str;
      this.bufferedTooltip = str;
    }

    private void Clear()
    {
      this.spriteView.SetValue((Sprite) null, true);
      this.tooltipTagView.StringValue = (string) null;
      this.bufferedTooltip = (string) null;
    }

    private bool Evaluate<T>(LoadWindowDataItem<T> item, int gameDay, int deathCount)
    {
      return (!item.LimitGameDay || gameDay >= item.MinGameDay && gameDay <= item.MaxGameDay) && (!item.LimitDeathCount || deathCount >= item.MinDeathCount && deathCount <= item.MaxDeathCount);
    }

    private T Evaluate<T>(LoadWindowDataItem<T>[] items, int gameDay, int deathCount)
    {
      int max = 0;
      for (int index = 0; index < items.Length; ++index)
      {
        LoadWindowDataItem<T> loadWindowDataItem = items[index];
        if (this.Evaluate<T>(loadWindowDataItem, gameDay, deathCount))
          max += loadWindowDataItem.Weight;
      }
      if (max == 0)
        return default (T);
      int num1 = UnityEngine.Random.Range(0, max);
      int num2 = 0;
      for (int index = 0; index < items.Length; ++index)
      {
        LoadWindowDataItem<T> loadWindowDataItem = items[index];
        if (this.Evaluate<T>(loadWindowDataItem, gameDay, deathCount))
        {
          num2 += loadWindowDataItem.Weight;
          if (num1 < num2)
            return loadWindowDataItem.Value;
        }
      }
      return default (T);
    }

    private void Invalidate()
    {
      if (!this.isActiveAndEnabled)
        return;
      this.Build();
    }

    private void OnDisable()
    {
      this.Clear();
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    }

    private void OnEnable()
    {
      this.loadingSpinner.SetActive(true);
      this.Build();
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    }

    private void OnJoystick(bool joystick)
    {
      if (this.bufferedTooltip == null)
        return;
      this.tooltipTagView.StringValue = (string) null;
      this.tooltipTagView.StringValue = this.bufferedTooltip;
    }
  }
}
