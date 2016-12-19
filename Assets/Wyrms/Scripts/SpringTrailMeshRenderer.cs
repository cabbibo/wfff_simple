﻿using UnityEngine;
using System.Collections;

public class SpringTrailMeshRenderer : MonoBehaviour {


  public Mesh mesh;

  public int numTransforms;
  public int totalTailSize;
 
  public int size;
  public float scaleDown;
  public float gooeyness;

  public GameObject hand1;
  public GameObject hand2;

  
  public Cubemap  cubeMap;
  public Texture2D  normalMap;

  private int tailStride;

  private int pNumTransforms;

  public ComputeShader physics;
  public Shader shader;
  public Transform OGTransform;

  private Matrix4x4[] trans;
  private Matrix4x4[] finalTransforms;

  private ComputeBuffer _vertBuffer;
  private ComputeBuffer _vertebraeBuffer;
  private ComputeBuffer _fullVertBuffer;

  private Material material;




  private int _kernel;
  
  struct Vertebrae{
    public float id;
    public Matrix4x4 localToWorld; 
  };

  private int VertebraeStructSize = 16+1;

  struct Vert{

    public Vector3 pos;
    public Vector3 norm;
    public Vector3 tan;
    public Vector3 color;
    public Vector2 uv;
    public float id;
    
  };

  struct SpringVert{

    public Vector3 pos;
    public Vector3 ogPos;
    public Vector3 vel;
    public Vector3 norm;
    public Vector3 tan;
    public Vector3 color;
    public Vector2 uv;
    public float id;
    
  };


  private int VertStructSize = 3+3+3+3+2+1;
  private int SpringVertStructSize = 3+3+3+3+3+3+2+1;

  private int[]     triangles;
  private Vector4[] tangents;
  private Vector3[] normals;
  private Vector2[] uvs;
  private Vector3[] positions;
  private Color[]   colors;

  private float[] vertebraeValues;
  

  // Use this for initialization
  void Awake() {

    material = new Material( shader );

    pNumTransforms = numTransforms;
    _kernel = physics.FindKernel("CSMain");

    tailStride =totalTailSize / pNumTransforms;
//    print( tailStride);

    trans = new Matrix4x4[ totalTailSize ];
    finalTransforms = new Matrix4x4[ pNumTransforms ];

    for( int i = 0; i < pNumTransforms; i++){
      finalTransforms[i] = new Matrix4x4();//transform;
      finalTransforms[i].SetColumn(0, transform.right);
      finalTransforms[i].SetColumn(1, transform.up);
      finalTransforms[i].SetColumn(2, transform.forward);
      Vector3 p = transform.position;
      finalTransforms[i].SetColumn(3, new Vector4(p.x, p.y, p.z, 1));
    }

    print( pNumTransforms );

    for( int i = 0; i < totalTailSize; i++){
      trans[i] = new Matrix4x4();//transform;
      trans[i].SetColumn(0, transform.right);
      trans[i].SetColumn(1, transform.up);
      trans[i].SetColumn(2, transform.forward);
      Vector3 p = transform.position;
      trans[i].SetColumn(3, new Vector4(p.x, p.y, p.z, 1));
    }





//    mesh = GetComponent<MeshFilter>().mesh;

    triangles = mesh.triangles; 
    positions = mesh.vertices; 
    normals   = mesh.normals; 
    tangents  = mesh.tangents; 
    colors    = mesh.colors; 
    uvs       = mesh.uv; 


    createVertBuffer();
    createSpringVertBuffer();
    createVertebraeBuffer();

    print( triangles.Length * pNumTransforms );
    Camera.onPostRender += Render;

    Set();
  
  }

  void Render(Camera c){

    if( material != null ){

    material.SetPass(0);
    material.SetInt( "_NumberTransforms" , pNumTransforms  );
    material.SetInt( "_NumberTriangles" , triangles.Length );

    material.SetTexture( "_NormalMap" , normalMap );
    material.SetTexture("_CubeMap" , cubeMap );  

    material.SetBuffer ("vertBuffer" , _fullVertBuffer);

    //print( pNumTransforms);

    Graphics.DrawProcedural(MeshTopology.Triangles, pNumTransforms * triangles.Length );
  }

  }


    //Remember to release buffers and destroy the material when play has been stopped.
  void OnDisable(){
    
    if( _vertBuffer != null ){ _vertBuffer.Release(); }
    if( _vertebraeBuffer != null ){_vertebraeBuffer.Release(); }
    //DestroyImmediate( material );
  }

