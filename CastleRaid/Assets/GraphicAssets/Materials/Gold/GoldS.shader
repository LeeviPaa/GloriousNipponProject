// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader ".Custom/Gold"
{
	Properties
	{
		[HideInInspector] __dirty("", Int) = 1
		_ASEOutlineWidth("Outline Width", Float) = 0
		_ASEOutlineColor("Outline Color", Color) = (0, 0, 0, 0)
		_MainColor("Main Color", Color) = (0.9411765, 0.8627451, 0.2196078, 0)
		_Smoothness("Smoothness", Float) = 0.6
		_Metallic("Metallic", Float) = 0.9
		_GlowColor("Glow Color", Color) = (0.9882353, 1, 0.7803922, 0)
		_GlowMultiplier("Glow Multiplier", Float) = 1
		_WaveColor("Wave Color", Color) = (0.9921569, 0.9411765, 0.6588235, 0)
		_WaveInterval("Wave Interval", Float) = 25
		_WaveSpeed("Wave Speed", Float) = 0.15
		_WaveOffset("Wave Offset", Float) = 0.6
		_WaveSizeinvert("Wave Size invert", Float) = 7.5
		_WaveEmission("Wave Emission", Float) = 0.15
		_GlowEmission("Glow Emission", Float) = 1
		_BaseEmission("Base Emission", Float) = 0.15
		_EffectSpeed("Effect Speed", Float) = 20
		[Toggle]_YWave("Y Wave", Float) = 1
		_Dirt("Dirt", 2D) = "white" {}
	_DirtAdd("Dirt Add", Range(0, 1.5)) = 1.5
		[Toggle]_RandomTiming("Random Timing", Int) = 0
		[Toggle]_Sweepingglow("Sweeping glow", Int) = 0
		_MainTexture("Main Texture", 2D) = "white" {}
	_SmoothnessTexture("Smoothness Texture", 2D) = "white" {}
	_MetallicTexture("Metallic Texture", 2D) = "white" {}
	[HideInInspector] _texcoord("", 2D) = "white" {}
	}
		SubShader
	{
		Tags{}
		Cull Front
		CGPROGRAM
#pragma target 3.0
#pragma surface outlineSurf Outline keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:outlineVertexDataFunc
#pragma multi_compile_instancing
		struct Input
	{
		fixed filler;
	};
	UNITY_INSTANCING_CBUFFER_START(CustomGold)
		UNITY_DEFINE_INSTANCED_PROP(fixed4, _ASEOutlineColor)
		UNITY_DEFINE_INSTANCED_PROP(fixed, _ASEOutlineWidth)
		UNITY_INSTANCING_CBUFFER_END
		void outlineVertexDataFunc(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);
		v.vertex.xyz += (v.normal * UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineWidth));
	}
	inline fixed4 LightingOutline(SurfaceOutput s, half3 lightDir, half atten) { return fixed4(0, 0, 0, s.Alpha); }
	void outlineSurf(Input i, inout SurfaceOutput o) { o.Emission = UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineColor).rgb; o.Alpha = 1; }
	ENDCG

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true" }
		Cull Back
		CGPROGRAM
