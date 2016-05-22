Shader "Custom/2DShadows/ShadowReceiver"
{
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1.0,1.0,1.0,1.0)

		_ShadowMap ("Shadow Map", 2D) = "white" {}
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _ShadowMap;

			struct appdata {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;

				float2 polarCoords : TEXCOORD1;
			};

			float2 CalculatePolarCoords(float4 lightPos, float4 vertPos) {				
				float r = sqrt(pow(vertPos.x - lightPos.x, 2) + pow(vertPos.y - lightPos.y, 2));
				float theta = atan2(vertPos.y - lightPos.y, vertPos.x - lightPos.x);
				// Transform theta from -PI, PI to 0, 2PI and then to 0, 1
				if(theta < 0) {
					theta = theta + 2 * UNITY_PI; // 0, 2PI
				}
				theta = theta / (2 * UNITY_PI);
				return float2(theta, r);
			}

			v2f vert(appdata IN) {
				v2f OUT;

				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				// Calculate polar coords
				OUT.polarCoords = CalculatePolarCoords(_WorldSpaceLightPos0, mul(_Object2World, IN.vertex));

				return OUT;
			}

			float4 frag(v2f IN) : COLOR {
				// In shadow by default
				fixed4 col = fixed4(0, 0, 0, 1);
				// Sample ShadowMap to find closest light occluder in the direction of the light
				float4 closest = tex2D (_ShadowMap, float2(IN.polarCoords.x, 0));
				// If closer to the light than the closest occluder, sample sprite for color
				if(IN.polarCoords.y <= closest.r) {
					col = tex2D (_MainTex, IN.texcoord) * IN.color;
				}
				return col;
			}
			ENDCG
		}
	}
}
