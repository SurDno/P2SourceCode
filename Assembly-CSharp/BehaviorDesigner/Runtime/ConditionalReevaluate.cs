using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
  public class ConditionalReevaluate
  {
    public int index;
    public TaskStatus taskStatus;
    public int compositeIndex = -1;
    public int stackIndex = -1;

    public void Initialize(int i, TaskStatus status, int stack, int composite)
    {
      this.index = i;
      this.taskStatus = status;
      this.stackIndex = stack;
      this.compositeIndex = composite;
    }
  }
}
