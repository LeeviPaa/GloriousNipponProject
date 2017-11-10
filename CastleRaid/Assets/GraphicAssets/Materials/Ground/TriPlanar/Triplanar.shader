// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader ".Custom/Triplanar"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Tiling("Tiling", Float) = 1
		_Falloff("Falloff", Float) = 140
		_Top("Top", 2D) = "white" {}
		_Middle("Middle", 2D) = "white" {}
		_Bottom("Bottom", 2D) = "white" {}
		_BottomNormal("Bottom Normal", 2D) = "white" {}
		_MiddleNormal("Middle Normal", 2D) = "white" {}
		_TopNormal("Top Normal", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _TopNormal;
		uniform sampler2D _MiddleNormal;
		uniform sampler2D _BottomNormal;
		uniform float _Tiling;
		uniform float _Falloff;
		uniform sampler2D _Top;
		uniform sampler2D _Middle;
		uniform sampler2D _Bottom;
		uniform float _Metallic;
		uniform float _Smoothness;


		inline float4 TriplanarSampling( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			if(vertex == 1){
			xNorm = ( tex2Dlod( midTexMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );
			yNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			yNormN = ( tex2Dlod( botTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			zNorm = ( tex2Dlod( midTexMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );
			} else {
			xNorm = ( tex2D( midTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.zx ) );
			yNormN = ( tex2D( botTexMap, tilling * worldPos.zx ) );
			zNorm = ( tex2D( midTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			}
			return xNorm* projNormal.x + yNorm* projNormal.y + yNormN * negProjNormalY + zNorm* projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar17 = TriplanarSampling( _TopNormal, _MiddleNormal, _BottomNormal, ase_worldPos, ase_worldNormal, _Falloff, _Tiling, 0 );
			o.Normal = triplanar17.xyz;
			float4 triplanar7 = TriplanarSampling( _Top, _Middle, _Bottom, ase_worldPos, ase_worldNormal, _Falloff, _Tiling, 0 );
			o.Albedo = triplanar7.xyz;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1710;67;1272;732;2138.187;185.2392;1.779223;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;18;-1282.38,745.2938;Float;True;Property;_MiddleNormal;Middle Normal;8;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;19;-1264.587,960.5802;Float;True;Property;_BottomNormal;Bottom Normal;7;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;15;-1275.075,523.7834;Float;True;Property;_TopNormal;Top Normal;9;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;10;-1258.687,-129.1785;Float;True;Property;_Top;Top;4;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;12;-1247.672,256.3645;Float;True;Property;_Bottom;Bottom;6;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1343.14,64.20502;Float;True;Property;_Middle;Middle;5;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;8;-1004.653,246.7842;Float;False;Property;_Falloff;Falloff;3;0;140;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-1003.043,433.9303;Float;False;Property;_Tiling;Tiling;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;17;-741.4962,754.1903;Float;True;Cylindrical;World;False;Top Texture 1;_TopTexture1;white;1;Assets/Ground/TriPlanar/bricks.png;Mid Texture 1;_MidTexture1;white;1;None;Bot Texture 1;_BotTexture1;white;4;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;7;-642.0495,-69.62132;Float;True;Cylindrical;World;False;Top Texture 0;_TopTexture0;white;1;Assets/Ground/TriPlanar/bricks.png;Mid Texture 0;_MidTexture0;white;2;None;Bot Texture 0;_BotTexture0;white;3;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;1;-398,-261;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-368.765,323.6815;Float;False;Property;_Smoothness;Smoothness;1;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-363.9826,225.7658;Float;False;Property;_Metallic;Metallic;0;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;.Custom/Triplanar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;15;0
WireConnection;17;1;18;0
WireConnection;17;2;19;0
WireConnection;17;3;20;0
WireConnection;17;4;8;0
WireConnection;7;0;10;0
WireConnection;7;1;11;0
WireConnection;7;2;12;0
WireConnection;7;3;20;0
WireConnection;7;4;8;0
WireConnection;0;0;7;0
WireConnection;0;1;17;0
WireConnection;0;3;13;0
WireConnection;0;4;14;0
ASEEND*/
//CHKSM=6C9EEA7C7E43DBD34D92AF5D17A7D15802B95D45