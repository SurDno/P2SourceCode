// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.OverrideFieldValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
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
