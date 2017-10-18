// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader ".Custom/Ocean"
{
	Properties
	{
		[HideInInspector] __dirty("", Int) = 1
		[HideInInspector] _DummyTex("", 2D) = "white" {}
	_Cutoff("Mask Clip Value", Float) = 0.5
		_TessValue("Max Tessellation", Range(1, 32)) = 4
		_TessMin("Tess Min Distance", Float) = 10
		_TessMax("Tess Max Distance", Float) = 25
		_Water_Tex("Water_Tex", 2D) = "white" {}
	_TextureSample1("Texture Sample 1", 2D) = "white" {}
	_T1Size("T1 Size", Float) = 1
		_T1XSpeed("T1 X Speed", Range(0 , 2)) = 0
		_Float2("Float 2", Range(-2 , 2)) = 0
		_Float3("Float 3", Range(-2 , 2)) = 1
		_Float1("Float 1", Range(0 , 2)) = 1
		_T2Size("T2 Size", Float) = 1
		_T2XSpeed("T2 X Speed", Range(0 , 2)) = 0
		_T2YSpeed("T2 Y Speed", Range(0 , 2)) = 1
		_Emission("Emission", Float) = 0.1
		_Fadedistance("Fade distance", Float) = 0
		_flowmap_random2("flowmap_random2", 2D) = "white" {}
	_CloseupTransparency("Closeup Transparency", Float) = 0
		_Texture0("Texture 0", 2D) = "white" {}
	_Cloudssize("Clouds size", Float) = 0
		_Debug("Debug", Float) = 0
	}

		SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		ZWrite On
		CGINCLUDE
#include "UnityShaderVariables.cginc"
#include "Tessellation.cginc"
#include "UnityPBSLighting.cginc"
#include "Lighting.cginc"
#pragma target 4.6
		struct Input
	{
		float2 uv_DummyTex;
		float3 worldPos;
	};

	struct appdata
	{
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
		float4 texcoord1 : TEXCOORD1;
		float4 texcoord2 : TEXCOORD2;
		float4 texcoord3 : TEXCOORD3;
		fixed4 color : COLOR;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	uniform sampler2D _Water_Tex;
	uniform float _T1XSpeed;
	uniform float _Float1;
	uniform sampler2D _DummyTex;
	uniform float _T1Size;
	uniform sampler2D _TextureSample1;
	uniform float _T2XSpeed;
	uniform float _T2YSpeed;
	uniform float _T2Size;
	uniform sampler2D _Texture0;
	uniform float _Float2;
	uniform float _Float3;
	uniform sampler2D _flowmap_random2;
	uniform float _Cloudssize;
	uniform float _Emission;
	uniform float _CloseupTransparency;
	uniform float _Fadedistance;
	uniform float _Debug;
	uniform float _Cutoff = 0.5;
	uniform float _TessValue;
	uniform float _TessMin;
	uniform float _TessMax;

	float4 tessFunction(appdata v0, appdata v1, appdata v2)
	{
		return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue);
	}

	void vertexDataFunc(inout appdata v)
	{
		float3 ase_vertexNormal = v.normal.xyz;
		float2 appendResult95 = (float2(_Float2 , _Float3));
		v.texcoord.xy = v.texcoord.xy * float2(1,1) + float2(0,0);
		float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex);
		float2 appendResult27 = (float2((v.texcoord.xy.x / ase_worldPos.x) , (v.texcoord.xy.y / ase_worldPos.z)));
		float2 temp_output_25_0 = (v.texcoord.xy / appendResult27);
		float2 temp_output_94_0 = (temp_output_25_0 / _Cloudssize);
		float2 panner97 = ((tex2Dlod(_flowmap_random2, float4(temp_output_94_0, 0.0 , 0.0)) + float4(temp_output_94_0, 0.0 , 0.0)).rg + _Time.y * appendResult95);
		float4 tex2DNode90 = tex2Dlod(_Texture0, float4(panner97, 0.0 , 0.0));
		v.vertex.xyz += ((float4(ase_vertexNormal , 0.0) * tex2DNode90) * (_Debug * 0.001)).rgb;
	}

	void surf(Input i , inout SurfaceOutputStandard o)
	{
		float2 appendResult34 = (float2(_T1XSpeed , _Float1));
		float2 texCoordDummy9 = i.uv_DummyTex*float2(1,1) + float2(0,0);
		float3 ase_worldPos = i.worldPos;
		float2 appendResult27 = (float2((texCoordDummy9.x / ase_worldPos.x) , (texCoordDummy9.y / ase_worldPos.z)));
		float2 temp_output_25_0 = (texCoordDummy9 / appendResult27);
		float2 panner4 = ((temp_output_25_0 / _T1Size) + _Time.y * appendResult34);
		float2 appendResult42 = (float2(_T2XSpeed , _T2YSpeed));
		float2 panner40 = ((temp_output_25_0 / _T2Size) + _Time.y * appendResult42);
		float2 appendResult95 = (float2(_Float2 , _Float3));
		float2 temp_output_94_0 = (temp_output_25_0 / _Cloudssize);
		float2 panner97 = ((tex2D(_flowmap_random2, temp_output_94_0) + float4(temp_output_94_0, 0.0 , 0.0)).rg + _Time.y * appendResult95);
		float4 tex2DNode90 = tex2D(_Texture0, panner97);
		float4 temp_output_91_0 = ((tex2D(_Water_Tex, panner4) * tex2D(_TextureSample1, panner40)) * (tex2DNode90 + _Emission));
		o.Albedo = temp_output_91_0.rgb;
		float4 ase_vertex4Pos = mul(unity_WorldToObject, float4(i.worldPos , 1));
		float3 ase_viewPos = UnityObjectToViewPos(ase_vertex4Pos);
		float ase_screenDepth = -ase_viewPos.z;
		float clampResult69 = clamp((_CloseupTransparency + ((ase_screenDepth * pow(ase_screenDepth , 1.5)) * (_Fadedistance * 1E-05))) , 0.0 , 1.0);
		float clampResult129 = clamp(clampResult69 , 0.5 , 1.0);
		o.Alpha = clampResult129;
	}

	ENDCG
		CGPROGRAM
