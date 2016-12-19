Shader "Custom/RainbowSpring" {

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

      #include "Chunks/uvNormalMap.cginc"



		  struct Vert{
	  	  float3 pos;
			  float3 ogPos;
			  float3 vel;
			  float3 norm;
			  float3 tan;
			  float3 color;
			  float2 uv;
			  float id;
		  };

 
      StructuredBuffer<Vert> vertBuffer;
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

        Vert vert = vertBuffer[id];

				o.pos = mul (UNITY_MATRIX_VP, float4(vert.pos,1.0f));
				o.uv = vert.uv;
				o.nor = normalize(vert.norm);
        o.col = vert.color;

        o.worldPos = vert.pos;
        o.eye = _WorldSpaceCameraPos - o.worldPos;

        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {
 
        
      	float3 fNorm = v.nor;//uvNormalMap(_NormalMap, v.worldPos , v.uv , v.nor , 1,3 );
        float3 col = v.nor * .5 + .5;

       	float3 fRefl = reflect( -normalize(v.eye) , fNorm );
       	float3 cubeCol = texCUBE(_CubeMap,fRefl ).rgb;

       	col =  (fNorm * .5 + .5 ) * cubeCol * 2 * v.col;

        col = cubeCol;// * float3( 1. , 0.6 , 0.4 ) * ( 10 * v.col.x * v.col.y * v.col.z);
        if( v.uv.y > .9 || v.uv.y < .1){ col = float3(1,1,1);}
        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}