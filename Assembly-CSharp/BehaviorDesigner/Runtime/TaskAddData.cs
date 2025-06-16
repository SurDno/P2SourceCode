using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
  public class TaskAddData
  {
    public bool fromExternalTask;
    public ParentTask parentTask = null;
    public int parentIndex = -1;
    public int depth;
    public int compositeParentIndex = -1;
    public Dictionary<string, OverrideFieldValue> overrideFields = null;
    public HashSet<object> overiddenFields = new HashSet<object>();
    public int errorTask = -1;
    public string errorTaskName = "";

    public void Initialize()
    {
      if (overrideFields != null)
      {
        foreach (KeyValuePair<string, OverrideFieldValue> overrideField in overrideFields)
          ObjectPool.Return(overrideField);
      }
      ObjectPool.Return(overrideFields);
      fromExternalTask = false;
      parentTask = null;
      parentIndex = -1;
      depth = 0;
      compositeParentIndex = -1;
      overrideFields = null;
    }
  }
}
