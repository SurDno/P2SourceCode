// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.SpeakingComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Speaking;
using Engine.Common.Generator;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components
{
  [Required(typeof (LipSyncComponent))]
  [Factory(typeof (ISpeakingComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class SpeakingComponent : EngineComponent, ISpeakingComponent, IComponent, INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    [StateSaveProxy(MemberEnum.CustomListReference)]
    [StateLoadProxy(MemberEnum.CustomListReference)]
    [Inspected]
    protected List<ILipSyncObject> initialPhrases = new List<ILipSyncObject>();
    private bool speakAvailable;

    public event Action OnBeginTalking;

    public event Action<ulong> OnSpeechReply;

    public event Action<LocalizedText, List<DialogString>> OnBeginSpeech;

    public event Action OnExitTalking;

    public event Action<bool> OnSpeakAvailableChange;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set => this.isEnabled = value;
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public bool SpeakAvailable
    {
      get => this.speakAvailable;
      set
      {
        if (this.speakAvailable == value)
          return;
        this.speakAvailable = value;
        Action<bool> speakAvailableChange = this.OnSpeakAvailableChange;
        if (speakAvailableChange == null)
          return;
        speakAvailableChange(value);
      }
    }

    public IEnumerable<ILipSyncObject> InitialPhrases
    {
      get => (IEnumerable<ILipSyncObject>) this.initialPhrases;
    }

    public void AddInitialPhrase(ILipSyncObject lipsync) => this.initialPhrases.Add(lipsync);

    public void RemoveInitialPhrase(ILipSyncObject lipsync) => this.initialPhrases.Remove(lipsync);

    public void ClearInitialPhrases() => this.initialPhrases.Clear();

    public bool NeedSave => true;

    public void FireBeginTalking()
    {
      Action onBeginTalking = this.OnBeginTalking;
      if (onBeginTalking == null)
        return;
      onBeginTalking();
    }

    public void FireSpeechReply(ulong reply)
    {
      Action<ulong> onSpeechReply = this.OnSpeechReply;
      if (onSpeechReply == null)
        return;
      onSpeechReply(reply);
    }

    public void Speech(LocalizedText speech, List<DialogString> replies)
    {
      Action<LocalizedText, List<DialogString>> onBeginSpeech = this.OnBeginSpeech;
      if (onBeginSpeech == null)
        return;
      onBeginSpeech(speech, replies);
    }

    public void ExitTalking()
    {
      Action onExitTalking = this.OnExitTalking;
      if (onExitTalking == null)
        return;
      onExitTalking();
    }
  }
}
