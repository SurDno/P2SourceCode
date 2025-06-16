// Decompiled with JetBrains decompiler
// Type: Visemes12
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class Visemes12
{
  public string[] visNames = (string[]) null;
  public string[][] mapping = (string[][]) null;

  public Visemes12()
  {
    this.visNames = new string[12]
    {
      "x",
      "AA",
      "AO",
      "EH",
      "y",
      "r",
      "L",
      "w",
      "m",
      "n",
      "CH",
      "f"
    };
    this.mapping = new string[this.visNames.Length][];
    this.mapping[0] = new string[1]{ "x" };
    this.mapping[1] = new string[3]{ "AA", "AH", "h" };
    this.mapping[2] = new string[6]
    {
      "AO",
      "AW",
      "OW",
      "OY",
      "UH",
      "UW"
    };
    this.mapping[3] = new string[4]
    {
      "EH",
      "AE",
      "IH",
      "AY"
    };
    this.mapping[4] = new string[3]{ "y", "IY", "EY" };
    this.mapping[5] = new string[2]{ "r", "ER" };
    this.mapping[6] = new string[1]{ "l" };
    this.mapping[7] = new string[1]{ "w" };
    this.mapping[8] = new string[3]{ "m", "b", "p" };
    this.mapping[9] = new string[10]
    {
      "n",
      "NG",
      "DH",
      "d",
      "g",
      "t",
      "z",
      "s",
      "TH",
      "k"
    };
    this.mapping[10] = new string[4]
    {
      "CH",
      "j",
      "SH",
      "ZH"
    };
    this.mapping[11] = new string[2]{ "f", "v" };
  }
}
