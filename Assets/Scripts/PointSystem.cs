using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{

    public static PointSystem Instance;

    [SerializeField]
    private int _comboCounter = 1;
    [SerializeField]
    private int _score = 0;
    public int ComboCounter { get { return _comboCounter; } }
    public int Score { get { return _score; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseCombo()
    {
        _comboCounter++;
    }

    public void ResetCombo()
    {
        _comboCounter = 1;
    }

    public void AddPoints(int points = 1)
    {
        _score += points * _comboCounter;
    }
}
