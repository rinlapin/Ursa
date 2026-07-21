using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class ConstellationEditor : EditorWindow
{
    private const float ConvertedLimitX = 600;
    private const float ConvertedLimitY = 300;
    
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
    private StyleSheet _styleSheet;

    private VisualElement _root;

    private VisualElement _rightPane, _constBox;
    private List<StarBox> _stars;
    private List<CoordField> _coordinates;
    
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

    /*private VisualElement Star(string index)
    {
        var newStar = new VisualElement()
        {
            style =
            {
                width = 20,
                height = 20,
                backgroundColor = Color.yellow,
                position = Position.Absolute
            }
        };

        var text = new Label()
        {
            text = index,
            style =
            {
                unityFontStyleAndWeight = FontStyle.Bold,
                color = Color.black
            }
        };
        
        newStar.Add(text);

        return newStar;
    }*/

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
        _constBox = _rightPane.Q<VisualElement>("constBox");
        
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
        
        _constBox.Clear();
        _stars = new List<StarBox>();
        _coordinates = new List<CoordField>();
        
        for (int i = 0; i < selection.positions.Count; i++)
        {
            var subBox = new Box()
            {
                style = {  }
            };

            var xyz = new CoordField(i, _positionProperty.GetArrayElementAtIndex(i), OnVector3ValueChanged);
            
            xyz.Field.BindProperty(_positionProperty.GetArrayElementAtIndex(i));
            xyz.Field.RegisterValueChangedCallback(OnVector3ValueChanged);
            _coordinates.Add(xyz);

            subBox.Add(xyz);
            _listBox.Add(subBox);
            _rightPane.Add(_listBox);
        }
    }

    private void OnVector3ValueChanged(ChangeEvent<Vector3> evt)
    {
        var source = evt.target as VisualElement;
        DrawStar(source, evt.newValue);
    }

    private void DrawStar(VisualElement source, Vector3 starPosition)
    {
        foreach (var star in _stars)
        {
            if (star.OwningElement == source)
            {
                star.RemoveFromHierarchy();
            }
        }
        
        var centerPoint = new Vector2(_constBox.contentRect.width / 2, _constBox.contentRect.height / 2);
        
        var newStar = new StarBox(source);
        _stars.Add(newStar);
        _constBox.Add(newStar);
        
        newStar.transform.position = centerPoint +
            new Vector2(
                starPosition.x * (_constBox.contentRect.width / 2) / ConvertedLimitX, 
                -starPosition.y * (_constBox.contentRect.height / 2) / ConvertedLimitY);

        var manipulator = new Draggable(newStar);
        newStar.RegisterCallback<PointerUpEvent>(SetValuesOnReleasePointer);
    }

    private void SetValuesOnReleasePointer(PointerUpEvent evt)
    {
        var visual = evt.currentTarget as StarBox;
        var vectorField = visual.OwningElement as Vector3Field;
        var convertedPosition = new Vector3(
            visual.transform.position.x * ConvertedLimitX / (_constBox.contentRect.width / 2) - ConvertedLimitX,
            -visual.transform.position.y * ConvertedLimitY / (_constBox.contentRect.height / 2) + ConvertedLimitY,
            vectorField.value.z
        );
        
        vectorField.value = convertedPosition;
    }

    private void OnAddClicked()
    {
        _positionProperty.InsertArrayElementAtIndex(_positionProperty.arraySize);
        _so.ApplyModifiedProperties();
        UpdateList(_selection);
        _selection.positions[^1] = Vector3.zero;
    }

    private void OnRemoveClicked()
    {
        _positionProperty.DeleteArrayElementAtIndex(_positionProperty.arraySize - 1);
        _so.ApplyModifiedProperties();
        UpdateList(_selection);
    }
}

public class StarBox : VisualElement
{
    public VisualElement OwningElement { get; private set; }
    
    public StarBox(VisualElement owningElement)
    {
        OwningElement = owningElement;
        
        var newStar = new VisualElement()
        {
            style =
            {
                width = 20,
                height = 20,
                backgroundColor = Color.yellow,
                position = Position.Absolute
            }
        };

        var text = new Label()
        {
            text = owningElement.name,
            style =
            {
                unityFontStyleAndWeight = FontStyle.Bold,
                color = Color.black
            }
        };
        
        newStar.Add(text);
        
        this.Add(newStar);
    }
}

public class CoordField : VisualElement
{
    public int Index { get; private set; }
    public SerializedProperty PositionProperty { get; private set; }
    public EventCallback<ChangeEvent<Vector3>> OnValueChanged { get; private set; }
    
    public Vector3Field Field { get; private set; }
    
    public CoordField(int index, SerializedProperty positionProperty, EventCallback<ChangeEvent<Vector3>> onValueChanged)
    {
        Index = index;
        PositionProperty = positionProperty;
        OnValueChanged = onValueChanged;
        
        Field = new Vector3Field($"Star {Index}") { name = $"{Index}" };
        
        this.Add(Field);
    }
}

