// Decompiled with JetBrains decompiler
// Type: IntensityParameter`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
public struct IntensityParameter<T> where T : struct
{
  public float Intensity;
  public T Value;

  public override string ToString()
  {
    return string.Format("{0} : {1}\n{2} : {3}", (object) "Intensity", (object) this.Intensity, (object) "Value", (object) this.Value.ToString());
  }
}
