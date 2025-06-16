// Decompiled with JetBrains decompiler
// Type: ZoneHit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
public struct ZoneHit
{
  public float Opacity;
  public PlagueZone PlagueZone;
  public byte Importance;
  public float Level;

  public ZoneHit(PlagueZone plagueZone, float opacity, byte importance, float level)
  {
    this.PlagueZone = plagueZone;
    this.Opacity = opacity;
    this.Importance = importance;
    this.Level = level;
  }

  public static int Comparison(ZoneHit x, ZoneHit y) => x.Importance.CompareTo(y.Importance);
}
