using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Command : MonoBehaviour {

    public enum CommandType {
        NONE, LOCK, FORWARD, TURN_LEFT, TURN_RIGHT, JUMP, WAIT, FUNCTION
    }

    public CommandType type = CommandType.NONE;
    public Functions function;
    public bool destroying = false;
    public bool blocking = false;

    public Image blockedImage;
    
    private LevelManager levelManager;

    public GameObject currentItemObject;
    public Functions parentFunction;

    private void Start() {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    private void Update() {

        if (destroying) {

            CommandItem commandItem = levelManager.FindCommandItemOfType(type, function);

            float distance = Vector3.Distance(transform.position, commandItem.transform.position);

            if (distance > 1) {
                transform.position = Vector3.Lerp(transform.position, commandItem.transform.position, 20 * Time.deltaTime);
            } else {
                Destroy(gameObject);
            }
        }

        if (blockedImage != null) {
            if (blocking) {
                blockedImage.enabled = true;
            } else {
                blockedImage.enabled = false;

                if (currentItemObject != null && !currentItemObject.GetComponent<Command>().destroying) {
                    Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, currentItemObject.transform.position.z);
                    currentItemObject.transform.position = Vector3.Lerp(currentItemObject.transform.position, targetPosition, 20 * Time.deltaTime);
                }
            }
        }
    }

    public Command(CommandType type, Functions function) {
        this.type = type;
        this.function = function;
    }

    public void SetCommandType(CommandType type) {
        this.type = type;
    }

    public CommandType GetCommandType() {
        return type;
    }

    public void SetFunctions(Functions function) {
        this.function = function;
    }

    public Functions GetFunctions() {
        return function;
    }

    public void OnClick() {

        if (levelManager.IsRunning()) {
            return;
        }

        if (type == CommandType.NONE || destroying) {
            return;
        }

        // Add item amount back to inventory

        CommandItem commandItem = levelManager.FindCommandItemOfType(type, function);
        commandItem.amount++;

        Command selectedCommand = null;
        // Find command of this command (item)
        foreach (Command command in parentFunction.GetCommandList()) {
            if (command.currentItemObject != null && command.currentItemObject.Equals(gameObject)) {
                selectedCommand = command;
                break;
            }
        }

        // Clear current command
        selectedCommand.type = CommandType.NONE;
        selectedCommand.function = null;

        // Remove selected command and rearrange index
        for (int i = 0; i < parentFunction.GetCommandList().Count; i++) {
            if (parentFunction.GetCommandList()[i].GetCommandType() == CommandType.NONE && !parentFunction.GetCommandList()[i].blocking) {

                if (i >= parentFunction.GetCommandList().Count) {
                    continue;
                }

                Command nextCommand = null;

                for (int x = i + 1; x < parentFunction.GetCommandList().Count; x++) {
                    if (parentFunction.GetCommandList()[x].GetCommandType() != CommandType.NONE && !parentFunction.GetCommandList()[x].blocking) {
                        nextCommand = parentFunction.GetCommandList()[x];
                        break;
                    }
                }

                if (nextCommand != null) {
                    parentFunction.GetCommandList()[i].SetCommandType(nextCommand.GetCommandType());
                    parentFunction.GetCommandList()[i].SetFunctions(nextCommand.GetFunctions());
                    parentFunction.GetCommandList()[i].currentItemObject = nextCommand.currentItemObject;

                    nextCommand.SetCommandType(CommandType.NONE);
                    nextCommand.SetFunctions(null);
                    nextCommand.currentItemObject = null;
                }
            }
        }

        destroying = true;
    }
}
