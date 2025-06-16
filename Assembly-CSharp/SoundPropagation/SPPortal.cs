// Decompiled with JetBrains decompiler
// Type: SoundPropagation.SPPortal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  public class SPPortal : MonoBehaviour
  {
    public SPCell CellA;
    public SPCell CellB;
    public float Occlusion = 0.0f;
    private bool initialized = false;
    private Shape[] shapes = (Shape[]) null;

    private void Check()
    {
      if (this.initialized)
        return;
      this.shapes = this.GetComponentsInChildren<Shape>();
      this.initialized = true;
    }

    public bool ClosestPointToSegment(Vector3 pointA, Vector3 pointB, out Vector3 output)
    {
      this.Check();
      if (this.shapes.Length < 1)
      {
        output = Vector3.zero;
        return false;
      }
      if (this.shapes.Length == 1)
        return this.shapes[0].ClosestPointToSegment(pointA, pointB, out output);
      float num1 = float.MaxValue;
      Vector3 vector3 = Vector3.zero;
      bool segment = false;
      for (int index = 0; index < this.shapes.Length; ++index)
      {
        Vector3 output1;
        if (this.shapes[0].ClosestPointToSegment(pointA, pointB, out output1))
        {
          float num2 = Vector3.Distance(pointA, output1) + Vector3.Distance(output1, pointB);
          if ((double) num2 < (double) num1)
          {
            vector3 = output1;
            num1 = num2;
            segment = true;
          }
        }
      }
      output = vector3;
      return segment;
    }

    public float Loss => this.Occlusion;
  }
}
