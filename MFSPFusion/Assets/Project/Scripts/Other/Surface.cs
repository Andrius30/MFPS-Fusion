using UnityEngine;

public enum SurfaceType
{
    NONE,
    Concrete,
    Dirt,
    Metal,
    Sand,
    SoftBody,
    Wood,
    Blood
}
public class Surface : MonoBehaviour
{
    public SurfaceType surfaceType;
}
