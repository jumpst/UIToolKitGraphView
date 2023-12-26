using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeTreeViewer : GraphView
{
    public new class UxmlFactory : UxmlFactory<NodeTreeViewer, GraphView.UxmlTraits> { }


    public NodeTreeViewer()
    {
        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIToolKit/Editor/UI/NodeTreeViewer.uss");
        styleSheets.Add(styleSheet);


    }

    public NodeTree nodeTree { get; private set; }


    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);

        var types = TypeCache.GetTypesDerivedFrom<Node>();

        foreach (var item in types)
        {
            evt.menu.AppendAction(item.Name, (a) => CreateNode(item));
        }
    }
    public Action<NodeViewer> OnNodeSelected;
    public void CreateNode(Type nodeType)
    {
        Node node = nodeTree.CreateNode(nodeType);
        EditorUtility.SetDirty(nodeTree);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        CreateNodeView(node);
    }

    public void CreateNodeView(Node node)
    {
        var viewer = new NodeViewer(node);
        viewer.OnNodeSelect = OnNodeSelected;
        AddElement(viewer);
    }


    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {

        return ports.ToList().Where((endPoint) =>
         {
             return endPoint.direction != startPort.direction && endPoint.node != startPort.node;
         }).ToList();
    }

    public GraphViewChange GraphViewChangedX(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null && graphViewChange.elementsToRemove.Count > 0)
        {
            graphViewChange.elementsToRemove.ForEach((a) =>
            {
                var nodeView = a as NodeViewer;
                if (nodeView != null)
                {
                    //不等于
                    nodeTree.DeleteNode(nodeView.node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null && graphViewChange.edgesToCreate.Count > 0)
        {
            graphViewChange.edgesToCreate.ForEach((edge) =>
            {
                ///对于每一个edge来说，先来获得output
                var parentNode = (edge.output.node as NodeViewer).node;
                var childNode = (edge.input.node as NodeViewer).node;
                if (parentNode != null && childNode != null)
                {
                    Debug.Log($"parentNode_{parentNode.Guid}_childNode{childNode.Guid}");
                    nodeTree.AddChild(parentNode, childNode);

                    var childC_guid = "";
                    if (childNode.Children.Count > 0)
                    {
                        childC_guid = childNode.Children[0].Guid;
                    }
                    Debug.Log($"parentNode_c_{parentNode.Children[0].Guid}_ChildNode_c_{childC_guid}");
                }
            });
        }



        return graphViewChange;
    }


    internal void PopulateView(NodeTree tree)
    {
        this.nodeTree = tree;
        graphViewChanged -= GraphViewChangedX;
        DeleteElements(graphElements);
        graphViewChanged += GraphViewChangedX;

        //填充试图

        
        Stack<Node> stack = new Stack<Node>();
        foreach (var tempRootNode in tree.nodes)
        {
            var node = tempRootNode;
            stack.Push(node);
            int counter = 0;
            while (stack.Count > 0)
            {
                counter++;
                if (counter > 999) { throw new Exception("Stack Excep"); }
                var tempNode = stack.Pop();
                CreateNodeView(tempNode);
                foreach (var item in tempNode.Children)
                {
                    stack.Push(item);
                }
            }
        }


        
        //再然后是创建边
        foreach (var tempRootNode in tree.nodes)
        {
            stack.Push(tempRootNode);
            int counter = 0;
            while (stack.Count > 0)
            {
                counter++;
                if (counter > 999) { throw new Exception("Stack Excep"); }
                var tempNode = stack.Pop();
                var parentNodeViewer = GetNodeByGuid(tempNode.Guid) as NodeViewer;

                foreach (var item in tempNode.Children)
                {
                    var childViewer = GetNodeByGuid(item.Guid) as NodeViewer;
                    var edge = parentNodeViewer.Output.ConnectTo(childViewer.Input);
                    AddElement(edge);
                    stack.Push(item);
                }
            }
        }



    }
}
