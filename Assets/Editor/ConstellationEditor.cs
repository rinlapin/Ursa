using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ConstellationEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
    private StyleSheet _styleSheet;

    private VisualElement _rightPane;

    [MenuItem("Window/ConstellationEditor")]
    public static void ShowExample()
    {
        ConstellationEditor wnd = GetWindow<ConstellationEditor>();
        wnd.titleContent = new GUIContent("Constellation Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        var allConstellationGuids = AssetDatabase.FindAssets("t:ConstellationData");
        var allConstellations = new List<ConstellationData>();
        foreach (var guid in allConstellationGuids)
        {
            allConstellations.Add(
                AssetDatabase.LoadAssetAtPath<ConstellationData>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);
        root.Add(splitView);

        var leftPane = new ListView();
        splitView.Add(leftPane);
        
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allConstellations[index].name; };
        leftPane.itemsSource = allConstellations;
        leftPane.selectionChanged += OnConstellationSelectionChanged;

        _rightPane = new VisualElement();
        splitView.Add(_rightPane);
    }

    private void OnConstellationSelectionChanged(IEnumerable<object> selectedItems)
    {
        _rightPane.Clear();

        var enumerator = selectedItems.GetEnumerator();
        if (enumerator.MoveNext())
        {
            VisualElement fromUxml = m_VisualTreeAsset.Instantiate();
            _rightPane.Add(fromUxml);

            var selection = enumerator.Current as ConstellationData;
            if (!selection) return;
            for (int i = 0; i < selection.positions.Count; i++)
            {
                Box itemBox = new Box();

                var label = new Label()
                {
                    name = $"Star {i}"
                };
                Vector3 item = selection.positions[i];
                
                var xyPosition = new Vector2Field();
                xyPosition.SetValueWithoutNotify(new Vector2(item.x, item.y));
                var zPosition = new FloatField()
                {
                    value = item.z,
                    name = "Z"
                };

                itemBox.Add(label);
                itemBox.Add(xyPosition);
                itemBox.Add(zPosition);
                
                _rightPane.Add(itemBox);

                var newStar = new VisualElement()
                {
                    name = $"Star {i}",
                    style =
                    {
                        backgroundColor = Color.white,
                        width = 20,
                        height = 20
                    }
                };
                Debug.Log("star");
                var constBox = _rightPane.Q<VisualElement>("constBox");
                constBox.Add(newStar);
                newStar.transform.position =
                    new Vector2(200,
                        50 * i); /*new Vector2(constBox.resolvedStyle.height / 2, constBox.resolvedStyle.width / 2)*/
                ;
                DragAndDropManipulator manipulator = new(newStar);
            }
        }
    }
    
    private void BindItem(StarDataVisual data) 
    {
        var label = data.Q<Label>(name: "label");
        var xy = data.Q<Vector2Field>(name: "xy");
        var z = data.Q <FloatField>(name: "z");
    }
}

public class StarDataVisual : VisualElement
{
    public StarDataVisual(Vector3 coordinates)
    {
        var root = new Box();
        var label = new Label()
        {
            name = "label"
        };
        root.Add(label);
        var xyCoordinates = new Vector2Field() { name = "xy" };
        xyCoordinates.SetValueWithoutNotify((Vector2)coordinates);
        root.Add(xyCoordinates);
        var zCoordinates = new FloatField() { name = "z" };
        zCoordinates.SetValueWithoutNotify((float)coordinates.z);
        root.Add(zCoordinates);
    }
}

[Serializable]
public class StarData
{
    public Vector2 xy;
    public float z;
}
