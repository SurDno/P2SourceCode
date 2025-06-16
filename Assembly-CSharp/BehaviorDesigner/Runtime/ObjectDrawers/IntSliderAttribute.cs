// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.ObjectDrawers.IntSliderAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;
using System;

#nullable disable
namespace BehaviorDesigner.Runtime.ObjectDrawers
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class IntSliderAttribute : ObjectDrawerAttribute
  {
    public int min;
    public int max;

    public IntSliderAttribute(int min, int max)
    {
      this.min = min;
      this.max = max;
    }
  }
}
