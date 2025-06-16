using Engine.Common.Binders;

namespace Engine.Common.Services.Mails;

[EnumType("MailState")]
public enum MailStateEnum {
	None,
	Available,
	Readed
}