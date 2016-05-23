Shader "Custom/Flat Water" {
	Properties {
		_Color ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Diffuse Texture", 2D) = "white"{}

		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 10

		_RimColor ("Rim Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0

		_Amplitude ("Amplitude", Float) = 5
		_Frequency("Frequency", Float) = 45

		_Spherical("Use spherical coordinates? (1 = yes, -1 = no)", Float) = -1
	}
	SubShader {
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Tags {
				// Necessary to have directional lights work with the shader
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
			// pragmas
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 3.0

			#include "noiseSimplex.cginc"
			


			// user defined variables
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _RimColor;
			uniform float _RimPower;

			// Water animation variables
			uniform float _Amplitude;
			uniform float _Frequency;
			uniform float _Radial;

			// Unity defined variables
			uniform float4 _LightColor0;

			// base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color: COLOR;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 color : COLOR;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};

			// vertex function
			vertexOutput vert(vertexInput i) {
				vertexOutput o;

				// Offset vertex position with sine wave
				//float4 pos = i.vertex + float4(0, 1, 0, 0) * (cos(_Time * _Frequency * i.vertex.x) + cos(_Time * _Frequency * i.vertex.z)) * _Amplitude;
				// Spherical
				float4 pos;
				if (_Radial > 0) {
					pos = i.vertex + normalize(i.vertex) * snoise(i.vertex * _Frequency * _Time) * _Amplitude;
				}
				// Cartesian
				else {
					pos = i.vertex + float4(0, 1, 0, 0) * snoise(i.vertex * _Frequency * _Time) * _Amplitude;
				}

				


				// Position
				o.pos = mul(UNITY_MATRIX_MVP, pos);
				o.posWorld = mul(_Object2World, pos);
				o.normalDir = normalize(mul(float4(i.normal, 0.0), _World2Object).xyz);
				o.tex = i.texcoord;

				o.color = i.color;

				return o;
			}

			// fragment function
			float4 frag(vertexOutput i) : COLOR {
				// Lambert light
				// Flat normals
				float3 dpdx = normalize(ddx(i.posWorld));
				float3 dpdy;
				if (_ProjectionParams.x < 0) {
					dpdy = -ddy(i.posWorld);
				}
				else {
					dpdy = ddy(i.posWorld);
				}
				float3 normalDirection =  normalize(cross(dpdx, dpdy));
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

				//// Adjust color based on steepness using normals
				//float4 modelNormal = mul(_World2Object, normalDirection);
				//float foaminess = 1 - abs(dot(modelNormal, float4(0, 1, 0, 0)));

				//float4 ColorWithFoam = lerp(float4(lightFinal, 1.0) * _Color * i.color, float4(0, 0.5, 0, 1), foaminess);
				//return float4(foaminess, 0, 0, 1);
				return float4(lightFinal, 1.0) * _Color * i.color;
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

			#pragma target 3.0

			#include "noiseSimplex.cginc"

			// user defined variables
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _RimColor;
			uniform float _RimPower;

			// Water animation variables
			uniform float _Amplitude;
			uniform float _Frequency;

			// Unity defined variables
			uniform float4 _LightColor0;

			// base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				float4 color : COLOR;
			};

			// vertex function
			vertexOutput vert(vertexInput i) {
				vertexOutput o;
				// Offset vertex position with sine wave
				//float4 pos = i.vertex + float4(0, 1, 0, 0) * (cos(_Time * _Frequency * i.vertex.x) + cos(_Time * _Frequency * i.vertex.z)) * _Amplitude;
				float4 pos = i.vertex + float4(0, 1, 0, 0) * snoise(i.vertex * _Frequency * _Time) * _Amplitude;


				// Position
				o.pos = mul(UNITY_MATRIX_MVP, pos);
				o.posWorld = mul(_Object2World, pos);
				o.normalDir = normalize(mul(float4(i.normal, 0.0), _World2Object).xyz);
				o.tex = i.texcoord;

				o.color = i.color;
				return o;
			}

			// fragment function
			float4 frag(vertexOutput i) : COLOR {
				// Lambert light
				// Flat normals
				float3 dpdx = ddx(i.posWorld);
				float3 dpdy;
				if (_ProjectionParams.x < 0) {
					dpdy = -ddy(i.posWorld);
				}
				else {
					dpdy = ddy(i.posWorld);
				}
				float3 normalDirection = normalize(cross(dpdx, dpdy));

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


				return float4(lightFinal, 1.0) * _Color * i.color;
			}
			ENDCG
		}

	}
	FallBack "Diffuse"
}