public class Visemes12
{
  public string[] visNames;
  public string[][] mapping;

  public Visemes12()
  {
    visNames = [
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
    ];
    mapping = new string[visNames.Length][];
    mapping[0] = ["x"];
    mapping[1] = ["AA", "AH", "h"];
    mapping[2] = [
      "AO",
      "AW",
      "OW",
      "OY",
      "UH",
      "UW"
    ];
    mapping[3] = [
      "EH",
      "AE",
      "IH",
      "AY"
    ];
    mapping[4] = ["y", "IY", "EY"];
    mapping[5] = ["r", "ER"];
    mapping[6] = ["l"];
    mapping[7] = ["w"];
    mapping[8] = ["m", "b", "p"];
    mapping[9] = [
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
    ];
    mapping[10] = [
      "CH",
      "j",
      "SH",
      "ZH"
    ];
    mapping[11] = ["f", "v"];
  }
}
