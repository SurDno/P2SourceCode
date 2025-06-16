// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.GUIPort
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace NodeCanvas.Framework
{
  public class GUIPort
  {
    public readonly int portIndex;
    public readonly Node parent;
    public readonly Vector2 pos;

    public GUIPort(int index, Node parent, Vector2 pos)
    {
      this.portIndex = index;
      this.parent = parent;
      this.pos = pos;
    }
  }
}
