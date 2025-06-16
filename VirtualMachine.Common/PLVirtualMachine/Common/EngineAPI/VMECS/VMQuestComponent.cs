using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("QuestComponent")]
  public class VMQuestComponent : VMComponent
  {
    public const string ComponentName = "QuestComponent";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Method("Wait", "", "")]
    public virtual void Idle()
    {
    }

    [Method("Is active", "", "")]
    public virtual bool IsPlay() => false;

    [Method("Lock object", "Object", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_QUEST_LOCK_OBJECT)]
    public virtual void LockObject(IObjRef objRef)
    {
    }

    [Method("Unloc object", "Object", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_QUEST_UNLOCK_OBJECT)]
    public virtual void UnLockObject(IObjRef objRef)
    {
    }

    [Method("Complete", "", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_END_QUEST)]
    public virtual void EndQuest()
    {
    }
  }
}
