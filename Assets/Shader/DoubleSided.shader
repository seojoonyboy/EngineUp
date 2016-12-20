Shader "Custom/2 Sided" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _Glossiness ("Smoothness", Range(0,1)) = 0.5
         _Metallic ("Metallic", Range(0,1)) = 0.0
         _BumpMap("Normal Map", 2D) = "bump" {}
         _BumpScale("Scale", Float) = 1.0
//         _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
//        _ParallaxMap ("Height Map", 2D) = "black" {}
     }
     SubShader {
         Tags { "RenderType"="Opaque" }
         LOD 300
         Cull Back
      // Fog {Mode Off}
        // ZWrite On
          //usePass"Custom/TransparentShadowReceiver"

          CGPROGRAM
             // Physically based Standard lighting model, and enable shadows on all light types

         #pragma surface surf Standard fullforwardshadows // alpha
         #pragma target 3.0
         sampler2D _MainTex;
         sampler2D _BumpMap;
         struct Input {
             float2 uv_MainTex;
         };
         half _Glossiness;
         half _Metallic;
         half _BumpScale;
         fixed4 _Color;
         void surf (Input IN, inout SurfaceOutputStandard o) {

             // Albedo comes from a texture tinted by color
             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
             o.Albedo = c.rgb;
             // Metallic and smoothness come from slider variables
             o.Normal=UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex),_BumpScale);
             o.Metallic = _Metallic;
             o.Smoothness = _Glossiness;
             o.Alpha = c.a;
         }


         ENDCG
   //       Tags { "LightMode"="Vertex" }
        Ztest LEqual
         Cull Front
       //  Fog {Mode Off}
         CGPROGRAM

         #pragma surface surf Standard fullforwardshadows approxview //alpha
         //   #pragma multi_compile_fwdbase
         #pragma target 3.0
         #pragma vertex vert
         sampler2D _MainTex;
         sampler2D _BumpMap;

         struct Input {
             float2 uv_MainTex;
             fixed facing :VFACE;
         };
         half _Glossiness;
         half _Metallic;
         fixed4 _Color;
         half _BumpScale;
         void vert (inout appdata_full v) {
        //     v.normal *= -1;
             v.tangent*=-1;
         }
         void surf (Input IN, inout SurfaceOutputStandard o) {
             // Albedo comes from a texture tinted by color
             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
             o.Albedo = c.rgb;
             // Metallic and smoothness come from slider variables
             o.Metallic = _Metallic;
             o.Smoothness = _Glossiness;
             o.Alpha = c.a;
            o.Normal=UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex),_BumpScale);
            o.Normal.z*=IN.facing;

         }
         ENDCG
         //

     }


     FallBack "Diffuse"
}
