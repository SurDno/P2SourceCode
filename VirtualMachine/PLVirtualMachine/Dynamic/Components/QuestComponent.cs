using Cofe.Loggers;
using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMQuestComponent))]
  public class QuestComponent : 
    VMQuestComponent,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    private QuestFSM questLogic;

    public override string GetComponentTypeName() => nameof (QuestComponent);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    public void StartQuest(QuestFSM questLogic) => this.questLogic = questLogic;

    public override void Idle()
    {
    }

    public override bool IsPlay() => this.questLogic == null && this.questLogic.Active;

    public override void LockObject(IObjRef objRef)
    {
      if (this.questLogic == null)
      {
        Logger.AddError(string.Format("Quest not active, cannot process operation"));
      }
      else
      {
        VMObjRef vmObjRef = (VMObjRef) objRef;
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(vmObjRef.EngineGuid);
        if (entityByEngineGuid != null)
          this.questLogic.LockObject(entityByEngineGuid.GetFSM());
        else
          Logger.AddError(string.Format("Locking error: dynamic FSM for object {0} not found", (object) vmObjRef.StaticInstance.BaseGuid));
      }
    }

    public override void UnLockObject(IObjRef objRef)
    {
      if (this.questLogic == null)
      {
        Logger.AddError(string.Format("Quest not active, cannot process operation"));
      }
      else
      {
        VMObjRef vmObjRef = (VMObjRef) objRef;
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(vmObjRef.EngineGuid);
        if (entityByEngineGuid != null)
          this.questLogic.UnLockObject(entityByEngineGuid.GetFSM());
        else
          Logger.AddError(string.Format("Unlocking error: dynamic FSM for object {0} not found", (object) vmObjRef.StaticInstance.BaseGuid));
      }
    }

    public override void EndQuest() => this.questLogic.EndQuest();
  }
}
