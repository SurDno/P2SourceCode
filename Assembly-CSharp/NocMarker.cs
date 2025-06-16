public class NocMarker
{
  public int msStart;
  public int msEnd;
  public int weight;

  public NocMarker()
  {
    msStart = msEnd = 0;
    weight = 50;
  }

  public int getDuration() => msEnd - msStart;

  public int getEndMs() => msEnd;

  public int getStartMs() => msStart;

  public float getEnergy() => weight;
}
