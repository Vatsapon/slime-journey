using System.Collections.Generic;
using UnityEngine;

public class Functions : MonoBehaviour {

    public bool main = false;
    public List<Command> commandList = new List<Command>();
    private Functions previousFunction;
    private int nextFunctionIndex;

    public Functions() {
        main = false;
        commandList = new List<Command>();
    }

    public void SetMainFunctions(bool value) {
        main = value;
    }

    public bool IsMainFunctions() {
        return main;
    }

    public List<Command> GetCommandList() {

        List<Command> availableCommand = new List<Command>();

        foreach (Command command in commandList) {
            if (command.gameObject.activeSelf) {
                availableCommand.Add(command);
            }
        }

        return availableCommand;
    }

    public void SetPreviousFunction(Functions function) {
        previousFunction = function;
    }

    public Functions GetPreviousFunction() {
        return previousFunction;
    }

    public void SetNextFunctionIndex(int value) {
        nextFunctionIndex = value;
    }

    public int GetNextFunctionIndex() {
        return nextFunctionIndex;
    }
}
