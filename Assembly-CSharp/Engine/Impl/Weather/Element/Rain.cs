using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;
using UnityEngine;

namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Rain : IBlendable<Rain>
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    protected Vector2 direction;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    protected float intensity;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    protected float puddlesDryTime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    protected float puddlesFillTime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    protected float terrainDryTime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    protected float terrainFillTime;

    public float Intensity
    {
      get => intensity;
      set => intensity = value;
    }

    public float PuddlesFillTime
    {
      get => puddlesFillTime;
      set => puddlesFillTime = value;
    }

    public float PuddlesDryTime
    {
      get => puddlesDryTime;
      set => puddlesDryTime = value;
    }

    public float TerrainFillTime
    {
      get => terrainFillTime;
      set => terrainFillTime = value;
    }

    public float TerrainDryTime
    {
      get => terrainDryTime;
      set => terrainDryTime = value;
    }

    public Vector2 Direction
    {
      get => direction;
      set => direction = value;
    }

    public void Blend(Rain a, Rain b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      Intensity = blendOperation.Blend(a.Intensity, b.Intensity);
      PuddlesDryTime = blendOperation.Blend(a.PuddlesDryTime, b.PuddlesDryTime);
      PuddlesFillTime = blendOperation.Blend(a.PuddlesFillTime, b.PuddlesFillTime);
      TerrainDryTime = blendOperation.Blend(a.TerrainDryTime, b.TerrainDryTime);
      TerrainFillTime = blendOperation.Blend(a.TerrainFillTime, b.TerrainFillTime);
      Direction = blendOperation.Blend(a.Direction, b.Direction);
    }
  }
}
