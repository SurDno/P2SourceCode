using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Source.Audio;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using UnityEngine;

namespace ProBuilder2.Examples
{
  [RequireComponent(typeof (AudioSource))]
  public class IcoBumpin : MonoBehaviour
  {
    private pb_Object ico;
    private Mesh icoMesh;
    private Transform icoTransform;
    private AudioSource audioSource;
    private FaceRef[] outsides;
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
    public bool rotateWaveformRing;
    public bool bounceWaveform;
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
    private float rms;
    private float rms_history;

    private void Start()
    {
      audioSource = GetComponent<AudioSource>();
      if (audioSource.clip == null)
        missingClipWarning.SetActive(true);
      ico = pb_ShapeGenerator.IcosahedronGenerator(icoRadius, icoSubdivisions);
      pb_Face[] faces = ico.faces;
      foreach (pb_Face pbFace in faces)
        pbFace.material = material;
      ico.Extrude(faces, ExtrudeMethod.IndividualFaces, startingExtrusion);
      ico.ToMesh();
      ico.Refresh();
      outsides = new FaceRef[faces.Length];
      Dictionary<int, int> dictionary = ico.sharedIndices.ToDictionary();
      for (int index = 0; index < faces.Length; ++index)
        outsides[index] = new FaceRef(faces[index], pb_Math.Normal(ico, faces[index]), ico.sharedIndices.AllIndicesWithValues(dictionary, faces[index].distinctIndices).ToArray());
      original_vertices = new Vector3[ico.vertices.Length];
      Array.Copy(ico.vertices, original_vertices, ico.vertices.Length);
      displaced_vertices = ico.vertices;
      icoMesh = ico.msh;
      icoTransform = ico.transform;
      faces_length = outsides.Length;
      icoPosition = icoTransform.position;
      waveform.positionCount = 1024;
      if (bounceWaveform)
        waveform.transform.parent = icoTransform;
      audioSource.PlayAndCheck();
    }

    private void Update()
    {
      audioSource.GetSpectrumData(fft, 0, FFTWindow.BlackmanHarris);
      audioSource.GetOutputData(data, 0);
      rms = RMS(data);
      for (int index1 = 0; index1 < outsides.Length; ++index1)
      {
        float time = index1 / faces_length;
        int index2 = (int) (time * (double) fftBounds);
        Vector3 vector3 = outsides[index1].nrm * (float) ((fft[index2] + (double) fft_history[index2]) * 0.5 * (frequencyCurve.Evaluate(time) * 0.5 + 0.5)) * extrusion;
        foreach (int index3 in outsides[index1].indices)
          displaced_vertices[index3] = original_vertices[index3] + vector3;
      }
      Vector3 zero = Vector3.zero;
      for (int index4 = 0; index4 < 1024; ++index4)
      {
        int index5 = index4 < 1023 ? index4 : 0;
        zero.x = Mathf.Cos((float) (index5 / 1024.0 * 6.2831850051879883)) * (waveformRadius + (float) ((data[index5] + (double) data_history[index5]) * 0.5) * waveformHeight);
        zero.z = Mathf.Sin((float) (index5 / 1024.0 * 6.2831850051879883)) * (waveformRadius + (float) ((data[index5] + (double) data_history[index5]) * 0.5) * waveformHeight);
        zero.y = 0.0f;
        waveform.SetPosition(index4, zero);
      }
      if (rotateWaveformRing)
        waveform.transform.localRotation = Quaternion.Euler(waveform.transform.localRotation.eulerAngles with
        {
          x = Mathf.PerlinNoise(Time.time * waveformSpeed, 0.0f) * 360f,
          y = Mathf.PerlinNoise(0.0f, Time.time * waveformSpeed) * 360f
        });
      icoPosition.y = (float) (-(double) verticalBounce + (rms + (double) rms_history) * verticalBounce);
      icoTransform.position = icoPosition;
      Array.Copy(fft, fft_history, 4096);
      Array.Copy(data, data_history, 1024);
      rms_history = rms;
      icoMesh.vertices = displaced_vertices;
    }

    private float RMS(float[] arr)
    {
      float num = 0.0f;
      float length = arr.Length;
      for (int index = 0; index < (double) length; ++index)
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
        face = f;
        nrm = n;
        indices = i;
      }
    }
  }
}
