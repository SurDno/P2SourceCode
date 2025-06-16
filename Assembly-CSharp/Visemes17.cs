public class Visemes17
{
  public string[] visNames = (string[]) null;
  public string[][] mapping = (string[][]) null;

  public Visemes17()
  {
    this.visNames = new string[17]
    {
      "x",
      "AA",
      "AH",
      "AO",
      "AW",
      "OY",
      "EH",
      "IH",
      "EY",
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
    this.mapping[1] = new string[1]{ "AA" };
    this.mapping[2] = new string[2]{ "AH", "h" };
    this.mapping[3] = new string[1]{ "AO" };
    this.mapping[4] = new string[2]{ "AW", "OW" };
    this.mapping[5] = new string[3]{ "OY", "UH", "UW" };
    this.mapping[6] = new string[2]{ "EH", "AE" };
    this.mapping[7] = new string[2]{ "IH", "AY" };
    this.mapping[8] = new string[1]{ "EY" };
    this.mapping[9] = new string[2]{ "y", "IY" };
    this.mapping[10] = new string[2]{ "r", "ER" };
    this.mapping[11] = new string[1]{ "l" };
    this.mapping[12] = new string[1]{ "w" };
    this.mapping[13] = new string[3]{ "m", "b", "p" };
    this.mapping[14] = new string[10]
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
    this.mapping[15] = new string[4]
    {
      "CH",
      "j",
      "SH",
      "ZH"
    };
    this.mapping[16] = new string[2]{ "f", "v" };
  }
}
