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

    Menu menu;

    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.Find("Canvas").GetComponent<Menu>();
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);
        cube.transform.localScale = new Vector3(1, 1, 1);
        highlightMesh = cube.GetComponent<MeshFilter>().mesh;
        Destroy(cube);
        highlightPosition = new Vector3(0, 0, 0);

        rp = new RenderParams(highlightMaterial);

        transform = firstPersonCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        highlightPosition = checkLookingAt();
        if (Input.GetMouseButtonDown(0) && menu.isPlayerInTheGame) {
            removeBlock();
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
            return new Vector3(0, -100, 0);
        }
    }

    public void removeBlock() {
        Vector3 block = checkLookingAt();
        if(block.y >= 0) {
            WorldGenerator.removeBlock(block);
        }
    }
}
