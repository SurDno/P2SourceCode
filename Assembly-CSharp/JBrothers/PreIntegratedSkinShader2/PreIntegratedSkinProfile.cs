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
      this.RecalculateDerived();
      this.gauss6_1.x = this._PSSProfileHigh_weighths1_var1.x;
      this.gauss6_1.y = this._PSSProfileHigh_weighths1_var1.y;
      this.gauss6_1.z = this._PSSProfileHigh_weighths1_var1.z;
      this.gauss6_2.x = this._PSSProfileHigh_weighths2_var2.x;
      this.gauss6_2.y = this._PSSProfileHigh_weighths2_var2.y;
      this.gauss6_2.z = this._PSSProfileHigh_weighths2_var2.z;
      this.gauss6_3.x = this._PSSProfileHigh_weighths3_var3.x;
      this.gauss6_3.y = this._PSSProfileHigh_weighths3_var3.y;
      this.gauss6_3.z = this._PSSProfileHigh_weighths3_var3.z;
      this.gauss6_4.x = this._PSSProfileHigh_weighths4_var4.x;
      this.gauss6_4.y = this._PSSProfileHigh_weighths4_var4.y;
      this.gauss6_4.z = this._PSSProfileHigh_weighths4_var4.z;
      this.gauss6_5.x = this._PSSProfileHigh_weighths5_var5.x;
      this.gauss6_5.y = this._PSSProfileHigh_weighths5_var5.y;
      this.gauss6_5.z = this._PSSProfileHigh_weighths5_var5.z;
      this.gauss6_6.x = this._PSSProfileHigh_weighths6_var6.x;
      this.gauss6_6.y = this._PSSProfileHigh_weighths6_var6.y;
      this.gauss6_6.z = this._PSSProfileHigh_weighths6_var6.z;
      this.needsRenormalize = false;
    }

    public void RecalculateDerived()
    {
      Vector3 zero = Vector3.zero;
      Vector3 gauss61 = (Vector3) this.gauss6_1;
      Vector3 gauss62 = (Vector3) this.gauss6_2;
      Vector3 gauss63 = (Vector3) this.gauss6_3;
      Vector3 gauss64 = (Vector3) this.gauss6_4;
      Vector3 gauss65 = (Vector3) this.gauss6_5;
      Vector3 gauss66 = (Vector3) this.gauss6_6;
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
      this._PSSProfileHigh_weighths1_var1 = new Vector4(gauss61.x, gauss61.y, gauss61.z, this.gauss6_1.w);
      this._PSSProfileHigh_weighths2_var2 = new Vector4(gauss62.x, gauss62.y, gauss62.z, this.gauss6_2.w);
      this._PSSProfileHigh_weighths3_var3 = new Vector4(gauss63.x, gauss63.y, gauss63.z, this.gauss6_3.w);
      this._PSSProfileHigh_weighths4_var4 = new Vector4(gauss64.x, gauss64.y, gauss64.z, this.gauss6_4.w);
      this._PSSProfileHigh_weighths5_var5 = new Vector4(gauss65.x, gauss65.y, gauss65.z, this.gauss6_5.w);
      this._PSSProfileHigh_weighths6_var6 = new Vector4(gauss66.x, gauss66.y, gauss66.z, this.gauss6_6.w);
      this._PSSProfileMedium_weighths1_var1 = new Vector4(gauss61.x + gauss62.x, gauss61.y + gauss62.y, gauss61.z + gauss62.z, (float) (((double) this.gauss6_1.w * (double) grayscale1 + (double) this.gauss6_2.w * (double) grayscale2) / ((double) grayscale1 + (double) grayscale2)));
      this._PSSProfileMedium_weighths2_var2 = new Vector4(gauss63.x + gauss64.x, gauss63.y + gauss64.y, gauss63.z + gauss64.z, (float) (((double) this.gauss6_3.w * (double) grayscale3 + (double) this.gauss6_4.w * (double) grayscale4) / ((double) grayscale3 + (double) grayscale4)));
      this._PSSProfileMedium_weighths3_var3 = new Vector4(gauss65.x + gauss66.x, gauss65.y + gauss66.y, gauss65.z + gauss66.z, (float) (((double) this.gauss6_5.w * (double) grayscale5 + (double) this.gauss6_6.w * (double) grayscale6) / ((double) grayscale5 + (double) grayscale6)));
      this._PSSProfileLow_weighths1_var1 = new Vector4(gauss61.x + gauss62.x + gauss63.x, gauss61.y + gauss62.y + gauss63.y, gauss61.z + gauss62.z + gauss63.z, (float) (((double) this.gauss6_1.w * (double) grayscale1 + (double) this.gauss6_2.w * (double) grayscale2 + (double) this.gauss6_3.w * (double) grayscale3) / ((double) grayscale1 + (double) grayscale2 + (double) grayscale3)));
      this._PSSProfileLow_weighths2_var2 = new Vector4(gauss64.x + gauss65.x + gauss66.x, gauss64.y + gauss65.y + gauss66.y, gauss64.z + gauss65.z + gauss66.z, (float) (((double) this.gauss6_4.w * (double) grayscale4 + (double) this.gauss6_5.w * (double) grayscale5 + (double) this.gauss6_6.w * (double) grayscale6) / ((double) grayscale4 + (double) grayscale5 + (double) grayscale6)));
      this._PSSProfileHigh_sqrtvar1234.x = Mathf.Sqrt(this._PSSProfileHigh_weighths1_var1.w);
      this._PSSProfileHigh_sqrtvar1234.y = Mathf.Sqrt(this._PSSProfileHigh_weighths2_var2.w);
      this._PSSProfileHigh_sqrtvar1234.z = Mathf.Sqrt(this._PSSProfileHigh_weighths3_var3.w);
      this._PSSProfileHigh_sqrtvar1234.w = Mathf.Sqrt(this._PSSProfileHigh_weighths4_var4.w);
      this._PSSProfileMedium_sqrtvar123.x = Mathf.Sqrt(this._PSSProfileMedium_weighths1_var1.w);
      this._PSSProfileMedium_sqrtvar123.y = Mathf.Sqrt(this._PSSProfileMedium_weighths2_var2.w);
      this._PSSProfileMedium_sqrtvar123.z = Mathf.Sqrt(this._PSSProfileMedium_weighths3_var3.w);
      this._PSSProfileLow_sqrtvar12.x = Mathf.Sqrt(this._PSSProfileLow_weighths1_var1.w);
      this._PSSProfileLow_sqrtvar12.y = Mathf.Sqrt(this._PSSProfileLow_weighths2_var2.w);
      this._PSSProfileHigh_transl123_sqrtvar5.w = Mathf.Sqrt(this._PSSProfileHigh_weighths5_var5.w);
      this._PSSProfileHigh_transl456_sqrtvar6.w = Mathf.Sqrt(this._PSSProfileHigh_weighths6_var6.w);
      float num = -1.442695f;
      this._PSSProfileHigh_transl123_sqrtvar5.x = num / this.gauss6_1.w;
      this._PSSProfileHigh_transl123_sqrtvar5.y = num / this.gauss6_2.w;
      this._PSSProfileHigh_transl123_sqrtvar5.z = num / this.gauss6_3.w;
      this._PSSProfileHigh_transl456_sqrtvar6.x = num / this.gauss6_4.w;
      this._PSSProfileHigh_transl456_sqrtvar6.y = num / this.gauss6_5.w;
      this._PSSProfileHigh_transl456_sqrtvar6.z = num / this.gauss6_6.w;
      this._PSSProfileMedium_transl123.x = num / this._PSSProfileMedium_weighths1_var1.w;
      this._PSSProfileMedium_transl123.y = num / this._PSSProfileMedium_weighths2_var2.w;
      this._PSSProfileMedium_transl123.z = num / this._PSSProfileMedium_weighths3_var3.w;
      Vector3 vector3_2;
      vector3_2.x = (float) ((double) this.gauss6_1.w * (double) gauss61.x + (double) this.gauss6_2.w * (double) gauss62.x + (double) this.gauss6_3.w * (double) gauss63.x + (double) this.gauss6_4.w * (double) gauss64.x + (double) this.gauss6_5.w * (double) gauss65.x + (double) this.gauss6_6.w * (double) gauss66.x);
      vector3_2.y = (float) ((double) this.gauss6_1.w * (double) gauss61.y + (double) this.gauss6_2.w * (double) gauss62.y + (double) this.gauss6_3.w * (double) gauss63.y + (double) this.gauss6_4.w * (double) gauss64.y + (double) this.gauss6_5.w * (double) gauss65.y + (double) this.gauss6_6.w * (double) gauss66.y);
      vector3_2.z = (float) ((double) this.gauss6_1.w * (double) gauss61.z + (double) this.gauss6_2.w * (double) gauss62.z + (double) this.gauss6_3.w * (double) gauss63.z + (double) this.gauss6_4.w * (double) gauss64.z + (double) this.gauss6_5.w * (double) gauss65.z + (double) this.gauss6_6.w * (double) gauss66.z);
      this._PSSProfileLow_transl.x = num / vector3_2.x;
      this._PSSProfileLow_transl.y = num / vector3_2.y;
      this._PSSProfileLow_transl.z = num / vector3_2.z;
      this.needsRecalcDerived = false;
    }

    public void ApplyProfile(Material material) => this.ApplyProfile(material, false);

    public void ApplyProfile(Material material, bool noWarn)
    {
      if (this.needsRecalcDerived)
        this.RecalculateDerived();
      material.SetVector("_PSSProfileHigh_weighths1_var1", this._PSSProfileHigh_weighths1_var1);
      material.SetVector("_PSSProfileHigh_weighths2_var2", this._PSSProfileHigh_weighths2_var2);
      material.SetVector("_PSSProfileHigh_weighths3_var3", this._PSSProfileHigh_weighths3_var3);
      material.SetVector("_PSSProfileHigh_weighths4_var4", this._PSSProfileHigh_weighths4_var4);
      material.SetVector("_PSSProfileHigh_weighths5_var5", this._PSSProfileHigh_weighths5_var5);
      material.SetVector("_PSSProfileHigh_weighths6_var6", this._PSSProfileHigh_weighths6_var6);
      material.SetVector("_PSSProfileHigh_sqrtvar1234", this._PSSProfileHigh_sqrtvar1234);
      material.SetVector("_PSSProfileHigh_transl123_sqrtvar5", this._PSSProfileHigh_transl123_sqrtvar5);
      material.SetVector("_PSSProfileHigh_transl456_sqrtvar6", this._PSSProfileHigh_transl456_sqrtvar6);
      material.SetVector("_PSSProfileMedium_weighths1_var1", this._PSSProfileMedium_weighths1_var1);
      material.SetVector("_PSSProfileMedium_weighths2_var2", this._PSSProfileMedium_weighths2_var2);
      material.SetVector("_PSSProfileMedium_weighths3_var3", this._PSSProfileMedium_weighths3_var3);
      material.SetVector("_PSSProfileMedium_transl123", this._PSSProfileMedium_transl123);
      material.SetVector("_PSSProfileMedium_sqrtvar123", this._PSSProfileMedium_sqrtvar123);
      material.SetVector("_PSSProfileLow_weighths1_var1", this._PSSProfileLow_weighths1_var1);
      material.SetVector("_PSSProfileLow_weighths2_var2", this._PSSProfileLow_weighths2_var2);
      material.SetVector("_PSSProfileLow_sqrtvar12", this._PSSProfileLow_sqrtvar12);
      material.SetVector("_PSSProfileLow_transl", this._PSSProfileLow_transl);
    }
  }
}
