using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using PlasticGui.Help;

public class NodeEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/NodeEditor")]
    public static void ShowExample()
    {
        NodeEditor wnd = GetWindow<NodeEditor>();
        wnd.titleContent = new GUIContent("NodeEditor");
    }


   
  

    NodeTreeViewer nodeTreeViewer;
    InspectorViewer inspectorViewer;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;


        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolKit/Editor/UI/NodeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIToolKit/Editor/UI/NodeEditor.uss");
        root.styleSheets.Add(styleSheet);

        nodeTreeViewer = root.Q<NodeTreeViewer>();
        inspectorViewer = root.Q<InspectorViewer>();
    }


  
    private void OnSelectionChange()
    {
        var nodeTree = Selection.activeObject as NodeTree;

        if (nodeTree != null)
        {
            nodeTreeViewer.PopulateView(nodeTree);
            nodeTreeViewer.OnNodeSelected = inspectorViewer.UpdateSelection;
        }
    }
}