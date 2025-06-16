using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Look at target OFF")]
  [TaskCategory("Pathologic/IK")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (LookAtOff))]
  public class LookAtOff : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private IKController ikController;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.ikController == (UnityEngine.Object) null)
      {
        this.ikController = this.gameObject.GetComponent<IKController>();
        if ((UnityEngine.Object) this.ikController == (UnityEngine.Object) null)
        {
          Debug.LogError((object) (this.gameObject.name + ": doesn't contain " + typeof (IKController).Name + " unity component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      this.ikController.LookTarget = (Transform) null;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    }
  }
}
