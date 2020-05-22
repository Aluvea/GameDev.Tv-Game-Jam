using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(UnityEngine.UI.Extensions.UILineRenderer))]
[ExecuteAlways]
public class UICircle : MonoBehaviour
{
    public int segments;
    public float radius;
    [SerializeField]
    UnityEngine.UI.Extensions.UILineRenderer line;
    [Range(-360.0f,360.0f)]
    [SerializeField] float startingDegree = 0.0f;
    [Range(0.0f,360.0f)]
    [SerializeField] float circlePortion = 360.0f;
    void Start()
    {
        line = gameObject.GetComponent<UnityEngine.UI.Extensions.UILineRenderer>();
        UpdateCircleSize(radius);
    }

    private void Update()
    {
        UpdateCircleSize(radius);
    }

    /// <summary>
    /// The circle's color
    /// </summary>
    public Color CircleColor
    {
        get {
            return line.color;
        }
    }

    /// <summary>
    /// The circle radius in pixels
    /// </summary>
    public float CircleRadius
    {
        get
        {
            return radius;
        }
    }

    public void UpdateCircleColor(Color color)
    {
        line.color = color;
    }

    public void UpdateCircleSize(float radius)
    {
        this.radius = radius;
        float x;
        float y;

        float angle = startingDegree;
        float endingDegree = startingDegree + circlePortion;
        Vector2 []circlePoints = new Vector2[segments + 1];
        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            circlePoints[i] = new Vector2(x, y);
            angle += (endingDegree / segments);
        }
        

        line.Points = circlePoints;
    }
}
