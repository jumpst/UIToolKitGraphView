using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


[Serializable]
public abstract class Data { }

[Serializable]
public class Student : Data
{

    public string Name;
    public string Description;
}

[CreateAssetMenu(menuName = "新建节点树")]
public class NodeTree : ScriptableObject
{
    //根节点


    [SerializeField]
    public List<Node> nodes = new List<Node>();
    public string path = "";

    public List<Data> students = new List<Data>();



    public Node CreateNode<T>() where T : Node
    {
        return CreateNode(typeof(T));
    }

    public Node CreateNode(Type type)
    {
        var nodeType = typeof(Node);
        if (!nodeType.IsAssignableFrom(type)) return null;

        var node = ScriptableObject.CreateInstance(type) as Node;

        var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
        AssetDatabase.CreateAsset(node, $"{path}/{node.Guid}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //var node = Activator.CreateInstance(type) as Node;
        // nodes.Add(node.Guid, node);
        nodes.Add(node);

        return node;
    }



    public void AddChild(Node parentNode, Node node)
    {
        if (parentNode.AddChild(node))
        {
            nodes.Remove(node);
        }

    }

    public void DeleteNode(Node node)
    {



        if (node.ParentNode == null)
        {
            nodes.Remove(node);
        }
        else
        {
            node.ParentNode.RemoveChild(node);
        }
        foreach (var child in node.Children)
        {
            child.ParentNode = null;
            nodes.Add(child);
        }
        node.Children.Clear();

        var path = AssetDatabase.GetAssetPath(node);
        AssetDatabase.DeleteAsset(path);

        EditorUtility.SetDirty(this);
        AssetDatabase.Refresh();
    }

}
