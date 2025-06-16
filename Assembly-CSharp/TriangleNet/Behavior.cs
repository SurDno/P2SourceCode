using System;
using TriangleNet.Log;

namespace TriangleNet
{
  public class Behavior
  {
    private TriangulationAlgorithm algorithm = TriangulationAlgorithm.Dwyer;
    private bool boundaryMarkers = true;
    private bool conformDel;
    private bool convex;
    internal bool fixedArea;
    internal double goodAngle;
    private bool jettison;
    private double maxAngle;
    private double maxArea = -1.0;
    internal double maxGoodAngle;
    private double minAngle;
    private int noBisect;
    private bool noHoles;
    internal double offconstant;
    private bool poly;
    private bool quality;
    private int steiner = -1;
    internal bool useRegions = false;
    private bool usertest;
    internal bool useSegments = true;
    private bool varArea;

    public static bool NoExact { get; set; }

    public static bool Verbose { get; set; }

    public bool Quality
    {
      get => this.quality;
      set
      {
        this.quality = value;
        if (!this.quality)
          return;
        this.Update();
      }
    }

    public double MinAngle
    {
      get => this.minAngle;
      set
      {
        this.minAngle = value;
        this.Update();
      }
    }

    public double MaxAngle
    {
      get => this.maxAngle;
      set
      {
        this.maxAngle = value;
        this.Update();
      }
    }

    public double MaxArea
    {
      get => this.maxArea;
      set
      {
        this.maxArea = value;
        this.fixedArea = value > 0.0;
      }
    }

    public bool VarArea
    {
      get => this.varArea;
      set => this.varArea = value;
    }

    public bool Poly
    {
      get => this.poly;
      set => this.poly = value;
    }

    public bool Usertest
    {
      get => this.usertest;
      set => this.usertest = value;
    }

    public bool Convex
    {
      get => this.convex;
      set => this.convex = value;
    }

    public bool ConformingDelaunay
    {
      get => this.conformDel;
      set => this.conformDel = value;
    }

    public TriangulationAlgorithm Algorithm
    {
      get => this.algorithm;
      set => this.algorithm = value;
    }

    public int NoBisect
    {
      get => this.noBisect;
      set
      {
        this.noBisect = value;
        if (this.noBisect >= 0 && this.noBisect <= 2)
          return;
        this.noBisect = 0;
      }
    }

    public int SteinerPoints
    {
      get => this.steiner;
      set => this.steiner = value;
    }

    public bool UseBoundaryMarkers
    {
      get => this.boundaryMarkers;
      set => this.boundaryMarkers = value;
    }

    public bool NoHoles
    {
      get => this.noHoles;
      set => this.noHoles = value;
    }

    public bool Jettison
    {
      get => this.jettison;
      set => this.jettison = value;
    }

    public Behavior(bool quality = false, double minAngle = 20.0)
    {
      if (!quality)
        return;
      this.quality = true;
      this.minAngle = minAngle;
      this.Update();
    }

    private void Update()
    {
      this.quality = true;
      if (this.minAngle < 0.0 || this.minAngle > 60.0)
      {
        this.minAngle = 0.0;
        this.quality = false;
        SimpleLog.Instance.Warning("Invalid quality option (minimum angle).", "Mesh.Behavior");
      }
      if (this.maxAngle != 0.0 && this.maxAngle < 90.0 || this.maxAngle > 180.0)
      {
        this.maxAngle = 0.0;
        this.quality = false;
        SimpleLog.Instance.Warning("Invalid quality option (maximum angle).", "Mesh.Behavior");
      }
      this.useSegments = this.Poly || this.Quality || this.Convex;
      this.goodAngle = Math.Cos(this.MinAngle * Math.PI / 180.0);
      this.maxGoodAngle = Math.Cos(this.MaxAngle * Math.PI / 180.0);
      this.offconstant = this.goodAngle != 1.0 ? 0.475 * Math.Sqrt((1.0 + this.goodAngle) / (1.0 - this.goodAngle)) : 0.0;
      this.goodAngle *= this.goodAngle;
    }
  }
}
