using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using InputServices;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof (CanvasRenderer))]
  public class SplitGraphic : 
    BaseGraphic,
    IPointerClickHandler,
    IEventSystemHandler,
    IPointerDownHandler
  {
    private IStorableComponent actor;
    private bool isCanceled;
    private IStorableComponent target;
    [SerializeField]
    private Slider unitySlider = (Slider) null;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Text itemAmountText;
    [SerializeField]
    private Button buttonSelect;
    [SerializeField]
    private Button buttonCancel;
    private bool isReverted;
    [SerializeField]
    private GameObject controls;

    public bool IsReverted
    {
      get => isReverted;
      set => isReverted = value;
    }

    public IStorableComponent Actor
    {
      get => actor;
      set
      {
        actor = value;
        target = null;
        ResetSlider();
        if (actor == null)
          return;
        itemImage.sprite = ((StorableComponent) actor).Placeholder.ImageInventorySlot.Value;
      }
    }

    public IStorableComponent Target => target;

    public bool IsCanceled => isCanceled;

    protected override void Awake()
    {
      base.Awake();
      buttonSelect.onClick.AddListener(new UnityAction(Select));
      buttonCancel.onClick.AddListener(new UnityAction(Cancel));
      unitySlider.onValueChanged.AddListener(new UnityAction<float>(OnSliderValueChange));
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      if ((UnityEngine.Object) EventSystem.current == (UnityEngine.Object) null)
        return;
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.Cancel, BasicListener);
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
      OnJoystick(InputService.Instance.JoystickUsed);
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.Submit, BasicListener);
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.LStickLeft, BasicListener);
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.LStickRight, BasicListener);
    }

    private bool BasicListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Submit & down)
      {
        Select();
        return true;
      }
      if (type == GameActionType.LStickLeft & down)
      {
        --unitySlider.value;
        return true;
      }
      if (type == GameActionType.LStickRight & down)
      {
        ++unitySlider.value;
        return true;
      }
      if (type == GameActionType.Cancel & down)
        Cancel();
      return false;
    }

    private void OnJoystick(bool joystick)
    {
      buttonCancel.gameObject.SetActive(!joystick);
      buttonSelect.gameObject.SetActive(!joystick);
      controls.SetActive(joystick);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.Cancel, BasicListener);
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.Submit, BasicListener);
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.LStickLeft, BasicListener);
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.LStickRight, BasicListener);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    private void OnSliderValueChange(float value) => UpdateCount();

    private void Select()
    {
      int count = Mathf.RoundToInt(unitySlider.value);
      if (isReverted)
        count = actor.Count - count;
      isCanceled = false;
      if (actor == null || actor.IsDisposed)
        isCanceled = true;
      else if (count == 0)
        isCanceled = true;
      else
        target = actor.Count != count ? actor.Split(count) : actor;
      IsEnabled = false;
      IsReverted = false;
    }

    private void Cancel()
    {
      isCanceled = true;
      IsEnabled = false;
      IsReverted = false;
    }

    protected void ResetSlider()
    {
      if (actor == null || actor.IsDisposed)
        return;
      unitySlider.minValue = 1f;
      unitySlider.maxValue = actor.Count - 1f;
      unitySlider.value = 1f;
      UpdateCount();
    }

    public void UpdateCount()
    {
      if (actor == null || actor.IsDisposed)
        return;
      itemAmountText.text = "× " + Mathf.RoundToInt(unitySlider.value).ToString();
    }

    public static SplitGraphic Instantiate(GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      return gameObject.GetComponent<SplitGraphic>();
    }

    private bool CancelListener(GameActionType type, bool down)
    {
      if (!down)
        return false;
      isCanceled = true;
      IsEnabled = false;
      return true;
    }
  }
}
