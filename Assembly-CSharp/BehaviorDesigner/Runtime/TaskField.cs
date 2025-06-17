using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
  public struct TaskField(Task t, FieldInfo f) {
    public Task task = t;
    public FieldInfo fieldInfo = f;
  }
}
