// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.ProfilerFrame
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SRDebugger.Services
{
  public struct ProfilerFrame
  {
    public double FrameTime;
    public double OtherTime;
    public double RenderTime;
    public double CustomTime;
    public double UpdateTime;
  }
}
