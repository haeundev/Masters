using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis axis = Axis.Y;
    public float speed = 1f;

    private void Update()
    {
        switch (axis)
        {
            case Axis.X:
                transform.Rotate(speed * Time.deltaTime, 0, 0);
                break;
            case Axis.Y:
                transform.Rotate(0, speed * Time.deltaTime, 0);
                break;
            case Axis.Z:
                transform.Rotate(0, 0, speed * Time.deltaTime);
                break;
        }
    }
}