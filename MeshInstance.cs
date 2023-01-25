using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class MeshInstance
{
    public int chunkX;
    public int chunkY;
    public int chunkZ;
    public Mesh mesh;
    int[] blockIds = new int[65536];
    public static MeshInstance[] meshList = new MeshInstance[600];
    List<int> blockIdConnected = new List<int>();
    int[] trianglesLenth = new int[11];
    int[,] triangles = new int[11, 2359296];
    



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public MeshInstance(Mesh mesh, int chunkX, int chunkY, int chunkZ, int[] blockIds) {
        this.mesh = mesh;
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.chunkZ = chunkZ;
        this.blockIds = blockIds;

        updateMesh();
    }




     public static int[] calculateTriangles(int cubeNumber) {
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

    public static void removeBlock(Vector3 block) {
        Vector2 chunkVector = getChunk(block);
        Vector3 blockVector = getBlock(block);
        int cubeIndex = WorldGenerator.calculateCubeIndex(blockVector);
        MeshInstance instance = findMeshInstance(chunkVector);
        int blockId = instance.blockIds[cubeIndex];
        if(blockId != 0) {
            instance.blockIds[cubeIndex] = 0;
            /*
            int[] newTriangles = instance.mesh.GetTriangles(blockId);
            for(int i = 0; i < 14; i++) {
                newTriangles = newTriangles.Where(e => (e != (cubeIndex * 14) + i)).ToArray();
            }
            instance.mesh.SetTriangles(newTriangles, blockId);
            */
            instance.updateMesh();
            instance.updateConnections(block);
            }
    }


     public static void setBlock(Vector3 block) {
        Vector2 chunkVector = MeshInstance.getChunk(block);
        Vector3 blockVector = MeshInstance.getBlock(block);
        int cubeIndex = WorldGenerator.calculateCubeIndex(blockVector);
        MeshInstance instance = findMeshInstance(chunkVector);
        int HotBarFocus = Menu.HotBarFocus + 1;
        instance.blockIds[cubeIndex] = HotBarFocus;
        int[] oldTriangles = instance.mesh.GetTriangles(HotBarFocus);
        int[] newTriangles = new int[oldTriangles.Length + 36];
        Array.Copy(oldTriangles, newTriangles, oldTriangles.Length);
        Array.Copy(calculateTriangles(cubeIndex), 0, newTriangles, oldTriangles.Length, 36);
        instance.mesh.SetTriangles(newTriangles, HotBarFocus);
        //instance.mesh.RecalculateNormals();
        instance.updateMesh();
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
        int z = (int) ((vector.z) % 16);

        if(((vector.x) % 16) < 0) {
            x += 15;
        }

        if(((vector.z) % 16) < 0) {
            z += 15;
        }

        return new Vector3(x, vector.y, z);
    }


        public static MeshInstance findMeshInstance(Vector2 chunkVector) {
        foreach(MeshInstance instance in MeshInstance.meshList) {
            if(instance != null) {
                if(instance.chunkX == chunkVector.x && instance.chunkZ == chunkVector.y) {
                    return instance;
                }
            }
        }
        return null;
    }


    public void updateMesh() {
        mesh.subMeshCount = 11;
        trianglesLenth = new int[11];
        triangles = new int[11, 2359296];
        
        for(int blockId = 0; blockId < 65536; blockId++) {
            if(blockIds[blockId] != 0) {
                if((blockId - 1 > 0 && blockId + 1 < 65536 && blockId - 16 > 0 && blockId + 16 < 65536 && blockId - 256 > 0 && blockId + 256 < 65536)) {
                    if(
                        !((blockIds[blockId - 1] != 0 && blockIds[blockId + 1] != 0 &&
                        blockIds[blockId - 16] != 0 && blockIds[blockId + 16] != 0 &&
                        blockIds[blockId - 256] != 0 && blockIds[blockId + 256] != 0))
                    ) {
                        updateTrianglesArray(blockId);
                    }
                } else {
                    
                }
                
                
            }
        }
        
        foreach(int conn in blockIdConnected) {
            updateTrianglesArray(conn);
        }

        for(int i = 0; i < 10; i++) {
            int[] resizedTriangles = new int[trianglesLenth[i]];
            for(int a = 0; a < trianglesLenth[i]; a++) {
                resizedTriangles[a] = triangles[i, a];
            }
            mesh.SetTriangles(resizedTriangles, i + 1);
        }
        GameObject.Find("Chunk(" + chunkX + "," + chunkY + "," + chunkZ + ")").GetComponent<MeshCollider>().sharedMesh = mesh;
    }


    public void updateConnections(Vector3 block) {
        Vector3 blockVector = getBlock(block);
        Vector2 chunkVector = getChunk(block);
        MeshInstance meshConn = null;
        MeshInstance meshConn2 = null;
        Vector3 newBlockVector = blockVector;
        Vector3 newBlockVector2 = blockVector;
        
        if(blockVector.x == 15) {
            meshConn = findMeshInstance(new Vector2(chunkVector.x + 1, chunkVector.y));
            newBlockVector = new Vector3(0, blockVector.y, blockVector.z);
        } else if(blockVector.x == 0) {
            meshConn = findMeshInstance(new Vector2(chunkVector.x - 1, chunkVector.y));
            newBlockVector = new Vector3(15, blockVector.y, blockVector.z);
        }
        if(blockVector.z == 15) {
            meshConn2 = findMeshInstance(new Vector2(chunkVector.x, chunkVector.y + 1));
            newBlockVector2 = new Vector3(blockVector.x, blockVector.y, 0);
        } else if(blockVector.z == 0) {
            meshConn2 = findMeshInstance(new Vector2(chunkVector.x, chunkVector.y - 1));
            newBlockVector2 = new Vector3(blockVector.x, blockVector.y, 15);
        }
        if(meshConn != null) {
            Debug.Log(chunkVector + " " + newBlockVector);
            meshConn.blockIdConnected.Add(WorldGenerator.calculateCubeIndex(newBlockVector));
            meshConn.updateMesh();
        }
        if(meshConn2 != null) {
            Debug.Log(chunkVector + " " + newBlockVector2);
            meshConn2.blockIdConnected.Add(WorldGenerator.calculateCubeIndex(newBlockVector2));
            meshConn2.updateMesh();
        }
    }

    public void updateTrianglesArray(int blockId) {
        int[] cubeTriangles = calculateTriangles(blockId);
        if(blockIds[blockId] != 0) {
            for(int i = 0; i < 36; i++) {
                triangles[blockIds[blockId] - 1, trianglesLenth[blockIds[blockId] - 1] + i] = cubeTriangles[i];
            }
            trianglesLenth[blockIds[blockId] - 1] += 36;
        }
    }
}
