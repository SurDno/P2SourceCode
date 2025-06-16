// Decompiled with JetBrains decompiler
// Type: NocMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class NocMarker
{
  public int msStart;
  public int msEnd;
  public int weight;

  public NocMarker()
  {
    this.msStart = this.msEnd = 0;
    this.weight = 50;
  }

  public int getDuration() => this.msEnd - this.msStart;

  public int getEndMs() => this.msEnd;

  public int getStartMs() => this.msStart;

  public float getEnergy() => (float) this.weight;
}
