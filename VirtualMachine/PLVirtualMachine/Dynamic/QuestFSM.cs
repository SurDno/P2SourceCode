using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cofe.Loggers;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Dynamic.Components;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic
{
  public class QuestFSM(VMEntity entity, VMLogicObject templateObj) : DynamicFSM(entity, templateObj) {
    private Dictionary<Guid, LockedFSMInfo> lockedObjects = new(GuidComparer.Instance);

    public override void Think()
    {
      long timestamp = Stopwatch.GetTimestamp();
      base.Think();
      if (Active)
      {
        foreach (KeyValuePair<Guid, LockedFSMInfo> lockedObject in lockedObjects)
        {
          LockedFSMInfo lockedFsmInfo = lockedObject.Value;
          if (lockedFsmInfo.NeedRestoreAction)
          {
            VMEngineAPIManager.ExecMethod(lockedFsmInfo.LastEntityMethodExecuteData);
            lockedFsmInfo.NeedRestoreAction = false;
          }
        }
      }
      double num = (Stopwatch.GetTimestamp() - (double) timestamp) / Stopwatch.Frequency;
    }

    public bool LockObject(DynamicFSM lockObjFSM)
    {
      int num = lockObjFSM.Lock(this) ? 1 : 0;
      if (num == 0)
        return num != 0;
      lockedObjects.Add(lockObjFSM.Entity.EngineGuid, new LockedFSMInfo(lockObjFSM));
      return num != 0;
    }

    public bool UnLockObject(DynamicFSM lockObjFSM)
    {
      int num = lockObjFSM.UnLock(this) ? 1 : 0;
      if (num == 0)
        return num != 0;
      lockedObjects.Remove(lockObjFSM.Entity.EngineGuid);
      return num != 0;
    }

    public void StartQuest()
    {
      if (Entity == null)
        return;
      ((QuestComponent) Entity.GetComponentByName("QuestComponent"))?.StartQuest(this);
    }

    public void EndQuest()
    {
      Active = false;
      foreach (KeyValuePair<Guid, LockedFSMInfo> lockedObject in lockedObjects)
        lockedObject.Value.LockedFSM.UnLock(this);
      lockedObjects.Clear();
    }

    public override void OnStart()
    {
      base.OnStart();
      StartQuest();
    }

    public override void SetLockedObjectNeedRestoreAction(DynamicFSM lockedFSM)
    {
      if (lockedObjects.ContainsKey(lockedFSM.Entity.EngineGuid))
        lockedObjects[lockedFSM.Entity.EngineGuid].NeedRestoreAction = true;
      else
        Logger.AddError(string.Format("Lock object restore action error: locked object with guid {0} not found", lockedFSM.Entity.EngineGuid));
    }

    public override void OnAddChildDynamicObject(DynamicFSM childDynFSM)
    {
    }

    public override void OnRemoveChildDynamicObject(DynamicFSM childDynFSM)
    {
      if (childDynFSM == null || childDynFSM.LockingFSM == null || !(EngineGuid == childDynFSM.EngineGuid))
        return;
      UnLockObject(childDynFSM);
    }

    protected override void RememberMetodExecData(EntityMethodExecuteData lastMethodExecData)
    {
      Guid targetEntityGuid = lastMethodExecData.TargetEntityGuid;
      if (!lockedObjects.ContainsKey(targetEntityGuid))
        return;
      lockedObjects[targetEntityGuid].SetLastActionMethodExecData(lastMethodExecData);
    }
  }
}
