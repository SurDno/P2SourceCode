// Decompiled with JetBrains decompiler
// Type: Scripts.Behaviours.LoadControllers.BaseLoadByDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Scripts.Behaviours.LoadControllers
{
  public abstract class BaseLoadByDistance : MonoBehaviour
  {
    public abstract float LoadDistance { get; }

    public abstract float UnloadDistance { get; }
  }
}
