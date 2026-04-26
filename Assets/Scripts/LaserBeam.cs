using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private Material laserMaterial;

    [Header("LineRenderer Widths")]
    [SerializeField] private float outerGlowWidth = 0.3f;
    [SerializeField] private float midGlowWidth = 0.15f;
    [SerializeField] private float coreWidth = 0.04f;
    [SerializeField] private float pulseMultipier = 0.1f;
    [SerializeField] private float pulseFrequency = 10f;

    [Header("LineRenderer Colors")]
    [SerializeField] private Color outerGlowColor = new Color(1f, 0f, 0f, 0.15f);
    [SerializeField] private Color midGlowColor = new Color(1f, 0.3f, 0f, 0.4f);
    [SerializeField] private Color coreColor = new Color(1f, 1f, 1f, 1f);


    private LineRenderer core;
    private LineRenderer midGlow;
    private LineRenderer outerGlow;
    private EdgeCollider2D edgeCollider;


    private void Awake()
    {
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.isTrigger = true;

        // Create layers — order matters (outer first, core last)
        outerGlow = CreateLineRenderer(outerGlowWidth, outerGlowColor);
        midGlow = CreateLineRenderer(midGlowWidth, midGlowColor);
        core = CreateLineRenderer(coreWidth, coreColor);

        Deactivate();
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        // Add a pulse effect to the laser beam
        float pulse = 1f + Mathf.Sin(Time.time * pulseFrequency) * pulseMultipier;
        core.startWidth = coreWidth * pulse;
        core.endWidth = coreWidth * pulse;
    }

    public void Activate(Vector3 start, Vector3 end)
    {
        gameObject.SetActive(true);

        SetBeamPositions(core, start, end);
        SetBeamPositions(midGlow, start, end);
        SetBeamPositions(outerGlow, start, end);

        var localStart = (Vector2)transform.InverseTransformPoint(start);
        var localEnd = (Vector2)transform.InverseTransformPoint(end);

        // Update collider to match the beam
        edgeCollider.SetPoints(new List<Vector2>
        {
            localStart, localEnd
        });
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private LineRenderer CreateLineRenderer(float width, Color color)
    {
        // Each layer needs its own child GameObject
        GameObject go = new GameObject("LaserLayer");
        go.transform.SetParent(transform);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = laserMaterial;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.startColor = color;
        lr.endColor = color;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.sortingLayerName = "Default";
        lr.sortingOrder = -5;

        return lr;
    }

    private void SetBeamPositions(LineRenderer lr, Vector3 start, Vector3 end)
    {
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}