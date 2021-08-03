using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectsController : MonoBehaviour
{
    GameController m_gc;
    Vector2 m_size;
    int m_dir;
    float m_vel;
    int m_index;

    // 後続する矩形
    [SerializeField]
    GameObject m_followingRectPrefab = null;
    const int kFRN = 32;
    GameObject[] m_frs = new GameObject[kFRN];


    public void Init(GameController gc, Vector2 pos, Vector2 size, int index)
    {
        this.m_gc = gc;
        this.m_size = size;
        this.m_dir = (int)Random.Range(0, 4);
        this.m_vel = Random.Range(0.01f, 0.1f);
        this.m_index = index;

        Vector2 centerPos = new Vector2(pos.x + size.x / 2, pos.y + size.y / 2);
        this.transform.position = centerPos;
        this.transform.localScale = this.m_size;
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        this.GetComponent<SpriteRenderer>().color = color;

        // 後続する矩形を生成
        for (int i = 0; i < kFRN; ++i)
        {
            GameObject go = Instantiate(this.m_followingRectPrefab);
            this.m_frs[i] = go;

            go.GetComponent<Transform>().position = centerPos;
            go.GetComponent<Transform>().localScale = this.m_size;
            go.GetComponent<SpriteRenderer>().color = color;
        }
    }

    void Move(float vel)
    {
        Vector3 pos = this.transform.position;

        switch (this.m_dir)
        {
            case 0:
                pos.y += vel;
                break;
            case 1:
                pos.x += vel;
                break;
            case 2:
                pos.y -= vel;
                break;
            case 3:
                pos.x -= vel;
                break;
            default:
                throw new System.Exception();
        }

        this.transform.position = pos;
    }

    void FixedUpdate()
    {
        // 先頭の矩形を前進
        this.Move(this.m_vel);

        // 衝突したら後退して方向転換
        Vector2 centerPos = this.transform.position;
        Vector2 size = this.m_size;
        Vector2 pos = new Vector2(centerPos.x - size.x / 2, centerPos.y - size.y / 2);
        bool hit = this.m_gc.CheckHit(pos.x, pos.y, size.x, size.y, this.m_index);
        if (hit)
        {
            this.Move(-this.m_vel);
            this.m_dir = (this.m_dir + 1) % 4;
        }

        // 後続も前進
        for (int i = kFRN - 1; i > 0; --i)
        {
            this.m_frs[i].transform.position = this.m_frs[i - 1].transform.position;
        }
        if (kFRN > 0)
        {
            this.m_frs[0].transform.position = this.transform.position;
        }
    }

    public bool CheckHit(float x, float y, float w, float h)
    {
        // 先頭
        {
            Vector2 centerPos = this.transform.position;
            Vector2 size = this.m_size;
            Vector2 pos = new Vector2(centerPos.x - size.x / 2, centerPos.y - size.y / 2);

            if (pos.x + size.x / 2 >= x &&
                pos.x <= x + w &&
                pos.y + size.y / 2 >= y &&
                pos.y <= y + h)
            {
                return true;
            }
        }

            // 後続
            for (int i = 0; i < kFRN; ++i)
        {
            Vector2 centerPos = this.m_frs[i].transform.position;
            Vector2 size = this.m_size;
            Vector2 pos = new Vector2(centerPos.x - size.x / 2, centerPos.y - size.y / 2);

            if (pos.x + size.x / 2 >= x     &&
                pos.x              <= x + w &&
                pos.y + size.y / 2 >= y     &&
                pos.y              <= y + h )
            {
                return true;
            }
        }

        return false;
    }
}
