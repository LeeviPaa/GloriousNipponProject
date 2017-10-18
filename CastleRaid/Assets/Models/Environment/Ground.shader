// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader ".Custom/TestAmplify"
{
	Properties
	{
		[HideInInspector] __dirty("", Int) = 1
		_Green("Green", 2D) = "white" {}
	_Blue("Blue", 2D) = "white" {}
	_Red("Red", 2D) = "white" {}
	[HideInInspector] _texcoord("", 2D) = "white" {}
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
#pragma target 3.0
#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
	{
		float2 uv_texcoord;
		float4 vertexColor : COLOR;
	};

	uniform sampler2D _Red;
	uniform float4 _Red_ST;
	uniform sampler2D _Green;
	uniform float4 _Green_ST;
	uniform sampler2D _Blue;
	uniform float4 _Blue_ST;

	void surf(Input i , inout SurfaceOutputStandard o)
	{
		float2 uv_Red = i.uv_texcoord * _Red_ST.xy + _Red_ST.zw;
		float4 lerpResult5 = lerp(float4(0,0,0,0) , tex2D(_Red, uv_Red) , i.vertexColor.r);
		float2 uv_Green = i.uv_texcoord * _Green_ST.xy + _Green_ST.zw;
		float4 tex2DNode6 = tex2D(_Green, uv_Green);
		float4 lerpResult7 = lerp(lerpResult5 , (lerpResult5 + tex2DNode6) , i.vertexColor.g);
		float2 uv_Blue = i.uv_texcoord * _Blue_ST.xy + _Blue_ST.zw;
		float4 lerpResult12 = lerp(lerpResult7 , (lerpResult7 + tex2D(_Blue, uv_Blue)) , i.vertexColor.b);
		o.Albedo = lerpResult12.rgb;
		o.Alpha = 1;
	}

	ENDCG
	}
		Fallback "Diffuse"
		CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1927;51;1476;767;2489.16;957.9744;2.448;True;True
Node;AmplifyShaderEditor.VertexColorNode;1;-1768.427,-404.4263;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;11;-1447.433,-704.598;Float;True;Property;_Red;Red;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-1777.068,28.3131;Float;True;Property;_Green;Green;0;0;Assets/green.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;5;-932.1821,-492.0893;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-994.8317,-34.61982;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;7;-532.6277,-194.4308;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;13;-1770.131,305.8954;Float;True;Property;_Blue;Blue;1;0;Assets/blue.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-661.5669,103.3544;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;12;-345.8077,29.94566;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-797.4208,-270.2387;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;.Custom/TestAmplify;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;1;11;0
WireConnection;5;2;1;1
WireConnection;17;0;5;0
WireConnection;17;1;6;0
WireConnection;7;0;5;0
WireConnection;7;1;17;0
WireConnection;7;2;1;2
WireConnection;18;0;7;0
WireConnection;18;1;13;0
WireConnection;12;0;7;0
WireConnection;12;1;18;0
WireConnection;12;2;1;3
WireConnection;19;0;5;0
WireConnection;19;1;6;0
WireConnection;0;0;12;0
ASEEND*/
//CHKSM=372E470125D4E9BF033D3306728B8136F19FB2EE