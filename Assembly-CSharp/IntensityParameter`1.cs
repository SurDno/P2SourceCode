public struct IntensityParameter<T> where T : struct
{
  public float Intensity;
  public T Value;

  public override string ToString()
  {
    return string.Format("{0} : {1}\n{2} : {3}", (object) "Intensity", (object) this.Intensity, (object) "Value", (object) this.Value.ToString());
  }
}
