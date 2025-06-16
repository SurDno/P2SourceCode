// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Design.SliderFieldAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class SliderFieldAttribute : Attribute
  {
    public float left;
    public float right;

    public SliderFieldAttribute(float left, float right)
    {
      this.left = left;
      this.right = right;
    }

    public SliderFieldAttribute(int left, int right)
    {
      this.left = (float) left;
      this.right = (float) right;
    }
  }
}
