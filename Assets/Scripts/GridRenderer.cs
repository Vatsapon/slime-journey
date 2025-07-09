using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour {

    public int gridSize = 5;
    public bool startFromCenter = false;
    public Color color = Color.black;
    public float width = 0.01f;
    public Material material;

    public GameObject lineObjectPrefab;
    private List<LineRenderer> lineListX = new List<LineRenderer>();
    private List<LineRenderer> lineListZ = new List<LineRenderer>();

    private void Start() {

        // X
        for (int i = -gridSize; i < gridSize + 1; i++) {
            GameObject lineObject = Instantiate(lineObjectPrefab, transform.position, transform.rotation);
            lineObject.transform.SetParent(transform);
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

            lineListX.Add(lineRenderer);
        }

        // Z
        for (int i = -gridSize; i < gridSize + 1; i++) {
            GameObject lineObject = Instantiate(lineObjectPrefab, transform.position, transform.rotation);
            lineObject.transform.SetParent(transform);
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

            lineListZ.Add(lineRenderer);
        }
    }

    private void Update() {

        float offSet;

        if (startFromCenter) {
            offSet = 0.5f;
        } else {
            offSet = 0;
        }

        int x = -gridSize;

        foreach (LineRenderer lineRenderer in lineListX) {

            lineRenderer.enabled = true;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.numCapVertices = 3;
            lineRenderer.material = material;

            Vector3 firstPosition = new Vector3(transform.position.x + x + offSet, transform.position.y, transform.position.z + offSet + gridSize);
            Vector3 lastPosition = new Vector3(transform.position.x + x + offSet, transform.position.y, transform.position.z + offSet - gridSize);

            lineRenderer.SetPosition(0, firstPosition);
            lineRenderer.SetPosition(1, lastPosition);

            x++;
        }

        int z = -gridSize;

        foreach (LineRenderer lineRenderer in lineListZ) {

            lineRenderer.enabled = true;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.numCapVertices = 3;
            lineRenderer.material = material;

            Vector3 firstPosition = new Vector3(transform.position.x + offSet + gridSize, transform.position.y, transform.position.z + z + offSet);
            Vector3 lastPosition = new Vector3(transform.position.x + offSet - gridSize, transform.position.y, transform.position.z + z + offSet);

            lineRenderer.SetPosition(0, firstPosition);
            lineRenderer.SetPosition(1, lastPosition);

            z++;
        }
    }
}
