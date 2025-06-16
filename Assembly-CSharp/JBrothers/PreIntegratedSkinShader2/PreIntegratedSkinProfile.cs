using UnityEngine;

namespace JBrothers.PreIntegratedSkinShader2
{
  public class PreIntegratedSkinProfile : ScriptableObject
  {
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_sqrtvar1234;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_transl123_sqrtvar5;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_transl456_sqrtvar6;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_weighths1_var1;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_weighths2_var2;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_weighths3_var3;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_weighths4_var4;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_weighths5_var5;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileHigh_weighths6_var6;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileLow_sqrtvar12;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileLow_transl;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileLow_weighths1_var1;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileLow_weighths2_var2;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileMedium_sqrtvar123;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileMedium_transl123;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileMedium_weighths1_var1;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileMedium_weighths2_var2;
    [HideInInspector]
    [SerializeField]
    private Vector4 _PSSProfileMedium_weighths3_var3;
    public Vector4 gauss6_1;
    public Vector4 gauss6_2;
    public Vector4 gauss6_3;
    public Vector4 gauss6_4;
    public Vector4 gauss6_5;
    public Vector4 gauss6_6;
    [HideInInspector]
    public bool needsRecalcDerived = true;
    [HideInInspector]
    public bool needsRenormalize = true;
    [HideInInspector]
    public Texture2D referenceTexture;

    public void NormalizeOriginalWeights()
    {
      RecalculateDerived();
      gauss6_1.x = _PSSProfileHigh_weighths1_var1.x;
      gauss6_1.y = _PSSProfileHigh_weighths1_var1.y;
      gauss6_1.z = _PSSProfileHigh_weighths1_var1.z;
      gauss6_2.x = _PSSProfileHigh_weighths2_var2.x;
      gauss6_2.y = _PSSProfileHigh_weighths2_var2.y;
      gauss6_2.z = _PSSProfileHigh_weighths2_var2.z;
      gauss6_3.x = _PSSProfileHigh_weighths3_var3.x;
      gauss6_3.y = _PSSProfileHigh_weighths3_var3.y;
      gauss6_3.z = _PSSProfileHigh_weighths3_var3.z;
      gauss6_4.x = _PSSProfileHigh_weighths4_var4.x;
      gauss6_4.y = _PSSProfileHigh_weighths4_var4.y;
      gauss6_4.z = _PSSProfileHigh_weighths4_var4.z;
      gauss6_5.x = _PSSProfileHigh_weighths5_var5.x;
      gauss6_5.y = _PSSProfileHigh_weighths5_var5.y;
      gauss6_5.z = _PSSProfileHigh_weighths5_var5.z;
      gauss6_6.x = _PSSProfileHigh_weighths6_var6.x;
      gauss6_6.y = _PSSProfileHigh_weighths6_var6.y;
      gauss6_6.z = _PSSProfileHigh_weighths6_var6.z;
      needsRenormalize = false;
    }

