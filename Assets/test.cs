using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int vertexCount = Random.Range(3, 6);
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[vertexCount];


        Vector3[] PossVertices = new Vector3[vertexCount];
        for (int i = 0; i < PossVertices.Length; i++)
        {
            PossVertices[i] = new Vector3(Random.Range(-100, 101), Random.Range(-100, 101));
            vertices[i] = PossVertices[i];
            uvs[i] = new Vector2((PossVertices[i].x + 100) / 200f, (PossVertices[i].y + 100) / 200f);
            triangles[i] = i;
        }
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
