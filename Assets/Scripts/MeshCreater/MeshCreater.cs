using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshCreater : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private Mesh mesh;

    [SerializeField] private Transform[] vertices;

    private Vector3[] verticesPositions;

    private void Awake()
    {
        AssignComponents();

        verticesPositions = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            verticesPositions[i] = vertices[i].position;
        }

        RenderMesh();
    }

    public void AssignComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

    private void RenderMesh()
    {
        mesh.Clear();

        mesh.vertices = verticesPositions;

        //mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        //mesh.uv = meshData.uv.ToArray();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

    }

}
