using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class Node :ScriptableObject
{

    public Node ParentNode;
    public Vector2 Position;
    public string Guid;

    public string DialogContent;
    public string Description;
    public List<Node> Children = new List<Node>();
    public string SpeakerName;
    public Sprite SpeakerImage;

    public abstract int LimitChildCount { get; }
    public abstract string Name { get; }


    public void OnEnable()
    {
        Guid = GUID.Generate().ToString();

    }

    public bool AddChild(Node node)
    {
        if (node == null)
        {
            return false;
        }
        if (node.ParentNode != null)
        {
            return false;
        }

        if (this.Children.Count >= LimitChildCount)
        {
            return false;
        }
        this.Children.Add(node);
        node.ParentNode = this;
        return true;
    }

    public bool RemoveChild(Node node)
    {
        if (node == null)
        {
            return false;
        }
        if (node.ParentNode != this)
        {
            return false;
        }

        if (!this.Children.Contains(node))
        {
            return false;
        }

        this.Children.Remove(node);
        node.ParentNode = null;
        return true;
    }



}




