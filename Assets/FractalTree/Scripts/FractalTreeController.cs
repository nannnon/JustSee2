using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalTreeController : MonoBehaviour
{
    [SerializeField]
    GameObject m_branchPrefab = null;

    List<BranchController> m_branchControllers = new List<BranchController>();
    Vector2 m_startPos;
    int m_blastedBranchesNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Instantiate(m_branchPrefab);
        BranchController bc = go.GetComponent<BranchController>();

        Vector2 areaLB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 areaRT = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float length = Random.Range(0.3f, 2f);
        m_startPos = new Vector2(Random.Range(areaLB.x, areaRT.x), Random.Range(areaLB.y, areaRT.y - 2 * length));

        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        bc.Set(m_startPos, Mathf.PI / 2f, length, color, 5, this);

        Invoke(nameof(StartBlasting), 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_blastedBranchesNum == m_branchControllers.Count)
        {
            Destroy(gameObject);
        }
    }

    void StartBlasting()
    {
        StartCoroutine(StartBlasting2());
    }

    IEnumerator StartBlasting2()
    {
        for (int i = m_branchControllers.Count - 1; i >= 0; --i)
        {
            m_branchControllers[i].StartBlasting(m_startPos.y);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void AddBC(BranchController bc)
    {
        m_branchControllers.Add(bc);
    }

    public void IncrementBlastedBranchesNum()
    {
        ++m_blastedBranchesNum;
    }
}
