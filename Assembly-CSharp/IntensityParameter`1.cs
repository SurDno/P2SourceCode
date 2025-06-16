public struct IntensityParameter<T> where T : struct {
	public float Intensity;
	public T Value;

	public override string ToString() {
		return string.Format("{0} : {1}\n{2} : {3}", "Intensity", Intensity, "Value", Value.ToString());
	}
}