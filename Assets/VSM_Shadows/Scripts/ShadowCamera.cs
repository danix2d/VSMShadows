using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CustomShadows : MonoBehaviour {

    [SerializeField]
    Shader _depthShader;

    [SerializeField]
    int _resolution = 512;

    [Range(0, 1)]
    public float maxShadowIntensity = 1;

    [Range(0, 1)]
    public float varianceShadowExpansion = 0.3f;

    [Range(0, 1)]
    public float _BlurSize;
    public Camera _shadowCam;
    RenderTexture _target;

   // RenderPipeline.StandardRequest request;

    public bool isStatic;

    public Light dirLight;

    private void Start() {

        UpdateRenderTexture();
        UpdateShadowCameraPos();

        _shadowCam.targetTexture = _target;

        // request = new RenderPipeline.StandardRequest();
        // request.destination = _target;
        // _shadowCam.GetUniversalAdditionalCameraData().SetRenderer(1);
        // RenderPipeline.SubmitRenderRequest(_shadowCam, request);

        _shadowCam.GetUniversalAdditionalCameraData().SetRenderer(1);
    }

    void Update ()
    {
    //    SetUpShadowCam();
        UpdateRenderTexture();
        UpdateShadowCameraPos();
        
     //   RenderPipeline.SubmitRenderRequest(_shadowCam, request);
        
        _shadowCam.Render();

        UpdateShaderValues();
    }

    void UpdateShaderValues()
    {
        // Set the qualities of the textures
        Shader.SetGlobalTexture("_ShadowTex", _target);
        Shader.SetGlobalMatrix("_LightMatrix", _shadowCam.transform.worldToLocalMatrix);
        Shader.SetGlobalFloat("_MaxShadowIntensity", maxShadowIntensity);
        Shader.SetGlobalFloat("_VarianceShadowExpansion", varianceShadowExpansion);
        Shader.SetGlobalFloat("_BlurSize", _BlurSize);

        Vector4 size = Vector4.zero;
        size.y = _shadowCam.orthographicSize * 2;
        size.x = _shadowCam.aspect * size.y;
        size.z = _shadowCam.farClipPlane;
        size.w = 1.0f / _resolution;
        Shader.SetGlobalVector("_ShadowTexScale", size);
    }

    // Refresh the render target if the scale has changed
    void UpdateRenderTexture()
    {
        if (_target == null)
        {
            _target = CreateTarget();
        }
    }

    // Update the camera view to encompass the geometry it will draw
    void UpdateShadowCameraPos()
    {
        // Update the position
   //     Light l = FindObjectOfType<Light>();
     //   cam.transform.position = l.transform.position;
        _shadowCam.transform.rotation = dirLight.transform.rotation;
     //   _shadowCam.transform.LookAt(_shadowCam.transform.position + _shadowCam.transform.forward, _shadowCam.transform.up);

    //    Vector3 center, extents;
    //    List<Renderer> renderers = new List<Renderer>();
    //    renderers.AddRange(FindObjectsOfType<Renderer>());

   //     GetRenderersExtents(renderers, cam.transform, out center, out extents);

        // center.z -= extents.z / 2;
        // cam.transform.position = cam.transform.TransformPoint(center);
        // cam.nearClipPlane = 0;
        // cam.farClipPlane = extents.z;

        // cam.aspect = extents.x / extents.y;
        // cam.orthographicSize = extents.y / 2;
    }

    RenderTexture CreateTarget()
    {
        RenderTexture tg = new RenderTexture(_resolution, _resolution, 0, RenderTextureFormat.RGHalf);
        tg.antiAliasing = 4;
        tg.filterMode = FilterMode.Bilinear;
        tg.wrapMode = TextureWrapMode.Clamp;
        tg.enableRandomWrite = false;
        tg.anisoLevel = 0;
        tg.Create();

        return tg;
    }
}
