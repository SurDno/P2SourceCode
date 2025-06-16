using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services.Mails;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;

namespace Engine.Source.Components;

[Factory(typeof(IMailComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class MailComponent : EngineComponent, IMailComponent, IComponent {
	public MailStateEnum State { get; set; }

	public LocalizedText Header { get; set; }

	public LocalizedText Text { get; set; }
}