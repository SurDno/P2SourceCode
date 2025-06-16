using System.Collections.Generic;
using AssetDatabases;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using InputServices;
using Inspectors;
using Scripts.Inspectors;

public class EngineInspector : MonoBehaviourInstance<EngineInspector> {
	[Inspected] private ICursorController CursorController => CursorService.Instance;

	[Inspected] private InputService InputService => InputService.Instance;

	[Inspected] private UnityEngineInfo UnityEngineInfo => UnityEngineInfo.Instance;

	[Inspected(Mode = ExecuteMode.EditAndRuntime)]
	private EngineApplication EngineApplication => InstanceByRequest<EngineApplication>.Instance;

	[Inspected] private UpdateService UpdateService => InstanceByRequest<UpdateService>.Instance;

	[Inspected(Mode = ExecuteMode.EditAndRuntime)]
	private IEnumerable<object> Services => ServiceLocator.GetServices();

	[Inspected] private UnityHierarchy UnityHierarchy => UnityHierarchy.Instance;

	[Inspected] private IAssetDatabase AssetDatabase => AssetDatabaseService.Instance;

	[Inspected] public IEntity Player => ServiceLocator.GetService<ISimulation>().Player;
}