using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.PlayerLoop;

//Contains all Commands and respective KeyCode for a character
//-> ReadInput() returns a list of Commands for all Commands executed on call
//-> StartChangeCommand(Command commandToChange) changes respective KeyCode for a command
public class InputHandler
{
    Dictionary<KeyCode, Command> keyToCommand = new Dictionary<KeyCode, Command>();

    Command commandToChangeKeyCode = null;
    bool awaitingNextInput = false;

    //Sets up Command to respective KeyCode and adds respective tree to (subject)Command's list of observers
    public InputHandler(Node tree, Dictionary<KeyCode, Command> characterCommandList)
    {
        foreach(KeyValuePair<KeyCode, Command> ktc in characterCommandList)
        {
            ktc.Value.AddObserver(tree);
            keyToCommand.Add(ktc.Key, ktc.Value);
            Debug.Log("Added " + ktc.Value + " for " + tree);
        }
    }

    //Goes through every entry in keyToCommand dictionary. Checking if there is an input equivalent to a entry's key and
    //executing the Command base on if the input was pressed, held, or released.
    //->Execute(false) = pressed
    //->Execute() = held
    //->Execute(true) = released
    public List<Command> ReadInput()
    {
        //Debug.Log("Reading Input");
        List<Command> inputCommands = new List<Command>();
        foreach (KeyValuePair<KeyCode, Command> pair in keyToCommand)
        {
            if(Input.GetKeyDown(pair.Key))
            {
                //Debug.Log("DOWN");
                inputCommands.Add(pair.Value);
                pair.Value.Execute(false);
            }
            if(Input.GetKey(pair.Key))
            {
                inputCommands.Add(pair.Value);
                pair.Value.Execute();
            }
            if(Input.GetKeyUp(pair.Key))
            {
                //Debug.Log("UP");
                inputCommands.Add(pair.Value);
                pair.Value.Execute(true);
            }
        }
        return inputCommands;
    }

    //Start listening for input to change KeyCode to Command relationship in keyToCommand dictionary
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
    //Returns any next KeyCode pressed
    KeyCode ReadAnyNextInput()
    { 
        foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key))
                return key;
        }
        return KeyCode.None;
    }
    //Removes targettedCommand in keyToCommand dictionary and
    //adds a new entry with newKey as the key and targettedCommand as the new value.
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
    //Waiting for input to change KeyCode to respective Command
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
}
