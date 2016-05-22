Shader "Custom/2DShadows/ShadowMap" {
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

				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				// Camera projection of object and light
				float4 worldVert = mul(_Object2World, IN.vertex);
				// Calculate polar coords
				OUT.polarCoords = CalculatePolarCoords(_WorldSpaceLightPos0, worldVert);
				// Use angle to determine where in the texture to write
				OUT.vertex = float4(OUT.polarCoords.x, 0, 0, 1);

				return OUT;
			}

			float4 frag(v2f IN) : COLOR {
				// Sample texture
				fixed4 col = tex2D (_MainTex, IN.texcoord) * IN.color;
				// Sample ShadowMap to find current closest light occluder in the direction of the light
				float4 closest = tex2D (_ShadowMap, float2(IN.polarCoords.x, 0));
				// Override if current is closer and alpha is 1
				if(col.a == 1) {
					closest.r = min(IN.polarCoords.y, closest.r);
				}
				return closest;
			}
			ENDCG
		}
	}
}