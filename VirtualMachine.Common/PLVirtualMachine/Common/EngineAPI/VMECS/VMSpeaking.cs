// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMSpeaking
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Speaking;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Speaking", typeof (ISpeakingComponent))]
  public class VMSpeaking : VMEngineComponent<ISpeakingComponent>
  {
    public const string ComponentName = "Speaking";
    protected bool contextTalkingAvailable;
    protected IStateRef currentTalking;
    protected List<IFiniteStateMachine> passedOnlyOnceTalkigs = new List<IFiniteStateMachine>();
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
      get => this.currentTalking;
      set
      {
        this.currentTalking = value;
        this.OnModify();
        if (this.currentTalking != null)
          this.EnableOnlyOnceTalking((IFiniteStateMachine) this.currentTalking.State);
        this.UpdateSpeakAvailable();
      }
    }

    [Property("Initial phrase", "", false)]
    public ILipSyncObject InitialPhrase
    {
      get
      {
        Logger.AddError(string.Format("GetInitialPhrase is not supported at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        return (ILipSyncObject) null;
      }
      set
      {
        this.Component.ClearInitialPhrases();
        this.Component.AddInitialPhrase(value);
      }
    }

    [Method("Clear Initial Phrases", "", "")]
    public void ClearInitialPhrases() => this.Component.ClearInitialPhrases();

    [Method("Add Initial Phrase", "lipsync", "")]
    public void AddInitialPhrase(ILipSyncObject lipsync)
    {
      this.Component.AddInitialPhrase(lipsync);
    }

    [Method("Remove Initial Phrase", "lipsync", "")]
    public void RemoveInitialPhrase(ILipSyncObject lipsync)
    {
      this.Component.RemoveInitialPhrase(lipsync);
    }

    [Method("Add Forced Dialog", "distance", "")]
    public void AddForcedDialog(float distance)
    {
      ServiceLocator.GetService<IForcedDialogService>().AddForcedDialog(this.Component.Owner, distance);
    }

    [Method("Remove Forced Dialog", "", "")]
    public void RemoveForcedDialog()
    {
      ServiceLocator.GetService<IForcedDialogService>().RemoveForcedDialog(this.Component.Owner);
    }

    [Method("Cancel talkng", "", "")]
    public void CancelTalking() => this.ActiveTalking = (IFiniteStateMachine) null;

    [Method("Remove talkng", "", "")]
    public void RemoveTalking() => this.CurrentTalking = (IStateRef) null;

    public void ExitTalking()
    {
      if (VMSpeaking.activeTalkingGlobal == null)
        return;
      ITalkingGraph actualTalking = this.GetActualTalking(VMSpeaking.activeTalkingGlobal);
      if (actualTalking == null)
      {
        Logger.AddError(string.Format("Invalid talking exit at {0} talking: cannot define actual talking !!! at {1}", (object) this.Parent.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        this.Component.ExitTalking();
      }
      else
      {
        if (actualTalking.OnlyOnce)
        {
          this.DisableOnlyOnceTalking((IFiniteStateMachine) actualTalking);
          this.CurrentTalking = (IStateRef) null;
        }
        ITalkingGraph exitTalking = actualTalking;
        this.ActiveTalking = (IFiniteStateMachine) null;
        this.DoExitTalking((IFiniteStateMachine) exitTalking);
      }
    }

    public bool SpeakAvailable => this.Component != null && this.Component.SpeakAvailable;

    public void SetContextTalkingAvailable(bool isTalkingAvailable)
    {
      this.contextTalkingAvailable = isTalkingAvailable;
      this.OnModify();
      this.UpdateSpeakAvailable();
    }

    public virtual void OnBeginTalking()
    {
      if (this.currentTalking != null)
      {
        if (this.currentTalking.State != null && !typeof (ITalkingGraph).IsAssignableFrom(this.currentTalking.State.GetType()))
        {
          Logger.AddError(string.Format("Invalid CurrentTalking param at {0} speaking component: it must be talking graph", (object) this.Parent.Name));
          return;
        }
        this.ActiveTalking = (IFiniteStateMachine) this.currentTalking.State;
      }
      this.BeginTalkingEvent();
      if (VMSpeaking.activeTalkingGlobal != null)
        return;
      this.DoExitTalking();
    }

    public virtual void OnSpeechReply(ulong replyGuid) => this.OnSpeechReplyEvent(replyGuid);

    public bool MakePlayerSpeech(
      ulong speechTextGuid,
      List<KeyValuePair<ulong, bool>> replyTextGuids,
      VMBaseEntity author = null)
    {
      LocalizedText engineTextInstance1 = EngineAPIManager.CreateEngineTextInstance(speechTextGuid);
      List<DialogString> replies = new List<DialogString>();
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
      this.Component.Speech(engineTextInstance1, replies);
      return true;
    }

    public IFiniteStateMachine ActiveTalking
    {
      get => VMSpeaking.activeTalkingGlobal;
      set
      {
        VMSpeaking.activeTalkingGlobal = value;
        if (VMSpeaking.activeTalkingGlobal != null)
          VMSpeaking.activeTalkingOwner = this.Parent;
        else
          VMSpeaking.activeTalkingOwner = (VMBaseEntity) null;
      }
    }

    public bool IsActiveTalking
    {
      get
      {
        if (VMSpeaking.activeTalkingGlobal == null)
          return false;
        if (VMSpeaking.activeTalkingOwner != null)
          return VMSpeaking.activeTalkingOwner == this.Parent;
        Logger.AddError(string.Format("Active talking owner consistency error in {0} !!!", (object) this.Parent.Name));
        return false;
      }
    }

    public bool IsTalkingOnlyOncePassed(IFiniteStateMachine talking)
    {
      return this.passedOnlyOnceTalkigs.Contains(talking);
    }

    public ITalkingGraph GetActualTalking(IFiniteStateMachine graph)
    {
      if (graph == null)
        return (ITalkingGraph) null;
      if (typeof (ITalkingGraph).IsAssignableFrom(graph.GetType()))
      {
        if (!graph.Abstract)
          return (ITalkingGraph) graph;
        return this.CurrentTalking != null && this.CurrentTalking.State != null ? (ITalkingGraph) this.CurrentTalking.State : (ITalkingGraph) null;
      }
      return graph.SubstituteGraph != null ? this.GetActualTalking(graph.SubstituteGraph) : (ITalkingGraph) null;
    }

    public override void AfterSaveLoading() => this.UpdateSpeakAvailable();

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnBeginTalking -= new Action(this.OnBeginTalking);
      this.Component.OnSpeechReply -= new Action<ulong>(this.OnSpeechReply);
      base.Clear();
    }

    public static void ClearAll()
    {
      VMSpeaking.activeTalkingGlobal = (IFiniteStateMachine) null;
      VMSpeaking.activeTalkingOwner = (VMBaseEntity) null;
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.OnBeginTalking += new Action(this.OnBeginTalking);
      this.Component.OnSpeechReply += new Action<ulong>(this.OnSpeechReply);
    }

    private static IStateRef GetRealRefByGuid(ulong baseGuid)
    {
      IStateRef instance = (IStateRef) Activator.CreateInstance(BaseSerializer.GetRealRefType(typeof (IStateRef)));
      instance.Initialize(baseGuid);
      return instance;
    }

    private void DoExitTalking(IFiniteStateMachine exitTalking = null)
    {
      this.Component.ExitTalking();
      if (exitTalking == null)
        return;
      this.EndTalkingEvent(VMSpeaking.GetRealRefByGuid(exitTalking.BaseGuid));
    }

    private void UpdateSpeakAvailable()
    {
      bool flag = false;
      if (this.currentTalking != null && this.currentTalking.State != null)
        flag = true;
      if (this.Component != null)
        this.Component.SpeakAvailable = flag || this.contextTalkingAvailable;
      else
        Logger.AddError(string.Format("Speaking component engine instance not inited at {0}", (object) this.Parent.Name));
    }

    private void EnableOnlyOnceTalking(IFiniteStateMachine talking)
    {
      if (talking != null && this.passedOnlyOnceTalkigs.Contains(talking))
        this.passedOnlyOnceTalkigs.Remove(talking);
      this.OnModify();
    }

    private void DisableOnlyOnceTalking(IFiniteStateMachine talking)
    {
      if (!this.passedOnlyOnceTalkigs.Contains(talking))
        this.passedOnlyOnceTalkigs.Add(talking);
      this.OnModify();
    }
  }
}
