using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScreenBrightness : MonoBehaviour
{
    public Shader   shader;
    public float brightness;
    private Material m_Material;
    // Called by the camera to apply the image effect

    protected Material material {
        get {
            if (m_Material == null) {
                m_Material = new Material (shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        } 
    }

    protected void OnDisable() {
        if( m_Material ) {
            DestroyImmediate( m_Material );
        }
    }
    void OnRenderImage (RenderTexture source, RenderTexture destination) {      
        material.SetFloat ("_Intensity", brightness);
        Graphics.Blit(source, destination, material);
    }   
}