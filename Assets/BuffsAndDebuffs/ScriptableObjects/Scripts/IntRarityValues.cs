using UnityEngine;

[CreateAssetMenu(fileName = "IntRarityValues", menuName = "Scriptable Objects/IntRarityValues")]
public class IntRarityValues : ScriptableObject
{
    public int minCommonValue;
    public int maxCommonValue;

    public int minRareValue;
    public int maxRareValue;

    public int minEpicValue;
    public int maxEpicValue;

    public int minLegendaryValue;
    public int maxLegendaryValue;
}