    public void RecalculateDerived()
    {
      Vector3 zero = Vector3.zero;
      Vector3 gauss61 = gauss6_1;
      Vector3 gauss62 = gauss6_2;
      Vector3 gauss63 = gauss6_3;
      Vector3 gauss64 = gauss6_4;
      Vector3 gauss65 = gauss6_5;
      Vector3 gauss66 = gauss6_6;
      Vector3 vector3_1 = zero + gauss61 + gauss62 + gauss63 + gauss64 + gauss65 + gauss66;
      gauss61.x /= vector3_1.x;
      gauss61.y /= vector3_1.y;
      gauss61.z /= vector3_1.z;
      gauss62.x /= vector3_1.x;
      gauss62.y /= vector3_1.y;
      gauss62.z /= vector3_1.z;
      gauss63.x /= vector3_1.x;
      gauss63.y /= vector3_1.y;
      gauss63.z /= vector3_1.z;
      gauss64.x /= vector3_1.x;
      gauss64.y /= vector3_1.y;
      gauss64.z /= vector3_1.z;
      gauss65.x /= vector3_1.x;
      gauss65.y /= vector3_1.y;
      gauss65.z /= vector3_1.z;
      gauss66.x /= vector3_1.x;
      gauss66.y /= vector3_1.y;
      gauss66.z /= vector3_1.z;
      float grayscale1 = new Color(gauss61.x, gauss61.y, gauss61.z).grayscale;
      float grayscale2 = new Color(gauss62.x, gauss62.y, gauss62.z).grayscale;
      float grayscale3 = new Color(gauss63.x, gauss63.y, gauss63.z).grayscale;
      float grayscale4 = new Color(gauss64.x, gauss64.y, gauss64.z).grayscale;
      float grayscale5 = new Color(gauss65.x, gauss65.y, gauss65.z).grayscale;
      float grayscale6 = new Color(gauss66.x, gauss66.y, gauss66.z).grayscale;
      _PSSProfileHigh_weighths1_var1 = new Vector4(gauss61.x, gauss61.y, gauss61.z, gauss6_1.w);
      _PSSProfileHigh_weighths2_var2 = new Vector4(gauss62.x, gauss62.y, gauss62.z, gauss6_2.w);
      _PSSProfileHigh_weighths3_var3 = new Vector4(gauss63.x, gauss63.y, gauss63.z, gauss6_3.w);
      _PSSProfileHigh_weighths4_var4 = new Vector4(gauss64.x, gauss64.y, gauss64.z, gauss6_4.w);
      _PSSProfileHigh_weighths5_var5 = new Vector4(gauss65.x, gauss65.y, gauss65.z, gauss6_5.w);
      _PSSProfileHigh_weighths6_var6 = new Vector4(gauss66.x, gauss66.y, gauss66.z, gauss6_6.w);
      _PSSProfileMedium_weighths1_var1 = new Vector4(gauss61.x + gauss62.x, gauss61.y + gauss62.y, gauss61.z + gauss62.z, (float) ((gauss6_1.w * (double) grayscale1 + gauss6_2.w * (double) grayscale2) / (grayscale1 + (double) grayscale2)));
      _PSSProfileMedium_weighths2_var2 = new Vector4(gauss63.x + gauss64.x, gauss63.y + gauss64.y, gauss63.z + gauss64.z, (float) ((gauss6_3.w * (double) grayscale3 + gauss6_4.w * (double) grayscale4) / (grayscale3 + (double) grayscale4)));
      _PSSProfileMedium_weighths3_var3 = new Vector4(gauss65.x + gauss66.x, gauss65.y + gauss66.y, gauss65.z + gauss66.z, (float) ((gauss6_5.w * (double) grayscale5 + gauss6_6.w * (double) grayscale6) / (grayscale5 + (double) grayscale6)));
      _PSSProfileLow_weighths1_var1 = new Vector4(gauss61.x + gauss62.x + gauss63.x, gauss61.y + gauss62.y + gauss63.y, gauss61.z + gauss62.z + gauss63.z, (float) ((gauss6_1.w * (double) grayscale1 + gauss6_2.w * (double) grayscale2 + gauss6_3.w * (double) grayscale3) / (grayscale1 + (double) grayscale2 + grayscale3)));
      _PSSProfileLow_weighths2_var2 = new Vector4(gauss64.x + gauss65.x + gauss66.x, gauss64.y + gauss65.y + gauss66.y, gauss64.z + gauss65.z + gauss66.z, (float) ((gauss6_4.w * (double) grayscale4 + gauss6_5.w * (double) grayscale5 + gauss6_6.w * (double) grayscale6) / (grayscale4 + (double) grayscale5 + grayscale6)));
      _PSSProfileHigh_sqrtvar1234.x = Mathf.Sqrt(_PSSProfileHigh_weighths1_var1.w);
      _PSSProfileHigh_sqrtvar1234.y = Mathf.Sqrt(_PSSProfileHigh_weighths2_var2.w);
      _PSSProfileHigh_sqrtvar1234.z = Mathf.Sqrt(_PSSProfileHigh_weighths3_var3.w);
      _PSSProfileHigh_sqrtvar1234.w = Mathf.Sqrt(_PSSProfileHigh_weighths4_var4.w);
      _PSSProfileMedium_sqrtvar123.x = Mathf.Sqrt(_PSSProfileMedium_weighths1_var1.w);
      _PSSProfileMedium_sqrtvar123.y = Mathf.Sqrt(_PSSProfileMedium_weighths2_var2.w);
      _PSSProfileMedium_sqrtvar123.z = Mathf.Sqrt(_PSSProfileMedium_weighths3_var3.w);
      _PSSProfileLow_sqrtvar12.x = Mathf.Sqrt(_PSSProfileLow_weighths1_var1.w);
      _PSSProfileLow_sqrtvar12.y = Mathf.Sqrt(_PSSProfileLow_weighths2_var2.w);
      _PSSProfileHigh_transl123_sqrtvar5.w = Mathf.Sqrt(_PSSProfileHigh_weighths5_var5.w);
      _PSSProfileHigh_transl456_sqrtvar6.w = Mathf.Sqrt(_PSSProfileHigh_weighths6_var6.w);
      float num = -1.442695f;
      _PSSProfileHigh_transl123_sqrtvar5.x = num / gauss6_1.w;
      _PSSProfileHigh_transl123_sqrtvar5.y = num / gauss6_2.w;
      _PSSProfileHigh_transl123_sqrtvar5.z = num / gauss6_3.w;
      _PSSProfileHigh_transl456_sqrtvar6.x = num / gauss6_4.w;
      _PSSProfileHigh_transl456_sqrtvar6.y = num / gauss6_5.w;
      _PSSProfileHigh_transl456_sqrtvar6.z = num / gauss6_6.w;
      _PSSProfileMedium_transl123.x = num / _PSSProfileMedium_weighths1_var1.w;
      _PSSProfileMedium_transl123.y = num / _PSSProfileMedium_weighths2_var2.w;
      _PSSProfileMedium_transl123.z = num / _PSSProfileMedium_weighths3_var3.w;
      Vector3 vector3_2;
      vector3_2.x = (float) (gauss6_1.w * (double) gauss61.x + gauss6_2.w * (double) gauss62.x + gauss6_3.w * (double) gauss63.x + gauss6_4.w * (double) gauss64.x + gauss6_5.w * (double) gauss65.x + gauss6_6.w * (double) gauss66.x);
      vector3_2.y = (float) (gauss6_1.w * (double) gauss61.y + gauss6_2.w * (double) gauss62.y + gauss6_3.w * (double) gauss63.y + gauss6_4.w * (double) gauss64.y + gauss6_5.w * (double) gauss65.y + gauss6_6.w * (double) gauss66.y);
      vector3_2.z = (float) (gauss6_1.w * (double) gauss61.z + gauss6_2.w * (double) gauss62.z + gauss6_3.w * (double) gauss63.z + gauss6_4.w * (double) gauss64.z + gauss6_5.w * (double) gauss65.z + gauss6_6.w * (double) gauss66.z);
      _PSSProfileLow_transl.x = num / vector3_2.x;
      _PSSProfileLow_transl.y = num / vector3_2.y;
      _PSSProfileLow_transl.z = num / vector3_2.z;
      needsRecalcDerived = false;
    }

