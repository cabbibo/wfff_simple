﻿Shader "Custom/BufferTailRenderer" {

	Properties {
        _NormalMap( "Normal Map" , 2D ) = "white" {}
        _CubeMap( "Cube Map" , Cube ) = "white" {}
    }
  SubShader{

  	

    
    Cull off
    Pass{


      CGPROGRAM
      #pragma target 5.0

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      uniform sampler2D _NormalMap;
      uniform samplerCUBE _CubeMap;

      uniform int _NumberTransforms;
      uniform int _NumberTriangles;


		  struct Vertebrae{
		    float id;
		    float4x4 localToWorld; 
		  };

		  struct Vert{

		    float3 pos;
		    float3 norm;
		    float3 tan;
		    float3 color;
		    float2 uv;
		    float id;
		    
		  };

 
      StructuredBuffer<Vert> vertBuffer;
      StructuredBuffer<Vertebrae> vertebraeBuffer;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos 			: SV_POSITION;
          float3 nor 			: TEXCOORD0;
          float2 uv  			: TEXCOORD1;
          float2 suv 			: TEXCOORD2;
          float3 col 			: TEXCOORD3;
          float  lamb 		: TEXCOORD4;
          float3 eye      : TEXCOORD5;
          float3 worldPos : TEXCOORD6;
      };

      varyings vert (uint id : SV_VertexID){

        varyings o;

        //o.id = id;
        
        uint vertebraeID = floor( id / _NumberTriangles );
        uint vertID = id % _NumberTriangles;

        Vertebrae vertebrae = vertebraeBuffer[vertebraeID];
        Vert vert = vertBuffer[vertID];

        float3 fPos = mul( vertebrae.localToWorld, float4( vert.pos * ((float(_NumberTransforms)-vertebrae.id) / float(_NumberTransforms)) ,1)).xyz;
        float3 fNorm = normalize( mul( vertebrae.localToWorld, float4( vert.norm,0)).xyz);

				o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));
				o.uv = vert.uv;
				o.nor = normalize(fNorm);
        o.col = vert.color;

        o.worldPos = fPos;
        o.eye = _WorldSpaceCameraPos - o.worldPos;

        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {

      	float3 fNorm = v.nor;
        float3 col = v.nor * .5 + .5;

       	float3 fRefl = reflect( -normalize(v.eye) , fNorm );
       	float3 cubeCol = texCUBE(_CubeMap,fRefl ).rgb;

       	col =  (fRefl * .5 + .5 ) * cubeCol * 2 * v.col;
        
        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}