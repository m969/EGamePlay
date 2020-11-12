Shader "MagicFire/Projector"{
	Properties{
			_Color ("Color", Color)=(1,1,1,1)
			_ShadowTex("Cookie", 2D) = "gray" {}
			_FalloffTex("FallOff", 2D) = "white" {}
	}
	Subshader{
		Tags{ "Queue" = "Transparent" }
		Pass{
			ZWrite Off
			ColorMask RGB
			//Blend DstColor Zero
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			v2f vert(float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul(unity_Projector, vertex);
				o.uvFalloff = mul(unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 _Color;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 res = lerp(fixed4(1, 1, 1, 0), texS*_Color, texF.r)*1.2;
				//fixed4 res = (texS.r, texS.g, texS.b, texS.a * texF.r);
				//UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(1,1,1,1));
				return res;
			}
			ENDCG
		}

		//CGPROGRAM
		//#pragma target 3.0
		//#pragma surface surf Projector

		//struct Input {
		//	float2 uv_texcoord;
		//	float3 worldNormal;
		//	float3 worldPos;
		//};

		//void surf(Input i , inout SurfaceOutput o)
		//{

		//}

		//inline fixed4 LightingProjector(inout SurfaceOutput s, half3 lightDir, half atten)
		//{
		//	return fixed4(0, 1, 1, 1);
		//}
		//ENDCG
	}
}