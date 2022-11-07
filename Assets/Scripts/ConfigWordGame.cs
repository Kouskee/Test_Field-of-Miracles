using UnityEngine;

[CreateAssetMenu(fileName = "Config", order = 0)]
public class ConfigWordGame : ScriptableObject
{
    [SerializeField] private int _minCountLetter = 5;
    [SerializeField] private int _countAttempts = 10;
    [SerializeField] private int _numberPointsAwarded = 5;

    public int MinCountLetter => _minCountLetter;
    public int CountAttempts => _countAttempts;
    public int NumberPointsAwarded => _numberPointsAwarded;
}