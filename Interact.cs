using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Interact : MonoBehaviour
{
    public GameObject firstPersonCamera;
    Transform transform;
    Mesh highlightMesh;
    public Material highlightMaterial;

    Mesh mesh;
    Vector3 highlightPosition;

    RenderParams rp;

    // Start is called before the first frame update
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);
        cube.transform.localScale = new Vector3(1, 1, 1);
        highlightMesh = cube.GetComponent<MeshFilter>().mesh;
        Destroy(cube);
        highlightPosition = new Vector3(0, 0, 0);

        rp = new RenderParams(highlightMaterial);

        transform = firstPersonCamera.transform;

        //highlightMesh = new Mesh();

        //Vector3[] vertices = new Vector3[8];
        //int[] triangles = new int[36];

        /*
        vertices[0] = new Vector3 (0, 0, 0);
        vertices[1] = new Vector3 (1, 0, 0);
        vertices[2] = new Vector3 (1, 1, 0);
        vertices[3] = new Vector3 (0, 1, 0);
        vertices[4] = new Vector3 (0, 1, 1);
        vertices[5] = new Vector3 (1, 1, 1);
        vertices[6] = new Vector3 (1, 0, 1);
        vertices[7] = new Vector3 (0, 0, 1);


        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        triangles[6] = 2;
        triangles[7] = 3;
        triangles[8] = 4;

        triangles[9] = 2;
        triangles[10] = 4;
        triangles[11] = 5;

        triangles[12] = 1;
        triangles[13] = 2;
        triangles[14] = 5;

        triangles[15] = 1;
        triangles[16] = 5;
        triangles[17] = 6;

        triangles[18] = 0;
        triangles[19] = 7;
        triangles[20] = 4;

        triangles[21] = 0;
        triangles[22] = 4;
        triangles[23] = 3;
        
        triangles[24] = 5;
        triangles[25] = 4;
        triangles[26] = 7;

        triangles[27] = 5;
        triangles[28] = 7;
        triangles[29] = 6;

        triangles[30] = 0;
        triangles[31] = 6;
        triangles[32] = 7;

        triangles[33] = 0;
        triangles[34] = 1;
        triangles[35] = 6;

        highlightMesh.vertices = vertices;
        highlightMesh.triangles = triangles;
        highlightMesh.Optimize ();
        highlightMesh.RecalculateNormals ();
        */
    }

    // Update is called once per frame
    void Update()
    {
        highlightPosition = checkLookingAt();
        if (Input.GetMouseButtonDown(0)) {
            WorldGenerator.removeBlock(highlightPosition);
        }
    }

    public Vector3 checkLookingAt() {
        Graphics.RenderMesh(rp, highlightMesh, 0, Matrix4x4.Translate(highlightPosition));

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, 3.0f)) {
            Vector3 point = hit.point;
            highlightPosition = new Vector3((int) Math.Ceiling(point.x - 0.01f) - 0.5f, (int) Math.Ceiling(point.y - 0.01f) - 0.5f, (int) Math.Ceiling(point.z - 0.01f) - 0.5f);
            return highlightPosition;
        } else {
            return new Vector3(0, 0, 0);
        }
    }
}
