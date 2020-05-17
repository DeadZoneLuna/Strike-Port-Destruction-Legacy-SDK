using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu( "Image Effects/Screen Space Motion Blur" )]
[RequireComponent( typeof( Camera ) )]
public class ScreenSpaceMotionBlur : MonoBehaviour
{
	public float Strength = 200.0f; //Strength is inverse, smaller values = more intense
	public Shader compositeShader;

	private Material compositeMaterial;

	private Material GetCompositeMaterial()
	{
		if( compositeMaterial == null )
		{
			compositeMaterial = new Material( compositeShader );
			compositeMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		return compositeMaterial;
	}

	void OnDisable()
	{
		DestroyImmediate( compositeMaterial );
	}

	void OnPreCull()
	{
		Matrix4x4 iView = ( GetComponent<Camera>().worldToCameraMatrix.inverse * GetComponent<Camera>().projectionMatrix );
		Shader.SetGlobalMatrix( "_Myview", iView.inverse );
	}

	void OnRenderImage( RenderTexture source, RenderTexture dest )
	{
		Material compositeMat = GetCompositeMaterial();
		compositeMat.SetFloat( "_Strength", Strength );

		Graphics.Blit( source, dest, compositeMat );
	}

	void OnPostRender()
	{
		StartCoroutine( renderlate() );
	}

	IEnumerator renderlate()
	{
		yield return new WaitForEndOfFrame();
		Matrix4x4 Iviewprev = ( GetComponent<Camera>().worldToCameraMatrix.inverse * GetComponent<Camera>().projectionMatrix );
		Shader.SetGlobalMatrix( "_Myviewprev", Iviewprev );
	}
}