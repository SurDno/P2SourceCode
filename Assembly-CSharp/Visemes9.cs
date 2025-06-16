public class Visemes9
{
  public string[] visNames = null;
  public string[][] mapping = null;

  public Visemes9()
  {
    visNames = new string[9]
    {
      "x",
      "AA",
      "AO",
      "EH",
      "y",
      "r",
      "L",
      "CH",
      "n"
    };
    mapping = new string[visNames.Length][];
    mapping[0] = new string[6]
    {
      "x",
      "f",
      "v",
      "m",
      "b",
      "p"
    };
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
    mapping[7] = new string[5]
    {
      "w",
      "CH",
      "j",
      "SH",
      "ZH"
    };
    mapping[8] = new string[10]
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
  }
}
