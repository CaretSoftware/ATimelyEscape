using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshCreater : MonoBehaviour
{
    [SerializeField] private bool removeVertexObjects;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private Mesh mesh;

    //[SerializeField] private Transform[] vertices;

    private Vector3[] vertices;
    //private Vector2[] uvs;

    private void Awake()
    {
        AssignComponents();

        vertices = new Vector3[transform.childCount];
        //uvs = new Vector2[vertices.Length];
        SetVertices();
        //SetUV();

        RenderMesh();
        if (removeVertexObjects)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void SetVertices()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            vertices[i] = transform.GetChild(i).localPosition;
        }
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
        meshCollider.sharedMesh = null;
        mesh.vertices = vertices;
        //mesh.uv = uvs;
        mesh.SetTriangles(AddTriangles(), 0);
        

        mesh.RecalculateNormals();
        
        meshCollider.sharedMesh = mesh;

    }

    //private void SetUV()
    //{
    //    for (int i = 0; i < uvs.Length; i++)
    //    {
    //        uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
    //    }
    //}

    private int[] AddTriangles()
    {
        int[] triangels = new int[vertices.Length * 3];
        for (int i = 0; i < vertices.Length; i++)
        {
            triangels[i + (2 * i)] = i;
            triangels[(i + 1) + (2 * i)] = (i + 2) % vertices.Length;
            triangels[(i + 2) + (2 * i)] = (i + 2) % vertices.Length;
        }
        return triangels;
    }

}
