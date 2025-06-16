using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using InputServices;

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
    private int gameDay;
    private LoadWindowMode mode = LoadWindowMode.Initial;
    private string bufferedTooltip;

    public static LoadWindow Instance { get; private set; }

    public int GameDay
    {
      get => gameDay;
      set
      {
        if (gameDay == value)
          return;
        gameDay = value;
        Invalidate();
      }
    }

    public LoadWindowMode Mode
    {
      get => mode;
      set
      {
        if (mode == value)
          return;
        mode = value;
        Invalidate();
      }
    }

    public bool Show
    {
      get => this.gameObject.activeSelf;
      set => this.gameObject.SetActive(value);
    }

    public bool ShowProgress
    {
      get => progress.gameObject.activeSelf;
      set => progress.gameObject.SetActive(value);
    }

    public float Progress
    {
      get => progress.value;
      set => progress.value = value;
    }

    private void Awake() => Instance = this;

    private void Build()
    {
      defaultSprite.Visible = mode == LoadWindowMode.Initial;
      Sprite sprite1 = (Sprite) null;
      string str = null;
      if (mode == LoadWindowMode.StartGameData || mode == LoadWindowMode.LoadSavedGame)
      {
        LoadWindowGameDataItem windowGameDataItem = gameDataSettings.GetItem(InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().Name);
        if (!windowGameDataItem.IsNull)
        {
          UnitySubAsset<Sprite> imageInformation;
          if (mode == LoadWindowMode.LoadSavedGame)
          {
            IProfilesService service = ServiceLocator.GetService<IProfilesService>();
            int intValue = service != null ? service.GetIntValue("Deaths") : 0;
            if ((UnityEngine.Object) windowGameDataItem.LoadStorables != (UnityEngine.Object) null && InstanceByRequest<EngineApplication>.Instance.IsInitialized)
            {
              IInventoryPlaceholderSerializable placeholderSerializable = Evaluate(windowGameDataItem.LoadStorables.Items, GameDay, intValue);
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
              str = Evaluate(windowGameDataItem.LoadTooltips.Items, GameDay, intValue);
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
      spriteView.SetValue(sprite1, true);
      tooltipTagView.StringValue = str;
      bufferedTooltip = str;
    }

    private void Clear()
    {
      spriteView.SetValue((Sprite) null, true);
      tooltipTagView.StringValue = null;
      bufferedTooltip = null;
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
        if (Evaluate(loadWindowDataItem, gameDay, deathCount))
          max += loadWindowDataItem.Weight;
      }
      if (max == 0)
        return default (T);
      int num1 = UnityEngine.Random.Range(0, max);
      int num2 = 0;
      for (int index = 0; index < items.Length; ++index)
      {
        LoadWindowDataItem<T> loadWindowDataItem = items[index];
        if (Evaluate(loadWindowDataItem, gameDay, deathCount))
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
      Build();
    }

    private void OnDisable()
    {
      Clear();
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    }

    private void OnEnable()
    {
      loadingSpinner.SetActive(true);
      Build();
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
    }

    private void OnJoystick(bool joystick)
    {
      if (bufferedTooltip == null)
        return;
      tooltipTagView.StringValue = null;
      tooltipTagView.StringValue = bufferedTooltip;
    }
  }
}
