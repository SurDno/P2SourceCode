using System;

namespace TriangleNet.Log;

public class SimpleLogItem : ILogItem {
	private string info;
	private LogLevel level;
	private string message;
	private DateTime time;

	public SimpleLogItem(LogLevel level, string message)
		: this(level, message, "") { }

	public SimpleLogItem(LogLevel level, string message, string info) {
		time = DateTime.Now;
		this.level = level;
		this.message = message;
		this.info = info;
	}

	public DateTime Time => time;

	public LogLevel Level => level;

	public string Message => message;

	public string Info => info;
}