public class NocMarker
{
  public int msStart;
  public int msEnd;
  public int weight;

  public NocMarker()
  {
    this.msStart = this.msEnd = 0;
    this.weight = 50;
  }

  public int getDuration() => this.msEnd - this.msStart;

  public int getEndMs() => this.msEnd;

  public int getStartMs() => this.msStart;

  public float getEnergy() => (float) this.weight;
}
