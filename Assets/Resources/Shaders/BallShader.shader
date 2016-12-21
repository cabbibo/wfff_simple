Shader "Custom/BallShader" {
	Properties {
    _NumberSteps( "Number Steps", Int ) = 30
    _MaxTraceDistance( "Max Trace Distance" , Float ) = 6.0
    _IntersectionPrecision( "Intersection Precision" , Float ) = 0.0001
     _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}
  }


  SubShader {

    Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
    LOD 200

    Pass {

      CULL Off
      



      CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag

      // Use shader model 3.0 target, to get nicer looking lighting
      #pragma target 3.0

      #include "UnityCG.cginc"


      uniform int _NumberSteps;
      uniform float  _IntersectionPrecision;
      uniform float _MaxTraceDistance;
      uniform float _Asleep;


      struct VertexIn{
         float4 position  : POSITION; 
         float3 normal    : NORMAL; 
         float4 texcoord  : TEXCOORD0; 
         float4 tangent   : TANGENT;
      };


      struct VertexOut {
          float4 pos      : POSITION; 
          float3 normal   : NORMAL; 
          float4 uv       : TEXCOORD0; 
          float3 ro       : TEXCOORD1;
          float3 rd       : TEXCOORD2;
          float3 camPos   : TEXCOORD3;
          float3 handL    :TEXCOORD4;
          float3 handR    :TEXCOORD5;
          float3 light    :TEXCOORD6;
      };

      

      #include "Chunks/hsv.cginc"
      #include "Chunks/sdfFunctions.cginc"
      #include "Chunks/noise.cginc"
      #include "Chunks/matrixFunctions.cginc"

     
    

      VertexOut vert(VertexIn v) {
        
        VertexOut o;

        o.normal = v.normal;
        
        o.uv = v.texcoord;

        o.camPos = _WorldSpaceCameraPos; 

        // Getting the position for actual position
        o.pos = mul( UNITY_MATRIX_MVP , v.position );
     
        float3 mPos = mul( unity_ObjectToWorld , v.position );

        o.ro = v.position;
        o.rd = normalize(mul( unity_WorldToObject , float4( _WorldSpaceCameraPos ,1)).xyz - v.position );
        o.light = _WorldSpaceLightPos0.xyz;//normalize(mul( unity_WorldToObject , float4(_WorldSpaceLightPos0.xyz ,1)).xyz );

        return o;

      }

      float fNoise( float3 pos ){

      	return float3(sin(pos.x* 3),sin(pos.y ) , sin( pos.z ));
      }




      float3 doCol( float3 ro , float3 rd ,float3 norm , float3 ior ){

      	float3 col = float3( 0 , 0 , 0 );
      	//rd = refract( rd , norm , ior );
      	for( int i = 0; i <3; i++ ){
      		float3 pos = ro + rd * float( i ) * .5;

      		float n = noise( pos * 3 + float3(0,-_Time.y* .13,0) ) * .8 + noise( pos * 10 + float3(0,-_Time.y* .1,0)) *.2 + noise( pos + float3(0,-_Time.y*.2,0) );
      			
      		//float3 dist = pos - unity_LightPosition[0];

      		col+= hsv( n * 2 , .7 ,1  );


      	}



      	col /= 3;
      	return col;
			}

      // Fragment Shader
      fixed4 frag(VertexOut i) : COLOR {

        float3 ro       = i.ro;

        float ior = .01;
        float s= .96;
        float3 rdR      = refract(-normalize(i.rd),i.normal,s); 
        //float3 rdG       = refract(-normalize(i.rd),i.normal,s-ior); 
        //float3 rdB      = refract(-normalize(i.rd),i.normal,s-ior * 2); 

       
        float3 col = doCol( ro , rdR , i.normal , s-ior*0);

       /* col += colR * float3(1,0,0);
        col += colG * float3(0,1,0);
        col += colB * float3(0,0,1);*/

        if( pow(1-abs(dot(i.normal , i.rd)),.8) > .6 ){
        	col = float3( 0,0,0);
        }

        if( _Asleep ==  1){
        	col = pow(length( col ), 5 ) * .4;
        }
	
        //col = lerp( col, float3(0,0,0), pow(1-abs(dot(i.normal , i.rd)),.8));
        //col = float3( 1 , i.uv.x , i.uv.y );
        fixed4 color;
        //col = normalize(i.light) *.5 + .5;// -i.ro);
        color = fixed4( col , 1. );
        return color;

      }

      ENDCG

    }

  }

  FallBack "Diffuse"

}