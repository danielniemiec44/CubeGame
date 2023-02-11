using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class createCubePrefab : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[14];
        vertices[0] = new Vector3(0, 1, 0);
        vertices[1] = new Vector3(0, 0, 0);
        vertices[2] = new Vector3(1, 1, 0);
        vertices[3] = new Vector3(1, 0, 0);
        vertices[4] = new Vector3(0, 0, 1);
        vertices[5] = new Vector3(1, 0, 1);
        vertices[6] = new Vector3(0, 1, 1);
        vertices[7] = new Vector3(1, 1, 1);

        vertices[8] = new Vector3(0, 1, 0);
        vertices[9] = new Vector3(1, 1, 0);
        vertices[10] = new Vector3(0, 1, 0);
        vertices[11] = new Vector3(0, 1, 1);
        vertices[12] = new Vector3(1, 1, 0);
        vertices[13] = new Vector3(1, 1, 1);


        int[] triangles = new int[36];

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        triangles[6] = 1;
        triangles[7] = 3;
        triangles[8] = 4;

        triangles[9] = 3;
        triangles[10] = 5;
        triangles[11] = 4;


        triangles[12] = 4;
        triangles[13] = 5;
        triangles[14] = 6;

        triangles[15] = 5;
        triangles[16] = 7;
        triangles[17] = 6;

        triangles[18] = 6;
        triangles[19] = 7;
        triangles[20] = 8;

        triangles[21] = 7;
        triangles[22] = 9;
        triangles[23] = 8;
        
        triangles[24] = 1;
        triangles[25] = 11;
        triangles[26] = 10;

        triangles[27] = 1;
        triangles[28] = 4;
        triangles[29] = 11;

        triangles[30] = 3;
        triangles[31] = 12;
        triangles[32] = 5;

        triangles[33] = 5;
        triangles[34] = 12;
        triangles[35] = 13;


        Vector2[] uv = new Vector2[14];
        uv[0] = new Vector2(0, 0.66f); //0
        uv[1] = new Vector2(0.25f, 0.66f); //1
        uv[2] = new Vector2(0, 0.33f); //2
        uv[3] = new Vector2(0.25f, 0.33f); //3
        uv[4] = new Vector2(0.5f, 0.66f); //4
        uv[5] = new Vector2(0.5f, 0.33f); //5
        uv[6] = new Vector2(0.75f, 0.66f); //6
        uv[7] = new Vector2(0.75f, 0.33f); //7
        uv[8] = new Vector2(1, 0.66f); //8
        uv[9] = new Vector2(1, 0.33f); //9
        uv[10] = new Vector2(0.25f, 1); //10
        uv[11] = new Vector2(0.5f, 1); //11
        uv[12] = new Vector2(0.25f, 0); //12
        uv[13] = new Vector2(0.5f, 0); //13

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = vertices;
        mesh.uv = uv;
        mesh.RecalculateNormals(); 

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        string localPath = "Assets/Prefabs/" + gameObject.name + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        bool prefabSuccess;
        PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out prefabSuccess);
        if (prefabSuccess == true)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);
        }

    // Update is called once per frame
    void Update()
    {
        
    }
}
