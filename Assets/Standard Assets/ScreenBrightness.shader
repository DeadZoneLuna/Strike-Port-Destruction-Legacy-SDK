// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Brightness" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "" {}
        _Intensity ("Brightness", Range (0, 1)) = 1
    }
    // Shader code pasted into all further CGPROGRAM blocks
    CGINCLUDE

    #pragma fragmentoption ARB_precision_hint_fastest
    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : POSITION;
        half2 uv : TEXCOORD0;
    };

    sampler2D _MainTex;
    float _Intensity;
    v2f vert( appdata_img v ) 
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;
        return o;
    } 
    fixed4 frag(v2f i) : COLOR 
    {
        fixed4 color = tex2D(_MainTex, i.uv); 

        return color * _Intensity;
    }

    ENDCG 
Subshader {
 Pass {
      ZTest Always Cull Off ZWrite Off
      Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}
Fallback off

} // shader