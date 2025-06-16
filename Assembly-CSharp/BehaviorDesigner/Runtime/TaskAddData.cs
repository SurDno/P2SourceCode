// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.TaskAddData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public class TaskAddData
  {
    public bool fromExternalTask = false;
    public ParentTask parentTask = (ParentTask) null;
    public int parentIndex = -1;
    public int depth = 0;
    public int compositeParentIndex = -1;
    public Dictionary<string, OverrideFieldValue> overrideFields = (Dictionary<string, OverrideFieldValue>) null;
    public HashSet<object> overiddenFields = new HashSet<object>();
    public int errorTask = -1;
    public string errorTaskName = "";

    public void Initialize()
    {
      if (this.overrideFields != null)
      {
        foreach (KeyValuePair<string, OverrideFieldValue> overrideField in this.overrideFields)
          ObjectPool.Return<KeyValuePair<string, OverrideFieldValue>>(overrideField);
      }
      ObjectPool.Return<Dictionary<string, OverrideFieldValue>>(this.overrideFields);
      this.fromExternalTask = false;
      this.parentTask = (ParentTask) null;
      this.parentIndex = -1;
      this.depth = 0;
      this.compositeParentIndex = -1;
      this.overrideFields = (Dictionary<string, OverrideFieldValue>) null;
    }
  }
}
