using System;

namespace Inspectors;

public class ContextMenuStub : IContextMenu {
	public void AddItem(string name, bool on, Action action) { }

	public void AddSeparator(string name) { }

	public void Show() { }
}