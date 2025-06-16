namespace BehaviorDesigner.Runtime
{
  public class OverrideFieldValue
  {
    private object value;
    private int depth;

    public object Value => this.value;

    public int Depth => this.depth;

    public void Initialize(object v, int d)
    {
      this.value = v;
      this.depth = d;
    }
  }
}
