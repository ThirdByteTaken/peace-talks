using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[] { new Vector3(-100, 0), new Vector3(0, 100), new Vector3(100, 0), new Vector3(0, -100) };
        Vector2[] uvs = new Vector2[] { new Vector2(0, 0.5f), new Vector2(0.5f, 1), new Vector2(1, 0.5f), new Vector2(0.5f, 0) };
        int[] triangles = new int[] { 0, 1, 2, 1, 2, 3 };



        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
