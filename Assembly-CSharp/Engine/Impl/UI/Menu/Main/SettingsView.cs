using System.Collections;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using InputServices;

namespace Engine.Impl.UI.Menu.Main
{
  public class SettingsView : MonoBehaviour
  {
    public static SettingsView Current;
    [SerializeField]
    protected LayoutContainer listLayoutPrefab;
    [SerializeField]
    protected FloatSettingsValueView floatValueViewPrefab;
    [SerializeField]
    protected NamedIntSettingsValueView namedIntValueViewPrefab;
    [SerializeField]
    protected ConfirmationWindow confirmationPrefab;
    [SerializeField]
    protected BoolSettingsValueView boolValueViewPrefab;
    [SerializeField]
    private Button buttonReset;
    private ISettingEntity currentEntity;
    private int currentIndex;
    protected LayoutContainer layout;
    private Coroutine scrollCoroutine;
    private Coroutine changingCoroutine;

    protected virtual void Awake()
    {
      SettingsMenuHelper.Instatnce.OnStateSelected += OnStateSelected;
      buttonReset?.onClick.AddListener(new UnityAction(OnButtonReset));
    }

    protected virtual void OnEnable()
    {
      Current = this;
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, OnNavigate, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, OnNavigate, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, OnValueChange, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, OnValueChange, true);
      OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected virtual bool OnResetGameAction(GameActionType type, bool down)
    {
      if (down)
        OnButtonReset();
      return down;
    }

    private bool OnValueChange(GameActionType type, bool down)
    {
      if (changingCoroutine != null)
        this.StopCoroutine(changingCoroutine);
      if (down)
      {
        switch (type)
        {
          case GameActionType.LStickLeft:
            currentEntity?.DecrementValue();
            changingCoroutine = this.StartCoroutine(ChangingCoroutine(true));
            break;
          case GameActionType.LStickRight:
            currentEntity?.IncrementValue();
            changingCoroutine = this.StartCoroutine(ChangingCoroutine(false));
            break;
        }
      }
      return down;
    }

    private bool OnNavigate(GameActionType type, bool down)
    {
      if (scrollCoroutine != null)
        this.StopCoroutine(scrollCoroutine);
      if (down)
      {
        switch (type)
        {
          case GameActionType.LStickUp:
            SelectItem(currentIndex - 1);
            scrollCoroutine = this.StartCoroutine(ScrollCoroutine(true));
            break;
          case GameActionType.LStickDown:
            SelectItem(currentIndex + 1);
            scrollCoroutine = this.StartCoroutine(ScrollCoroutine(false));
            break;
        }
      }
      return down;
    }

    protected virtual void OnJoystick(bool isUsed)
    {
      if (isUsed)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, OnResetGameAction);
      else
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, OnResetGameAction);
      if (currentEntity != null)
        currentEntity.Selected = isUsed;
      if (isUsed)
        return;
      OnStateSelected(true);
    }

    protected virtual void OnDestroy()
    {
      SettingsMenuHelper.Instatnce.OnStateSelected -= OnStateSelected;
    }

    protected virtual void OnDisable()
    {
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, OnNavigate);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, OnNavigate);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, OnValueChange);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, OnValueChange);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, OnResetGameAction);
    }

    private void OnStateSelected(bool isSelected)
    {
      isSelected = isSelected || !InputService.Instance.JoystickUsed;
      this.gameObject.SetActive(isSelected);
      SelectItem(0);
      if (currentEntity == null)
        return;
      currentEntity.Selected = InputService.Instance.JoystickUsed;
    }

    protected bool SelectItem(int index)
    {
      List<ISettingEntity> all = new List<ISettingEntity>((IEnumerable<ISettingEntity>) layout.transform.GetComponentsInChildren<ISettingEntity>()).FindAll(e => e.IsActive() && e.Interactable);
      if (all.Count == 0)
        return false;
      currentIndex = index >= all.Count ? 0 : (index < 0 ? all.Count - 1 : index);
      if (currentEntity != null)
      {
        if (InputService.Instance.JoystickUsed)
          currentEntity.OnDeSelect();
        currentEntity.Selected = false;
      }
      currentEntity = all[currentIndex];
      currentEntity.Selected = true;
      if (InputService.Instance.JoystickUsed)
        currentEntity.OnSelect();
      FillForSelected();
      return currentEntity.Interactable;
    }

    public void FillForSelected()
    {
      float height = layout.Content.parent.parent.GetComponent<RectTransform>().rect.height;
      float num1 = 84f;
      float num2 = -((Component) currentEntity).GetComponent<RectTransform>().anchoredPosition.y + num1;
      RectTransform component = layout.Content.parent.GetComponent<RectTransform>();
      Vector2 anchoredPosition = component.anchoredPosition;
      if (num2 - (double) anchoredPosition.y > height)
        anchoredPosition.y = num2 + (double) anchoredPosition.y > height ? num2 - height : 0.0f;
      else if (num2 - num1 * 2.0 - (double) anchoredPosition.y < 0.0)
        anchoredPosition.y = num2 - num1 * 2f;
      component.anchoredPosition = anchoredPosition;
    }

    protected virtual void OnButtonReset()
    {
    }

    private IEnumerator ScrollCoroutine(bool isUp)
    {
      yield return (object) new WaitForSeconds(0.5f);
      while (true)
      {
        int sellected = !isUp ? currentIndex + 1 : currentIndex - 1;
        SelectItem(sellected);
        yield return (object) new WaitForSeconds(0.05f);
      }
    }

    private IEnumerator ChangingCoroutine(bool isDecrement)
    {
      yield return (object) new WaitForSeconds(0.5f);
      while (true)
      {
        if (isDecrement)
          currentEntity?.DecrementValue();
        else
          currentEntity?.IncrementValue();
        yield return (object) new WaitForSeconds(0.05f);
      }
    }
  }
}
