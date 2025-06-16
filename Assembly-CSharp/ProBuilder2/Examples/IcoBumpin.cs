// Decompiled with JetBrains decompiler
// Type: ProBuilder2.Examples.IcoBumpin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace ProBuilder2.Examples
{
  [RequireComponent(typeof (AudioSource))]
  public class IcoBumpin : MonoBehaviour
  {
    private pb_Object ico;
    private Mesh icoMesh;
    private Transform icoTransform;
    private AudioSource audioSource;
    private IcoBumpin.FaceRef[] outsides;
    private Vector3[] original_vertices;
    private Vector3[] displaced_vertices;
    [Range(1f, 10f)]
    public float icoRadius = 2f;
    [Range(0.0f, 3f)]
    public int icoSubdivisions = 2;
    [Range(0.0f, 1f)]
    public float startingExtrusion = 0.1f;
    public Material material;
    [Range(1f, 50f)]
    public float extrusion = 30f;
    [Range(8f, 128f)]
    public int fftBounds = 32;
    [Range(0.0f, 10f)]
    public float verticalBounce = 4f;
    public AnimationCurve frequencyCurve;
    public LineRenderer waveform;
    public float waveformHeight = 2f;
    public float waveformRadius = 20f;
    public float waveformSpeed = 0.1f;
    public bool rotateWaveformRing = false;
    public bool bounceWaveform = false;
    public GameObject missingClipWarning;
    private Vector3 icoPosition = Vector3.zero;
    private float faces_length;
    private const float TWOPI = 6.283185f;
    private const int WAVEFORM_SAMPLES = 1024;
    private const int FFT_SAMPLES = 4096;
    private float[] fft = new float[4096];
    private float[] fft_history = new float[4096];
    private float[] data = new float[1024];
    private float[] data_history = new float[1024];
    private float rms = 0.0f;
    private float rms_history = 0.0f;

    private void Start()
    {
      this.audioSource = this.GetComponent<AudioSource>();
      if ((UnityEngine.Object) this.audioSource.clip == (UnityEngine.Object) null)
        this.missingClipWarning.SetActive(true);
      this.ico = pb_ShapeGenerator.IcosahedronGenerator(this.icoRadius, this.icoSubdivisions);
      pb_Face[] faces = this.ico.faces;
      foreach (pb_Face pbFace in faces)
        pbFace.material = this.material;
      this.ico.Extrude(faces, ExtrudeMethod.IndividualFaces, this.startingExtrusion);
      this.ico.ToMesh();
      this.ico.Refresh();
      this.outsides = new IcoBumpin.FaceRef[faces.Length];
      Dictionary<int, int> dictionary = pb_IntArrayUtility.ToDictionary(this.ico.sharedIndices);
      for (int index = 0; index < faces.Length; ++index)
        this.outsides[index] = new IcoBumpin.FaceRef(faces[index], pb_Math.Normal(this.ico, faces[index]), pb_IntArrayUtility.AllIndicesWithValues(this.ico.sharedIndices, dictionary, (IList<int>) faces[index].distinctIndices).ToArray<int>());
      this.original_vertices = new Vector3[this.ico.vertices.Length];
      Array.Copy((Array) this.ico.vertices, (Array) this.original_vertices, this.ico.vertices.Length);
      this.displaced_vertices = this.ico.vertices;
      this.icoMesh = this.ico.msh;
      this.icoTransform = this.ico.transform;
      this.faces_length = (float) this.outsides.Length;
      this.icoPosition = this.icoTransform.position;
      this.waveform.positionCount = 1024;
      if (this.bounceWaveform)
        this.waveform.transform.parent = this.icoTransform;
      this.audioSource.PlayAndCheck();
    }

    private void Update()
    {
      this.audioSource.GetSpectrumData(this.fft, 0, FFTWindow.BlackmanHarris);
      this.audioSource.GetOutputData(this.data, 0);
      this.rms = this.RMS(this.data);
      for (int index1 = 0; index1 < this.outsides.Length; ++index1)
      {
        float time = (float) index1 / this.faces_length;
        int index2 = (int) ((double) time * (double) this.fftBounds);
        Vector3 vector3 = this.outsides[index1].nrm * (float) (((double) this.fft[index2] + (double) this.fft_history[index2]) * 0.5 * ((double) this.frequencyCurve.Evaluate(time) * 0.5 + 0.5)) * this.extrusion;
        foreach (int index3 in this.outsides[index1].indices)
          this.displaced_vertices[index3] = this.original_vertices[index3] + vector3;
      }
      Vector3 zero = Vector3.zero;
      for (int index4 = 0; index4 < 1024; ++index4)
      {
        int index5 = index4 < 1023 ? index4 : 0;
        zero.x = Mathf.Cos((float) ((double) index5 / 1024.0 * 6.2831850051879883)) * (this.waveformRadius + (float) (((double) this.data[index5] + (double) this.data_history[index5]) * 0.5) * this.waveformHeight);
        zero.z = Mathf.Sin((float) ((double) index5 / 1024.0 * 6.2831850051879883)) * (this.waveformRadius + (float) (((double) this.data[index5] + (double) this.data_history[index5]) * 0.5) * this.waveformHeight);
        zero.y = 0.0f;
        this.waveform.SetPosition(index4, zero);
      }
      if (this.rotateWaveformRing)
        this.waveform.transform.localRotation = Quaternion.Euler(this.waveform.transform.localRotation.eulerAngles with
        {
          x = Mathf.PerlinNoise(Time.time * this.waveformSpeed, 0.0f) * 360f,
          y = Mathf.PerlinNoise(0.0f, Time.time * this.waveformSpeed) * 360f
        });
      this.icoPosition.y = (float) (-(double) this.verticalBounce + ((double) this.rms + (double) this.rms_history) * (double) this.verticalBounce);
      this.icoTransform.position = this.icoPosition;
      Array.Copy((Array) this.fft, (Array) this.fft_history, 4096);
      Array.Copy((Array) this.data, (Array) this.data_history, 1024);
      this.rms_history = this.rms;
      this.icoMesh.vertices = this.displaced_vertices;
    }

    private float RMS(float[] arr)
    {
      float num = 0.0f;
      float length = (float) arr.Length;
      for (int index = 0; (double) index < (double) length; ++index)
        num += Mathf.Abs(arr[index]);
      return Mathf.Sqrt(num / length);
    }

    private struct FaceRef
    {
      public pb_Face face;
      public Vector3 nrm;
      public int[] indices;

      public FaceRef(pb_Face f, Vector3 n, int[] i)
      {
        this.face = f;
        this.nrm = n;
        this.indices = i;
      }
    }
  }
}
