Shader "Custom/SplatShader" {
	Properties {
		_SplatTex ("RGB Splat", 2D) = "white" {}
		_RedTex ("Red Texture", 2D) = "white" {}
		_GreenTex ("Green Texture", 2D) = "white" {}
		_BlueTex ("Blue Texture", 2D) = "white" {}
		_NormalizeValues("Value Normalization", Range(0,1)) = 0
		_ShowRedSplat ("Show Red Splat", Range(0,1)) = 0
		_ShowGreenSplat ("Show Green Splat", Range(0,1)) = 0
		_ShowBlueSplat ("Show Blue Splat", Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
//		Lighting Off
		
		CGPROGRAM
		#pragma surface surf NoLighting 

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	    {
	        fixed4 c;
	        c.rgb = s.Albedo; 
	        c.a = s.Alpha;
	        return c;
	    }
	    
		sampler2D _SplatTex;
		sampler2D _RedTex;
		sampler2D _GreenTex;
		sampler2D _BlueTex;
		float _ShowRedSplat;
		float _ShowGreenSplat;
		float _ShowBlueSplat;
		float _NormalizeValues;


		struct Input {
			float2 uv_SplatTex;
			float2 uv_RedTex;
			float2 uv_GreenTex;
			float2 uv_BlueTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 splat = tex2D (_SplatTex, IN.uv_SplatTex);
			half4 r = tex2D (_RedTex, IN.uv_RedTex);
			half4 g = tex2D (_GreenTex, IN.uv_GreenTex);
			half4 b = tex2D (_BlueTex, IN.uv_BlueTex);
			
			r = lerp(r, splat, _ShowRedSplat);
			g = lerp(g, splat, _ShowGreenSplat);
			b = lerp(b, splat, _ShowBlueSplat);

			// calculate the total weight
			float totalWeight = splat.r + splat.b + splat.g;
			totalWeight = max(1, totalWeight);
			splat = lerp(splat, splat / totalWeight, _NormalizeValues);

			o.Albedo = splat.r*r + splat.g*g + splat.b*b;
			o.Alpha = splat.a;
		}
		ENDCG
		
	} 
	FallBack "Diffuse"
}
