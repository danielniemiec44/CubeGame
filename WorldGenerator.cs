using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum Heading
{
    North,
    East,
    South,
    West
}

public class WorldGenerator : MonoBehaviour
{
    public Texture texture;
    public GameObject chunkPrefab;


    //List<Vector3> verticesList;
    //List<int> trianglesList;
    //int[,,] cubes = new int[16,16,16];
    int cubesCountTotal = 0;


    public GameObject player;
    List<Vector2> chunksLoaded;
    Queue<Vector2> chunksQueued;

    GameObject canvas;

    Menu menu;

    int framesInterval = 60;
    int framesBehind = 0;

    public Material material;
    public Material material2;
    
    static MeshInstance[] meshList;
    int meshCount = 0;

    Mesh cubeMesh;
    RenderParams rp;
    int renderDistance = 2;

    //Vector3[] cubeVertices;
    //int[] cubeTriangles;
    //int cubeVerticesCount;
    //int cubeTrianglesCount;


    [Header("Debug")]
    [SerializeField] [Range(0f, 360f)] private float northHeading;

    [Header("OutputValues")]
    [SerializeField] private float myHeading;
    [SerializeField] private float dif;
    [SerializeField] private Heading heading;

    public MaterialConstructor materialConstructor;



    Vector3[] vertices = new Vector3[917504];
    Vector2[] uv = new Vector2[917504];

    





    // Start is called before the first frame update
    void Start()
    {
        
        meshList = new MeshInstance[6000];


        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);
        cube.transform.localScale = new Vector3(1, 1, 1);
        cubeMesh = cube.GetComponent<MeshFilter>().mesh;
        cubeMesh.Optimize();
        cubeMesh.RecalculateNormals();
        Vector3[] cubeVertices = cubeMesh.vertices;
        int[] cubeTriangles = cubeMesh.triangles;
        Vector3[] cubeNormals = cubeMesh.normals;
        Vector2[] cubeUv = cubeMesh.uv;
        //cubeVerticesCount = cubeVertices.Length;
        //cubeTrianglesCount = cubeTriangles.Length;
        
        //Debug.Log(String.Join(", ", cubeVertices));
        //Debug.Log(String.Join(", ", cubeTriangles));
        //Debug.Log(String.Join(", ", cubeNormals));
        //Debug.Log(String.Join(", ", cubeUv));

        //cube.GetComponent<Renderer>().material = material;
        Destroy(cube);

        chunksLoaded = new List<Vector2>();
        chunksQueued = new Queue<Vector2>();
        canvas = GameObject.Find("Canvas");
        menu = canvas.GetComponent<Menu>();
        
        

        player.transform.position = new Vector3(0, 20, 0);
        
        menu.hideBackground();
        
        materialConstructor = GameObject.Find("Materials").GetComponent<MaterialConstructor>();
        rp = new RenderParams(materialConstructor.materials[0]);


        int cubeNumber = 0;
        for(int y = 0; y < 256; y++) {
            for(int z = 0; z < 16; z++) {
                for(int x = 0; x < 16; x++) {
                    vertices[(cubeNumber * 14)] = new Vector3(x + 0, y + 1, z + 0);
                    vertices[(cubeNumber * 14) + 1] = new Vector3(x + 0, y + 0, z + 0);
                    vertices[(cubeNumber * 14) + 2] = new Vector3(x + 1, y + 1, z + 0);
                    vertices[(cubeNumber * 14) + 3] = new Vector3(x + 1, y + 0, z + 0);
                    vertices[(cubeNumber * 14) + 4] = new Vector3(x + 0, y + 0, z + 1);
                    vertices[(cubeNumber * 14) + 5] = new Vector3(x + 1, y + 0, z + 1);
                    vertices[(cubeNumber * 14) + 6] = new Vector3(x + 0, y + 1, z + 1);
                    vertices[(cubeNumber * 14) + 7] = new Vector3(x + 1, y + 1, z + 1);

                    vertices[(cubeNumber * 14) + 8] = new Vector3(x + 0, y + 1, z + 0);
                    vertices[(cubeNumber * 14) + 9] = new Vector3(x + 1, y + 1, z + 0);
                    vertices[(cubeNumber * 14) + 10] = new Vector3(x + 0, y + 1, z + 0);
                    vertices[(cubeNumber * 14) + 11] = new Vector3(x + 0, y + 1, z + 1);
                    vertices[(cubeNumber * 14) + 12] = new Vector3(x + 1, y + 1, z + 0);
                    vertices[(cubeNumber * 14) + 13] = new Vector3(x + 1, y + 1, z + 1);


                    uv[(cubeNumber * 14)] = new Vector2(x + 0, z + 0.66f); //0
                    uv[(cubeNumber * 14) + 1] = new Vector2(x + 0.25f, z + 0.66f); //1
                    uv[(cubeNumber * 14) + 2] = new Vector2(x + 0, z + 0.33f); //2
                    uv[(cubeNumber * 14) + 3] = new Vector2(x + 0.25f, z + 0.33f); //3
                    uv[(cubeNumber * 14) + 4] = new Vector2(x + 0.5f, z + 0.66f); //4
                    uv[(cubeNumber * 14) + 5] = new Vector2(x + 0.5f, z + 0.33f); //5
                    uv[(cubeNumber * 14) + 6] = new Vector2(x + 0.75f, z + 0.66f); //6
                    uv[(cubeNumber * 14) + 7] = new Vector2(x + 0.75f, z + 0.33f); //7
                    uv[(cubeNumber * 14) + 8] = new Vector2(x + 1, z + 0.66f); //8
                    uv[(cubeNumber * 14) + 9] = new Vector2(x + 1, z + 0.33f); //9
                    uv[(cubeNumber * 14) + 10] = new Vector2(x + 0.25f, z + 1); //10
                    uv[(cubeNumber * 14) + 11] = new Vector2(x + 0.5f, z + 1); //11
                    uv[(cubeNumber * 14) + 12] = new Vector2(x + 0.25f, z + 0); //12
                    uv[(cubeNumber * 14) + 13] = new Vector2(x + 0.5f, z + 0); //13
                    cubeNumber++;
                }
            }
        }


