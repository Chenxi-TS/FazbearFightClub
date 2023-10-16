using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
    public class Node: Observer
    {
        protected List<Node> childrenNodes = new List<Node>();
        protected Node parent = null;
        protected Node root = null;
        protected Dictionary<string, object> nodeData = new Dictionary<string, object>();

        public Node() {}
        public Node(List<Node> childrenNodes)
        {
            foreach(Node child in childrenNodes)
            {
                this.childrenNodes.Add(child);
                child.SetParent(this);
                Debug.Log(child + "'s parent is " + child.parent);
            }
        }
        public Node(Node child)
        {
            this.childrenNodes.Add(child);
            child.SetParent(this);
            Debug.Log(child + "'s parent is " + child.parent);
        }
        public void SetParent(Node parent)
        {
            this.parent = parent;
        }
        public virtual NodeState Evaluate() => NodeState.FAILURE;
        public object findData(string key)
        {
            if (nodeData.ContainsKey(key))
                return nodeData[key];

            if(parent != null)
            {
                object data = parent.findData(key);
                if (data != null)
                {
                    return data;
                }
            }
            return null;
        }
        public bool removeData(string key)
        {
            if (nodeData.ContainsKey(key))
            {
                nodeData.Remove(key);
                return true;
            }

            if(parent != null)
            {
                parent.removeData(key);
            }
            return false;
        }
        public void addData(string key, object value)
        {
            if (nodeData.ContainsKey(key))
                return;
            nodeData.Add(key, value);
            //Debug.Log("Added data " + key);
        }
        protected Node findRoot()
        {
            if (parent == null)
            {
                return this;
            }
            else
                return parent.findRoot();
        }
        protected bool checkDataStatus(string dataKey, object defaultData)
        {
            //Checking status of "dataKey" data
            //-> adds "dataKey" with defaultData on initial run to root, return false
            //-> checks if "dataKey" data type is type of defaultData.GetType(), return false
            //-> if "dataKey" exist and is of defaultData.GetType(), return true
            if (root == null)
                root = findRoot();
            //Debug.Log(root);
            if (findData(dataKey) == null)
            {
                Debug.LogWarning(dataKey + " data not found");
                root.addData(dataKey, defaultData);
                return false;
            }
            if (findData(dataKey).GetType() != defaultData.GetType())
            {
                Debug.LogError(dataKey + " data in tree is not of type " + defaultData.GetType());
                return false;
            }
            return true;
        }
    }
}
