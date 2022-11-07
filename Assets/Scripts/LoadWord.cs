using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class LoadWord : MonoBehaviour
{
    [SerializeField] private ConfigWordGame _config;
    public static LoadWord Singleton;
    
    private bool _isReady;
    private string _filePath;
    private List<string> _wordsList = new List<string>();
    
    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else if (Singleton != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _filePath = Path.Combine(Application.streamingAssetsPath, "alice30.txt");
        StartCoroutine(LoadFile());
    }

    private IEnumerator LoadFile()
    {
        if (File.Exists(_filePath))
        {
            string dataChars = "";
            if (_filePath.Contains("://"))
            {
                var request = new UnityWebRequest(_filePath);
                yield return request;
                if (string.IsNullOrEmpty(request.error))
                {
                    dataChars = request.ToString();
                }
            }
            else
            {
                dataChars = File.ReadAllText(_filePath);
            }

            var words = ComposingText(dataChars);
            _wordsList = SearchRepetitions(words);
        }
        else
        {
            Debug.LogError("Не удается найти файл!");
        }

        _isReady = true;
    }
    
    private string ComposingText(string chars)
    {
        var words = " ";

        for (var i = 0; i < chars.Length; i++)
        {
            if (!char.IsLetter(chars[i]) && chars[i] != ' ') continue;
            words += chars[i];
        }

        words += " ";
        words = words.ToUpper();
        return words;
    }

    private List<string> SearchRepetitions(string words)
    {
        var wordsList = words.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        var result = wordsList.GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => new {Word = x.Key, Frequency = x.Count()});

        var wordsLessMin = new List<string>(wordsList.Count);
        for (var i = 0; i < wordsList.Count; i++)
        {
            if(wordsList[i].Length <= _config.MinCountLetter) wordsLessMin.Add(wordsList[i]);
        }
        
        foreach (var item in result)
        {
            for (var i = 0; i < item.Frequency - 1; i++)
            {
                wordsList.Remove(item.Word);
            }
        }
        foreach (var word in wordsLessMin)
        {
            wordsList.Remove(word);
        }
    
        return wordsList;
    }

    public List<string> GetWords() => _wordsList;
    public bool GetStateReady() => _isReady;
}