using Engine.Common;
using Inspectors;

namespace Engine.Source.Components.Crowds;

public class IndoorPointInfo : PointInfo {
	[Inspected] public CrowdTemplateInfo TemplateInfo;
	[Inspected] public IEntity Template;
}