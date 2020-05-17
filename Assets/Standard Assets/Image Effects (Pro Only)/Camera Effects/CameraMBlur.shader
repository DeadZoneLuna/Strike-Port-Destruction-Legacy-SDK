// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'samplerRECT' with 'sampler2D'
// Upgrade NOTE: replaced 'texRECT' with 'tex2D'

Shader "CameraMBlur" {
Properties {
	_MainTex ("", RECT) = "white" {}
	_Strength ("Strength", Range (1, 30)) = 15.0
}

SubShader {
	Pass {
		ZTest Always Cull off ZWrite Off Fog { Mode off }

CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex;

struct v2f { 
	float4 pos	: POSITION;
	float2 uv	: TEXCOORD0;

}; 

v2f vert (appdata_base v)
{

	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv.xy=v.texcoord;
	return o;
}

uniform float4x4 _Myview;
uniform float4x4 _Myviewprev;
uniform float _Strength;

half4 frag (v2f i) : COLOR
{
float2 Texcoord =i.uv;
  // Get the depth buffer value at this pixel.   
  float zOverW = 1;   
// H is the viewport position at this pixel in the range -1 to 1.   
float4 H = float4(Texcoord.x * 2 - 1, (1 - Texcoord.y) * 2 - 1,zOverW, 1);   
// Transform by the view-projection inverse.   
   float4 D = mul(H, _Myview);   
// Divide by w to get the world position.   
   float4 worldPos = D / D.w;  
     // Current viewport position   
   float4 currentPos = H;   
// Use the world position, and transform by the previous view-   
   // projection matrix.   
   float4 previousPos = mul(worldPos, _Myviewprev);   
// Convert to nonhomogeneous points [-1,1] by dividing by w.   
previousPos /= previousPos.w;   
// Use this frame's position and last frame's to compute the pixel   
   // velocity.   
   float2 velocity = (currentPos - previousPos)/_Strength;  
   
    // Get the initial color at this pixel.   
   float4 color = tex2D(_MainTex, Texcoord);   
Texcoord += velocity;   
for(int i = 1; i < 12; ++i, Texcoord += velocity)   
{   
  // Sample the color buffer along the velocity vector.   
   float4 currentColor = tex2D(_MainTex, Texcoord);   
  // Add the current color to our color sum.   
  color += currentColor;   

}   
// Average all of the samples to get the final blur color.   
   float4 finalColor = color / 12;  
    	
	return finalColor;
}
ENDCG
	}
}

Fallback off

}