using UnityEngine;
using System.Collections.Generic;
using System;

public class MeshInstance
{
    public int chunkX;
    public int chunkZ;
    public Mesh mesh;
    public int[] blockIds = new int[65536];
    public static MeshInstance[] meshList = new MeshInstance[600];



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public MeshInstance(Mesh mesh, int chunkX, int chunkZ, int[] blockIds) {
        this.mesh = mesh;
        this.chunkX = chunkX;
        this.chunkZ = chunkZ;
        this.blockIds = blockIds;

        mesh.subMeshCount = 11;
        int[,] triangles = new int[11, 2359296];
        int[] trianglesLenth = new int[11];
        for(int blockId = 0; blockId < 65536; blockId++) {
            if(blockIds[blockId] != 0) {
                int[] cubeTriangles = calculateTriangles(blockId);
                for(int i = 0; i < 36; i++) {
                    triangles[blockIds[blockId] - 1, trianglesLenth[blockIds[blockId] - 1] + i] = cubeTriangles[i];
                }
                trianglesLenth[blockIds[blockId] - 1] += 36;
            }
        }
        for(int i = 0; i < 10; i++) {
            int[] resizedTriangles = new int[trianglesLenth[i]];
            for(int a = 0; a < trianglesLenth[i]; a++) {
                resizedTriangles[a] = triangles[i, a];
            }
            mesh.SetTriangles(resizedTriangles, i + 1);
        }
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
            int[] newTriangles = instance.mesh.GetTriangles(blockId);
            for(int i = 0; i < 14; i++) {
                newTriangles = Array.FindAll(newTriangles, i => (i != (cubeIndex * 14) + i));
            }
            instance.mesh.SetTriangles(newTriangles, blockId);
            GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")").GetComponent<MeshCollider>().sharedMesh = instance.mesh;
        }
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
            if(instance.chunkX == chunkVector.x && instance.chunkZ == chunkVector.y) {
                return instance;
            }
        }
        return null;
    }

}
