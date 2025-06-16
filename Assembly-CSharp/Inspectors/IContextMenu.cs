using System;

namespace Inspectors;

public interface IContextMenu {
	void AddItem(string name, bool on, Action action);

	void AddSeparator(string name);

	void Show();
}