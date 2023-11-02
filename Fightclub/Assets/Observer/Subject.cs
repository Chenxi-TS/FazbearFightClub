using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject
{
    List<Observer> observers = new List<Observer>();
    protected void OnNotify(string eventKey)
    {
        foreach(Observer obser in observers)
        {
            obser.OnNotify(eventKey);
        }
    }
    public void AddObserver(Observer newObserver)
    {
        if (newObserver == null)
        {
            Debug.LogError("Trying to add null observer to subject");
            return;
        }
        if (observers.Contains(newObserver))
        {
            Debug.LogError("Subject already contains observer");
            return;
        }
        observers.Add(newObserver);
    }
    public void RemoveObserver(Observer unwantedObserver)
    {
        if(unwantedObserver == null)
        {
            Debug.LogError("Trying to remove null observer from subject");
            return;
        }
        if(!observers.Contains(unwantedObserver))
        {
            Debug.LogError("Subject does not contain observer to remove");
            return;
        }
        observers.Remove(unwantedObserver);
    }
}
