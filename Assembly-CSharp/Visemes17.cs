public class Visemes17
{
  public string[] visNames;
  public string[][] mapping;

  public Visemes17()
  {
    visNames = [
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
    ];
    mapping = new string[visNames.Length][];
    mapping[0] = ["x"];
    mapping[1] = ["AA"];
    mapping[2] = ["AH", "h"];
    mapping[3] = ["AO"];
    mapping[4] = ["AW", "OW"];
    mapping[5] = ["OY", "UH", "UW"];
    mapping[6] = ["EH", "AE"];
    mapping[7] = ["IH", "AY"];
    mapping[8] = ["EY"];
    mapping[9] = ["y", "IY"];
    mapping[10] = ["r", "ER"];
    mapping[11] = ["l"];
    mapping[12] = ["w"];
    mapping[13] = ["m", "b", "p"];
    mapping[14] = [
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
    mapping[15] = [
      "CH",
      "j",
      "SH",
      "ZH"
    ];
    mapping[16] = ["f", "v"];
  }
}
