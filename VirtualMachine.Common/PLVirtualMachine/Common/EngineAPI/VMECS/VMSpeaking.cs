using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Speaking;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Speaking", typeof (ISpeakingComponent))]
  public class VMSpeaking : VMEngineComponent<ISpeakingComponent>
  {
    public const string ComponentName = "Speaking";
    protected bool contextTalkingAvailable;
    protected IStateRef currentTalking;
    protected List<IFiniteStateMachine> passedOnlyOnceTalkigs = [];
    private static IFiniteStateMachine activeTalkingGlobal;
    private static VMBaseEntity activeTalkingOwner;

    public static string BeginTalkingEventName => "BeginTalkingEvent";

    [Event("Begin talking", "", true)]
    [SpecialEvent(ESpecialEventName.SEN_BEGIN_TALKING)]
    public event Action BeginTalkingEvent;

    [Event("End talking", "talking graph:TalkingGraph", false)]
    public event Action<IStateRef> EndTalkingEvent;

    [Event("", "text_guid", true)]
    [SpecialEvent(ESpecialEventName.SEN_SPEECH_REPLY)]
    public event Action<ulong> OnSpeechReplyEvent;

    [Property("Talking", "TalkingGraph", false)]
    public IStateRef CurrentTalking
    {
      get => currentTalking;
      set
      {
        currentTalking = value;
        OnModify();
        if (currentTalking != null)
          EnableOnlyOnceTalking((IFiniteStateMachine) currentTalking.State);
        UpdateSpeakAvailable();
      }
    }

    [Property("Initial phrase", "", false)]
    public ILipSyncObject InitialPhrase
    {
      get
      {
        Logger.AddError(string.Format("GetInitialPhrase is not supported at {0}", EngineAPIManager.Instance.CurrentFSMStateInfo));
        return null;
      }
      set
      {
        Component.ClearInitialPhrases();
        Component.AddInitialPhrase(value);
      }
    }

    [Method("Clear Initial Phrases", "", "")]
    public void ClearInitialPhrases() => Component.ClearInitialPhrases();

    [Method("Add Initial Phrase", "lipsync", "")]
    public void AddInitialPhrase(ILipSyncObject lipsync)
    {
      Component.AddInitialPhrase(lipsync);
    }

    [Method("Remove Initial Phrase", "lipsync", "")]
    public void RemoveInitialPhrase(ILipSyncObject lipsync)
    {
      Component.RemoveInitialPhrase(lipsync);
    }

    [Method("Add Forced Dialog", "distance", "")]
    public void AddForcedDialog(float distance)
    {
      ServiceLocator.GetService<IForcedDialogService>().AddForcedDialog(Component.Owner, distance);
    }

    [Method("Remove Forced Dialog", "", "")]
    public void RemoveForcedDialog()
    {
      ServiceLocator.GetService<IForcedDialogService>().RemoveForcedDialog(Component.Owner);
    }

    [Method("Cancel talkng", "", "")]
    public void CancelTalking() => ActiveTalking = null;

    [Method("Remove talkng", "", "")]
    public void RemoveTalking() => CurrentTalking = null;

    public void ExitTalking()
    {
      if (activeTalkingGlobal == null)
        return;
      ITalkingGraph actualTalking = GetActualTalking(activeTalkingGlobal);
      if (actualTalking == null)
      {
        Logger.AddError(string.Format("Invalid talking exit at {0} talking: cannot define actual talking !!! at {1}", Parent.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
        Component.ExitTalking();
      }
      else
      {
        if (actualTalking.OnlyOnce)
        {
          DisableOnlyOnceTalking(actualTalking);
          CurrentTalking = null;
        }
        ITalkingGraph exitTalking = actualTalking;
        ActiveTalking = null;
        DoExitTalking(exitTalking);
      }
    }

    public bool SpeakAvailable => Component != null && Component.SpeakAvailable;

    public void SetContextTalkingAvailable(bool isTalkingAvailable)
    {
      contextTalkingAvailable = isTalkingAvailable;
      OnModify();
      UpdateSpeakAvailable();
    }

    public virtual void OnBeginTalking()
    {
      if (currentTalking != null)
      {
        if (currentTalking.State != null && !typeof (ITalkingGraph).IsAssignableFrom(currentTalking.State.GetType()))
        {
          Logger.AddError(string.Format("Invalid CurrentTalking param at {0} speaking component: it must be talking graph", Parent.Name));
          return;
        }
        ActiveTalking = (IFiniteStateMachine) currentTalking.State;
      }
      BeginTalkingEvent();
      if (activeTalkingGlobal != null)
        return;
      DoExitTalking();
    }

    public virtual void OnSpeechReply(ulong replyGuid) => OnSpeechReplyEvent(replyGuid);

    public bool MakePlayerSpeech(
      ulong speechTextGuid,
      List<KeyValuePair<ulong, bool>> replyTextGuids,
      VMBaseEntity author = null)
    {
      LocalizedText engineTextInstance1 = EngineAPIManager.CreateEngineTextInstance(speechTextGuid);
      List<DialogString> replies = [];
      for (int index = 0; index < replyTextGuids.Count; ++index)
      {
        KeyValuePair<ulong, bool> replyTextGuid = replyTextGuids[index];
        LocalizedText engineTextInstance2 = EngineAPIManager.CreateEngineTextInstance(replyTextGuid.Key);
        DialogString dialogString1 = new DialogString();
        dialogString1.String = engineTextInstance2;
        ref DialogString local1 = ref dialogString1;
        replyTextGuid = replyTextGuids[index];
        long key = (long) replyTextGuid.Key;
        local1.Id = (ulong) key;
        DialogString dialogString2 = dialogString1;
        ref DialogString local2 = ref dialogString2;
        replyTextGuid = replyTextGuids[index];
        int num = replyTextGuid.Value ? 0 : 1;
        local2.Type = (DialogStringEnum) num;
        replies.Add(dialogString2);
      }
      Component.Speech(engineTextInstance1, replies);
      return true;
    }

    public IFiniteStateMachine ActiveTalking
    {
      get => activeTalkingGlobal;
      set
      {
        activeTalkingGlobal = value;
        if (activeTalkingGlobal != null)
          activeTalkingOwner = Parent;
        else
          activeTalkingOwner = null;
      }
    }

    public bool IsActiveTalking
    {
      get
      {
        if (activeTalkingGlobal == null)
          return false;
        if (activeTalkingOwner != null)
          return activeTalkingOwner == Parent;
        Logger.AddError(string.Format("Active talking owner consistency error in {0} !!!", Parent.Name));
        return false;
      }
    }

    public bool IsTalkingOnlyOncePassed(IFiniteStateMachine talking)
    {
      return passedOnlyOnceTalkigs.Contains(talking);
    }

    public ITalkingGraph GetActualTalking(IFiniteStateMachine graph)
    {
      if (graph == null)
        return null;
      if (typeof (ITalkingGraph).IsAssignableFrom(graph.GetType()))
      {
        if (!graph.Abstract)
          return (ITalkingGraph) graph;
        return CurrentTalking != null && CurrentTalking.State != null ? (ITalkingGraph) CurrentTalking.State : null;
      }
      return graph.SubstituteGraph != null ? GetActualTalking(graph.SubstituteGraph) : null;
    }

    public override void AfterSaveLoading() => UpdateSpeakAvailable();

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.OnBeginTalking -= OnBeginTalking;
      Component.OnSpeechReply -= OnSpeechReply;
      base.Clear();
    }

    public static void ClearAll()
    {
      activeTalkingGlobal = null;
      activeTalkingOwner = null;
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.OnBeginTalking += OnBeginTalking;
      Component.OnSpeechReply += OnSpeechReply;
    }

    private static IStateRef GetRealRefByGuid(ulong baseGuid)
    {
      IStateRef instance = (IStateRef) Activator.CreateInstance(BaseSerializer.GetRealRefType(typeof (IStateRef)));
      instance.Initialize(baseGuid);
      return instance;
    }

    private void DoExitTalking(IFiniteStateMachine exitTalking = null)
    {
      Component.ExitTalking();
      if (exitTalking == null)
        return;
      EndTalkingEvent(GetRealRefByGuid(exitTalking.BaseGuid));
    }

    private void UpdateSpeakAvailable()
    {
      bool flag = false;
      if (currentTalking != null && currentTalking.State != null)
        flag = true;
      if (Component != null)
        Component.SpeakAvailable = flag || contextTalkingAvailable;
      else
        Logger.AddError(string.Format("Speaking component engine instance not inited at {0}", Parent.Name));
    }

    private void EnableOnlyOnceTalking(IFiniteStateMachine talking)
    {
      if (talking != null && passedOnlyOnceTalkigs.Contains(talking))
        passedOnlyOnceTalkigs.Remove(talking);
      OnModify();
    }

    private void DisableOnlyOnceTalking(IFiniteStateMachine talking)
    {
      if (!passedOnlyOnceTalkigs.Contains(talking))
        passedOnlyOnceTalkigs.Add(talking);
      OnModify();
    }
  }
}