        for(int x = -2; x < 2; x++) {
            for(int z = -2; z < 2; z++) {
                GenerateChunk(x, z);
            }
        }




        
    }












    // Update is called once per frame
    void Update()
    {
        int playerChunkX = (int) (player.transform.position.x / 16);
        int playerChunkZ = (int) (player.transform.position.z / 16);

        
        for(int x = playerChunkX - renderDistance; x < playerChunkX + renderDistance; x++) {
            for(int z = playerChunkZ - renderDistance; z < playerChunkZ + renderDistance; z++) {
                Vector2 chunk = new Vector2(x, z);
                if(!chunksLoaded.Contains(chunk) && !chunksQueued.Contains(chunk)) {
                    chunksQueued.Enqueue(new Vector2(x, z));
                }
            }
        }
            


        if(framesBehind > framesInterval) {
            if(chunksQueued.Count != 0) {
                Vector2 chunk = chunksQueued.Dequeue();
                GenerateChunk((int) chunk.x, (int) chunk.y);
            }
            framesBehind = 0;
        }
        framesBehind++;








        // only use the Y component of the objects orientation
        // always returns a value between 0 and 360
        myHeading = player.transform.eulerAngles.y;
        // also this is always a value between 0 and 360
        northHeading = Input.compass.magneticHeading;

        dif = myHeading - northHeading;
        // wrap the value so it is always between 0 and 360
        if (dif < 0) dif += 360f;

        if (dif > 45 && dif <= 135)
        {
            heading = Heading.East;
        }
        else if (dif > 135 && dif <= 225)
        {
            heading = Heading.South;
        }
        else if (dif > 225 && dif <= 315)
        {
            heading = Heading.West;
        }
        else
        {
            heading = Heading.North;
        }










        
        
        //foreach(MeshInstance meshInstance in meshList) {
            for(int i = 0; i < meshCount; i++){
                if(heading == Heading.East) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkX + 1 >= playerChunkX && meshInstance.chunkX < playerChunkX + renderDistance) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, 0, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                    }
                } else if(heading == Heading.West) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkX - 1 <= playerChunkX && meshInstance.chunkX < playerChunkX + renderDistance) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, 0, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                    }
                } else if(heading == Heading.North) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkZ + 1 >= playerChunkZ && meshInstance.chunkZ < playerChunkZ + renderDistance) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, 0, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                    }
                } else if(heading == Heading.South) {
                MeshInstance meshInstance = meshList[i];
                    if(meshInstance.chunkZ - 1 <= playerChunkZ && meshInstance.chunkZ < playerChunkZ + renderDistance) {
                        //Graphics.DrawMesh(meshInstance.mesh, new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16), Quaternion.identity, material, meshIndex);
                            Graphics.RenderMesh(rp, meshInstance.mesh, 0, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, 0, meshInstance.chunkZ * 16)));
                    }
                }
            }
            
        //}
            }






    void GenerateChunk(int chunkX, int chunkZ)
    {

        //int cubeNumber = 0;
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkX * 16, 0, chunkZ * 16), Quaternion.identity);
        chunk.name = "Chunk(" + chunkX + "," + chunkZ + ")";
        //MeshFilter meshFilter = chunk.GetComponent<MeshFilter> ();
        //Renderer renderer = chunk.GetComponent<Renderer>();
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();

        

        Vector2[] noiseMap = new Vector2[600000];
        int i = 0;
        for(int x = 0; x < 16; x++) {
            for(int z = 0; z < 16; z++) {
                noiseMap[i] = new Vector2(x + (chunkX * 16), (chunkZ * 16) + z);
                i++;
            }
        }

        float[] heightMap = CalculateHeights(noiseMap);


        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        int[] meshTriangles = new int[600000];
        int trianglesCount = 0;
        int heightMapPosition = 0;
        for(int x = 0; x < 16; x++) {
            for(int z = 0; z < 16; z++) {
                Array.Copy(calculateTriangles(calculateCubeIndex(new Vector3(x, (int) (heightMap[heightMapPosition] * 10), z))), 0, meshTriangles, trianglesCount, 36);
                trianglesCount += 36;
                heightMapPosition++;
            }
        }
        //Array.Copy(calculateTriangles(0), 0, meshTriangles, 0, 36);
        //Array.Copy(calculateTriangles(calculateCubeIndex(new Vector3(0, 0, 10))), 0, meshTriangles, 36, 36);
        Array.Resize(ref meshTriangles, trianglesCount);
        mesh.triangles = meshTriangles;
        mesh.normals = vertices;
        mesh.uv = uv;


        //mesh.Optimize ();
		mesh.RecalculateNormals ();

        meshCollider.sharedMesh = mesh;

        meshList[meshCount] = new MeshInstance(mesh, chunkX, chunkZ);
        meshCount++;

        chunksLoaded.Add(new Vector2(chunkX, chunkZ));
    }





    float[] CalculateHeights(Vector2[] map) {
        return NoiseS3D.NoiseArrayGPU(map, 0.01f, true);
    }


    public static void removeBlock(Vector3 block) {
        //int cubeIndex = calculateCubeIndex(block);
        //int[] newTriangles = Array.FindAll(array, i => i != item).ToArray();
        Vector2 chunkVector = getChunk(block);
        Vector3 blockVector = getBlock(block);
        int cubeIndex = calculateCubeIndex(blockVector);
        foreach(MeshInstance instance in meshList) {
            if(instance.chunkX == chunkVector.x && instance.chunkZ == chunkVector.y) {
                int[] newTriangles = instance.mesh.triangles;
                for(int i = 0; i < 14; i++) {
                    newTriangles = newTriangles.Where(e => (e != (cubeIndex * 14) + i)).ToArray();
                }
                instance.mesh.triangles = newTriangles;
                GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")").GetComponent<MeshCollider>().sharedMesh = instance.mesh;
                break;
            }
        }

    }

    public static int[] calculateTriangles(int cubeNumber) {
        int[] triangles = new int[36];

        triangles[0] = 0 + (cubeNumber * 14);
        triangles[1] = 1 + (cubeNumber * 14);
        triangles[2] = 2 + (cubeNumber * 14);

        triangles[3] = 2 + (cubeNumber * 14);
        triangles[4] = 1 + (cubeNumber * 14);
        triangles[5] = 3 + (cubeNumber * 14);

        triangles[6] = 1 + (cubeNumber * 14);
        triangles[7] = 4 + (cubeNumber * 14);
        triangles[8] = 3 + (cubeNumber * 14);

        triangles[9] = 3 + (cubeNumber * 14);
        triangles[10] = 4 + (cubeNumber * 14);
        triangles[11] = 5 + (cubeNumber * 14);


        triangles[12] = 4 + (cubeNumber * 14);
        triangles[13] = 6 + (cubeNumber * 14);
        triangles[14] = 5 + (cubeNumber * 14);

        triangles[15] = 5 + (cubeNumber * 14);
        triangles[16] = 6 + (cubeNumber * 14);
        triangles[17] = 7 + (cubeNumber * 14);

        triangles[18] = 6 + (cubeNumber * 14);
        triangles[19] = 7 + (cubeNumber * 14);
        triangles[20] = 8 + (cubeNumber * 14);

        triangles[21] = 8 + (cubeNumber * 14);
        triangles[22] = 7 + (cubeNumber * 14);
        triangles[23] = 9 + (cubeNumber * 14);
        
        triangles[24] = 1 + (cubeNumber * 14);
        triangles[25] = 11 + (cubeNumber * 14);
        triangles[26] = 10 + (cubeNumber * 14);

        triangles[27] = 1 + (cubeNumber * 14);
        triangles[28] = 4 + (cubeNumber * 14);
        triangles[29] = 11 + (cubeNumber * 14);

        triangles[30] = 3 + (cubeNumber * 14);
        triangles[31] = 12 + (cubeNumber * 14);
        triangles[32] = 5 + (cubeNumber * 14);

        triangles[33] = 5 + (cubeNumber * 14);
        triangles[34] = 12 + (cubeNumber * 14);
        triangles[35] = 13 + (cubeNumber * 14);

        return triangles;
    }


    public static int calculateCubeIndex(Vector3 vector) {
        int x = (int) vector.x;
        int y = (int) vector.y;
        int z = (int) vector.z;

        return x + (z * 16) + (y * 256);
    }

    public static Vector2 getChunk(Vector3 vector) {
        int chunkX = (int) ((vector.x) / 16);
        int chunkZ = (int) ((vector.z) / 16);

        if(vector.x < 0) {
            chunkX -= 1;
        }

        if(vector.z < 0) {
            chunkZ -= 1;
        }

        return new Vector2(chunkX, chunkZ);
    }

    public static Vector3 getBlock(Vector3 vector) {
        int x = (int) ((vector.x) % 16);
        int y = (int) ((vector.y) % 16);
        int z = (int) ((vector.z) % 16);

        if(((vector.x) % 16) < 0) {
            x += 15;
        }

        if(((vector.z) % 16) < 0) {
            z += 15;
        }

        return new Vector3(x, y, z);
    }



}