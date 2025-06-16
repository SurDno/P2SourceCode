// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.HasReceivedEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (HasReceivedEvent))]
  [TaskDescription("Returns success as soon as the event specified by eventName has been received.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=123")]
  [TaskIcon("{SkinColor}HasReceivedEventIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class HasReceivedEvent : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The name of the event to receive")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedString eventName = (SharedString) "";
    [Tooltip("Optionally store the first sent argument")]
    [SharedRequired]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVariable storedValue1;
    private bool eventReceived = false;
    private bool registered = false;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedString>(writer, "EventName", this.eventName);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVariable>(writer, "StoredValue1", this.storedValue1);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.eventName = BehaviorTreeDataReadUtility.ReadShared<SharedString>(reader, "EventName", this.eventName);
      this.storedValue1 = BehaviorTreeDataReadUtility.ReadShared<SharedVariable>(reader, "StoredValue1", this.storedValue1);
    }

    public override void OnStart()
    {
      if (this.registered)
        return;
      this.Owner.RegisterEvent(this.eventName.Value, new System.Action(this.ReceivedEvent));
      this.Owner.RegisterEvent<object>(this.eventName.Value, new Action<object>(this.ReceivedEvent));
      this.Owner.RegisterEvent<object, object>(this.eventName.Value, new Action<object, object>(this.ReceivedEvent));
      this.Owner.RegisterEvent<object, object, object>(this.eventName.Value, new Action<object, object, object>(this.ReceivedEvent));
      this.registered = true;
    }

    public override TaskStatus OnUpdate()
    {
      return this.eventReceived ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override void OnEnd()
    {
      if (this.eventReceived)
      {
        this.Owner.UnregisterEvent(this.eventName.Value, new System.Action(this.ReceivedEvent));
        this.Owner.UnregisterEvent<object>(this.eventName.Value, new Action<object>(this.ReceivedEvent));
        this.Owner.UnregisterEvent<object, object>(this.eventName.Value, new Action<object, object>(this.ReceivedEvent));
        this.Owner.UnregisterEvent<object, object, object>(this.eventName.Value, new Action<object, object, object>(this.ReceivedEvent));
        this.registered = false;
      }
      this.eventReceived = false;
    }

    private void ReceivedEvent() => this.eventReceived = true;

    private void ReceivedEvent(object arg1)
    {
      this.ReceivedEvent();
      if (this.storedValue1 == null || this.storedValue1.IsNone)
        return;
      this.storedValue1.SetValue(arg1);
    }

    private void ReceivedEvent(object arg1, object arg2)
    {
      this.ReceivedEvent();
      if (this.storedValue1 == null || this.storedValue1.IsNone)
        return;
      this.storedValue1.SetValue(arg1);
    }

    private void ReceivedEvent(object arg1, object arg2, object arg3)
    {
      this.ReceivedEvent();
      if (this.storedValue1 == null || this.storedValue1.IsNone)
        return;
      this.storedValue1.SetValue(arg1);
    }

    public override void OnBehaviorComplete()
    {
      this.Owner.UnregisterEvent(this.eventName.Value, new System.Action(this.ReceivedEvent));
      this.Owner.UnregisterEvent<object>(this.eventName.Value, new Action<object>(this.ReceivedEvent));
      this.Owner.UnregisterEvent<object, object>(this.eventName.Value, new Action<object, object>(this.ReceivedEvent));
      this.Owner.UnregisterEvent<object, object, object>(this.eventName.Value, new Action<object, object, object>(this.ReceivedEvent));
      this.eventReceived = false;
      this.registered = false;
    }

    public override void OnReset() => this.eventName = (SharedString) "";
  }
}
