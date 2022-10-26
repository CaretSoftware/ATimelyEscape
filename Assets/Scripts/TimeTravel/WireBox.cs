using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireBox : MonoBehaviour {
    private LineRenderer lRenderer;

    private Vector3[] linePositions = new[] {
        new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f)
    };

    public float BoxSize { get; set; }

    public Color Color {
        get => lRenderer.material.color;
        set {
            if (lRenderer != null) lRenderer.material.color = value;
        }
    }

    public void SetLinePositions() {
        if (lRenderer == null) lRenderer = GetComponent<LineRenderer>();
        lRenderer.positionCount = 16;
        lRenderer.SetPositions(linePositions);
        lRenderer.endWidth = 0.1f;
        lRenderer.startWidth = 0.1f;
        lRenderer.useWorldSpace = false;
    }
}