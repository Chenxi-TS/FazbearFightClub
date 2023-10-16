using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InputHandler
{
    Dictionary<KeyCode, Command> keyToCommand = new Dictionary<KeyCode, Command>();

    Command commandToChangeKeyCode = null;
    bool awaitingNextInput = false;

    public InputHandler(Node tree, Dictionary<KeyCode, Command> characterCommandList)
    {
        foreach(KeyValuePair<KeyCode, Command> ktc in characterCommandList)
        {
            ktc.Value.AddObserver(tree);
            keyToCommand.Add(ktc.Key, ktc.Value);
            Debug.Log("Added " + ktc.Value + " for " + tree);
        }
    }
    public List<Command> ReadInput()
    {
        //Debug.Log("Reading Input");
        List<Command> inputCommands = new List<Command>();
        foreach (KeyValuePair<KeyCode, Command> pair in keyToCommand)
        {
            if(Input.GetKey(pair.Key))
            {
                inputCommands.Add(pair.Value);
                pair.Value.Execute();
            }
            if(Input.GetKeyUp(pair.Key))
            {
                inputCommands.Add(pair.Value);
                pair.Value.Execute();
            }
        }
        return inputCommands;
    }
    public void StartChangeComannd(Command commandToChange)
    {
        if (commandToChange == null)
        {
            Debug.LogError("Command to change does not exist");
            return;
        }
        this.commandToChangeKeyCode = commandToChange;
        awaitingNextInput = true;
    }
    void Update()
    {
        if (awaitingNextInput)
        {
            KeyCode newKey = ReadAnyNextInput();
            if (newKey != KeyCode.None)
            {
                ChangeCommand(commandToChangeKeyCode, newKey);
                commandToChangeKeyCode = null;
                awaitingNextInput = false;
            }
        }
    }
    KeyCode ReadAnyNextInput()
    { 
        foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key))
                return key;
        }
        return KeyCode.None;
    }
    void ChangeCommand(Command targettedCommand, KeyCode newKey)
    {
        foreach(KeyValuePair<KeyCode, Command> pair in keyToCommand)
        {
            if(pair.Key == newKey)
            {
                Command tempCommandStorage = pair.Value;
                keyToCommand.Remove(pair.Key);
                keyToCommand.Add(KeyCode.None, tempCommandStorage);
            }
            if(pair.Value == targettedCommand)
            {
                keyToCommand.Remove(pair.Key);
                keyToCommand.Add(newKey, targettedCommand);
            }
        }
    }
}
