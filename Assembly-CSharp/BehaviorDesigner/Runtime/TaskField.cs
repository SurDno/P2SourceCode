// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.TaskField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;
using System.Reflection;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public struct TaskField
  {
    public Task task;
    public FieldInfo fieldInfo;

    public TaskField(Task t, FieldInfo f)
    {
      this.task = t;
      this.fieldInfo = f;
    }
  }
}
