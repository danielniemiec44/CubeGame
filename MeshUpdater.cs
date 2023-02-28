using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMeshAsync(MeshInstance instance) {
        StartCoroutine(updateMesh(instance));
    }


    public IEnumerator updateMesh(MeshInstance instance) {
        instance.mesh.subMeshCount = 11;
        instance.trianglesLenth = new int[11];
        instance.triangles = new int[11, 2359296];
        
        for(int blockId = 0; blockId < 65536; blockId++) {
            if(instance.blockIds[blockId] != 0) {
                if((blockId - 1 > 0 && blockId + 1 < 65536 && blockId - 16 > 0 && blockId + 16 < 65536 && blockId - 256 > 0 && blockId + 256 < 65536)) {
                    if(
                        !((instance.blockIds[blockId - 1] != 0 && instance.blockIds[blockId + 1] != 0 &&
                        instance.blockIds[blockId - 16] != 0 && instance.blockIds[blockId + 16] != 0 &&
                        instance.blockIds[blockId - 256] != 0 && instance.blockIds[blockId + 256] != 0))
                    ) {
                        instance.updateTrianglesArray(blockId);
                    }
                } else {
                    
                }
                
                
            }
        }
        
        foreach(int conn in instance.blockIdConnected) {
            instance.updateTrianglesArray(conn);
        }

        for(int i = 0; i < 10; i++) {
            int[] resizedTriangles = new int[instance.trianglesLenth[i]];
            for(int a = 0; a < instance.trianglesLenth[i]; a++) {
                resizedTriangles[a] = instance.triangles[i, a];
            }
            instance.mesh.SetTriangles(resizedTriangles, i + 1);
            Debug.Log("Triangles set!");
        }
        //GameObject.Find("Chunk(" + instance.chunkX + "," + instance.chunkZ + ")").GetComponent<MeshCollider>().sharedMesh = instance.mesh;
        
        instance.chunkObject.GetComponent<MeshCollider>().sharedMesh = instance.mesh;
        
        yield return null;
    }
}
