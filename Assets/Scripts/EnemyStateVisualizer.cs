using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateVisualizer : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> flags = new List<GameObject>();
    private EnemyController enemy;
    private int currentFlagIndex = 0;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        enemy.ChangeStateAction += SwapFlag;
    }

    private void OnDisable()
    {
        enemy.ChangeStateAction -= SwapFlag;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwapFlag(EnemyState newState)
    {
        flags[currentFlagIndex].SetActive(false);

        currentFlagIndex = (int)newState;

        flags[currentFlagIndex].SetActive(true);

    }
}