    public void ApplyProfile(Material material) => ApplyProfile(material, false);

    public void ApplyProfile(Material material, bool noWarn)
    {
      if (needsRecalcDerived)
        RecalculateDerived();
      material.SetVector("_PSSProfileHigh_weighths1_var1", _PSSProfileHigh_weighths1_var1);
      material.SetVector("_PSSProfileHigh_weighths2_var2", _PSSProfileHigh_weighths2_var2);
      material.SetVector("_PSSProfileHigh_weighths3_var3", _PSSProfileHigh_weighths3_var3);
      material.SetVector("_PSSProfileHigh_weighths4_var4", _PSSProfileHigh_weighths4_var4);
      material.SetVector("_PSSProfileHigh_weighths5_var5", _PSSProfileHigh_weighths5_var5);
      material.SetVector("_PSSProfileHigh_weighths6_var6", _PSSProfileHigh_weighths6_var6);
      material.SetVector("_PSSProfileHigh_sqrtvar1234", _PSSProfileHigh_sqrtvar1234);
      material.SetVector("_PSSProfileHigh_transl123_sqrtvar5", _PSSProfileHigh_transl123_sqrtvar5);
      material.SetVector("_PSSProfileHigh_transl456_sqrtvar6", _PSSProfileHigh_transl456_sqrtvar6);
      material.SetVector("_PSSProfileMedium_weighths1_var1", _PSSProfileMedium_weighths1_var1);
      material.SetVector("_PSSProfileMedium_weighths2_var2", _PSSProfileMedium_weighths2_var2);
      material.SetVector("_PSSProfileMedium_weighths3_var3", _PSSProfileMedium_weighths3_var3);
      material.SetVector("_PSSProfileMedium_transl123", _PSSProfileMedium_transl123);
      material.SetVector("_PSSProfileMedium_sqrtvar123", _PSSProfileMedium_sqrtvar123);
      material.SetVector("_PSSProfileLow_weighths1_var1", _PSSProfileLow_weighths1_var1);
      material.SetVector("_PSSProfileLow_weighths2_var2", _PSSProfileLow_weighths2_var2);
      material.SetVector("_PSSProfileLow_sqrtvar12", _PSSProfileLow_sqrtvar12);
      material.SetVector("_PSSProfileLow_transl", _PSSProfileLow_transl);
    }
  }
}
