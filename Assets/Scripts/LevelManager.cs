using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Command;
using static Player;
using static WalkPath;
using System;

public class LevelManager : MonoBehaviour {

    [Header("Setting")]
    public float commandDelay = 1;
    public float cameraRotateSpeed = 5;

    [Header("Components")]
    public GameObject currentCommandProcessFrame;
    public GameObject inProcessButton;
    public GameObject reverseButton;
    public TextMeshProUGUI gameSpeedTextOn;
    public TextMeshProUGUI gameSpeedTextOff;
    public Image cooldownFrame;

    [Space(10)]
    public GameObject pauseScreen;
    public GameObject endScreen;
    public GameObject cheatCodeScreen;

    [Space(10)]
    public GameObject mainFunctionObject;

    private Vector3 targetCameraRotation;

    private bool running = false;

    private Player player;
    private Player clone;
    private Transform cameraRotator;

    private GameObject currentFunctionsObject;
    private int currentCommandIndex;
    private int gameSpeed = 1;
    private string gameSpeedFormat;
    private int currentTimeScale = 1;

    private Functions selectedFunction;

    // Cooldown on Click Play and Replay
    private float currentCooldown = 0.5f;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        try {
            clone = GameObject.FindGameObjectWithTag("Clone").GetComponent<Player>();
        } catch (NullReferenceException) {
        } catch (UnityException) { }
        
        cameraRotator = GameObject.FindGameObjectWithTag("Camera Rotator").GetComponent<Transform>();
        targetCameraRotation = cameraRotator.eulerAngles;
        gameSpeedFormat = gameSpeedTextOn.text;
        selectedFunction = mainFunctionObject.GetComponent<Functions>();

        currentFunctionsObject = mainFunctionObject;
        currentCommandIndex = 0;

