using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour
{
    [Header("Alternating level backgrounds")]
    public GameObject skyA;
    public GameObject skyB;

    [Header("First run behavior")]
    public bool randomizeFirst = false;

    const string KeyNext = "bg_next"; // 0 or 1

    void Start()
    {
        // decide which index to use this run
        int next = PlayerPrefs.GetInt(KeyNext, -1);
        if (next == -1) next = randomizeFirst ? Random.Range(0, 2) : 0;

        // enable chosen, disable the other
        bool useA = (next == 0);
        if (skyA) skyA.SetActive(useA);
        if (skyB) skyB.SetActive(!useA);

        // store the opposite for next game
        PlayerPrefs.SetInt(KeyNext, useA ? 1 : 0);
        PlayerPrefs.Save();
    }
}