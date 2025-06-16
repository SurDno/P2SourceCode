using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/GroupBehaviour/SuokCircle")]
  [TaskIcon("{SkinColor}SequenceIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (SuokCircleCombatAction))]
  public class SuokCircleCombatAction : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private SuokCircleService suokService;

    public override void OnStart()
    {
      this.suokService = ServiceLocator.GetService<SuokCircleService>();
      if (this.suokService == null)
        return;
      this.suokService.RegisterCombatant(this.Owner.gameObject);
    }

    public override TaskStatus OnUpdate() => TaskStatus.Running;

    public override void OnEnd()
    {
      if (this.suokService == null)
        return;
      this.suokService.UnregisterCombatant(this.Owner.gameObject);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    }
  }
}
