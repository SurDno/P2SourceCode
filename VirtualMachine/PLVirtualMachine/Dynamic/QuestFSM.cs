using Cofe.Loggers;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Dynamic.Components;
using PLVirtualMachine.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PLVirtualMachine.Dynamic
{
  public class QuestFSM : DynamicFSM
  {
    private Dictionary<Guid, LockedFSMInfo> lockedObjects = new Dictionary<Guid, LockedFSMInfo>((IEqualityComparer<Guid>) GuidComparer.Instance);

    public QuestFSM(VMEntity entity, VMLogicObject templateObj)
      : base(entity, templateObj)
    {
    }

    public override void Think()
    {
      long timestamp = Stopwatch.GetTimestamp();
      base.Think();
      if (this.Active)
      {
        foreach (KeyValuePair<Guid, LockedFSMInfo> lockedObject in this.lockedObjects)
        {
          LockedFSMInfo lockedFsmInfo = lockedObject.Value;
          if (lockedFsmInfo.NeedRestoreAction)
          {
            VMEngineAPIManager.ExecMethod(lockedFsmInfo.LastEntityMethodExecuteData);
            lockedFsmInfo.NeedRestoreAction = false;
          }
        }
      }
      double num = ((double) Stopwatch.GetTimestamp() - (double) timestamp) / (double) Stopwatch.Frequency;
    }

    public bool LockObject(DynamicFSM lockObjFSM)
    {
      int num = lockObjFSM.Lock((DynamicFSM) this) ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.lockedObjects.Add(lockObjFSM.Entity.EngineGuid, new LockedFSMInfo(lockObjFSM));
      return num != 0;
    }

    public bool UnLockObject(DynamicFSM lockObjFSM)
    {
      int num = lockObjFSM.UnLock((DynamicFSM) this) ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.lockedObjects.Remove(lockObjFSM.Entity.EngineGuid);
      return num != 0;
    }

    public void StartQuest()
    {
      if (this.Entity == null)
        return;
      ((QuestComponent) this.Entity.GetComponentByName("QuestComponent"))?.StartQuest(this);
    }

    public void EndQuest()
    {
      this.Active = false;
      foreach (KeyValuePair<Guid, LockedFSMInfo> lockedObject in this.lockedObjects)
        lockedObject.Value.LockedFSM.UnLock((DynamicFSM) this);
      this.lockedObjects.Clear();
    }

    public override void OnStart()
    {
      base.OnStart();
      this.StartQuest();
    }

    public override void SetLockedObjectNeedRestoreAction(DynamicFSM lockedFSM)
    {
      if (this.lockedObjects.ContainsKey(lockedFSM.Entity.EngineGuid))
        this.lockedObjects[lockedFSM.Entity.EngineGuid].NeedRestoreAction = true;
      else
        Logger.AddError(string.Format("Lock object restore action error: locked object with guid {0} not found", (object) lockedFSM.Entity.EngineGuid));
    }

    public override void OnAddChildDynamicObject(DynamicFSM childDynFSM)
    {
    }

    public override void OnRemoveChildDynamicObject(DynamicFSM childDynFSM)
    {
      if (childDynFSM == null || childDynFSM.LockingFSM == null || !(this.EngineGuid == childDynFSM.EngineGuid))
        return;
      this.UnLockObject(childDynFSM);
    }

    protected override void RememberMetodExecData(EntityMethodExecuteData lastMethodExecData)
    {
      Guid targetEntityGuid = lastMethodExecData.TargetEntityGuid;
      if (!this.lockedObjects.ContainsKey(targetEntityGuid))
        return;
      this.lockedObjects[targetEntityGuid].SetLastActionMethodExecData(lastMethodExecData);
    }
  }
}
