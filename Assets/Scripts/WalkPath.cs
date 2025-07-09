using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class WalkPath : MonoBehaviour {

    public enum PathType {
        PATH, GOAL, BRIDGE, SPIKE, PLATE, SPIKE_TRIGGER
    }

    public enum PathColor {
        RED, BLUE
    }

    public bool enable = true;
    public bool startActivate = false;
    public PathType pathType;
    public float walkPointOffset;

    [Header("Setting")]
    public int countDown = 3;
    public PathColor pathColor;

    [Header("Components")]
    public Transform canvasTransform;
    public TextMeshProUGUI countDownText;
    public Animator animator;

    private LevelManager levelManager;

    private int currentCountDown;
    private bool activate = false;

    [Space(20)]
    public List<WalkPath> possiblePath = new List<WalkPath>();

    private void Start() {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        TryGetComponent(out animator);

        if (pathType == PathType.SPIKE_TRIGGER) {
            if (startActivate) {
                Activate();
            } else {
                Deactivate();
            }
        }

        for (int i = 0; i < possiblePath.Count; i++) {
            if (possiblePath[i] == null) {
                possiblePath.RemoveAt(i);
            }
        }

        currentCountDown = countDown;
    }

    private void Update() {
        if (canvasTransform != null) {
            canvasTransform.LookAt(canvasTransform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
        }

        if (countDownText != null) {
            countDownText.text = currentCountDown.ToString();
        }
    }

    public void ResetWalkPath() {
        switch (pathType) {
            case PathType.BRIDGE:
            enable = true;
            animator.SetBool("collapsing", false);
            break;

            case PathType.SPIKE:
            Deactivate();
            break;

            case PathType.SPIKE_TRIGGER:
            if (startActivate) {
                Activate();
            } else {
                Deactivate();
            }
            break;

            default:
            break;
        }
    }

    public Vector3 GetWalkPoint() {
        return transform.position + transform.up * walkPointOffset;
    }

    // When countdown = 0
    public void Activate() {
        switch (pathType) {
            case PathType.SPIKE:
            animator.SetBool("active", true);
            break;

            case PathType.SPIKE_TRIGGER:
            animator.SetBool("active", true);
            break;

            default:
            break;
        }

        activate = true;
    }

    // After Activate
    public void Deactivate() {
        switch (pathType) {
            case PathType.SPIKE:
            animator.SetBool("active", false);
            break;

            case PathType.SPIKE_TRIGGER:
            animator.SetBool("active", false);
            break;

            default:
            break;
        }

        activate = false;
        currentCountDown = countDown;
    }

    /*
    private void OnDrawGizmos() {

        Gizmos.color = Color.gray;

        Gizmos.DrawSphere(GetWalkPoint() + new Vector3(0, -0.5f, 0), 0.1f);

        if (possiblePath == null) {
            return;
        }

        foreach (WalkPath p in possiblePath) {
            if (p == null) {
                return;
            }

            Vector3 startLinePosition = GetWalkPoint() + new Vector3(0, -0.5f, 0);
            Vector3 nextLinePosition = p.GetComponent<WalkPath>().GetWalkPoint() + new Vector3(0, -0.5f, 0);

            Handles.DrawBezier(startLinePosition, nextLinePosition, startLinePosition, nextLinePosition, Color.black, null, 1);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(GetWalkPoint() + new Vector3(0, -0.5f, 0), 0.1f);

        if (possiblePath == null) {
            return;
        }

        foreach (WalkPath p in possiblePath) {
            if (p == null) {
                continue;
            }

            Vector3 startLinePosition = GetWalkPoint() + new Vector3(0, -0.5f, 0);
            Vector3 nextLinePosition = p.GetComponent<WalkPath>().GetWalkPoint() + new Vector3(0, -0.5f, 0);

            Handles.DrawBezier(startLinePosition, nextLinePosition, startLinePosition, nextLinePosition, Color.red, null, 10);
        }
    }
    */

    public void SetEnable(bool value) {
        enable = value;
    }

    public void SetCountdown(int value) {
        currentCountDown = value;
    }

    public int GetCountdown() {
        return currentCountDown;
    }

    public bool IsActivated() {
        return activate;
    }
}
