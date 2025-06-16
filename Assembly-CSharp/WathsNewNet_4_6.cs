using System;

public class WathsNewNet_4_6 {
	private bool TestProperty { get; set; } = true;

	private void StringFormat() {
		string.Format("Строка с захватом переменных {0}", 1);
	}

	private event Action Event;

	private void NullOperator() {
		if (Event != null)
			Event();
		var action = Event;
		if (action != null)
			action();
		TestClass testClass = null;
		var obj = testClass?.Value;
		obj = testClass?.Value;
	}

	private void NameofOperator() { }

	public class TestClass {
		public object Value;
	}
}