// Decompiled with JetBrains decompiler
// Type: TOD_Clouds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TOD_Clouds
{
  private static TOD_Clouds instance;
  private const float HexX = 0.6666667f;
  private const float HexHalfX = 0.333333343f;
  private const float HexY = 0.471404523f;
  private readonly int cloudAttenuationId;
  private readonly int cloudCoverageId;
  private readonly int cloudDensityId;
  private readonly int cloudOffsetId;
  private readonly int cloudOpacityId;
  private readonly int cloudSizeId;
  private readonly int cloudWindId;
  private readonly Vector4 stepsize;
  private readonly Vector4 sumy;
  private readonly Vector4 sumz;

  public static TOD_Clouds Instance
  {
    get
    {
      if (TOD_Clouds.instance == null)
        TOD_Clouds.instance = new TOD_Clouds();
      return TOD_Clouds.instance;
    }
  }

  public Vector3 CloudLayerDensity(Texture2D densityTex, Vector4 uv, Vector3 viewDir)
  {
    Vector3 zero = Vector3.zero;
    Vector2 vector2 = new Vector2(viewDir.x, viewDir.z);
    Color pixelBilinear1 = densityTex.GetPixelBilinear(uv.x + vector2.x * this.stepsize.x, uv.y + vector2.y * this.stepsize.x);
    Color pixelBilinear2 = densityTex.GetPixelBilinear(uv.z + vector2.x * this.stepsize.y, uv.w + vector2.y * this.stepsize.y);
    Color pixelBilinear3 = densityTex.GetPixelBilinear(uv.x + vector2.x * this.stepsize.z, uv.y + vector2.y * this.stepsize.z);
    Color pixelBilinear4 = densityTex.GetPixelBilinear(uv.z + vector2.x * this.stepsize.w, uv.w + vector2.y * this.stepsize.w);
    Vector4 a1 = new Vector4(pixelBilinear1.r, pixelBilinear1.g + pixelBilinear2.g, pixelBilinear1.b + pixelBilinear2.b + pixelBilinear3.b, pixelBilinear1.a + pixelBilinear2.a + pixelBilinear3.a + pixelBilinear4.a);
    Vector4 a2 = new Vector4(pixelBilinear1.r, pixelBilinear2.g, pixelBilinear3.b, pixelBilinear4.a);
    zero.y += Vector4.Dot(a1, this.sumy);
    zero.z += Vector4.Dot(a2, this.sumz);
    float globalFloat1 = Shader.GetGlobalFloat(this.cloudCoverageId);
    float globalFloat2 = Shader.GetGlobalFloat(this.cloudAttenuationId);
    float globalFloat3 = Shader.GetGlobalFloat(this.cloudDensityId);
    zero.y = (zero.y - globalFloat1) * globalFloat2;
    zero.z = (zero.z - globalFloat1) * globalFloat3;
    zero.x = Mathf.Clamp01(zero.z);
    return zero;
  }

  public float CloudOpacity() => Shader.GetGlobalFloat(this.cloudOpacityId);

  public Vector3 CloudPosition(Vector3 viewDir)
  {
    float num = 1f / Mathf.Lerp(0.15f, 1f, viewDir.y);
    Vector3 globalVector = (Vector3) Shader.GetGlobalVector(this.cloudSizeId);
    return new Vector3(viewDir.x * num / globalVector.x, 1f / globalVector.y, viewDir.z * num / globalVector.z);
  }

  public float LightAttenuation(Texture2D densityTexture, Vector3 lightPosition)
  {
    lightPosition = this.CloudPosition(lightPosition);
    Vector4 uv = this.CloudUV(lightPosition);
    try
    {
      return Mathf.Clamp01((float) (1.0 - (double) this.CloudLayerDensity(densityTexture, uv, lightPosition).x * (double) this.CloudOpacity()));
    }
    catch
    {
      return 1f;
    }
  }

  public float LightAttenuation(Texture2D densityTexture, Vector3 lightPosition, float lightSize)
  {
    Vector3 normalized1 = Vector3.Cross(lightPosition, Vector3.up).normalized;
    Vector3 normalized2 = Vector3.Cross(lightPosition, normalized1).normalized;
    Vector3 vector3_1 = normalized1 * (lightSize * 0.6666667f);
    Vector3 vector3_2 = vector3_1 / 2f;
    Vector3 vector3_3 = normalized2 * (lightSize * 0.471404523f);
    float num1 = this.LightAttenuation(densityTexture, lightPosition);
    Vector3 normalized3 = (lightPosition + vector3_1).normalized;
    float num2 = num1 + this.LightAttenuation(densityTexture, normalized3);
    Vector3 normalized4 = (lightPosition - vector3_1).normalized;
    float num3 = num2 + this.LightAttenuation(densityTexture, normalized4);
    Vector3 normalized5 = (lightPosition - vector3_2 - vector3_3).normalized;
    float num4 = num3 + this.LightAttenuation(densityTexture, normalized5);
    Vector3 normalized6 = (lightPosition - vector3_2 + vector3_3).normalized;
    float num5 = num4 + this.LightAttenuation(densityTexture, normalized6);
    Vector3 normalized7 = (lightPosition + vector3_2 - vector3_3).normalized;
    float num6 = num5 + this.LightAttenuation(densityTexture, normalized7);
    Vector3 normalized8 = (lightPosition + vector3_2 + vector3_3).normalized;
    return (num6 + this.LightAttenuation(densityTexture, normalized8)) / 7f;
  }

  public Vector4 CloudUV(Vector3 cloudPos)
  {
    Vector4 zero = Vector4.zero;
    Vector3 globalVector1 = (Vector3) Shader.GetGlobalVector(this.cloudOffsetId);
    Vector3 globalVector2 = (Vector3) Shader.GetGlobalVector(this.cloudWindId);
    float f = 0.17453292f;
    Vector2 vector2_1 = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
    Vector2 vector2_2 = new Vector2(Mathf.Sin(f), Mathf.Cos(f));
    Vector2 vector2_3 = vector2_1 * cloudPos.x + vector2_2 * cloudPos.z;
    zero.x = cloudPos.x + globalVector1.x + globalVector2.x;
    zero.y = cloudPos.z + globalVector1.z + globalVector2.z;
    zero.z = vector2_3.x + globalVector1.x + globalVector2.x;
    zero.w = vector2_3.y + globalVector1.z + globalVector2.z;
    return zero;
  }

  public TOD_Clouds()
  {
    this.cloudAttenuationId = Shader.PropertyToID("TOD_CloudAttenuation");
    this.cloudCoverageId = Shader.PropertyToID("TOD_CloudCoverage");
    this.cloudDensityId = Shader.PropertyToID("TOD_CloudDensity");
    this.cloudOffsetId = Shader.PropertyToID("TOD_CloudOffset");
    this.cloudOpacityId = Shader.PropertyToID("TOD_CloudOpacity");
    this.cloudSizeId = Shader.PropertyToID("TOD_CloudSize");
    this.cloudWindId = Shader.PropertyToID("TOD_CloudWind");
    this.stepsize = new Vector4(0.0f, 1f, 2f, 3f) * 0.1f;
    this.sumy = new Vector4(0.5f, 0.125f, 0.0416666679f, 1f / 64f);
    this.sumz = new Vector4(0.5f, 0.25f, 0.125f, 1f / 16f);
  }
}
