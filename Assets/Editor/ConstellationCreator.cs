using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ConstellationCreator : EditorWindow
{
    private VisualElement _rightPane;
    private Button _plusButton;
    private VisualTreeAsset _visualTreeAsset;
    
    [MenuItem("Window/Constellation Creator")]
    public static void ShowExample()
    {
        
        ConstellationCreator wnd = GetWindow<ConstellationCreator>();
        wnd.titleContent = new GUIContent("Constellation Creator");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        var allConstellationGuids = AssetDatabase.FindAssets("t:ConstellationData");
        var allConstellations = new List<ConstellationData>();
        foreach (var guid in allConstellationGuids)
        {
            allConstellations.Add(AssetDatabase.LoadAssetAtPath<ConstellationData>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        _visualTreeAsset = Resources.Load<VisualTreeAsset>("Assets/Editor/");

        /*var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);

        root.Add(splitView);

        var leftPane = new ListView();
        splitView.Add(leftPane);

        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allConstellations[index].name; };
        leftPane.itemsSource = allConstellations;
        leftPane.selectionChanged += OnConstellationSelectionChanged;

        _rightPane = new VisualElement();
        _creatorVisualTree = Resources.Load<VisualTreeAsset>("ConstellationCreator_Main");

        splitView.Add(_rightPane);*/
    }

    private void OnConstellationSelectionChanged(IEnumerable<object> selectedItems)
    {
        _rightPane.Clear();
        Draggable manipulator =
            new(_rightPane.Q<VisualElement>("object"));
        
        var enumerator = selectedItems.GetEnumerator();
        if (enumerator.MoveNext())
        {
            /*_creatorVisualTree.CloneTree(_rightPane);
            
            var selection = enumerator.Current as ConstellationData;
            if (!selection) return;
            foreach (var position in selection.positions)
            {
                var vector = new Vector3Field();
                vector.SetValueWithoutNotify(position);
                
                _rightPane.Add(vector);
            }

            var box = new Box();
            box.style.height = 20;
            //box.style.flexDirection = FlexDirection.Row;
            box.style.justifyContent = Justify.SpaceBetween;
            
            var saveButton = new Button();
            saveButton.text = "Save";
            saveButton.style.display = DisplayStyle.Flex;
            box.Add(saveButton);
            
            
            var revertButton = new Button();
            revertButton.text = "Revert";
            revertButton.style.display = DisplayStyle.Flex;
            box.Add(revertButton);
            
            _rightPane.Add(box);*/
        }
    }
}
