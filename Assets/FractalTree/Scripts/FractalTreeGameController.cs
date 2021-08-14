using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalTreeGameController : MonoBehaviour
{
    [SerializeField]
    GameObject m_fractalTreePrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(GenerateFractalTree), 0, 0.5f);
    }

    void GenerateFractalTree()
    {
        GameObject gc = Instantiate(m_fractalTreePrefab);
    }
}
