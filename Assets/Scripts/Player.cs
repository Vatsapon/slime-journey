using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static WalkPath;

public class Player : MonoBehaviour {

    public enum SlimeType {
        PLAYER, CLONE
    }

    public enum Direction {
        UP, DOWN, LEFT, RIGHT
    }

    [Header("Setting")]
    public SlimeType type;
    public Direction startDirection = Direction.UP;
    public float moveSpeed = 1;
    public float rotateSpeed = 1;
    public WalkPath targetWalkPath;

    [Header("Component")]
    public Animator playerAnimator;
    public Animator faceAnimator;
    public GameObject deathEffect;

    private Direction currentDirection = Direction.UP;
    private Quaternion targetRotation;

    private bool moveable = false;

    private WalkPath startWalkPath;
    private Quaternion startRotation;

    private LevelManager levelManager;

    private void Start() {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        targetRotation = transform.rotation;
        startWalkPath = targetWalkPath;
        startRotation = targetRotation;
        currentDirection = startDirection;
    }

    private void Update() {

        Vector3 targetPosition = targetWalkPath.GetWalkPoint();

        if (moveable) {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    public void MoveForward(bool jump) {
        
        if (playerAnimator.GetBool("Die")) {
            return;
        }

        transform.position = targetWalkPath.GetWalkPoint();
        WalkPath walkPath = GetTargetWalkDirection(currentDirection);

        if (walkPath != null) {

            // Different Height
            if (!jump && walkPath.transform.position.y > targetWalkPath.transform.position.y) {
                return;
            }

            if (targetWalkPath.pathType == PathType.BRIDGE) {
                targetWalkPath.enable = false;
                targetWalkPath.GetComponent<Animator>().SetBool("collapsing", true);
            }

            moveable = false;
            targetWalkPath = walkPath;
            playerAnimator.Play("Jump");
        }
    }

    public void BeforeReachBlock() {
        if (targetWalkPath.pathType == PathType.PLATE) {
            foreach (GameObject walkPathList in GameObject.FindGameObjectsWithTag("Path")) {
                WalkPath selectedWalkPath = walkPathList.GetComponent<WalkPath>();

                if (selectedWalkPath.pathType == PathType.SPIKE_TRIGGER && targetWalkPath.pathColor == selectedWalkPath.pathColor) {

                    levelManager.GetComponent<AudioManager>().PlaySound("Plate_Activate");
                    levelManager.GetComponent<AudioManager>().PlaySound("Spike_Activate");

                    if (selectedWalkPath.IsActivated()) {
                        selectedWalkPath.Deactivate();
                    } else {
                        selectedWalkPath.Activate();
                    }
                }
            }
        }
    }

    public void ReachBlock() {

        switch (targetWalkPath.pathType) {
            case PathType.SPIKE:
            if (targetWalkPath.IsActivated()) {
                Die();
            }
            break;

            case PathType.SPIKE_TRIGGER:
            if (targetWalkPath.IsActivated()) {
                Die();
            }
            break;

            case PathType.GOAL:
            if (type == SlimeType.PLAYER) {
                levelManager.CompleteLevel();
            }
            break;

            default:
            break;
        }
    }

    public void Rotate(Direction direction) {

        if (playerAnimator.GetBool("Die")) {
            return;
        }

        transform.rotation = targetRotation;

        switch (direction) {
            case Direction.LEFT:

            switch (currentDirection) {
                case Direction.UP:
                currentDirection = Direction.LEFT;
                break;

                case Direction.DOWN:
                currentDirection = Direction.RIGHT;
                break;

                case Direction.LEFT:
                currentDirection = Direction.DOWN;
                break;

                case Direction.RIGHT:
                currentDirection = Direction.UP;
                break;

                default:
                break;
            }

            moveable = false;
            Transform currentTransform = transform;
            currentTransform.Rotate(new Vector3(0, -90, 0));
            targetRotation = currentTransform.rotation;
            currentTransform.Rotate(new Vector3(0, 90, 0));
            playerAnimator.Play("Jump");

            break;

            case Direction.RIGHT:

            switch (currentDirection) {
                case Direction.UP:
                currentDirection = Direction.RIGHT;
                break;

                case Direction.DOWN:
                currentDirection = Direction.LEFT;
                break;

                case Direction.LEFT:
                currentDirection = Direction.UP;
                break;

                case Direction.RIGHT:
                currentDirection = Direction.DOWN;
                break;

                default:
                break;
            }

            moveable = false;
            Transform currentTransform2 = transform;
            currentTransform2.Rotate(new Vector3(0, 90, 0));
            targetRotation = currentTransform2.rotation;
            currentTransform2.Rotate(new Vector3(0, -90, 0));
            playerAnimator.Play("Jump");
            break;

            default:
            break;
        }
    }

    private WalkPath GetTargetWalkDirection(Direction direction) {

        foreach (WalkPath walkPath in targetWalkPath.possiblePath) {

            if (walkPath == null) {
                continue;
            }

            if (!walkPath.enable) {
                continue;
            }

            if (walkPath.transform.position.x > targetWalkPath.transform.position.x && direction == Direction.UP) {
                return walkPath;
            }

            if (walkPath.transform.position.x < targetWalkPath.transform.position.x && direction == Direction.DOWN) {
                return walkPath;
            }

            if (walkPath.transform.position.z > targetWalkPath.transform.position.z && direction == Direction.LEFT) {
                return walkPath;
            }

            if (walkPath.transform.position.z < targetWalkPath.transform.position.z && direction == Direction.RIGHT) {
                return walkPath;
            }
        }

        return null;
    }

    public void Die() {

        if (playerAnimator.GetBool("Die")) {
            return;
        }

        playerAnimator.SetBool("Die", true);
        playerAnimator.Play("Die");
    }

    public void PlayDeathEffect() {
        Instantiate(deathEffect, transform.position, deathEffect.transform.rotation);
    }

    public void SetMoveable(int num) {

        if (num > 0) {
            moveable = true;
        } else {
            moveable = false;
        }
        
    }

    public bool IsMoveable() {
        return moveable;
    }

    public void ResetBack() {
        playerAnimator.StopPlayback();
        playerAnimator.SetBool("Die", false);
        playerAnimator.Play("Start");

        targetWalkPath = startWalkPath;
        targetRotation = startRotation;
        currentDirection = startDirection;

        transform.position = startWalkPath.GetWalkPoint();
        transform.rotation = startRotation;
    }
}