#include "UnityShaderVariables.cginc"
#pragma target 3.0
#pragma multi_compile_instancing
#pragma shader_feature _RANDOMTIMING_ON
#pragma shader_feature _SWEEPINGGLOW_ON
#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
	{
		float2 uv_texcoord;
		float3 worldPos;
	};
	uniform sampler2D _Dirt;
	uniform float4 _Dirt_ST;
	uniform float _DirtAdd;
	uniform float4 _WaveColor;
	uniform float4 _MainColor;
	uniform float _YWave;
	uniform float _WaveInterval;
	uniform float _WaveSpeed;
	uniform float _WaveSizeinvert;
	uniform float4 _GlowColor;
	uniform float _GlowMultiplier;
	uniform sampler2D _MainTexture;
	uniform float4 _MainTexture_ST;
	uniform float _BaseEmission;
	uniform float _GlowEmission;
	uniform float _WaveEmission;
	uniform float _Metallic;
	uniform sampler2D _MetallicTexture;
	uniform float4 _MetallicTexture_ST;
	uniform float _Smoothness;
	uniform sampler2D _SmoothnessTexture;
	uniform float4 _SmoothnessTexture_ST;
	UNITY_INSTANCING_CBUFFER_START(CustomGold)
		UNITY_DEFINE_INSTANCED_PROP(float, _WaveOffset)
		UNITY_DEFINE_INSTANCED_PROP(float, _EffectSpeed)
		UNITY_INSTANCING_CBUFFER_END
		void surf(Input i, inout SurfaceOutputStandard o)
	{
		float2 uv_Dirt = i.uv_texcoord * _Dirt_ST.xy + _Dirt_ST.zw;
		float4 temp_output_162_0 = (tex2D(_Dirt, uv_Dirt) + _DirtAdd);
		float _WaveOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(_WaveOffset);
		float3 ase_vertex3Pos = mul(unity_WorldToObject, float4(i.worldPos, 1));
		float _EffectSpeed_Instance = UNITY_ACCESS_INSTANCED_PROP(_EffectSpeed);
		float clampResult45 = clamp(abs((((_WaveOffset_Instance + lerp(ase_vertex3Pos.x, ase_vertex3Pos.y, _YWave)) - (fmod(((_Time.y * 0.5) * _EffectSpeed_Instance), (_WaveInterval * 2.0)) * _WaveSpeed)) * _WaveSizeinvert)), 0.0, 1.0);
		float4 lerpResult74 = lerp(_WaveColor, _MainColor, clampResult45);
		float EffectTime103 = ((_EffectSpeed_Instance / _WaveInterval) * 0.5);
		float3 ase_worldPos = i.worldPos;
#ifdef _RANDOMTIMING_ON
		float staticSwitch181 = (ase_worldPos.z + (ase_worldPos.x + ase_worldPos.y));
#else
		float staticSwitch181 = (1.0 - ase_vertex3Pos.y);
#endif
		float WorldOffset184 = staticSwitch181;
#ifdef _SWEEPINGGLOW_ON
		float staticSwitch198 = WorldOffset184;
#else
		float staticSwitch198 = 0.0;
#endif
		float clampResult112 = clamp(pow(sin((((EffectTime103 * UNITY_PI) * _Time.y) + staticSwitch198)), 5.0), 0.0, 1.0);
		float4 temp_output_138_0 = ((clampResult112 * _GlowColor) * _GlowMultiplier);
		float4 temp_output_159_0 = (temp_output_162_0 * (lerpResult74 + temp_output_138_0));
		float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
		o.Albedo = (temp_output_159_0 * tex2D(_MainTexture, uv_MainTexture)).rgb;
		o.Emission = (((_BaseEmission * temp_output_159_0) + ((_GlowEmission * temp_output_138_0) * temp_output_159_0)) + (_WaveEmission * (1.0 - clampResult45))).rgb;
		float2 uv_MetallicTexture = i.uv_texcoord * _MetallicTexture_ST.xy + _MetallicTexture_ST.zw;
		o.Metallic = (_Metallic * tex2D(_MetallicTexture, uv_MetallicTexture).r);
		float2 uv_SmoothnessTexture = i.uv_texcoord * _SmoothnessTexture_ST.xy + _SmoothnessTexture_ST.zw;
		o.Smoothness = (_Smoothness * tex2D(_SmoothnessTexture, uv_SmoothnessTexture)).r;
		o.Alpha = 1;
	}
	ENDCG
	}
		Fallback "Diffuse"
		CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1927;95;1312;754;-410.128;51.78259;1.498586;True;False
