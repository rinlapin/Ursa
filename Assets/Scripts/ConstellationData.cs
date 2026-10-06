using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ConstellationData", menuName = "Scriptable Objects/ConstellationData")]
public class ConstellationData : ScriptableObject
{
    public List<Vector3> positions;
    public List<Vector2Int> connections;
}