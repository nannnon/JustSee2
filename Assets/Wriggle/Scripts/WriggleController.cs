using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WriggleController : MonoBehaviour
{
    [SerializeField]
    GameObject _linePrefab;
    
    CameraController _cameraController;
    Vector3 _position = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 prePos = _position;
        const float dLen = 0.002f;
        _position = prePos + new Vector3(Random.Range(-dLen, dLen), Random.Range(-dLen, dLen), Random.Range(-dLen, dLen));

        _cameraController.LookAtPos = _position;

        {
            GameObject go = Instantiate(_linePrefab);
            LineRenderer lr = go.GetComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.SetPosition(0, prePos);
            lr.SetPosition(1, _position);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            lr.startColor = color;
            lr.endColor = color;
            float widht = 0.001f;
            lr.startWidth = widht;
            lr.endWidth = widht;
        }

        Thread.Sleep(100);
    }
}
