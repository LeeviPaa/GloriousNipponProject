// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader ".Custom/Triplanar2"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[Toggle]_ToggleSwitch5("Toggle Switch5", Float) = 0
		[Toggle]_ToggleSwitch1("Toggle Switch1", Float) = 0
		_TopXTiling("Top X Tiling", Float) = 1
		_TopYTiling("Top Y Tiling", Float) = 1
		_MidXTiling("Mid X Tiling", Float) = 1
		_MidYTiling("Mid Y Tiling", Float) = 1
		_MidRotatethreshold("Mid Rotate threshold", Float) = -0.15
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_BtmAngle("Btm Angle", Range( -1 , 1)) = -0.5
		_TopYAngle("Top Y Angle", Range( -1 , 1)) = 0.5
		_Top("Top", 2D) = "white" {}
		_Middle("Middle", 2D) = "white" {}
		_Bottom("Bottom", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform sampler2D _Middle;
		uniform float _MidXTiling;
		uniform float _MidYTiling;
		uniform float _MidRotatethreshold;
		uniform sampler2D _Top;
		uniform float _TopXTiling;
		uniform float _TopYTiling;
		uniform float _ToggleSwitch1;
		uniform float _TopYAngle;
		uniform sampler2D _Bottom;
		uniform float _ToggleSwitch5;
		uniform float _BtmAngle;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float2 appendResult212 = (float2(( ase_worldPos.x * _MidXTiling ) , ( ase_worldPos.y * _MidYTiling )));
			float2 appendResult213 = (float2(( ase_worldPos.z * _MidXTiling ) , ( ase_worldPos.y * _MidYTiling )));
			float clampResult211 = clamp( round( abs( ( i.worldNormal.x + _MidRotatethreshold ) ) ) , 0.0 , 1.0 );
			float2 lerpResult215 = lerp( appendResult212 , appendResult213 , clampResult211);
			float2 appendResult218 = (float2(( ase_worldPos.x * _TopXTiling ) , ( ase_worldPos.z * _TopYTiling )));
			float clampResult208 = clamp( ( i.worldNormal.y + ( 1.0 - ( _TopYAngle + 1.0 ) ) ) , 0.0 , 1.0 );
			float4 lerpResult222 = lerp( tex2D( _Middle, lerpResult215 ) , tex2D( _Top, appendResult218 ) , lerp(ceil( clampResult208 ),0.0,_ToggleSwitch1));
			float2 appendResult241 = (float2(ase_worldPos.x , ase_worldPos.z));
			float clampResult236 = clamp( ( i.worldNormal.y + ( 1.0 - _BtmAngle ) ) , 0.0 , 1.0 );
			float4 lerpResult245 = lerp( lerpResult222 , tex2D( _Bottom, appendResult241 ) , lerp(ceil( ( 1.0 - clampResult236 ) ),0.0,_ToggleSwitch5));
			o.Albedo = lerpResult245.rgb;
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
				float3 worldPos : TEXCOORD6;
				float3 worldNormal : TEXCOORD1;
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
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
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
1699;54;1522;788;4078.939;2972.759;4.150599;True;False
Node;AmplifyShaderEditor.RangedFloatNode;33;-3597.783,-1512.796;Float;False;Property;_TopYAngle;Top Y Angle;10;0;0.5;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;187;-1949.442,-1465.265;Float;False;Property;_MidRotatethreshold;Mid Rotate threshold;6;0;-0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;188;-2328.955,-1492.63;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;189;-3173.588,-1482.839;Float;False;838.5416;426.3157;;5;217;208;200;193;192;NoLerp;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;230;-480.8179,-1897.712;Float;False;838.5416;426.3157;;3;236;233;232;NoLerp;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-818.2254,-1012.116;Float;False;Property;_BtmAngle;Btm Angle;9;0;-0.5;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;192;-3123.588,-1432.838;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-2121.844,-1431.555;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;224;-707.578,-1607.131;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;196;-2561.341,-1685.825;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;232;-304.7167,-1839.913;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;180;-3400.348,-1192.257;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;195;-2011.149,-1598.933;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;193;-2997.487,-1425.04;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;198;-2532.599,-1991.582;Float;False;Property;_MidYTiling;Mid Y Tiling;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;197;-2536.5,-2078.682;Float;False;Property;_MidXTiling;Mid X Tiling;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-2180.837,-1823.617;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;233;-172.3926,-1750.65;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;-0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;202;-2183.437,-1914.618;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RoundOpNode;201;-2010.468,-1708.302;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;204;-2517.2,-1794.512;Float;False;Property;_TopYTiling;Top Y Tiling;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;205;-2191.238,-2005.618;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;207;-2521.1,-1881.612;Float;False;Property;_TopXTiling;Top X Tiling;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;200;-2865.163,-1335.776;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;-0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-2193.838,-2096.618;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-2177.881,-1573.984;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;212;-2024.684,-2018.76;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.ClampOpNode;211;-1927.574,-1803.364;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;214;-2172.464,-1666.073;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;236;-17.96235,-1724.397;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;213;-2026.884,-1911.26;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.ClampOpNode;208;-2710.733,-1309.523;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;218;-1800.271,-1579.593;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1914.209,-1110.902;Float;True;Property;_Middle;Middle;14;0;Assets/Ground/TriPlanar/bricks.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;35;-1946.423,-917.6755;Float;True;Property;_Top;Top;13;0;Assets/Ground/TriPlanar/bricks.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.LerpOp;215;-1821.695,-1980.094;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.OneMinusNode;238;247.8287,-1721.847;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CeilOpNode;217;-2489.046,-1312.84;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;239;672.8859,-1793.329;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;12;514.164,-2072.07;Float;True;Property;_Bottom;Bottom;15;0;Assets/Ground/TriPlanar/bricks.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.DynamicAppendNode;241;864.0927,-1746.819;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.ToggleSwitchNode;219;-1291.531,-1212.536;Float;False;Property;_ToggleSwitch1;Toggle Switch1;1;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;220;-1618.778,-1391.749;Float;True;Property;_TextureSample3;Texture Sample 3;10;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CeilOpNode;242;445.7225,-1695.549;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;221;-1385.93,-1717.409;Float;True;Property;_TextureSample4;Texture Sample 4;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;222;-1069.924,-1326.092;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ToggleSwitchNode;244;1400.239,-1628.41;Float;False;Property;_ToggleSwitch5;Toggle Switch5;0;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;243;1089.551,-1832.099;Float;True;Property;_TextureSample6;Texture Sample 6;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;245;1622.846,-1740.965;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;98;-818.9793,-938.1766;Float;False;Property;_BTopYAngleIntensity;B Top Y Angle Intensity;12;0;0.01;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;1474.051,-1424.399;Float;False;Property;_Smoothness;Smoothness;8;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;50;-3598.538,-1438.856;Float;False;Property;_TopYAngleIntensity;Top Y Angle Intensity;11;0;0.01;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;1471.879,-1499.712;Float;False;Property;_Metallic;Metallic;7;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1858.464,-1532.483;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;.Custom/Triplanar2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;33;0
WireConnection;190;0;188;1
WireConnection;190;1;187;0
WireConnection;232;0;99;0
WireConnection;195;0;190;0
WireConnection;193;0;192;0
WireConnection;199;0;196;2
WireConnection;199;1;198;0
WireConnection;233;0;224;2
WireConnection;233;1;232;0
WireConnection;202;0;196;3
WireConnection;202;1;197;0
WireConnection;201;0;195;0
WireConnection;205;0;196;2
WireConnection;205;1;198;0
WireConnection;200;0;180;2
WireConnection;200;1;193;0
WireConnection;203;0;196;1
WireConnection;203;1;197;0
WireConnection;210;0;196;3
WireConnection;210;1;204;0
WireConnection;212;0;203;0
WireConnection;212;1;205;0
WireConnection;211;0;201;0
WireConnection;214;0;196;1
WireConnection;214;1;207;0
WireConnection;236;0;233;0
WireConnection;213;0;202;0
WireConnection;213;1;199;0
WireConnection;208;0;200;0
WireConnection;218;0;214;0
WireConnection;218;1;210;0
WireConnection;215;0;212;0
WireConnection;215;1;213;0
WireConnection;215;2;211;0
WireConnection;238;0;236;0
WireConnection;217;0;208;0
WireConnection;241;0;239;1
WireConnection;241;1;239;3
WireConnection;219;0;217;0
WireConnection;220;0;35;0
WireConnection;220;1;218;0
WireConnection;242;0;238;0
WireConnection;221;0;11;0
WireConnection;221;1;215;0
WireConnection;222;0;221;0
WireConnection;222;1;220;0
WireConnection;222;2;219;0
WireConnection;244;0;242;0
WireConnection;243;0;12;0
WireConnection;243;1;241;0
WireConnection;245;0;222;0
WireConnection;245;1;243;0
WireConnection;245;2;244;0
WireConnection;0;0;245;0
WireConnection;0;3;13;0
WireConnection;0;4;14;0
ASEEND*/
//CHKSM=8B95D44686FAA1920C50F10450219C3158DEE563