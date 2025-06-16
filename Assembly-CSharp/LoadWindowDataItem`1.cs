// Decompiled with JetBrains decompiler
// Type: LoadWindowDataItem`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Serializable]
public class LoadWindowDataItem<T>
{
  public T Value;
  public int Weight;
  public bool LimitGameDay;
  public int MinGameDay;
  public int MaxGameDay;
  public bool LimitDeathCount;
  public int MinDeathCount;
  public int MaxDeathCount;
}
