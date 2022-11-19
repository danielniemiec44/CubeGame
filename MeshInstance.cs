using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInstance
{
    public int chunkX;
    public int chunkZ;
    public Mesh mesh;
    Material material;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public MeshInstance(Mesh mesh, int chunkX, int chunkZ) {
        this.mesh = mesh;
        this.chunkX = chunkX;
        this.chunkZ = chunkZ;
    }
}
