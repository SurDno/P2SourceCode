public class Visemes17
{
  public string[] visNames = null;
  public string[][] mapping = null;

  public Visemes17()
  {
    visNames = new string[17]
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
    mapping = new string[visNames.Length][];
    mapping[0] = new string[1]{ "x" };
    mapping[1] = new string[1]{ "AA" };
    mapping[2] = new string[2]{ "AH", "h" };
    mapping[3] = new string[1]{ "AO" };
    mapping[4] = new string[2]{ "AW", "OW" };
    mapping[5] = new string[3]{ "OY", "UH", "UW" };
    mapping[6] = new string[2]{ "EH", "AE" };
    mapping[7] = new string[2]{ "IH", "AY" };
    mapping[8] = new string[1]{ "EY" };
    mapping[9] = new string[2]{ "y", "IY" };
    mapping[10] = new string[2]{ "r", "ER" };
    mapping[11] = new string[1]{ "l" };
    mapping[12] = new string[1]{ "w" };
    mapping[13] = new string[3]{ "m", "b", "p" };
    mapping[14] = new string[10]
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
    mapping[15] = new string[4]
    {
      "CH",
      "j",
      "SH",
      "ZH"
    };
    mapping[16] = new string[2]{ "f", "v" };
  }
}
