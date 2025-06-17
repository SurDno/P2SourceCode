using System;
using System.Collections.Generic;
using System.Linq;

namespace TriangleNet
{
  internal class Sampler
  {
    private static Random rand = new(DateTime.Now.Millisecond);
    private static int samplefactor = 11;
    private int[] keys;
    private int samples = 1;
    private int triangleCount;

    public void Reset()
    {
      samples = 1;
      triangleCount = 0;
    }

    public void Update(Mesh mesh) => Update(mesh, false);

    public void Update(Mesh mesh, bool forceUpdate)
    {
      int count = mesh.triangles.Count;
      if (!(triangleCount != count | forceUpdate))
        return;
      triangleCount = count;
      while (samplefactor * samples * samples * samples < count)
        ++samples;
      keys = mesh.triangles.Keys.ToArray();
    }

    public int[] GetSamples(Mesh mesh)
    {
      List<int> intList = new List<int>(samples);
      int num = triangleCount / samples;
      for (int index1 = 0; index1 < samples; ++index1)
      {
        int index2 = rand.Next(index1 * num, (index1 + 1) * num - 1);
        if (!mesh.triangles.Keys.Contains(keys[index2]))
        {
          Update(mesh, true);
          --index1;
        }
        else
          intList.Add(keys[index2]);
      }
      return intList.ToArray();
    }
  }
}
