using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeViewer : UnityEditor.Experimental.GraphView.Node
{

    public Node node;
    public Port Output;
    public Port Input;

    public Action<NodeViewer> OnNodeSelect;

  


 
    public NodeViewer(Node node):base("Assets/UIToolKit/Editor/UI/NodeViewer.uxml")
    {
      
        
        this.node = node;
        this.title = node.Name;
        this.viewDataKey = node.Guid;
        style.left = node.Position.x;
        style.top = node.Position.y;

        CreateInput();
        CreateOutput();
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIToolKit/Editor/UI/Node.uss");
        //styleSheets.Add(styleSheet);
    }

    
   
    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelect.Invoke(this);
    }

    public void CreateInput()
    {
         Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));

        inputContainer.Add(Input);

    }

    public void CreateOutput()
    {
        Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        outputContainer.Add(Output);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        node.Position = new Vector2(newPos.xMin, newPos.yMin);
    }


  
}
