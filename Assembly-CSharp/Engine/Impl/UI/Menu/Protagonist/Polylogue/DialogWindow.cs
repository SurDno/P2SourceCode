using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Speaking;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Source.Components;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Engine.Source.UI;
using InputServices;
using Inspectors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Polylogue
{
  [RequireComponent(typeof (Canvas))]
  public class DialogWindow : UIWindow, IDialogWindow, IWindow
  {
    private DialogKind mode = DialogKind.None;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Answer_Prefab")]
    private GameObject unityAnswerPrefab;
    [SerializeField]
    private GameObject unityFinalAnswerPrefab;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Answers_Content")]
    private RectTransform unityAnswersContent;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Day")]
    private Text characterName;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Speech_Content")]
    private RectTransform unitySpeechContent;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Speech_Prefab")]
    private GameObject unitySpeechPrefab;
    [Inspected]
    private IEntity entityEffect;
    private DialogModeController dialogModeController = new DialogModeController {
      TargetCameraKind = CameraKindEnum.Dialog
    };
    [SerializeField]
    private GameObject helpPanel;
    private int currentIndex;
    private Button[] buttons;
    private Text finalAnswerSign;
    private Color normalColor;
    private Color highlightedColor;
    private bool InputBlocked;

    public ISpeakingComponent Actor { get; set; }

    public ISpeakingComponent Target { get; set; }

    public void Answer(ISpeakingComponent actor, IList<DialogString> answers)
    {
      mode = DialogKind.Answering;
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      for (int index = 0; index < answers.Count; ++index)
      {
        DialogString answer = answers[index];
        GameObject gameObject = Instantiate(unityAnswerPrefab);
        gameObject.GetComponentInChildren<Text>().text = service.GetText(answer.String);
        RectTransform component = gameObject.GetComponent<RectTransform>();
        component.SetParent(unityAnswersContent, false);
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
          if (mode != DialogKind.Answering)
            return;
          mode = DialogKind.Saying;
          AnswersClear();
          ((SpeakingComponent) Target).FireSpeechReply(answer.Id);
        });
        if (answer.Type == DialogStringEnum.Final)
          Instantiate(unityFinalAnswerPrefab).GetComponent<RectTransform>().SetParent(component, false);
      }
      buttons = GetComponentsInChildren<Button>();
      currentIndex = 0;
      ChangeSelection();
      LayoutRebuilder.MarkLayoutForRebuild(unityAnswersContent);
    }

    private void Update()
    {
      if (mode != DialogKind.Answering)
        return;
      int index = 0;
      for (int key = 49; key < 57 && index < unityAnswersContent.childCount; ++index)
      {
        if (Input.GetKeyDown((KeyCode) key))
          unityAnswersContent.GetChild(index).gameObject.GetComponent<Button>().onClick.Invoke();
        ++key;
      }
    }

    public void Speech(ISpeakingComponent actor, LocalizedText localizedText)
    {
      SpeechClear();
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      GameObject gameObject = Instantiate(unitySpeechPrefab);
      Text component1 = gameObject.GetComponent<Text>();
      string str = service.GetText(localizedText);
      if (str.Length > 750)
      {
        str = str.Substring(0, 750) + " (max length: 750)";
        Debug.LogError("Speech text is too long to render (over 750) , tag : " + localizedText.Id + " , localization : " + ServiceLocator.GetService<LocalizationService>().CurrentLanguage + " , actor : " + actor.Owner.GetInfo());
      }
      component1.text = str.Replace("\n", "\n<size=" + (component1.fontSize / 2) + ">\n</size>");
      InteractableComponent component2 = actor.GetComponent<InteractableComponent>();
      if (component2 != null)
        characterName.text = service.GetText(component2.Title);
      gameObject.transform.SetParent(unitySpeechContent, false);
      LayoutRebuilder.MarkLayoutForRebuild(unitySpeechContent);
    }

    private void Clear()
    {
      dialogModeController.SetDialogMode(Target?.Owner, false);
      if (Target != null)
      {
        SpeakingComponent target = (SpeakingComponent) Target;
        target.OnBeginSpeech -= OnBeginSpeech;
        target.OnExitTalking -= OnExitTalking;
        Target = null;
      }
      Actor = null;
      if (entityEffect != null)
      {
        entityEffect.Dispose();
        entityEffect = null;
      }
      AnswersClear();
      SpeechClear();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      SpeakingComponent target = (SpeakingComponent) Target;
      target.OnBeginSpeech += OnBeginSpeech;
      target.OnExitTalking += OnExitTalking;
      target.FireBeginTalking();
      dialogModeController.EnableCameraKind(Target?.Owner);
      dialogModeController.SetDialogMode(Target?.Owner, true);
      PlayInitialPhrase();
      buttons = GetComponentsInChildren<Button>();
      if (buttons.Length != 0)
      {
        highlightedColor = buttons[currentIndex].colors.highlightedColor;
        normalColor = buttons[currentIndex].colors.normalColor;
      }
      currentIndex = 0;
      OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected override void OnDisable()
    {
      Clear();
      mode = DialogKind.None;
      dialogModeController.DisableCameraKind();
      dialogModeController.SetDialogMode(Target?.Owner, false);
      CursorService.Instance.Free = CursorService.Instance.Visible = false;
      currentIndex = 0;
      InputBlocked = false;
      RemoveDialogListeners();
      base.OnDisable();
    }

    private void AddDialogListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.LStickDown, ChangeSelectedItem);
      service.AddListener(GameActionType.LStickUp, ChangeSelectedItem);
      service.AddListener(GameActionType.DPadUp, ChangeSelectedItem);
      service.AddListener(GameActionType.DPadDown, ChangeSelectedItem);
      service.AddListener(GameActionType.Submit, ChangeSelectedItem);
    }

    private void RemoveDialogListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickDown, ChangeSelectedItem);
      service.RemoveListener(GameActionType.LStickUp, ChangeSelectedItem);
      service.RemoveListener(GameActionType.DPadUp, ChangeSelectedItem);
      service.RemoveListener(GameActionType.DPadDown, ChangeSelectedItem);
      service.RemoveListener(GameActionType.Submit, ChangeSelectedItem);
    }

    private bool ChangeSelectedItem(GameActionType type, bool down)
    {
      if (InputBlocked)
      {
        InputBlocked = false;
        return false;
      }
      if (!helpPanel.activeInHierarchy)
        return false;
      if (((type == GameActionType.LStickUp ? 1 : (type == GameActionType.DPadUp ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        --currentIndex;
        ChangeSelection();
        return true;
      }
      if (((type == GameActionType.LStickDown ? 1 : (type == GameActionType.DPadDown ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        ++currentIndex;
        ChangeSelection();
        return true;
      }
      if (!(type == GameActionType.Submit & down))
        return false;
      ExecuteEvents.Execute(buttons[currentIndex].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      return true;
    }

    private void ChangeSelection()
    {
      if (!InputService.Instance.JoystickUsed || buttons == null || buttons.Length == 0)
        return;
      if (currentIndex > buttons.Length - 1)
        currentIndex = 0;
      if (currentIndex < 0)
        currentIndex = buttons.Length - 1;
      if (finalAnswerSign != null)
      {
        Color white = Color.white;
        finalAnswerSign.color = normalColor;
        finalAnswerSign = null;
      }
      EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
      if (buttons[currentIndex].transform.childCount <= 0)
        return;
      finalAnswerSign = buttons[currentIndex].transform.GetChild(0).GetComponent<Text>();
      finalAnswerSign.color = highlightedColor;
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      helpPanel.SetActive(joystick);
      EventSystem.current.SetSelectedGameObject(null);
      if (joystick)
      {
        ChangeSelection();
        AddDialogListeners();
      }
      else
      {
        EventSystem.current.SetSelectedGameObject(null);
        InputBlocked = true;
        if (finalAnswerSign != null)
        {
          Color white = Color.white;
          finalAnswerSign.color = normalColor;
          finalAnswerSign = null;
        }
        RemoveDialogListeners();
      }
    }

    private void AnswersClear()
    {
      for (int index = unityAnswersContent.childCount - 1; index >= 0; --index)
      {
        GameObject gameObject = unityAnswersContent.GetChild(index).gameObject;
        gameObject.SetActive(false);
        Destroy(gameObject);
      }
    }

    private void SpeechClear()
    {
      for (int index = unitySpeechContent.childCount - 1; index >= 0; --index)
      {
        GameObject gameObject = unitySpeechContent.GetChild(index).gameObject;
        gameObject.SetActive(false);
        Destroy(gameObject);
      }
    }

    private void OnBeginSpeech(LocalizedText speech, List<DialogString> replies)
    {
      Speech(Target, speech);
      Answer(Target, replies);
    }

    private void OnExitTalking() => CoroutineService.Instance.Route(WaitingTranslate());

    private IEnumerator WaitingTranslate()
    {
      UIService ui = ServiceLocator.GetService<UIService>();
      while (ui.IsTransition)
        yield return null;
      ui.Pop();
    }

    public override void Initialize()
    {
      RegisterLayer<IDialogWindow>(this);
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (IDialogWindow);

    private void PlayInitialPhrase()
    {
      if (Target == null)
        Debug.LogError("Dialog Target is null!");
      else
        Target.GetComponent<ILipSyncComponent>()?.Play3D(Target.InitialPhrases.RandomUniform(), ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMax, false);
    }
  }
}
