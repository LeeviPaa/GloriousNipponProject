// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader ".Custom/Gold"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MainColor("Main Color", Color) = (0.9411765,0.8627451,0.2196078,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.6
		_Metallic("Metallic", Range( 0 , 1)) = 0.9
		_GlowColor("Glow Color", Color) = (0.9882353,1,0.7803922,0)
		_Glowmultiplier("Glow multiplier", Float) = 1
		_WaveColor("Wave Color", Color) = (0.9921569,0.9411765,0.6588235,0)
		_WaveInterval("Wave Interval", Float) = 25
		_WaveSpeed("Wave Speed", Float) = 0.15
		_WaveOffset("Wave Offset", Float) = 0.6
		_WaveSizeinvert("Wave Size invert", Float) = 9.7
		_EffectSpeed("Effect Speed", Float) = 20
		[Toggle]_YWave("Y Wave", Float) = 1
		_Dirt("Dirt", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Dirt;
		uniform float4 _Dirt_ST;
		uniform float4 _WaveColor;
		uniform float4 _MainColor;
		uniform float _YWave;
		uniform float _WaveInterval;
		uniform float _WaveSpeed;
		uniform float _WaveSizeinvert;
		uniform float4 _GlowColor;
		uniform float _Glowmultiplier;
		uniform float _Metallic;
		uniform float _Smoothness;

		UNITY_INSTANCING_CBUFFER_START(CustomGold)
			UNITY_DEFINE_INSTANCED_PROP(float, _WaveOffset)
			UNITY_DEFINE_INSTANCED_PROP(float, _EffectSpeed)
		UNITY_INSTANCING_CBUFFER_END

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Dirt = i.uv_texcoord * _Dirt_ST.xy + _Dirt_ST.zw;
			float _WaveOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(_WaveOffset);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float _EffectSpeed_Instance = UNITY_ACCESS_INSTANCED_PROP(_EffectSpeed);
			float clampResult45 = clamp( abs( ( ( ( _WaveOffset_Instance + lerp(ase_vertex3Pos.x,ase_vertex3Pos.y,_YWave) ) - ( fmod( ( ( _Time.y * 0.5 ) * _EffectSpeed_Instance ) , ( _WaveInterval * 2.0 ) ) * _WaveSpeed ) ) * _WaveSizeinvert ) ) , 0.0 , 1.0 );
			float4 lerpResult74 = lerp( _WaveColor , _MainColor , clampResult45);
			float EffectTime103 = ( ( _EffectSpeed_Instance / _WaveInterval ) * 0.5 );
			float clampResult112 = clamp( pow( sin( ( ( ( EffectTime103 * UNITY_PI ) * _Time.y ) + 0.8 ) ) , 5.0 ) , 0.0 , 1.0 );
			o.Albedo = ( ( tex2D( _Dirt, uv_Dirt ) + float4( 0.3823529,0.3823529,0.3823529,0 ) ) * ( lerpResult74 + ( ( clampResult112 * _GlowColor ) * _Glowmultiplier ) ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1618;72;1312;754;1466.959;1275.343;2.158856;True;False
Node;AmplifyShaderEditor.CommentaryNode;119;-1116.091,-618.6005;Float;False;606.3503;773.7742;;2;73;70;Calculate Panning;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1072.39,-47.9103;Float;False;InstancedProperty;_EffectSpeed;Effect Speed;10;0;20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;47;-1075.215,53.02468;Float;False;Property;_WaveInterval;Wave Interval;6;0;25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;117;-940.285,258.5813;Float;False;1822.969;372.96;;12;101;102;103;104;114;107;109;105;111;112;116;115;White Glow;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;101;-890.285,402.9618;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-759.7015,396.6829;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;46;-1106.156,-337.5381;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-595.9689,415.4258;Float;False;EffectTime;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-944.6348,-337.5491;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;114;-367.2253,504.9256;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.PiNode;104;-363.4787,382.5193;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-830.1296,14.45785;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;140;-476.8972,-199.2507;Float;False;260.2574;304.1501;Wave Speed;2;134;137;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-792.2769,-337.2635;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;50;-1203.375,-778.3071;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FmodOpNode;44;-648.1797,-337.7704;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-166.307,481.6974;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;70;-859.0556,-564.7051;Float;False;InstancedProperty;_WaveOffset;Wave Offset;8;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;137;-426.8972,-10.10062;Float;False;Property;_WaveSpeed;Wave Speed;7;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;157;-995.304,-760.9833;Float;False;Property;_YWave;Y Wave;11;0;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-46.73642,377.2995;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.8;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;118;-189.9871,-218.052;Float;False;702.348;306.3702;;3;64;66;45;Highlight Line;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-385.6398,-149.2507;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-666.679,-466.3713;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;154;-48.54895,-428.3402;Float;False;Property;_WaveSizeinvert;Wave Size invert;9;0;9.7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;105;72.94904,370.7131;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;64;-183.9871,-169.6818;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;75.45105,-331.3402;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;36.1;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;111;255.8173,378.5413;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;112;509.6758,308.8927;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;116;648.6846,412.4041;Float;False;Property;_GlowColor;Glow Color;3;0;0.9882353,1,0.7803922,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;66;197.4197,-174.6119;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;2;-334.1518,-441.728;Float;False;Property;_MainColor;Main Color;0;0;0.9411765,0.8627451,0.2196078,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;139;580.9588,182.115;Float;False;Property;_Glowmultiplier;Glow multiplier;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;166;-37.79587,-813.3479;Float;True;Property;_Dirt;Dirt;12;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;682.1701,308.5813;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;75;-328.4942,-634.0158;Float;False;Property;_WaveColor;Wave Color;5;0;0.9921569,0.9411765,0.6588235,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;45;385.361,-173.052;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;158;245.913,-628.9619;Float;True;Property;_TextureSample0;Texture Sample 0;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;773.3593,167.815;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;74;542.3457,-263.8673;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;162;593.8986,-563.3672;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.3823529,0.3823529,0.3823529,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;95;786.2745,-264.3088;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;779.1199,-521.5017;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;4;959.2906,143.5238;Float;False;Property;_Metallic;Metallic;2;0;0.9;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;962.8427,232.3175;Float;False;Property;_Smoothness;Smoothness;1;0;0.6;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1024.544,-313.5898;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;.Custom/Gold;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;101;0;73;0
WireConnection;101;1;47;0
WireConnection;102;0;101;0
WireConnection;103;0;102;0
WireConnection;133;0;46;0
WireConnection;104;0;103;0
WireConnection;81;0;47;0
WireConnection;72;0;133;0
WireConnection;72;1;73;0
WireConnection;44;0;72;0
WireConnection;44;1;81;0
WireConnection;107;0;104;0
WireConnection;107;1;114;0
WireConnection;157;0;50;1
WireConnection;157;1;50;2
WireConnection;109;0;107;0
WireConnection;134;0;44;0
WireConnection;134;1;137;0
WireConnection;61;0;70;0
WireConnection;61;1;157;0
WireConnection;105;0;109;0
WireConnection;64;0;61;0
WireConnection;64;1;134;0
WireConnection;153;0;64;0
WireConnection;153;1;154;0
WireConnection;111;0;105;0
WireConnection;112;0;111;0
WireConnection;66;0;153;0
WireConnection;115;0;112;0
WireConnection;115;1;116;0
WireConnection;45;0;66;0
WireConnection;158;0;166;0
WireConnection;138;0;115;0
WireConnection;138;1;139;0
WireConnection;74;0;75;0
WireConnection;74;1;2;0
WireConnection;74;2;45;0
WireConnection;162;0;158;0
WireConnection;95;0;74;0
WireConnection;95;1;138;0
WireConnection;159;0;162;0
WireConnection;159;1;95;0
WireConnection;0;0;159;0
WireConnection;0;3;4;0
WireConnection;0;4;3;0
ASEEND*/
//CHKSM=A65A0111A698719FC78CA246E3BB04BF6BBFFAE6