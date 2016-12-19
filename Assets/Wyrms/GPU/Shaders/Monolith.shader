Shader "Custom/Monolith" {

  Properties {
    _NumberSteps( "Number Steps", Int ) = 30
    _MaxTraceDistance( "Max Trace Distance" , Float ) = 6.0
    _IntersectionPrecision( "Intersection Precision" , Float ) = 0.0001
     _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}
  }


  SubShader {

    Tags { "RenderType"="Opaque" }
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

      uniform samplerCUBE _CubeMap;

      uniform float3 _Scale;

      uniform float3 _MyHead;
      uniform float3 _MyHandL;
      uniform float3 _MyHandR;

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
      };

      

      #include "Chunks/hsv.cginc"
      #include "Chunks/sdfFunctions.cginc"
      #include "Chunks/noise.cginc"
      #include "Chunks/matrixFunctions.cginc"

      float4x4 mm;

      float getSmoothDif( float2 r1 , float2 r2 , float k ){
          float hard = hardU( r1.x , r2.x );
          float smooth = smin( r1.x , r2.x , k );
          return smooth-hard;
      }

      float3 handL;
      float3 handR;


      // Map needs to come before calcIntersection
      // and calcNormal, because they use it.
      // This function holds the 'map' of the scene
      float2 map( float3 pos ){ 

        float2 res;
        float2 res2;

        float n = noise( (pos + float3( 0 , _Time.x * .15 , 0 )) * normalize(_Scale)* 10);
        float n2 = noise( (pos - float3( 0 , _Time.x * .3,0 )) * normalize(_Scale)* 20);
        res = float2(sdBox( pos , float3( .54,.54,.54) ) , 1 );

        res.x += n * .1 + n2 * .06;

        res.x = hardS(  sdSphere( (pos - float3( 0, .24, 0) ) * normalize( _Scale) , .16 ) , res.x);

        /*float2 hand = float2( sdSphere( (pos - handL) * normalize( _Scale ) ,.03 ),2);
        float2 resFlat = hardU( res , hand );
        float2 resSmooth = smoothU( res , hand , .15);
        float2 resSub = smoothS( hand , res , 16 );
        res = hardU( resSub , hand );
        float dif = resSmooth.x - resFlat.x;

        hand = float2( sdSphere( (pos - handR) * normalize( _Scale )  ,.03 ),2);
        resFlat = hardU( res , hand );
        resSmooth = smoothU( res , hand , .15);
        resSub = smoothS( hand , res , 12 );
        res = hardU( resSub , hand );
        dif = resSmooth.x - resFlat.x;
        res.x -= 3*(n2 * .5+ n) * dif;*/
      
        
        //res = ring;
        return res;

      }

      #include "Chunks/calcIntersection.cginc"
      #include "Chunks/calcNormal.cginc"
      #include "Chunks/calcAO.cginc"
      #include "Chunks/tex3D.cginc"
    

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
        
        o.handL = mul( unity_WorldToObject , float4( _MyHandL ,1)).xyz;
        o.handR = mul( unity_WorldToObject , float4( _MyHandR ,1)).xyz;
        return o;

      }

      // Fragment Shader
      fixed4 frag(VertexOut i) : COLOR {

        handL = i.handL;
        handR = i.handR;

        float3 ro       = i.ro;
        float3 rd       = -i.rd; 

       
        float2 res = calcIntersection( ro , rd );

        float3 col = float3( 1,1,1 );

        if( res.y > -0.5 ){

         
          float3 pos = ro + rd * res.x;
          float3 norm = calcNormal( pos );
          float ao = calcAO( pos , norm);

          float match = max( 0 , dot(rd , -norm));

          float3 refl = reflect( rd , norm );
          float3 cubeCol = texCUBE( _CubeMap , normalize( refl));

          col = norm * .5 + .5;
          col *= ao;


          col =(1-match) * cubeCol * ao *ao*ao* float3( 1, .6 , .2);

  
        }else{
          discard;//col = float3(0,0,0 );
        }


       /* float3 fogCol = float3(1,1,1)*shakeVal;
        col = lerp( fogCol , col , clamp( _Value1 * 15 , 0 , 1 ) );
        col = lerp( col, fogCol , clamp( (_Value1-.98)* 50 , 0 , 1 ) );*/

       //col = fogCol;
        fixed4 color;
        color = fixed4( col , 1. );
        return color;

      }

      ENDCG

    }

  }

  FallBack "Diffuse"

}