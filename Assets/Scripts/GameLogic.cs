using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private ConfigWordGame _config;
    
    [Header("Panels")]
    [SerializeField] private GameObject _panelLetterPrefab;
    [SerializeField] private Transform _panelWord;
    [SerializeField] private Transform _panelKeyboard;

    [Header("Texts")]
    [SerializeField] private Text _currentAttempts;
    [SerializeField] private Text _currentPoints;

    [Header("Interfaces")] 
    [SerializeField] private GameObject _interface;
    [SerializeField] private GameObject _victory;
    [SerializeField] private GameObject _defeat;
    [SerializeField] private GameObject _passed;

    private int _remainingLetters;
    private string _wordCurrent;
    
    private Button[] _buttonsKeyboard;
    private List<string> _wordsList;
    
    private readonly List<GameObject> _panels = new List<GameObject>();
    private readonly List<string> _pastWords = new List<string>();
    
    private void Awake()
    {
        if (LoadWord.Singleton == null)
        {
            Debug.LogError($"Запускайте сцену: {SceneManager.GetSceneByBuildIndex(0).name}");
        }
        
        _buttonsKeyboard = _panelKeyboard.GetComponentsInChildren<Button>();

        _currentAttempts.text = _config.CountAttempts.ToString();
        _currentPoints.text = "0";
        _wordCurrent = "";
        _remainingLetters = 0;
        
        _wordsList = LoadWord.Singleton.GetWords();
        
        StartRound();
    }

    private void StartRound()
    {
        _wordCurrent = GetWord();
        
        if (_wordCurrent.Length > 0)
            _remainingLetters = _wordCurrent.Length;
        else
        {
            _passed.SetActive(true);

            StartCoroutine(Background());
            RestartGame();
        }
        
        foreach (var button in _buttonsKeyboard)
        {
            button.gameObject.SetActive(true);
        }

        var lettersCount = _panels.Count;
        for (var i = 0; i < lettersCount; i++)
        {
            var letter = _panels[0];
            _panels.Remove(letter);
            Destroy(letter);
        }

        for (var i = 0; i < _wordCurrent.Length; i++)
        {
            var panelLetter = Instantiate(_panelLetterPrefab, Vector3.zero, Quaternion.identity);
            panelLetter.transform.SetParent(_panelWord);
            panelLetter.GetComponentInChildren<Text>().text = "?";
            _panels.Add(panelLetter);
        }
    }

    private string GetWord()
    {
        for (var i = 0; i < _wordsList.Count; i++)
        {
            var randomWord = Random.Range(0,_wordsList[i].Length);

            var word = _wordsList[randomWord];
            if (_pastWords.Contains(word)) continue;
            
            _pastWords.Add(word);
            return word;
        }
        
        return "";
    }

    public void EnterWord(GameObject button)
    {
        var selectedLetter = button.GetComponentInChildren<Text>().text;
        button.SetActive(false);
        
        if (_wordCurrent.Length <= 0) return;

        if (_wordCurrent.Contains(selectedLetter))
        {
            for (var i = 0; i < _wordCurrent.Length; i++)
            {
                if (_wordCurrent[i] != selectedLetter[0]) continue;
                
                _currentPoints.text = (int.Parse(_currentPoints.text) + _config.NumberPointsAwarded).ToString();
                _panels[i].GetComponentInChildren<Text>().text = _wordCurrent[i].ToString();
                
                _remainingLetters -= 1;
                if (_remainingLetters <= 0)
                {
                    _victory.SetActive(true);

                    StartCoroutine(Background());
                    StartRound();
                    return;
                }
            }
        }
        else
        {
            _currentAttempts.text = (int.Parse(_currentAttempts.text) - 1).ToString();
            if (int.Parse(_currentAttempts.text) <= 0)
            {
                _defeat.SetActive(true);

                StartCoroutine(Background());
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        _currentAttempts.text = _config.CountAttempts.ToString();
        _currentPoints.text = "0";
        _wordCurrent = "";

        StartRound();
    }

    private IEnumerator Background()
    {
        _interface.SetActive(false);
        yield return new WaitForSeconds(2);
        _interface.SetActive(true);
        _victory.SetActive(false);
        _defeat.SetActive(false);
        _passed.SetActive(false);
    }
}