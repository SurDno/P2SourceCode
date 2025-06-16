namespace NodeCanvas.Framework
{
  public class GUIPort
  {
    public readonly int portIndex;
    public readonly Node parent;
    public readonly Vector2 pos;

    public GUIPort(int index, Node parent, Vector2 pos)
    {
      portIndex = index;
      this.parent = parent;
      this.pos = pos;
    }
  }
}
