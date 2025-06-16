namespace UnityEngine.PostProcessing
{
  public class PostProcessingContext
  {
    public PostProcessingProfile profile;
    public Camera camera;
    public MaterialFactory materialFactory;
    public RenderTextureFactory renderTextureFactory;

    public bool interrupted { get; private set; }

    public void Interrupt() => this.interrupted = true;

    public PostProcessingContext Reset()
    {
      this.profile = (PostProcessingProfile) null;
      this.camera = (Camera) null;
      this.materialFactory = (MaterialFactory) null;
      this.renderTextureFactory = (RenderTextureFactory) null;
      this.interrupted = false;
      return this;
    }

    public bool isGBufferAvailable
    {
      get => this.camera.actualRenderingPath == RenderingPath.DeferredShading;
    }

    public bool isHdr => this.camera.allowHDR;

    public int width => this.camera.pixelWidth;

    public int height => this.camera.pixelHeight;

    public Rect viewport => this.camera.rect;
  }
}
