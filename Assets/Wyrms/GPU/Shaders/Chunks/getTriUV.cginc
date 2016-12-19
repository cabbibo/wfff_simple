void getTriUV( in uint index , out float2 triPos , out float2 triUV){

        uint triIndex = index % 6;

        //float2 triPos = float2( 0 , 0 );
        //float2 triUV = float2( 0 , 0 );

        if( triIndex == 0 ){
          triPos = float2( -1 , -1 );
          triUV  = float2( 0 , 0 );
        }else if( triIndex == 1 ){
          triPos = float2( 1 , 1 );
          triUV  = float2( 1 , 1 );
          
        }else if( triIndex == 2 ){
          triPos = float2( 1 , -1 );
          triUV  = float2( 1 , 0 );
          
        }else if( triIndex == 3 ){
          triPos = float2( 1 , 1 );
          triUV  = float2( 1 , 1 );
        }else if( triIndex == 4 ){
          
          triPos = float2( -1 , -1 );
          triUV  = float2( 0 , 0 );
          
        }else if( triIndex == 5 ){
          triPos = float2( -1 , 1 );
          triUV  = float2( 0 , 1 );

        }

      }