  Vector3 ToV3(Vector4 parent)
  {
     return new Vector3(parent.x, parent.y, parent.z);
  }
  

  void createVertBuffer(){

    _vertBuffer = new ComputeBuffer( triangles.Length , VertStructSize * sizeof(float) );    

    float[] inValues = new float[ VertStructSize * triangles.Length ]; 

          // Used for assigning to our buffer;
    int index = 0;


    for (int i = 0; i < triangles.Length; i++) {

      int id           = triangles[i];

      Vector3 position = OGTransform.TransformPoint(positions[ id ]); 
      Vector3 normal   = OGTransform.TransformDirection(normals[ id ]); 
      Vector4 tangent  = OGTransform.TransformDirection(tangents[ id ]); 
      Vector3 color    = ToV3( Color.white );

      if( colors.Length > 0 ){ color = ToV3(colors[id]); }   

      Vector2 uv       = uvs[ id ]; 

      inValues[index++] = position.x;
      inValues[index++] = position.y;
      inValues[index++] = position.z;
      inValues[index++] = normal.x;
      inValues[index++] = normal.y;
      inValues[index++] = normal.z;
      inValues[index++] = tangent.x;
      inValues[index++] = tangent.y;
      inValues[index++] = tangent.z;
      inValues[index++] = color.x;
      inValues[index++] = color.y;
      inValues[index++] = color.z;
      inValues[index++] = uv.x;
      inValues[index++] = uv.y;

      inValues[index++] = i;

      //inValues[index++] = (float)j % 3;
   

    }

    _vertBuffer.SetData(inValues);

      
  }


  void createSpringVertBuffer(){

    _fullVertBuffer = new ComputeBuffer( triangles.Length * pNumTransforms , SpringVertStructSize * sizeof(float) );    

    /*float[] inValues = new float[ VertStructSize * triangles.Length* pNumTransforms ]; 
    _fullVertBuffer.SetData(inValues);*/

      
  }

  void createVertebraeBuffer(){

    _vertebraeBuffer = new ComputeBuffer( pNumTransforms , VertebraeStructSize * sizeof(float) );    

    vertebraeValues = new float[ VertebraeStructSize * pNumTransforms ]; 

          // Used for assigning to our buffer;
    int index = 0;


    for (int i = 0; i < pNumTransforms; i++) {

      vertebraeValues[index++] = i;
      for(int j = 0; j <16; j++){
        vertebraeValues[index++] = 0;
      }

    }

    _vertebraeBuffer.SetData(vertebraeValues);

      
  }

  void updateVertabrae(){

    for( int i = totalTailSize-1; i > 0; i-- ){
      trans[i] = trans[i-1];
    }

    trans[0] = transform.localToWorldMatrix;

    for( int i = 0; i < pNumTransforms; i++){
      finalTransforms[i] = trans[i * tailStride];
    }


    for (int i = 0; i < pNumTransforms; i++) {

      int baseID = i * 17;
      //vertebraeValues[baseID + j+1] = trans[i][x,y];

      for( int j = 0; j < 16; j++ ){
        int x = j % 4;
        int y = (int) Mathf.Floor(j / 4);
        vertebraeValues[baseID + j+1] = finalTransforms[i][x,y];
      }

    }
    _vertebraeBuffer.SetData(vertebraeValues);


  }


  void computePhysics( int set ){

    //int size = 6;
    //print( pNumTransforms );
    physics.SetInt( "_Set" , set );

    physics.SetBuffer( _kernel , "ogVertBuffer"     , _vertBuffer );
    physics.SetBuffer( _kernel , "vertBuffer"     , _fullVertBuffer );
    physics.SetBuffer( _kernel , "vertebraeBuffer"     , _vertebraeBuffer );

    physics.SetInt( "_StrideX" , size );
    physics.SetInt( "_StrideY" , size );
    physics.SetInt( "_StrideZ" , size );
    
    physics.SetVector("_Hand1",hand1.transform.position);
    physics.SetVector("_Hand2",hand2.transform.position);

    physics.SetFloat( "_ScaleDown" , scaleDown);
    physics.SetFloat("_Gooeyness", gooeyness);
    physics.SetFloat("_Time", Time.time);

    physics.SetInt( "_NumberTriangles" , triangles.Length);
    physics.SetInt( "_NumberTransforms" , pNumTransforms);
  
    physics.Dispatch( _kernel, size , size , size );

  }

  void Set(){
    computePhysics(1);
  }

  // Update is called once per frame
  void FixedUpdate () {

    updateVertabrae();
    computePhysics( 0 );




  }


}
