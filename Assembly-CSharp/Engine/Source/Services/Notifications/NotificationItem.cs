using Engine.Common.Commons;
using Inspectors;

namespace Engine.Source.Services.Notifications;

public class NotificationItem {
	[Inspected] public NotificationEnum Type;
	[Inspected] public object[] Values;
}