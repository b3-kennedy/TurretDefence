using UnityEngine;

[CreateAssetMenu(fileName = "FloatRarityValues", menuName = "Scriptable Objects/FloatRarityValues")]
public class FloatRarityValues : ScriptableObject
{
    public float minCommonValue;
    public float maxCommonValue;

    public float minRareValue;
    public float maxRareValue;

    public float minEpicValue;
    public float maxEpicValue;

    public float minLegendaryValue;
    public float maxLegendaryValue;

    public float minDivineValue;
    public float maxDivineValue;

    public float minDemonicValue;
    public float maxDemonicValue;
}