        Time.timeScale = 1;
    }

    private void Update() {
        cameraRotator.rotation = Quaternion.Lerp(cameraRotator.rotation, Quaternion.Euler(targetCameraRotation), cameraRotateSpeed * Time.deltaTime);
        gameSpeedTextOn.text = gameSpeedFormat.Replace("<speed>", gameSpeed.ToString());
        gameSpeedTextOff.text = gameSpeedFormat.Replace("<speed>", gameSpeed.ToString());

        if (currentCooldown > 0) {
            cooldownFrame.enabled = true;
            cooldownFrame.fillAmount = (float) ((currentCooldown * 100) / 0.5f) / 100;
            currentCooldown -= Time.deltaTime;
        } else {
            cooldownFrame.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            cheatCodeScreen.SetActive(true);
        }
    }

    private IEnumerator Runner() {

        if (!running) {
            yield break;
        }

        Functions function = currentFunctionsObject.GetComponent<Functions>();
        Command selectedCommand = function.GetCommandList()[currentCommandIndex];

        if (selectedCommand.GetCommandType() != CommandType.NONE) {

            // No Delay at start of each function
            if (currentCommandIndex != 0) {
                if (!running) {
                    yield break;
                }

                yield return new WaitForSeconds(commandDelay);
            }

            if (!running) {
                yield break;
            }

            currentCommandProcessFrame.SetActive(true);
            currentCommandProcessFrame.transform.position = new Vector3(selectedCommand.gameObject.transform.position.x, selectedCommand.gameObject.transform.position.y, currentCommandProcessFrame.transform.position.z);
        }

        bool outOfTurn = false;

        currentCommandIndex++;

        if (currentCommandIndex >= function.GetCommandList().Count) {
            outOfTurn = true;
        }

        if (!selectedCommand.blocking) {
            RunCommand(selectedCommand);
            
            // Run in RunCommand instead (Prevent from OutOfTurn())
            if (selectedCommand.GetCommandType() == CommandType.FUNCTION) {
                yield break;
            }
        } else {
            // Check if there's command while blocking
            if (selectedCommand.GetCommandType() != CommandType.NONE) {
                RunCommand(selectedCommand);

                if (selectedCommand.GetCommandType() == CommandType.FUNCTION) {
                    yield break;
                }
            }
        }

        if (player.targetWalkPath.pathType == PathType.GOAL) {
            running = false;
        } else {
            if (outOfTurn) {

                if (!function.IsMainFunctions()) {
                    Functions previousFunction = function.GetPreviousFunction();
                    int nextIndexFromPreviousFunction = function.GetNextFunctionIndex();
                    currentFunctionsObject = previousFunction.gameObject;
                    currentCommandIndex = nextIndexFromPreviousFunction;

                    if (currentCommandIndex >= function.GetCommandList().Count) {
                        if (currentCommandIndex >= previousFunction.GetCommandList().Count) {
                            currentCommandIndex = previousFunction.GetNextFunctionIndex();
                            currentFunctionsObject = mainFunctionObject;
                        }
                    }
                    StartCoroutine(Runner());
                } else {
                    OutOfTurn();
                }
            } else {

                if (player.playerAnimator.GetBool("Die")) {
                    OutOfTurn();
                    yield break;
                }

                StartCoroutine(Runner());
            }
        }
    }

    private void RunCommand(Command command) {
        if (!running) {
            return;
        }

        if (player.playerAnimator.GetBool("Die")) {
            return;
        }

        switch (command.GetCommandType()) {
            case CommandType.NONE:
            break;

            case CommandType.FORWARD:
            player.MoveForward(false);

            if (clone != null) {
                clone.MoveForward(false);
            }
            
            break;

            case CommandType.TURN_LEFT:
            player.Rotate(Direction.LEFT);

            if (clone != null) {
                clone.Rotate(Direction.LEFT);
            }
            
            break;

            case CommandType.TURN_RIGHT:
            player.Rotate(Direction.RIGHT);

            if (clone != null) {
                clone.Rotate(Direction.RIGHT);
            }
            
            break;

            case CommandType.JUMP:
            player.MoveForward(true);

            if (clone != null) {
                clone.MoveForward(true);
            }

            break;

            case CommandType.WAIT:
            break;

            case CommandType.FUNCTION:
            Functions nextFunction = command.GetFunctions();
            nextFunction.SetNextFunctionIndex(currentCommandIndex);
            nextFunction.SetPreviousFunction(currentFunctionsObject.GetComponent<Functions>());
            currentFunctionsObject = nextFunction.gameObject;
            currentCommandIndex = 0;
            StartCoroutine(Runner());
            break;

            default:
            break;
        }

        if (command.GetCommandType() != CommandType.NONE && command.GetCommandType() != CommandType.FUNCTION) {

            bool activateChange = false;

            // Check Obstacle Block
            foreach (GameObject pathBlock in GameObject.FindGameObjectsWithTag("Path")) {
                WalkPath walkPath = pathBlock.GetComponent<WalkPath>();

                switch (walkPath.pathType) {
                    case PathType.SPIKE:
                    if (walkPath.IsActivated()) {
                        activateChange = true;
                        walkPath.Deactivate();
                    } else {
                        if (walkPath.GetCountdown() > 1) {
                            walkPath.SetCountdown(walkPath.GetCountdown() - 1);
                        } else {
                            walkPath.Activate();
                            activateChange = true;
                        }
                    }
                        
                    break;

                    default:
                    break;
                }
            }

            if (activateChange) {
                GetComponent<AudioManager>().PlaySound("Spike_Activate");
            }
        }
    }

    public void Run() {
        if (!running) {
            running = true;

            StartCoroutine(Runner());
        }
    }

    public bool IsRunning() {
        return running;
    }

    public void CompleteLevel() {
        running = false;
        endScreen.SetActive(true);
        GetComponent<AudioManager>().PlaySound("Winning");
    }

    private void OutOfTurn() {
        reverseButton.SetActive(true);
        inProcessButton.SetActive(false);
    }

    public CommandItem FindCommandItemOfType(CommandType type, Functions function) {
        foreach (GameObject commandItemObject in GameObject.FindGameObjectsWithTag("CommandItem")) {
            CommandItem commandItem = commandItemObject.GetComponent<CommandItem>();

            if (commandItem.type == type) {
                if (commandItem.function != null) {
                    if (commandItem.function == function) {
                        return commandItem;
                    }
                } else {
                    return commandItem;
                }
            }
        }

        return null;
    }

    public void IncreaseGameSpeed() {
        switch (gameSpeed) {
            case 1:
            gameSpeed = 2;
            Time.timeScale = 2;
            currentTimeScale = 2;
            break;
            case 2:
            gameSpeed = 4;
            Time.timeScale = 4;
            currentTimeScale = 4;
            break;
            case 4:
            gameSpeed = 1;
            Time.timeScale = 1;
            currentTimeScale = 1;
            break;
            default:
            break;
        }
    }

    public void ResetBack() {
        running = false;
        player.ResetBack();

        if (clone != null) {
            clone.ResetBack();
        }

        foreach (GameObject blockObject in GameObject.FindGameObjectsWithTag("Path")) {
            WalkPath walkPath = blockObject.GetComponent<WalkPath>();
            walkPath.ResetWalkPath();
        }

        currentCommandProcessFrame.SetActive(false);
        currentFunctionsObject = mainFunctionObject;
        currentCommandIndex = 0;
        currentCooldown = 0.5f;
    }

    public void PauseGame() {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void UnPauseGame() {
        Time.timeScale = currentTimeScale;
        pauseScreen.SetActive(false);
    }

    public void CameraRotate(string rotate) {
        switch (rotate) {
            case "LEFT":
            targetCameraRotation += new Vector3(0, 90, 0);

            break;

            case "RIGHT":
            if (targetCameraRotation.y == 0) {
                targetCameraRotation.y = 270;
            } else {
                targetCameraRotation += new Vector3(0, -90, 0);
            }
            
            break;

            case "VERTICAL":
            targetCameraRotation += new Vector3(0, -90, 180);

            break;

            default:
            break;
        }
    }

    public void SetSelectedFunction(Functions function) {

        foreach (GameObject selectedPanel in GameObject.FindGameObjectsWithTag("Function Panel")) {
            selectedPanel.GetComponent<Image>().enabled = false;
        }

        foreach (GameObject selectedPanel in GameObject.FindGameObjectsWithTag("Function Normal")) {
            selectedPanel.GetComponent<Image>().enabled = true;
        }

        selectedFunction = function;
    }

    public Functions GetSelectedFunction() {
        return selectedFunction;
    }
}
