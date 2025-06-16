public class Visemes12
{
  public string[] visNames;
  public string[][] mapping;

  public Visemes12()
  {
    visNames = new string[12]
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
    mapping = new string[visNames.Length][];
    mapping[0] = new string[1]{ "x" };
    mapping[1] = new string[3]{ "AA", "AH", "h" };
    mapping[2] = new string[6]
    {
      "AO",
      "AW",
      "OW",
      "OY",
      "UH",
      "UW"
    };
    mapping[3] = new string[4]
    {
      "EH",
      "AE",
      "IH",
      "AY"
    };
    mapping[4] = new string[3]{ "y", "IY", "EY" };
    mapping[5] = new string[2]{ "r", "ER" };
    mapping[6] = new string[1]{ "l" };
    mapping[7] = new string[1]{ "w" };
    mapping[8] = new string[3]{ "m", "b", "p" };
    mapping[9] = new string[10]
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
    mapping[10] = new string[4]
    {
      "CH",
      "j",
      "SH",
      "ZH"
    };
    mapping[11] = new string[2]{ "f", "v" };
  }
}
