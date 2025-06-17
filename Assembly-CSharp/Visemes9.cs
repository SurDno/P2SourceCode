public class Visemes9
{
  public string[] visNames;
  public string[][] mapping;

  public Visemes9()
  {
    visNames = [
      "x",
      "AA",
      "AO",
      "EH",
      "y",
      "r",
      "L",
      "CH",
      "n"
    ];
    mapping = new string[visNames.Length][];
    mapping[0] = [
      "x",
      "f",
      "v",
      "m",
      "b",
      "p"
    ];
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
    mapping[7] = [
      "w",
      "CH",
      "j",
      "SH",
      "ZH"
    ];
    mapping[8] = [
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
  }
}
