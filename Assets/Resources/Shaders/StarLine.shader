Shader "Custom/StarLine" {

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
      #include "Chunks/noise.cginc"
      #include "Chunks/hsv.cginc"

    	uniform float3 _Ball;
    	uniform float3 _Scale;

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
          float3 ballDir  : TEXCOORD7;
      };

      varyings vert (uint id : SV_VertexID){

        varyings o;

        float fID = float(id);

        float pID = floor(fID / 2);
        float tID = float(id % 2);

        float3 offset = float3( 0 , 0, 0);

        float3 basePos = float3( (hash( pID ) -.5) * 2 ,hash( pID * 20 )  , (hash( pID * 30 )-.5) * 2);
        basePos = normalize( basePos ) * hash( pID * 10);//pow(hash( pID * 10 ),1.2);
        basePos *= _Scale;

        o.ballDir = basePos - _Ball;
        

       // if( o.ballDir.y > 10 ){ basePos.y += 20; }
        //o.ballDir = basePos - _Ball;

        float d = length( o.ballDir);
        float2 uv = float2( 0, 0);
        if( tID == 0 ){ offset = float3( 0,  0, 0);}
        if( tID == 1 ){
        	if( d < .3 * length(_Ball-_WorldSpaceCameraPos.xyz)){
        		uv = float2( 1 , 0);
        		offset = o.ballDir;
        	}
        }


        float3 viewBPos =  mul( UNITY_MATRIX_MV , float4(basePos- offset ,1.0f));

    

        float3 fPos = viewBPos + offset;
        o.worldPos = basePos;
        o.uv = uv;

				o.pos = mul ( UNITY_MATRIX_P ,  float4(viewBPos ,1.0f));

        return o;


      }

      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {

      	float3 fNorm = normalize(v.ballDir);
        float3 col = hsv( v.uv.x * 2 + _Time.y * .2, .5,1);//fNorm * .5 + .5;//float3(1,1,1);

        col *= abs(v.worldPos.y - _Scale.y)/_Scale.y;
        col *= abs(abs(v.worldPos.x) - _Scale.x * .5)/(_Scale.x* .5);
        col *= abs(abs(v.worldPos.z) - _Scale.z * .5)/(_Scale.z* .5);
        //col *= abs(basePos.y - _Scale.y)/_Scale.y;
       // col *= v.uv.x;

        if( hash( v.uv.x * .01) * 2 > v.uv.x  ){
        	discard;
        }
       	//col =  (fRefl * .5 + .5 ) * cubeCol * 2 * v.col;
        
        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}