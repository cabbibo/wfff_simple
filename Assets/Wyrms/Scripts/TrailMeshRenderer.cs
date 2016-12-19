using UnityEngine;
using System.Collections;

public class TrailMeshRenderer : MonoBehaviour {


  public int numTransforms;
  public int totalTailSize;
  private int tailStride;
  public Material material;
  public Transform OGTransform;

  private Matrix4x4[] transforms;
  private Matrix4x4[] finalTransforms;

  public ComputeBuffer _vertBuffer;
  public ComputeBuffer _vertebraeBuffer;

  public Mesh mesh;

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

  private int VertStructSize = 3+3+3+3+2+1;

  private int[]     triangles;
  private Vector4[] tangents;
  private Vector3[] normals;
  private Vector2[] uvs;
  private Vector3[] positions;
  private Color[]   colors;

  private float[] vertebraeValues;
	

  // Use this for initialization
	void Start() {

    tailStride =totalTailSize / numTransforms;
    print( tailStride);

    transforms = new Matrix4x4[ totalTailSize ];
    finalTransforms = new Matrix4x4[ numTransforms ];

    for( int i = 0; i < numTransforms; i++){
      finalTransforms[i] = new Matrix4x4();//transform;
      finalTransforms[i].SetColumn(0, transform.right);
      finalTransforms[i].SetColumn(1, transform.up);
      finalTransforms[i].SetColumn(2, transform.forward);
      Vector3 p = Random.insideUnitSphere;
      finalTransforms[i].SetColumn(3, new Vector4(p.x, p.y, p.z, 1));
    }

    for( int i = 0; i < totalTailSize; i++){
      transforms[i] = new Matrix4x4();//transform;
      transforms[i].SetColumn(0, transform.right);
      transforms[i].SetColumn(1, transform.up);
      transforms[i].SetColumn(2, transform.forward);
      Vector3 p = Random.insideUnitSphere;
      transforms[i].SetColumn(3, new Vector4(p.x, p.y, p.z, 1));
    }



//    mesh = GetComponent<MeshFilter>().mesh;

    triangles = mesh.triangles; 
    positions = mesh.vertices; 
    normals   = mesh.normals; 
    tangents  = mesh.tangents; 
    colors    = mesh.colors; 
    uvs       = mesh.uv; 


    createVertBuffer();
    createVertebraeBuffer();

    Camera.onPostRender += Render;
	
	}

  void Render(Camera c){

    material.SetPass(0);
    material.SetInt( "_NumberTransforms" , numTransforms  );
    material.SetInt( "_NumberTriangles" , triangles.Length );
    material.SetBuffer( "vertBuffer" , _vertBuffer );
    material.SetBuffer( "vertebraeBuffer" , _vertebraeBuffer );

    Graphics.DrawProcedural(MeshTopology.Triangles, numTransforms * triangles.Length );

  }


    //Remember to release buffers and destroy the material when play has been stopped.
  void OnDisable(){
    if( _vertBuffer != null ){ _vertBuffer.Release(); }
    if( _vertebraeBuffer != null ){_vertebraeBuffer.Release(); }
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

  void createVertebraeBuffer(){

    _vertebraeBuffer = new ComputeBuffer( numTransforms , VertebraeStructSize * sizeof(float) );    

    vertebraeValues = new float[ VertebraeStructSize * numTransforms ]; 

          // Used for assigning to our buffer;
    int index = 0;


    for (int i = 0; i < numTransforms; i++) {

      vertebraeValues[index++] = i;
      for(int j = 0; j <16; j++){
        vertebraeValues[index++] = 0;
      }

    }

    _vertebraeBuffer.SetData(vertebraeValues);

      
  }

	// Update is called once per frame
	void FixedUpdate () {

    

    for( int i = totalTailSize-1; i > 0; i-- ){
      transforms[i] = transforms[i-1];
    }

    transforms[0] = transform.localToWorldMatrix;

    for( int i = 0; i < numTransforms; i++){
      finalTransforms[i] = transforms[i * tailStride];
    }


    for (int i = 0; i < numTransforms; i++) {

      int baseID = i * 17;
      //vertebraeValues[baseID + j+1] = transforms[i][x,y];

      for( int j = 0; j < 16; j++ ){
        int x = j % 4;
        int y = (int) Mathf.Floor(j / 4);
        vertebraeValues[baseID + j+1] = finalTransforms[i][x,y];
      }

    }
    _vertebraeBuffer.SetData(vertebraeValues);

	}
}