Node;AmplifyShaderEditor.CommentaryNode;119;-1116.091,-618.6005;Float;False;606.3503;773.7742;;3;73;61;196;Calculate Panning;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1075.215,53.02468;Float;False;Property;_WaveInterval;Wave Interval;6;0;25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;186;-2669.728,-463.8224;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;73;-1072.39,-47.9103;Float;False;InstancedProperty;_EffectSpeed;Effect Speed;13;0;20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;117;-940.285,258.5813;Float;False;1822.969;372.96;;12;101;102;103;104;114;107;109;105;111;112;116;115;White Glow;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;50;-1721.397,-783.6439;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-2379.57,-381.8152;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;101;-890.285,402.9618;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;192;-2075.105,-383.5142;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;197;-1628.723,-577.3341;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-759.7015,396.6829;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;46;-1106.156,-337.5381;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;181;-1726.219,-439.7179;Float;False;Property;_RandomTiming;Random Timing;17;0;0;False;True;;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-944.6348,-337.5491;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-595.9689,415.4258;Float;False;EffectTime;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PiNode;104;-363.4787,382.5193;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;199;-1480.447,411.1576;Float;False;Constant;_Float0;Float 0;19;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;184;-1455.706,-400.0924;Float;False;WorldOffset;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-830.1296,14.45785;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;114;-367.2253,504.9256;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-792.2769,-337.2635;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FmodOpNode;44;-648.1797,-337.7704;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;157;-995.304,-760.9833;Float;False;Property;_YWave;Y Wave;14;0;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;70;-734.1558,-700.0047;Float;False;InstancedProperty;_WaveOffset;Wave Offset;8;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-166.307,481.6974;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;137;-426.8972,-10.10062;Float;False;Property;_WaveSpeed;Wave Speed;7;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;198;-1233.323,375.3673;Float;False;Property;_Sweepingglow;Sweeping glow;18;0;0;False;True;;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-385.6398,-149.2507;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;118;-189.9871,-218.052;Float;False;702.348;306.3702;;3;64;66;45;Highlight Line;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-46.73642,377.2995;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-666.679,-466.3713;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;64;-183.9871,-169.6818;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;154;-48.54895,-428.3402;Float;False;Property;_WaveSizeinvert;Wave Size invert;9;0;7.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;105;72.94904,370.7131;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;75.45105,-331.3402;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;36.1;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;111;255.8173,378.5413;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;116;648.6846,412.4041;Float;False;Property;_GlowColor;Glow Color;3;0;0.9882353,1,0.7803922,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;112;509.6758,308.8927;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;66;197.4197,-174.6119;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;139;580.9588,182.115;Float;False;Property;_GlowMultiplier;Glow Multiplier;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;166;6.677364,-675.2467;Float;True;Property;_Dirt;Dirt;15;0;Assets/Gold/Clouds.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.ClampOpNode;45;385.361,-173.052;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;2;-334.1518,-441.728;Float;False;Property;_MainColor;Main Color;0;0;0.9411765,0.8627451,0.2196078,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;682.1701,308.5813;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;75;-328.4942,-634.0158;Float;False;Property;_WaveColor;Wave Color;5;0;0.9921569,0.9411765,0.6588235,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;158;255.6165,-671.0945;Float;True;Property;_TextureSample0;Texture Sample 0;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;773.3593,167.815;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.CommentaryNode;175;724.8976,-951.2914;Float;False;856.5009;559.5641;Emission;10;168;172;169;173;171;174;176;177;178;179;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;74;542.3457,-263.8673;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;180;224.8846,-447.4137;Float;False;Property;_DirtAdd;Dirt Add;16;0;1.5;0;1.5;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;162;571.3818,-596.6083;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;95;779.8683,-272.8506;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;169;711.1528,-858.7411;Float;False;Property;_GlowEmission;Glow Emission;11;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;1034.222,-291.8082;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;173;1137.2,-901.2914;Float;False;Property;_BaseEmission;Base Emission;12;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;939.4172,-788.6987;Float;True;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;178;707.7361,-681.3295;Float;False;Property;_WaveEmission;Wave Emission;10;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;1142.411,-721.4083;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;176;778.1365,-512.1863;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;1331.981,-788.9269;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.TexturePropertyNode;203;897.7915,-35.70391;Float;True;Property;_MainTexture;Main Texture;19;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;208;942.9474,252.1731;Float;True;Property;_MetallicTexture;Metallic Texture;21;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;210;1030.138,642.378;Float;True;Property;_SmoothnessTexture;Smoothness Texture;20;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SimpleAddOpNode;172;1366.701,-610.5924;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;209;1273.718,548.8352;Float;True;Property;_TextureSample3;Texture Sample 3;19;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;979.8171,496.3601;Float;False;Property;_Smoothness;Smoothness;1;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;1020.08,-525.8844;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;202;1151.614,-64.8789;Float;True;Property;_TextureSample1;Texture Sample 1;19;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;207;1186.528,158.6304;Float;True;Property;_TextureSample2;Texture Sample 2;19;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;4;949.8604,193.3714;Float;False;Property;_Metallic;Metallic;2;0;0.9;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;205;1469.43,-95.9821;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;1503.379,218.9829;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;204;1358.756,-285.1501;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;179;1441.894,-481.464;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;-1023.495,-517.1639;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1846.815,-313.3666;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;.Custom/Gold;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;True;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;140;-476.8972,-199.2507;Float;False;260.2576;304.1504;Wave Speed;0;;1,1,1,1;0;0
WireConnection;191;0;186;1
WireConnection;191;1;186;2
WireConnection;101;0;73;0
WireConnection;101;1;47;0
WireConnection;192;0;186;3
WireConnection;192;1;191;0
WireConnection;197;0;50;2
WireConnection;102;0;101;0
WireConnection;181;0;192;0
WireConnection;181;1;197;0
WireConnection;133;0;46;0
WireConnection;103;0;102;0
WireConnection;104;0;103;0
WireConnection;184;0;181;0
WireConnection;81;0;47;0
WireConnection;72;0;133;0
WireConnection;72;1;73;0
WireConnection;44;0;72;0
WireConnection;44;1;81;0
WireConnection;157;0;50;1
WireConnection;157;1;50;2
WireConnection;107;0;104;0
WireConnection;107;1;114;0
WireConnection;198;0;184;0
WireConnection;198;1;199;0
WireConnection;134;0;44;0
WireConnection;134;1;137;0
WireConnection;109;0;107;0
WireConnection;109;1;198;0
WireConnection;61;0;70;0
WireConnection;61;1;157;0
WireConnection;64;0;61;0
WireConnection;64;1;134;0
WireConnection;105;0;109;0
WireConnection;153;0;64;0
WireConnection;153;1;154;0
WireConnection;111;0;105;0
WireConnection;112;0;111;0
WireConnection;66;0;153;0
WireConnection;45;0;66;0
WireConnection;115;0;112;0
WireConnection;115;1;116;0
WireConnection;158;0;166;0
WireConnection;138;0;115;0
WireConnection;138;1;139;0
WireConnection;74;0;75;0
WireConnection;74;1;2;0
WireConnection;74;2;45;0
WireConnection;162;0;158;0
WireConnection;162;1;180;0
WireConnection;95;0;74;0
WireConnection;95;1;138;0
WireConnection;159;0;162;0
WireConnection;159;1;95;0
WireConnection;171;0;169;0
WireConnection;171;1;138;0
WireConnection;168;0;171;0
WireConnection;168;1;159;0
WireConnection;176;0;45;0
WireConnection;174;0;173;0
WireConnection;174;1;159;0
WireConnection;172;0;174;0
WireConnection;172;1;168;0
WireConnection;209;0;210;0
WireConnection;177;0;178;0
WireConnection;177;1;176;0
WireConnection;202;0;203;0
WireConnection;207;0;208;0
WireConnection;205;0;4;0
WireConnection;205;1;207;1
WireConnection;206;0;3;0
WireConnection;206;1;209;0
WireConnection;204;0;159;0
WireConnection;204;1;202;0
WireConnection;179;0;172;0
WireConnection;179;1;177;0
WireConnection;196;0;184;0
WireConnection;196;1;70;0
WireConnection;0;0;204;0
WireConnection;0;2;179;0
WireConnection;0;3;205;0
WireConnection;0;4;206;0
ASEEND*/
//CHKSM=24FAE74C8F26F00FE4EACD3405EDD9CF865D8460