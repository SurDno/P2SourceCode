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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    private DialogModeController dialogModeController = new DialogModeController()
    {
      TargetCameraKind = CameraKindEnum.Dialog
    };
    [SerializeField]
    private GameObject helpPanel;
    private int currentIndex = 0;
    private Button[] buttons = (Button[]) null;
    private Text finalAnswerSign = (Text) null;
    private Color normalColor;
    private Color highlightedColor;
    private bool InputBlocked = false;

    public ISpeakingComponent Actor { get; set; }

    public ISpeakingComponent Target { get; set; }

    public void Answer(ISpeakingComponent actor, IList<DialogString> answers)
    {
      this.mode = DialogKind.Answering;
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      for (int index = 0; index < answers.Count; ++index)
      {
        DialogString answer = answers[index];
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.unityAnswerPrefab);
        gameObject.GetComponentInChildren<Text>().text = service.GetText(answer.String);
        RectTransform component = gameObject.GetComponent<RectTransform>();
        component.SetParent((Transform) this.unityAnswersContent, false);
        gameObject.GetComponent<Button>().onClick.AddListener((UnityAction) (() =>
        {
          if (this.mode != DialogKind.Answering)
            return;
          this.mode = DialogKind.Saying;
          this.AnswersClear();
          ((SpeakingComponent) this.Target).FireSpeechReply(answer.Id);
        }));
        if (answer.Type == DialogStringEnum.Final)
          UnityEngine.Object.Instantiate<GameObject>(this.unityFinalAnswerPrefab).GetComponent<RectTransform>().SetParent((Transform) component, false);
      }
      this.buttons = this.GetComponentsInChildren<Button>();
      this.currentIndex = 0;
      this.ChangeSelection();
      LayoutRebuilder.MarkLayoutForRebuild(this.unityAnswersContent);
    }

    private void Update()
    {
      if (this.mode != DialogKind.Answering)
        return;
      int index = 0;
      for (int key = 49; key < 57 && index < this.unityAnswersContent.childCount; ++index)
      {
        if (Input.GetKeyDown((KeyCode) key))
          this.unityAnswersContent.GetChild(index).gameObject.GetComponent<Button>().onClick.Invoke();
        ++key;
      }
    }

    public void Speech(ISpeakingComponent actor, LocalizedText localizedText)
    {
      this.SpeechClear();
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.unitySpeechPrefab);
      Text component1 = gameObject.GetComponent<Text>();
      string str = service.GetText(localizedText);
      if (str.Length > 750)
      {
        str = str.Substring(0, 750) + " (max length: 750)";
        Debug.LogError((object) ("Speech text is too long to render (over 750) , tag : " + (object) localizedText.Id + " , localization : " + ServiceLocator.GetService<LocalizationService>().CurrentLanguage.ToString() + " , actor : " + actor.Owner.GetInfo()));
      }
      component1.text = str.Replace("\n", "\n<size=" + (component1.fontSize / 2).ToString() + ">\n</size>");
      InteractableComponent component2 = actor.GetComponent<InteractableComponent>();
      if (component2 != null)
        this.characterName.text = service.GetText(component2.Title);
      gameObject.transform.SetParent((Transform) this.unitySpeechContent, false);
      LayoutRebuilder.MarkLayoutForRebuild(this.unitySpeechContent);
    }

    private void Clear()
    {
      this.dialogModeController.SetDialogMode(this.Target?.Owner, false);
      if (this.Target != null)
      {
        SpeakingComponent target = (SpeakingComponent) this.Target;
        target.OnBeginSpeech -= new Action<LocalizedText, List<DialogString>>(this.OnBeginSpeech);
        target.OnExitTalking -= new Action(this.OnExitTalking);
        this.Target = (ISpeakingComponent) null;
      }
      this.Actor = (ISpeakingComponent) null;
      if (this.entityEffect != null)
      {
        this.entityEffect.Dispose();
        this.entityEffect = (IEntity) null;
      }
      this.AnswersClear();
      this.SpeechClear();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      SpeakingComponent target = (SpeakingComponent) this.Target;
      target.OnBeginSpeech += new Action<LocalizedText, List<DialogString>>(this.OnBeginSpeech);
      target.OnExitTalking += new Action(this.OnExitTalking);
      target.FireBeginTalking();
      this.dialogModeController.EnableCameraKind(this.Target?.Owner);
      this.dialogModeController.SetDialogMode(this.Target?.Owner, true);
      this.PlayInitialPhrase();
      this.buttons = this.GetComponentsInChildren<Button>();
      if (this.buttons.Length != 0)
      {
        this.highlightedColor = this.buttons[this.currentIndex].colors.highlightedColor;
        this.normalColor = this.buttons[this.currentIndex].colors.normalColor;
      }
      this.currentIndex = 0;
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected override void OnDisable()
    {
      this.Clear();
      this.mode = DialogKind.None;
      this.dialogModeController.DisableCameraKind();
      this.dialogModeController.SetDialogMode(this.Target?.Owner, false);
      CursorService.Instance.Free = CursorService.Instance.Visible = false;
      this.currentIndex = 0;
      this.InputBlocked = false;
      this.RemoveDialogListeners();
      base.OnDisable();
    }

    private void AddDialogListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.DPadUp, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.DPadDown, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.ChangeSelectedItem));
    }

    private void RemoveDialogListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.DPadUp, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.DPadDown, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.ChangeSelectedItem));
    }

    private bool ChangeSelectedItem(GameActionType type, bool down)
    {
      if (this.InputBlocked)
      {
        this.InputBlocked = false;
        return false;
      }
      if (!this.helpPanel.activeInHierarchy)
        return false;
      if (((type == GameActionType.LStickUp ? 1 : (type == GameActionType.DPadUp ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        --this.currentIndex;
        this.ChangeSelection();
        return true;
      }
      if (((type == GameActionType.LStickDown ? 1 : (type == GameActionType.DPadDown ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        ++this.currentIndex;
        this.ChangeSelection();
        return true;
      }
      if (!(type == GameActionType.Submit & down))
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(this.buttons[this.currentIndex].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      return true;
    }

    private void ChangeSelection()
    {
      if (!InputService.Instance.JoystickUsed || this.buttons == null || this.buttons.Length == 0)
        return;
      if (this.currentIndex > this.buttons.Length - 1)
        this.currentIndex = 0;
      if (this.currentIndex < 0)
        this.currentIndex = this.buttons.Length - 1;
      if ((UnityEngine.Object) this.finalAnswerSign != (UnityEngine.Object) null)
      {
        Color white = Color.white;
        this.finalAnswerSign.color = this.normalColor;
        this.finalAnswerSign = (Text) null;
      }
      EventSystem.current.SetSelectedGameObject(this.buttons[this.currentIndex].gameObject);
      if (this.buttons[this.currentIndex].transform.childCount <= 0)
        return;
      this.finalAnswerSign = this.buttons[this.currentIndex].transform.GetChild(0).GetComponent<Text>();
      this.finalAnswerSign.color = this.highlightedColor;
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.helpPanel.SetActive(joystick);
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      if (joystick)
      {
        this.ChangeSelection();
        this.AddDialogListeners();
      }
      else
      {
        EventSystem.current.SetSelectedGameObject((GameObject) null);
        this.InputBlocked = true;
        if ((UnityEngine.Object) this.finalAnswerSign != (UnityEngine.Object) null)
        {
          Color white = Color.white;
          this.finalAnswerSign.color = this.normalColor;
          this.finalAnswerSign = (Text) null;
        }
        this.RemoveDialogListeners();
      }
    }

    private void AnswersClear()
    {
      for (int index = this.unityAnswersContent.childCount - 1; index >= 0; --index)
      {
        GameObject gameObject = this.unityAnswersContent.GetChild(index).gameObject;
        gameObject.SetActive(false);
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      }
    }

    private void SpeechClear()
    {
      for (int index = this.unitySpeechContent.childCount - 1; index >= 0; --index)
      {
        GameObject gameObject = this.unitySpeechContent.GetChild(index).gameObject;
        gameObject.SetActive(false);
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      }
    }

    private void OnBeginSpeech(LocalizedText speech, List<DialogString> replies)
    {
      this.Speech(this.Target, speech);
      this.Answer(this.Target, (IList<DialogString>) replies);
    }

    private void OnExitTalking() => CoroutineService.Instance.Route(this.WaitingTranslate());

    private IEnumerator WaitingTranslate()
    {
      UIService ui = ServiceLocator.GetService<UIService>();
      while (ui.IsTransition)
        yield return (object) null;
      ui.Pop();
    }

    public override void Initialize()
    {
      this.RegisterLayer<IDialogWindow>((IDialogWindow) this);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (IDialogWindow);

    private void PlayInitialPhrase()
    {
      if (this.Target == null)
        Debug.LogError((object) "Dialog Target is null!");
      else
        this.Target.GetComponent<ILipSyncComponent>()?.Play3D(this.Target.InitialPhrases.RandomUniform(), ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMax, false);
    }
  }
}
