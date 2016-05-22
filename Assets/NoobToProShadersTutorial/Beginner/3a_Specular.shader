﻿Shader "UnityCookie/Beginner/3a_Specular" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", float) = 10
	}
	SubShader {
		Pass {
			Tags {
			// Necessary to have directional lights work with the shader
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
			// pragmas
			#pragma vertex vert
			#pragma fragment frag

			// user defined variables
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			// Unity defined variables
			uniform float4 _LightColor0;

			// base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			// vertex function
			vertexOutput vert(vertexInput i) {
				vertexOutput o;
				// Position
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);

				// Lambert light
				float3 normalDirection = normalize(mul(float4(i.normal, 0.0), _World2Object).xyz);
				float3 viewDirection = normalize(float3((float4(_WorldSpaceCameraPos.xyz, 1.0) - mul(_Object2World, i.vertex).xyz)));
				float atten = 1.0;
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseReflection = atten * _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection));
				float3 specularReflection = atten * _SpecColor.rgb * max(0.0, dot(normalDirection, lightDirection)) * pow(max(0.0, dot(reflect(- lightDirection, normalDirection), viewDirection)), _Shininess);
				float3 lightFinal = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT.xyz;
				o.col = float4(lightFinal * _Color.rgb, 1.0);


				return o;
			}

			// fragment function
			float4 frag(vertexOutput i) : COLOR {
				return i.col;
			}
			ENDCG
		}
	}
	// fallback commented out during developments
	//Fallback "Diffuse"
}