using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OffsetsLineRenderersManager : MonoBehaviour
{
    public static OffsetsLineRenderersManager Singleton { get; private set; }
    [SerializeField] private Material _lineRendererMaterial;
    [SerializeField] private float _scale = 10;
    private List<LineRenderer> _lineRenderers;
    private List<GameObject> _lineRendererGameObjects;

    void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            throw new Exception($"Detected more than one instance of {nameof(OffsetsLineRenderersManager)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        _lineRenderers = new List<LineRenderer>();
        _lineRendererGameObjects = new List<GameObject>();
    }

    void Update()
    {
        
    }

    public void AddRenderer(Vector3 from, Vector3 to) {
        GameObject _addedLineRendererGameObject = new GameObject();
        LineRenderer _addedLineRenderer = _addedLineRendererGameObject.AddComponent<LineRenderer>();
        _addedLineRenderer.startWidth = 0.1f;
        _addedLineRenderer.endWidth = 0.1f;
        _addedLineRenderer.useWorldSpace = true;
        _addedLineRenderer.material = _lineRendererMaterial;
        _addedLineRenderer.positionCount = 2;
        _addedLineRenderer.SetPosition(0, from * _scale);
        _addedLineRenderer.SetPosition(1, to * _scale);
        _addedLineRendererGameObject.transform.parent = transform;
        _lineRenderers.Add(_addedLineRenderer);
        _lineRendererGameObjects.Add(_addedLineRendererGameObject);
    }

    public void RemoveAllLineRenderers() {
        int numberOfLineRemoved = 0;
        foreach(LineRenderer lineRenderer in _lineRenderers)
        {
            GameObject tempGameObject = lineRenderer.gameObject;
            if (lineRenderer != null )
            {
                Destroy(lineRenderer);
                Destroy(tempGameObject);
                numberOfLineRemoved++;
            }
        }
        _lineRenderers.Clear();
        NetworkDebugConsole.Singleton.SetDebugString($"{numberOfLineRemoved} lines removed.");
    }
}
