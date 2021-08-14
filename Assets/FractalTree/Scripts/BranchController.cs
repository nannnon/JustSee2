using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchController : MonoBehaviour
{
    enum State
    {
        Growing,
        Stopped,
        Blasting,
        None
    }

    [SerializeField]
    GameObject m_branchPrefab = null;

    State m_state;
    Vector2 m_startPos;
    float m_theta;
    float m_currentLength;
    float m_length;
    Color m_color;
    int m_n;
    FractalTreeController m_ftc;
    LineRenderer m_lr;
    bool m_generateNextBranch;
    float m_earthPosY;


    // Update is called once per frame
    void Update()
    {
        if (m_state == State.Growing)
        {
            m_currentLength += 1 * Time.deltaTime;
            if (m_currentLength > m_length)
            {
                m_currentLength = m_length;
                m_state = State.Stopped;
                m_generateNextBranch = true;
            }

            Vector2 endPos = new Vector2();
            endPos.x = m_startPos.x + m_currentLength * Mathf.Cos(m_theta);
            endPos.y = m_startPos.y + m_currentLength * Mathf.Sin(m_theta);

            m_lr.SetPosition(1, endPos);
        }
        else if (m_state == State.Stopped)
        {
            if (m_generateNextBranch)
            {
                if (m_n > 0)
                {
                    Vector2 startPos = m_lr.GetPosition(1);
                    float deltaTheta = Random.Range(Mathf.PI / 7, Mathf.PI / 5);
                    float length = m_length / Random.Range(1f, 2f);
                    int n = m_n - 1;

                    // 右
                    {
                        GameObject go = Instantiate(m_branchPrefab);
                        BranchController bc = go.GetComponent<BranchController>();
                        bc.Set(startPos, m_theta + deltaTheta, length, m_color, n, m_ftc);
                    }
                    // 左
                    {
                        GameObject go = Instantiate(m_branchPrefab);
                        BranchController bc = go.GetComponent<BranchController>();
                        bc.Set(startPos, m_theta - deltaTheta, length, m_color, n, m_ftc);
                    }
                }

                m_generateNextBranch = false;
            }
        }
        else if (m_state == State.Blasting)
        {
            // 落下
            Vector3[] pos = new Vector3[m_lr.positionCount];
            m_lr.GetPositions(pos);

            for (int i = 0; i < pos.Length; ++i)
            {
                pos[i].y -= 1 * Time.deltaTime;
            }

            m_lr.SetPositions(pos);

            // 地面についたら削除
            if (pos[0].y <= m_earthPosY)
            {
                m_state = State.None;
                m_ftc.IncrementBlastedBranchesNum();
                Destroy(gameObject);
            }
        }
    }

    public void Set(Vector2 startPos, float theta, float length, Color color, int n, FractalTreeController ftc)
    {
        m_state = State.Growing;
        m_startPos = startPos;
        m_theta = theta;
        m_currentLength = 0;
        m_length = length;
        m_color = color;
        m_n = n;
        m_ftc = ftc;
        m_lr = GetComponent<LineRenderer>();
        m_generateNextBranch = false;

        m_lr.material = new Material(Shader.Find("Sprites/Default"));
        m_lr.positionCount = 2;
        m_lr.SetPosition(0, m_startPos);
        m_lr.SetPosition(1, m_startPos);
        m_lr.startColor = m_color;
        m_lr.endColor = m_color;
        m_lr.startWidth = 0.05f;
        m_lr.endWidth = 0.05f;

        m_ftc.AddBC(this);
    }

    public void StartBlasting(float earthPosY)
    {
        m_state = State.Blasting;
        m_earthPosY = earthPosY;
    }
}
