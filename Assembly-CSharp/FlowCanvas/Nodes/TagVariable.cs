using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[DoNotList]
[Name("Easy Tag")]
[Description("An easy way to get a Tag name")]
public class TagVariable : VariableNode {
	[TagField] public string value = "Untagged";

	protected override void RegisterPorts() {
		AddValueOutput("Tag", () => value);
	}

	public override void SetVariable(object o) {
		if (!(o is string))
			return;
		value = (string)o;
	}
}