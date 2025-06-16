using BehaviorDesigner.Runtime.Tasks;
using System.Reflection;

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