#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
	{
		Name "ShadowCaster"
		Tags{ "LightMode" = "ShadowCaster" }
		ZWrite On
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 4.6
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
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};
	v2f vert(appdata_full v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		Input customInputData;
		vertexDataFunc(v);
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		o.worldPos = worldPos;
		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			return o;
	}
	fixed4 frag(v2f IN
#if !defined( CAN_SKIP_VPOS )
		, UNITY_VPOS_TYPE vpos : VPOS
#endif
	) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(IN);
	Input surfIN;
	UNITY_INITIALIZE_OUTPUT(Input, surfIN);
	float3 worldPos = IN.worldPos;
	fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
	surfIN.worldPos = worldPos;
	SurfaceOutputStandard o;
	UNITY_INITIALIZE_OUTPUT(SurfaceOutputStandard, o)
		surf(surfIN, o);
#if defined( CAN_SKIP_VPOS )
	float2 vpos = IN.pos;
#endif
	half alphaRef = tex3D(_DitherMaskLOD, float3(vpos.xy * 0.25, o.Alpha * 0.9375)).a;
	clip(alphaRef - 0.01);
	SHADOW_CASTER_FRAGMENT(IN)
	}
		ENDCG
	}
	}
		Fallback "Diffuse"
		CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
908;100;620;694;1305.221;1767.15;2.743437;False;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-2581.352,-825.5911;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;3;-2561.992,-663.5544;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;28;-2223.35,-629.1313;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-2276.811,-759.1131;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;27;-2125.034,-741.9531;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;82;-1589.896,33.02644;Float;False;1095.919;460.3334;;6;73;69;78;86;62;87;Distance Fade;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;25;-2004.631,-874.3183;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;98;-1885.555,-1197.11;Float;False;Property;_Cloudssize;Clouds size;21;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;94;-1595.423,-1276.23;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SurfaceDepthNode;62;-1539.896,83.0264;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;85;-1593.891,-897.5197;Float;False;1183.862;416.4309;;6;35;36;31;34;5;4;Primary texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-2006.78,-708.9698;Float;False;Property;_T1Size;T1 Size;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;139;-1567.041,-1562.902;Float;True;Property;_flowmap_random2;flowmap_random2;18;0;Assets/flowmap_random2.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-1543.891,-602.6399;Float;False;Property;_Float1;Float 1;12;0;1;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;35;-1523.672,-714.5049;Float;False;Property;_T1XSpeed;T1 X Speed;9;0;0;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-2013.56,-622.0081;Float;False;Property;_T2Size;T2 Size;13;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;93;-1623.924,-1041.815;Float;False;Property;_Float3;Float 3;11;0;1;-2;2;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;41;-1541.662,-180.4091;Float;False;Property;_T2YSpeed;T2 Y Speed;15;0;1;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;73;-1354.403,170.1728;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.5;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-1603.705,-1153.68;Float;False;Property;_Float2;Float 2;10;0;0;-2;2;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;-1540.045,-292.7175;Float;False;Property;_T2XSpeed;T2 X Speed;14;0;0;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;71;-1529.016,371.5602;Float;False;Property;_Fadedistance;Fade distance;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;34;-1230.991,-716.6182;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1313.419,360.36;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1E-05;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-1261.461,-1423.596;Float;True;2;2;0;COLOR;0,0;False;1;FLOAT2;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.DynamicAppendNode;95;-1311.024,-1155.793;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleTimeNode;83;-1271.578,-126.4276;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1209.402,88.82617;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;-1296.674,-847.5197;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleDivideOpNode;79;-1242.772,-330.8988;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;42;-1250.818,-228.2189;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1227.154,-591.0888;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;96;-1307.187,-1030.264;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;89;-932.0869,-1417.435;Float;True;Property;_Texture0;Texture 0;20;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Sand/Sand_height.tga;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-974.607,245.2047;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.01;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;97;-1132.922,-1175.794;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;87;-995.0488,69.46;Float;False;Property;_CloseupTransparency;Closeup Transparency;19;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;40;-1079.093,-326.2649;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;4;-1052.889,-736.6194;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;39;-799.8339,-278.6824;Float;True;Property;_TextureSample1;Texture Sample 1;7;0;Assets/TexturesCom_WaterPlain0017_1_seamless_S.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;90;-582.621,-1234.843;Float;True;Property;_sand_dunes_height_map__seamless__by_elmininostockd9s4yz3;sand_dunes_height_map__seamless__by_elmininostock-d9s4yz3;11;0;Assets/sand_dunes_height_map__seamless__by_elmininostock-d9s4yz3.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;107;-267.8379,-1013.766;Float;False;Property;_Emission;Emission;16;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;103;-317.2567,-124.2971;Float;False;Property;_Debug;Debug;22;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;104;-348.6591,-797.343;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-833.0294,-753.587;Float;True;Property;_Water_Tex;Water_Tex;6;0;Assets/TexturesCom_WaterPlain0017_1_seamless_S.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-672.7078,103.6908;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-76.00848,-269.2899;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-68.40963,-1036.83;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-114.0654,-99.60797;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.001;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-336.9485,-509.0305;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;69;-794.8383,301.6284;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;116;-284.1938,324.4037;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;78;-639.614,319.1627;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;129;401.1063,57.34803;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.5;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;459.2906,194.2145;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;55.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;118;111.1165,-43.66638;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;5.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;90.47746,-229.7638;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;115;-76.59293,353.7419;Float;False;Constant;_Float4;Float 4;17;0;7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CameraWorldClipPlanes;135;583.0775,-543.7596;Float;False;Left;0;1;FLOAT4
Node;AmplifyShaderEditor.ZBufferParams;136;367.0476,-677.3417;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;501.6464,-487.7693;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.1,0.1,0.01,0.01;False;1;FLOAT4
Node;AmplifyShaderEditor.FresnelNode;113;97.69527,220.8877;Float;True;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;119;-145.4315,230.5724;Float;False;Constant;_Float5;Float 5;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;124;397.4153,-176.7871;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.6;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;125;527.8886,-365.8143;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;137.2656,-520.2294;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WorldNormalVector;121;25.57637,-383.8863;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;773.0081,-289.1255;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;.Custom/Ocean;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;1;0;False;0;0;Custom;0.5;True;True;0;True;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0.1;0,0,0,0;VertexScale;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;44;-1591.662,-376.2648;Float;False;1214.827;327.5825;;0;Secondary texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;84;-2631.352,-924.3183;Float;False;820.5718;428.187;Prevents stretching etc;0;World position;1,1,1,1;0;0
WireConnection;28;0;9;2
WireConnection;28;1;3;3
WireConnection;26;0;9;1
WireConnection;26;1;3;1
WireConnection;27;0;26;0
WireConnection;27;1;28;0
WireConnection;25;0;9;0
WireConnection;25;1;27;0
WireConnection;94;0;25;0
WireConnection;94;1;98;0
WireConnection;139;1;94;0
WireConnection;73;0;62;0
WireConnection;34;0;35;0
WireConnection;34;1;36;0
WireConnection;72;0;71;0
WireConnection;128;0;139;0
WireConnection;128;1;94;0
WireConnection;95;0;92;0
WireConnection;95;1;93;0
WireConnection;74;0;62;0
WireConnection;74;1;73;0
WireConnection;31;0;25;0
WireConnection;31;1;32;0
WireConnection;79;0;25;0
WireConnection;79;1;80;0
WireConnection;42;0;43;0
WireConnection;42;1;41;0
WireConnection;70;0;74;0
WireConnection;70;1;72;0
WireConnection;97;0;128;0
WireConnection;97;2;95;0
WireConnection;97;1;96;0
WireConnection;40;0;79;0
WireConnection;40;2;42;0
WireConnection;40;1;83;0
WireConnection;4;0;31;0
WireConnection;4;2;34;0
WireConnection;4;1;5;0
WireConnection;39;1;40;0
WireConnection;90;0;89;0
WireConnection;90;1;97;0
WireConnection;2;1;4;0
WireConnection;86;0;87;0
WireConnection;86;1;70;0
WireConnection;105;0;104;0
WireConnection;105;1;90;0
WireConnection;101;0;90;0
WireConnection;101;1;107;0
WireConnection;106;0;103;0
WireConnection;45;0;2;0
WireConnection;45;1;39;0
WireConnection;69;0;86;0
WireConnection;78;0;69;0
WireConnection;129;0;69;0
WireConnection;120;0;113;0
WireConnection;118;0;104;0
WireConnection;102;0;105;0
WireConnection;102;1;106;0
WireConnection;137;0;136;0
WireConnection;113;0;121;0
WireConnection;113;1;119;0
WireConnection;113;3;115;0
WireConnection;124;0;120;0
WireConnection;125;0;91;0
WireConnection;125;1;124;0
WireConnection;91;0;45;0
WireConnection;91;1;101;0
WireConnection;0;0;91;0
WireConnection;0;9;129;0
WireConnection;0;11;102;0
ASEEND*/
//CHKSM=D7574BA2AD787F3C24A93CB298A522BE59CB76A4