using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
  public struct TaskField
  {
    public Task task;
    public FieldInfo fieldInfo;

    public TaskField(Task t, FieldInfo f)
    {
      task = t;
      fieldInfo = f;
    }
  }
}
