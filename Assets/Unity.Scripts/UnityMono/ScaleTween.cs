using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    public Vector3 ToScale = Vector3.one;
    public float Duration = 1f;
    private float StartTime { get; set; }
    private Vector3 StartScale { get; set; }


    // Start is called before the first frame update
    void Awake()
    {
        StartScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        var elapsedTime = Time.time - StartTime;
        if (elapsedTime > Duration)
        {
            //gameObject.SetActive(false);
            return;
        }
        var progress = elapsedTime / Duration;
        var addScale = (ToScale - StartScale) * progress;
        transform.localScale = StartScale + addScale;
    }
}
