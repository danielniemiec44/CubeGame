using System.Collections.Generic;
using UnityEngine;
using System;

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

    int framesInterval = 0;
    int framesBehind = 0;

    public Material material;
    public Material material2;
    


    Mesh cubeMesh;
    RenderParams[] rps;
    int renderDistance = 4;

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


    static int playerChunkX;
    static int playerChunkZ;
    static int playerChunkY;

    

    





    // Start is called before the first frame update
    void Start()
    {
        

        chunksLoaded = new List<Vector2>();
        chunksQueued = new Queue<Vector2>();
        canvas = GameObject.Find("Canvas");
        menu = canvas.GetComponent<Menu>();
        
        

        player.transform.position = new Vector3(0, 100, 0);
        
        menu.hideBackground();
        
        materialConstructor = GameObject.Find("Materials").GetComponent<MaterialConstructor>();
        rps = new RenderParams[11];
        rps[0] = new RenderParams(materialConstructor.materials[0]);
        rps[1] = new RenderParams(materialConstructor.materials[1]);
        rps[2] = new RenderParams(materialConstructor.materials[2]);
        rps[3] = new RenderParams(materialConstructor.materials[3]);
        rps[4] = new RenderParams(materialConstructor.materials[4]);
        rps[5] = new RenderParams(materialConstructor.materials[5]);
        rps[6] = new RenderParams(materialConstructor.materials[6]);
        rps[7] = new RenderParams(materialConstructor.materials[7]);
        rps[8] = new RenderParams(materialConstructor.materials[8]);
        rps[9] = new RenderParams(materialConstructor.materials[9]);
        rps[10] = new RenderParams(materialConstructor.materials[10]);

        

        for(int x = -renderDistance; x < renderDistance; x++) {
            for(int z = -renderDistance; z < renderDistance; z++) {
                for(int y = -2; y < 2; y++) {
                    GenerateChunk(x, y, z);
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
        playerChunkX = (int) (player.transform.position.x / 16);
        playerChunkZ = (int) (player.transform.position.z / 16);
        playerChunkY = (int) (player.transform.position.y / 16);

        


        for(int x = playerChunkX - renderDistance; x < playerChunkX + renderDistance; x++) {
            for(int z = playerChunkZ - renderDistance; z < playerChunkZ + renderDistance; z++) {
                for(int y = playerChunkY - 2; y < playerChunkY + 2; y++) {
                    Vector3 chunk = new Vector3(x, y, z);
                    if(!chunksLoaded.Contains(chunk) && !chunksQueued.Contains(chunk)) {
                        chunksQueued.Enqueue(new Vector3(x, y, z));
                    }
            }
        }
            


        if(framesBehind > framesInterval) {
            if(chunksQueued.Count != 0) {
                Vector3 chunk = chunksQueued.Dequeue();
                GenerateChunk((int) chunk.x, (int) chunk.y, (int) chunk.z);
            }
            framesBehind = 0;
        }
        framesBehind++;


        

        int i = 0;
        foreach(MeshInstance instance in MeshInstance.meshList)
        {
            if(instance != null) {
                if((instance.chunkX < playerChunkX - renderDistance || instance.chunkX > playerChunkX + renderDistance) || (instance.chunkZ < playerChunkZ - renderDistance || instance.chunkZ > playerChunkZ + renderDistance || instance.chunkY > playerChunkY + 2 || instance.chunkY < playerChunkY - 2)) {
                    //Vector2 oldChunk = new Vector2(instance.chunkX, instance.chunkZ);
                    int a = 0;
                    foreach(Vector3 oldChunk in chunksLoaded) {
                        if(oldChunk.x == instance.chunkX && oldChunk.z == instance.chunkZ && oldChunk.y == instance.chunkY) {
                            chunksLoaded.Remove(oldChunk);
                            break;
                        }
                        a++;
                    }
                    Destroy(GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkY + "," + instance.chunkZ + ")"));
                    MeshInstance.meshList[i] = null;
                    MeshPooling.meshPrefabs[i].triangles = null;
                    break;
                }
            }
            i++;
        }






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

            foreach(MeshInstance meshInstance in MeshInstance.meshList){
                if(meshInstance != null) {
                    for(int submeshIndex = 1; submeshIndex < meshInstance.mesh.subMeshCount; submeshIndex++) {
                        Graphics.RenderMesh(rps[submeshIndex - 1], meshInstance.mesh, submeshIndex, Matrix4x4.Translate(new Vector3(meshInstance.chunkX * 16, meshInstance.chunkY * 16, meshInstance.chunkZ * 16)));
                    }
                }
            }
            
        //}
            }
    }






    void GenerateChunk(int chunkX, int chunkY, int chunkZ)
    {
        //int cubeNumber = 0;
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkX * 16, chunkY * 16, chunkZ * 16), Quaternion.identity);
        chunk.name = "Chunk(" + chunkX + "," + chunkY + "," + chunkZ + ")";
        //MeshFilter meshFilter = chunk.GetComponent<MeshFilter> ();
        //Renderer renderer = chunk.GetComponent<Renderer>();
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();

        int heightMapPosition = 0;
        int[] blockIds = new int[65536];
        
        for(int x = 0; x < 16; x++) {
            for(int z = 0; z < 16; z++) {
                int y = (int) (MeshPooling.heightMap[chunkX + 50, chunkZ + 50, heightMapPosition] * 10);
                if(y > chunkY * 16 && y < (chunkY + 1) * 16) {
                    y = (int) (y / 16);
                blockIds[calculateCubeIndex(new Vector3(x, y, z))] = 1;
                    /*
                    blockIds[calculateCubeIndex(new Vector3(x, 0, z))] = 3;
                    for(int y = 1; y < standardHeight; y++) {
                        blockIds[calculateCubeIndex(new Vector3(x, y, z))] = 4;
                    }
                    for(int y = standardHeight; y < (int) (MeshPooling.heightMap[chunkX + 50, chunkY, chunkZ + 50, heightMapPosition] * 10); y++) {
                        blockIds[calculateCubeIndex(new Vector3(x, y, z))] = 2;
                    }
                    */
                } else {
                    blockIds[calculateCubeIndex(new Vector3(x, y, z))] = 1;
                }
                heightMapPosition++;
            }
        }


        int meshCount = getFreeMesh();
        Mesh mesh = MeshPooling.meshPrefabs[meshCount];

        MeshInstance.meshList[meshCount] = new MeshInstance(mesh, chunkX, chunkY, chunkZ, blockIds);
        meshCollider.sharedMesh = MeshInstance.meshList[meshCount].mesh;

        chunksLoaded.Add(new Vector3(chunkX, chunkY, chunkZ));


        
        
    }





    

    


   

    static int[] calculateTriangles(int cubeNumber) {
        int[] triangles = new int[36];

        triangles[0] = 0 + (cubeNumber * 14);
        triangles[1] = 2 + (cubeNumber * 14);
        triangles[2] = 1 + (cubeNumber * 14);

        triangles[3] = 1 + (cubeNumber * 14);
        triangles[4] = 2 + (cubeNumber * 14);
        triangles[5] = 3 + (cubeNumber * 14);

        triangles[6] = 1 + (cubeNumber * 14);
        triangles[7] = 3 + (cubeNumber * 14);
        triangles[8] = 4 + (cubeNumber * 14);

        triangles[9] = 3 + (cubeNumber * 14);
        triangles[10] = 5 + (cubeNumber * 14);
        triangles[11] = 4 + (cubeNumber * 14);


        triangles[12] = 4 + (cubeNumber * 14);
        triangles[13] = 5 + (cubeNumber * 14);
        triangles[14] = 6 + (cubeNumber * 14);

        triangles[15] = 5 + (cubeNumber * 14);
        triangles[16] = 7 + (cubeNumber * 14);
        triangles[17] = 6 + (cubeNumber * 14);

        triangles[18] = 6 + (cubeNumber * 14);
        triangles[19] = 7 + (cubeNumber * 14);
        triangles[20] = 8 + (cubeNumber * 14);

        triangles[21] = 7 + (cubeNumber * 14);
        triangles[22] = 9 + (cubeNumber * 14);
        triangles[23] = 8 + (cubeNumber * 14);
        
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

    static Vector3 calculateCubeVector(int cubeIndex) {
        float blockX = cubeIndex % 16;
        float blockY = (int) (cubeIndex / 256);
        float blockZ = (int) ((cubeIndex - (blockY * 256)) / 16);

        return new Vector3(blockX, blockY, blockZ);
    }

    

    

    public int getFreeMesh() {
        int i = 0;
        foreach(MeshInstance instance in MeshInstance.meshList) {
            if(instance == null) {
                return i;
            }
            i++;
        }
        return i;
    }




 



}