﻿Shader "UnityCookie/Beginner/6_Texture" {
	Properties {
		_Color ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Diffuse Texture", 2D) = "white"{}

		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 10

		_RimColor ("Rim Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0


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
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _RimColor;
			uniform float _RimPower;

			// Unity defined variables
			uniform float4 _LightColor0;

			// base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};

			// vertex function
			vertexOutput vert(vertexInput i) {
				vertexOutput o;
				// Position
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.posWorld = mul(_Object2World, i.vertex);
				o.normalDir = normalize(mul(float4(i.normal, 0.0), _World2Object).xyz);
				o.tex = i.texcoord;

				return o;
			}

			// fragment function
			float4 frag(vertexOutput i) : COLOR {
				// Lambert light
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
				// Directional / point light
				float atten;
				float3 lightDirection;
				if(_WorldSpaceLightPos0.w == 0.0) {
					atten = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				} else {
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(fragmentToLightSource);
					atten = 1.0 / distance;
					lightDirection = normalize(fragmentToLightSource);
				}
				// Diffuse and specular
				float3 diffuseReflection = atten * _LightColor0.rgb * saturate(dot(normalDirection, lightDirection));
				float3 specularReflection = diffuseReflection * _SpecColor.rgb * pow(saturate(dot(reflect(- lightDirection, normalDirection), viewDirection)), _Shininess);
				// Rim lighting
				float rim = 1 - saturate(dot(normalize(viewDirection), normalDirection));
				float3 rimLighting = atten * _LightColor0.rgb * _RimColor.rgb * saturate(dot(normalDirection, lightDirection)) * pow(rim , _RimPower);

				float3 lightFinal = diffuseReflection + specularReflection + rimLighting + UNITY_LIGHTMODEL_AMBIENT.xyz;

				// Texture maps
				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);


				return float4(tex.rgb * lightFinal * _Color.rgb, 1.0);
			}
			ENDCG
		}

		Pass {
			Tags {
			// Necessary to have directional lights work with the shader
				"LightMode" = "ForwardAdd"
			}
			Blend One One
			CGPROGRAM
			// pragmas
			#pragma vertex vert
			#pragma fragment frag

			// user defined variables
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _RimColor;
			uniform float _RimPower;

			// Unity defined variables
			uniform float4 _LightColor0;

			// base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};

			// vertex function
			vertexOutput vert(vertexInput i) {
				vertexOutput o;
				// Position
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.posWorld = mul(_Object2World, i.vertex);
				o.normalDir = normalize(mul(float4(i.normal, 0.0), _World2Object).xyz);
				o.tex = i.texcoord;

				return o;
			}

			// fragment function
			float4 frag(vertexOutput i) : COLOR {
				// Lambert light
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
				// Directional / point light
				float atten;
				float3 lightDirection;
				if(_WorldSpaceLightPos0.w == 0.0) {
					atten = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				} else {
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(fragmentToLightSource);
					atten = 1.0 / distance;
					lightDirection = normalize(fragmentToLightSource);
				}
				// Diffuse and specular
				float3 diffuseReflection = atten * _LightColor0.rgb * saturate(dot(normalDirection, lightDirection));
				float3 specularReflection = diffuseReflection * _SpecColor.rgb * pow(saturate(dot(reflect(- lightDirection, normalDirection), viewDirection)), _Shininess);
				// Rim lighting
				float rim = 1 - saturate(dot(normalize(viewDirection), normalDirection));
				float3 rimLighting = atten * _LightColor0.rgb * _RimColor.rgb * saturate(dot(normalDirection, lightDirection)) * pow(rim , _RimPower);

				float3 lightFinal = diffuseReflection + specularReflection + rimLighting;


				return float4(lightFinal * _Color.rgb, 1.0);
			}
			ENDCG
		}

	// fallback commented out during developments
	//Fallback "Diffuse"
	}
}