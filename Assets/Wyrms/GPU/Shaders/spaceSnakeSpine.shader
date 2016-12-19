Shader "Custom/spaceSnakeSpine" {
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
      #include "Chunks/noise.cginc"



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
          float3 vel 			: TEXCOORD8;
          float  lamb 		: TEXCOORD4;
          float3 eye      : TEXCOORD5;
          float3 worldPos : TEXCOORD6;
          float3 ogPos : TEXCOORD9;
          float id : TEXCOORD7;
      };

      varyings vert (uint id : SV_VertexID){

        varyings o;

        Vert vert = vertBuffer[id];

				o.pos = mul (UNITY_MATRIX_VP, float4(vert.pos,1.0f));
				o.uv = vert.uv;
				o.nor = normalize(vert.norm);
        o.col = vert.color;
        o.vel = vert.vel;

        o.worldPos = vert.pos;
        o.eye = _WorldSpaceCameraPos - o.worldPos;
        o.id = vert.id;
        o.ogPos = vert.ogPos;

        return o;


      }

      float3 doBGCol( float3 pos , float id ){
      	pos = pos * 8;
      	float n1 = noise(  pos+ id * 100  + float3(0,_Time.y,0));
      	//float n2 = noise(  pos * 6 + float3(0,0,_Time.y*2));
      	//float n3 = noise(  pos * 10+ float3(_Time.y*2,0,0) );
//
      	//float n = (n1*2 + n2 + n3 * .4);
      //n = fNoise( pos * 2 );
      	float3 final = n1 * n1 ;
 				//float3 c = texCUBE( _CubeMap , normalize( pos));
 				//final = c;
      	//final = tex3D(_CubeMap, normalize(pos));


      	return final;
      }


      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {
			


      	float3 fNorm = v.nor;//uvNormalMap(_NormalMap, v.worldPos , v.uv , v.nor , .5 , 1.2 );

      	float ior = .3;
        float s= .96;
        float3 col;

       
       	float3 fRefl = reflect( -normalize(v.eye) , fNorm );
       	float3 cubeCol = texCUBE(_CubeMap,fRefl ).rgb;

       	float eyeMatch  = dot( fNorm , -normalize(v.eye) );
       	float n  = noise( 60  * float3( v.uv.x , v.uv.y * .3 , _Time.x + 200 * v.id ));
       	float distFromCenter = abs( v.uv.y - .5 ) * 2;
       	col =  lerp((normalize(-v.eye) * .5 + .5) ,( fRefl * .5 + .5) * eyeMatch , v.id * .8 + .4 * n ) * cubeCol;

       	col = normalize( v.vel ) * .5 + .5  ;
       	col *= cubeCol;

        //col = cubeCol;// * float3( 1. , 0.6 , 0.4 ) * ( 10 * v.col.x * v.col.y * v.col.z);
        
        //col = fRefl * .5 + .5;
        //col *= cubeCol;
        

        col *= (((distFromCenter * distFromCenter)+.6) + .4 * n );

        float3 rd = normalize(v.eye - v.worldPos);


        float3 rdR      = refract(normalize(rd),fNorm,s); 
        float3 rdG      = refract(normalize(rd),fNorm,s-ior); 
        float3 rdB      = refract(normalize(rd),fNorm,s-ior * 2); 

        float3 r = float3(1,0,0) * doBGCol( v.worldPos+ rdR * .1 , v.id );
        float3 g = float3(0,1,0) * doBGCol( v.worldPos+ rdG * .1 , v.id );
        float3 b = float3(0,0,1) * doBGCol( v.worldPos+ rdB * .1 , v.id );
         
         col *= r+g+b;//((r+g+b) + .5) / 1.5;

        //col *= lerp( float3(0,.4,1) , float3( .2,.6, ), v.id);

        //col *= .5 + sin( v.uv.x * 20 + _Time.y * 5 ) + n * .2;

        if( distFromCenter > 1 - n * .3 ){ discard; }
        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}