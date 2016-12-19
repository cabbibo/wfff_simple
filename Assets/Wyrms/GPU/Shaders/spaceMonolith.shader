﻿Shader "Custom/spaceMonolith" {

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
      uniform float3 _CenterPos;

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



float doRing( float3 p ){
  
    float3 pos = p;
    
     
    float lor = sign( pos.x );
    
    //pos.x = abs( pos.x );
    
    //pos.x = mod( pos.x , 1.5 );
    float degree = atan2( pos.x , pos.y );
    
    float ogD = degree;
    
   // degree += iGlobalTime;// * (1. + lor * .2);
    float l = length( pos.xz );
    //       r<0 ? r+m : r;

    float d = (degree - 3.14159  / 8.);
    float m = (3.14159  / 4.);
    degree = d % m;
    if( degree < 0 ){ degree += m; }
    //degree < 0 ? 
 
    pos.x = l * sin( degree );
    pos.z = l * cos( degree );
    //06 *( 1. + abs(ogD) )
    return sdBox( pos - float3(1 , 1 , 2.39 ) * .5  , float3( .2   , 1.2 , .2 ));
    
}


      // Map needs to come before calcIntersection
      // and calcNormal, because they use it.
      // This function holds the 'map' of the scene
      float2 map( float3 pos ){ 

        float2 res;
        float2 res2;

        float3 aPos =pos - _CenterPos;

        float n = noise( (pos + float3( 0 , _Time.x * .15 , 0 )) * normalize(_Scale)* 30);
        float n2 = noise( (pos - float3( 0 , _Time.x * .3,0 )) * normalize(_Scale)* 50);
        /*res = float2(sdBox( pos , float3( .54,.54,.54) ) , 1 );

        res.x += n * .1 + n2 * .06;

        res.x = hardS(  sdSphere( (pos - float3( 0, .24, 0) ) * normalize( _Scale) , .16 ) , res.x);
*/
				
				float3 q = aPos  * .9;// mul(mul(xrotate( _Time.y ) ,zrotate(_Time.y * .89)) ,aPos) ;
				res2 = float2( sdBox(q , float3(.3,.3,.3)) , 1);

				res = float2( sdSphere(aPos, .35), 1);
				res.x -= noise( pos * 20+ float3( 0 , _Time.y , 0 ) )* .1;

				res = hardS( res , res2 );
				res = hardU( res , float2( sdSphere(aPos, .2), 3));

				

				//res.x = hardS(  sdPlane( aPos, float4( normalize(float3( 1 , 1, 1)) , .1) ), res.x );
				//res.x = hardS(  sdPlane( aPos, float4( normalize(float3( -1 , -1, 1)) , .1) ), res.x );

				//res.x = hardS(  sdSphere( aPos -float3(0,.3,.3), .3), res.x );

				float3 dir = float3(-1,0,1);
				float3 p1 = float3(0,0,0)-dir;
				float3 p2 = float3(0,0,0)+dir;
			//	res = hardS( float2(sdCapsule( aPos , p1 , p2, .2 ),2),res );
			//	res = hardU( res , float2(sdCapsule( aPos , p1, p2, .04 ),3));

        float2 hand = float2( sdSphere( (pos - handL) ,.1 ),2);
        //res = smoothU( res , hand , .1 );
     	
     		//res = float2(10000,-10);
        
        //res = float2( sdPlane(pos , float4(0,1,0,-4)), 1 );
        //res.x -= n * .1;
       // res.x -= n * ( 1/ pos.y);

        res = float2( n + n2 - 1.6 , 1 );
        //res = float2(doRing( pos ) , 1);
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

      float fNoise( float3 pos ){

      	return float3(sin(pos.x* 3),sin(pos.y ) , sin( pos.z ));
      }

      float3 doBGCol( float3 pos ){
      	pos = pos * 10;
      	float n1 = noise( normalize(_Scale) * pos * 2 + float3(0,_Time.y,0));
      	float n2 = noise( normalize(_Scale) * pos * 6 + float3(0,0,_Time.y*2));
      	float n3 = noise( normalize(_Scale) * pos * 10+ float3(_Time.y*2,0,0) );

      	float n = (n1*2 + n2 + n3 * .4);
      //n = fNoise( pos * 2 );
      	float3 final = n * n * .2 + .1;
 				float3 c = texCUBE( _CubeMap , normalize( pos));
 				final = c;
      	//final = tex3D(_CubeMap, normalize(pos));


      	return final;
      }


      float3 doCol( float3 ro , float3 rd ,float3 norm , float3 ior ){

      	rd = refract( rd , norm , ior );

				float2 res = calcIntersection( ro , rd);

				float3 col;
				if( res.y > -0.5 ){

         
          float3 pos = ro + rd * res.x;
          float3 norm = calcNormal( pos );
          float ao = calcAO( pos , norm);

          float match = max( 0 , dot(rd , -norm));

          float3 refl = reflect( rd , norm );
          float3 cubeCol = texCUBE( _CubeMap , normalize( refl));
          cubeCol = length( cubeCol);
          float3 newR = refract( rd , norm , .9 );

          if( res.y == 1 ){
              float n = noise( pos * 30 + float3(0,-_Time.y* .13,0) ) * .8 + noise( pos * 100 + float3(0,-_Time.y* .1,0)) *.2 + noise( pos * 10 + float3(0,-_Time.y*.2,0) );
            
        
          	col =n;// texCUBE( _CubeMap , normalize( newR))+ pow(( 1 - match ),3);

          }else if( res.y == 2 ){
          	col = float3(1,0,0);//cubeCol * .1;
          }else{
						col = cubeCol * float3(.6,.3 , .1);
          }

          // float3( match , match , match );
          //col *= ao;*/


          //col = (1-match) * cubeCol * ao *ao*ao* float3( 1, .6 , .2);

  
        }else{

        	//col = doBGCol( ro + rd * .3);

        	col = texCUBE( _CubeMap , normalize( rd));
          //discard;//col = float3(0,0,0 );
        }

        return col;

      }

      // Fragment Shader
      fixed4 frag(VertexOut i) : COLOR {

        handL = i.handL;
        handR = i.handR;

        float3 ro       = i.ro;

        float ior = .003 + .01 * i.uv.y;
        float s= .96;

        float faceMatch = dot( i.rd , i.normal );
        float3 rdR      = refract(-normalize(i.rd),i.normal,s); 
        float3 rdG       = refract(-normalize(i.rd),i.normal,s-ior); 
        float3 rdB      = refract(-normalize(i.rd),i.normal,s-ior * 2); 

       
        float3 colR = doCol( ro , rdR , i.normal , s-ior*0);
        float3 colG = doCol( ro , rdG , i.normal , s-ior*1);
        float3 colB = doCol( ro , rdB , i.normal , s-ior*2);

        float3 col = float3( 0,0,0 );

        col += colR * float3(1,0,0);
        col += colG * float3(0,1,0);
        col += colB * float3(0,0,1);

        col *= 1-faceMatch;

        //col = float3( 1 , i.uv.x , i.uv.y );
        fixed4 color;
        col *= i.uv.y;
        color = fixed4( col , 1. );
        return color;

      }

      ENDCG

    }

  }

  FallBack "Diffuse"

}