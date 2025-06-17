using UnityEngine;

namespace NodeCanvas.Framework
{
  public class GUIPort(int index, Node parent, Vector2 pos) {
    public readonly int portIndex = index;
    public readonly Node parent = parent;
    public readonly Vector2 pos = pos;
  }
}
