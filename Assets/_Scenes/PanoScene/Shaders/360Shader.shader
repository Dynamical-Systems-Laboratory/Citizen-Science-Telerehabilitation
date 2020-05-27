// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/360Shader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "grey" {}
	}

		SubShader
	{
		Tags{ "Queue" = "Background" "RenderType" = "Background" }
		Cull Front ZWrite Off

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		sampler2D _MainTex;

	struct appdata_t
	{
		float4 vertex : POSITION;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 texcoord : TEXCOORD0;
	};

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.vertex.xyz;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float3 dir = normalize(i.texcoord);
		float2 longlat = float2(atan2(dir.x, dir.z) + UNITY_PI, acos(-dir.y));
		float2 uv = longlat / float2(2.0 * UNITY_PI, UNITY_PI);
		half4 tex = tex2D(_MainTex, uv);

		return tex;
	}
		ENDCG
	}
	}

		Fallback Off
}