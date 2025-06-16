using System;
using SRF;

namespace SRDebugger.Services;

public class Info : ISystemInfo {
	private Func<object> _valueGetter;

	public string Title { get; set; }

	public object Value {
		get {
			try {
				return _valueGetter();
			} catch (Exception ex) {
				return "Error ({0})".Fmt(ex.GetType().Name);
			}
		}
	}

	public bool IsPrivate { get; set; }

	public static Info Create(string name, Func<object> getter, bool isPrivate = false) {
		return new Info {
			Title = name,
			_valueGetter = getter,
			IsPrivate = isPrivate
		};
	}

	public static Info Create(string name, object value, bool isPrivate = false) {
		return new Info {
			Title = name,
			_valueGetter = () => value,
			IsPrivate = isPrivate
		};
	}
}