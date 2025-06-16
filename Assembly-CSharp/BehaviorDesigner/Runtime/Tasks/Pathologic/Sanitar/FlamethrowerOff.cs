using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar
{
  [TaskDescription("Disable sanitar flamethrower.")]
  [TaskCategory("Pathologic/Sanitar")]
  [TaskIcon("Pathologic_SanitarIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FlamethrowerOff))]
  public class FlamethrowerOff : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private PivotSanitar pivot;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
      {
        pivot = gameObject.GetComponent<PivotSanitar>();
        if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) ("Doesn't contain " + typeof (PivotSanitar).Name + " component"), (UnityEngine.Object) gameObject);
          return TaskStatus.Failure;
        }
      }
      pivot.Flamethrower = false;
      pivot.TargetObject = (Transform) null;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    }
  }
}
