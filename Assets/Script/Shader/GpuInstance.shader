﻿Shader "Custom/InstancedColorSurfaceShader" {

	Properties{

		_Color("Color", Color) = (1,1,1,1)

		_MainTex("Albedo (RGB)", 2D) = "white" {}

	_Glossiness("Smoothness", Range(0,1)) = 0.5

		_Metallic("Metallic", Range(0,1)) = 0.0

	}

		SubShader{

		Tags{ "RenderType" = "Opaque" }

		LOD 200



		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types

	#pragma surface surf Standard fullforwardshadows

		// Use Shader model 3.0 target

	#pragma target 3.0

		sampler2D _MainTex;

	struct Input {

		float2 uv_MainTex;

	};

	half _Glossiness;

	half _Metallic;

		__UNITY_INSTANCING_CBUFFER_START(Props)__

		__UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color); __

		__UNITY_INSTANCING_CBUFFER_END__

		void surf(Input IN, inout SurfaceOutputStandard o) {

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * __UNITY_ACCESS_INSTANCED_PROP(_Color)__;

		o.Albedo = c.rgb;

		o.Metallic = _Metallic;

		o.Smoothness = _Glossiness;

		o.Alpha = c.a;

	}

	ENDCG

	}

		FallBack "Diffuse"

}