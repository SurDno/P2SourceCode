// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Weather.Element.Rain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Rain : IBlendable<Rain>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected Vector2 direction;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float intensity;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float puddlesDryTime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float puddlesFillTime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float terrainDryTime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float terrainFillTime;

    public float Intensity
    {
      get => this.intensity;
      set => this.intensity = value;
    }

    public float PuddlesFillTime
    {
      get => this.puddlesFillTime;
      set => this.puddlesFillTime = value;
    }

    public float PuddlesDryTime
    {
      get => this.puddlesDryTime;
      set => this.puddlesDryTime = value;
    }

    public float TerrainFillTime
    {
      get => this.terrainFillTime;
      set => this.terrainFillTime = value;
    }

    public float TerrainDryTime
    {
      get => this.terrainDryTime;
      set => this.terrainDryTime = value;
    }

    public Vector2 Direction
    {
      get => this.direction;
      set => this.direction = value;
    }

    public void Blend(Rain a, Rain b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Intensity = blendOperation.Blend(a.Intensity, b.Intensity);
      this.PuddlesDryTime = blendOperation.Blend(a.PuddlesDryTime, b.PuddlesDryTime);
      this.PuddlesFillTime = blendOperation.Blend(a.PuddlesFillTime, b.PuddlesFillTime);
      this.TerrainDryTime = blendOperation.Blend(a.TerrainDryTime, b.TerrainDryTime);
      this.TerrainFillTime = blendOperation.Blend(a.TerrainFillTime, b.TerrainFillTime);
      this.Direction = blendOperation.Blend(a.Direction, b.Direction);
    }
  }
}
