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
        // 1秒毎にフラクタルツリーを生成
        InvokeRepeating(nameof(GenerateFractalTree), 0, 1);
    }

    void GenerateFractalTree()
    {
        GameObject gc = Instantiate(m_fractalTreePrefab);
    }
}
