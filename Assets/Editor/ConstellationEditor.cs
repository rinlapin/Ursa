using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ConstellationEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
    private StyleSheet _styleSheet;

    private VisualElement _root;
    
    private VisualElement _rightPane;
    
    private Vector2Field _xy;

    private Button _removeButton;

    private Box _listBox;
    private VisualElement _buttonBox;
    
    private ConstellationData _selection;
    private SerializedObject _so;
    private SerializedProperty _positionProperty;

    [MenuItem("Window/ConstellationEditor")]
    public static void ShowExample()
    {
        ConstellationEditor wnd = GetWindow<ConstellationEditor>();
        wnd.titleContent = new GUIContent("Constellation Editor");
    }

    public void CreateGUI()
    {
        _root = rootVisualElement;

        var allConstellationGuids = AssetDatabase.FindAssets("t:ConstellationData");
        var allConstellations = new List<ConstellationData>();
        foreach (var guid in allConstellationGuids)
        {
            allConstellations.Add(
                AssetDatabase.LoadAssetAtPath<ConstellationData>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);
        _root.Add(splitView);

        var leftPane = new ListView()
        {
            fixedItemHeight =  40,
            style = {
                flexGrow = 1,
            }
        };
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

        using var enumerator = selectedItems.GetEnumerator();
        if (!enumerator.MoveNext()) return;

        _selection = enumerator.Current as ConstellationData;
        if (!_selection) return;
        
        var nameLabel = new Label()
        {
            style =
            {
                fontSize = 16,
                unityFontStyleAndWeight = FontStyle.Bold,
                marginTop = 5,
                marginBottom = 5,
                marginLeft = 5
            },
            text = _selection.name
        };
        
        _rightPane.Add(nameLabel);
        
        VisualElement fromUxml = m_VisualTreeAsset.Instantiate();
        _rightPane.Add(fromUxml);
        
        _buttonBox = _rightPane.Q<VisualElement>("buttons");
        
        var addButton = new Button()
        {
            text = "Add"
        };
        addButton.clicked += OnAddClicked;

        _removeButton = new Button()
        {
            text = "Remove",
        };
        _removeButton.clicked += OnRemoveClicked;
        
        _buttonBox.Add(addButton);
        _buttonBox.Add(_removeButton);
        
        _so = new SerializedObject(_selection);
        _positionProperty = _so.FindProperty("positions");

        UpdateList(_selection);
    }

    private void UpdateList(ConstellationData selection)
    {
        _removeButton.enabledSelf = selection.positions.Count > 0;
        
        _listBox?.Clear();
        if (_rightPane.Contains(_listBox))
        {
            _rightPane.Remove(_listBox);
        }
        _listBox = new Box();
        
        for (int i = 0; i < selection.positions.Count; i++)
        {
            var subBox = new Box()
            {
                style = {  }
            };
            var xyz = new Vector3Field($"Star {i}") { name = "xyz" };
            xyz.BindProperty(_positionProperty.GetArrayElementAtIndex(i));

            subBox.Add(xyz);
            _listBox.Add(subBox);
            _rightPane.Add(_listBox);
        }
    }

    private void OnAddClicked()
    {
        _positionProperty.InsertArrayElementAtIndex(_positionProperty.arraySize);
        _so.ApplyModifiedProperties();
        UpdateList(_selection);
    }

    private void OnRemoveClicked()
    {
        _positionProperty.DeleteArrayElementAtIndex(_positionProperty.arraySize - 1);
        _so.ApplyModifiedProperties();
        UpdateList(_selection);
    }
}
