using UnityEngine;

namespace AssetDatabases;

public class AsyncLoadFromResources : IAsyncLoad {
	private ResourceRequest operation;

	public AsyncLoadFromResources(ResourceRequest operation) {
		this.operation = operation;
	}

	public object Asset => operation.asset;

	public bool IsDone => operation.isDone;
}