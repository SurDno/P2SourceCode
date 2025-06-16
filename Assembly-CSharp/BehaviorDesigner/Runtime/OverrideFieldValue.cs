namespace BehaviorDesigner.Runtime;

public class OverrideFieldValue {
	private object value;
	private int depth;

	public object Value => value;

	public int Depth => depth;

	public void Initialize(object v, int d) {
		value = v;
		depth = d;
	}
}