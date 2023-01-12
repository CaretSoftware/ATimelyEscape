using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author Emil Wessman
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class WireBox : MonoBehaviour {
    private LineRenderer lRenderer;
    private float scale = 1f;

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

    public void SetLinePositions(float scale = 1f) {
        if (lRenderer == null) lRenderer = GetComponent<LineRenderer>();

        if (scale != this.scale) {
            linePositions = new[] {
                new Vector3(-0.5f * scale, -0.5f * scale, -0.5f * scale),
                new Vector3(-0.5f * scale, 0.5f * scale, -0.5f * scale),
                new Vector3(-0.5f * scale, 0.5f * scale, 0.5f * scale),
                new Vector3(-0.5f * scale, -0.5f * scale, 0.5f * scale),
                new Vector3(0.5f * scale, -0.5f * scale, 0.5f * scale),
                new Vector3(0.5f * scale, 0.5f * scale, 0.5f * scale),
                new Vector3(0.5f * scale, 0.5f * scale, -0.5f * scale),
                new Vector3(0.5f * scale, -0.5f * scale, -0.5f * scale),
                new Vector3(-0.5f * scale, -0.5f * scale, -0.5f * scale),
                new Vector3(-0.5f * scale, -0.5f * scale, 0.5f * scale),
                new Vector3(-0.5f * scale, 0.5f * scale, 0.5f * scale),
                new Vector3(0.5f * scale, 0.5f * scale, 0.5f * scale),
                new Vector3(0.5f * scale, -0.5f * scale, 0.5f * scale),
                new Vector3(0.5f * scale, -0.5f * scale, -0.5f * scale),
                new Vector3(0.5f * scale, 0.5f * scale, -0.5f * scale),
                new Vector3(-0.5f * scale, 0.5f * scale, -0.5f * scale)
            };
            this.scale = scale;
        }

        lRenderer.positionCount = 16;
        lRenderer.SetPositions(linePositions);
        lRenderer.endWidth = 0.1f;
        lRenderer.startWidth = 0.1f;
        lRenderer.useWorldSpace = false;
    }
}