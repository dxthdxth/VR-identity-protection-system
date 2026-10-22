using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos +
            new Vector3(0, Mathf.Sin(Time.time) * 0.05f, 0);
    }
}