using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] int _startMoves;
    int _currentMoves;
    public static MoveManager Instance;
    private void Awake()
    {
        Instance = this;
        _currentMoves = _startMoves;
        _scoreText.text = $"{_startMoves}";
    }
    public void UseMove()
    {
        _currentMoves--;
        if (_currentMoves < 0)
        {
            _currentMoves = 0; 
            //call lose event.
        }
        _scoreText.text = $"{_currentMoves}";
    }
    // Update is called once per frame

}
