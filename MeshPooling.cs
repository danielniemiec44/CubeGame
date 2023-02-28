using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;

public class MeshPooling : MonoBehaviour
{
    public static float meshCount = 150.0f;
    public static Mesh[] meshPrefabs = new Mesh[(int) meshCount];
    Vector3[] vertices = new Vector3[(int) Math.Pow(WorldGenerator.chunkSize, 2) * 1400];
    Vector2[] uv = new Vector2[(int) Math.Pow(WorldGenerator.chunkSize, 2) * 1400];
    public RectTransform loadingBarTransform;
    public GameObject loadingScreen;

    

    public static int progress = 0;
    
    



    // Start is called before the first frame update
    IEnumerator Start()
    {
        MeshInstance.meshUpdater = GetComponent<MeshUpdater>();

        Application.targetFrameRate = 9999;
        int cubeNumber = 0;
        for(int y = 0; y < 100; y++) {
            for(int z = 0; z < WorldGenerator.chunkSize; z++) {
                for(int x = 0; x < WorldGenerator.chunkSize; x++) {
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

        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.normals = vertices;
        mesh.uv = uv;
        mesh.normals = vertices;
        

        for(int i = 0; i < meshCount; i++) {
            meshPrefabs[i] = Instantiate(mesh);
            Debug.Log(WorldGenerator.chunkSize);
            loadingBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (i / meshCount) * 1520);
            yield return new WaitForSeconds(0.01f);
        }
        loadingScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    

}
