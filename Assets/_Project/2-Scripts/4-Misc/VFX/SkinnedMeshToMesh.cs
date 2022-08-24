using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public MeshFilter meshFilter;
    public VisualEffect VFXGraph;
    public float refreshRate;
    
    // Start is called before the first frame update
    // void Start()
    // {
    //     StartCoroutine(UpdateVFXGraph());
    // }
    //
    // private IEnumerator UpdateVFXGraph()
    // {
    //     while (gameObject.activeSelf)
    //     {
    //         // Mesh m = new Mesh();
    //         // skinnedMesh.BakeMesh(m);
    //         Vector3[] vertices = meshFilter.mesh.vertices;
    //         Mesh m = new Mesh();
    //         m.vertices = vertices;
    //         VFXGraph.SetMesh("Mesh", m);
    //         
    //         yield return new WaitForSeconds(refreshRate);
    //     }
    // }
}
