using System;
using System.Collections;
using Engine.Services.Engine.Assets;
using Inspectors;

namespace Engine.Source.Services.Assets;

public class AssetState {
	[Inspected] public IAsset Asset;
	public Func<bool, IEnumerator> OnLoad;
	public Action OnDispose;
	public IEnumerator Processor;
	public bool NeedDispose;
	public string DisposeReason;
}