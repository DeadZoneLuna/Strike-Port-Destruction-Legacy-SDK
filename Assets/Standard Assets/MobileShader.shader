// Simplified VertexLit Blended Particle shader. Differences from regular VertexLit Blended Particle one:
// - no AlphaTest
// - no ColorMask

Shader "Mobile/Particles/VertexLit Blended2" {
Properties {
	_EmisColor ("Emissive Color", Color) = (.2,.2,.2,0)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite Off Fog { Color (0,0,0,0) }
	
	Lighting On
	Material { Emission [_EmisColor] }
	ColorMaterial AmbientAndDiffuse

	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}