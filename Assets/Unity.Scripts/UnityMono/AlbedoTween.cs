using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbedoTween : MonoBehaviour
{
    public Renderer Target;
    public Color ToValue = Color.white;
    public float Delay = 0f;
    public float Duration = 1f;
    private float StartTime { get; set; }
    private Color StartValue { get; set; }


    // Start is called before the first frame update
    void Awake()
    {
        StartValue = Target.material.color;
    }

    private void OnEnable()
    {
        StartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        var elapsedTime = Time.time - StartTime - Delay;
        if (elapsedTime > Duration)
        {
            //gameObject.SetActive(false);
            return;
        }
        var progress = elapsedTime / Duration;
        var addScale = (ToValue - StartValue) * progress;
        Target.material.color = StartValue + addScale;
    }
}
