using System.Collections.Generic;
using UnityEngine;

public class NoiseController : MonoBehaviour
{
    public NoiseMode noiseMode;

    [SerializeField] private List<GameObject> npcObjects;

    public void Init(NoiseMode mode, bool isWeek2)
    {
        noiseMode = mode;

        switch (noiseMode)
        {
            case NoiseMode.None:
                break;

            case NoiseMode.SingleTalker:

                break;

            case NoiseMode.Environmental:

                break;
        }
    }
}