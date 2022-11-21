using Andrius.Core.Utils;
using Fusion;
using UnityEngine;

public class Launcher : NetworkBehaviour
{
    [SerializeField] Transform handTransform;
    [SerializeField] Camera cam;
    [SerializeField] GameObject granadePrefab;
    [SerializeField] GameObject cursor;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float maxDistance;
    [SerializeField] float time = 2f;
    RaycastHit hit = default;
    Vector3 vo = Vector3.zero;

    public void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            cursor.SetActive(true);
            hit = StaticFunctions.GetHitInfo(cam, maxDistance, groundMask);
            cursor.transform.position = hit.point + Vector3.up * 0.1f;
            vo = StaticFunctions.CalculateVelocity(hit.point, handTransform.position, time);
            handTransform.rotation = Quaternion.LookRotation(vo);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            cursor.SetActive(false);
            Attack(vo);
        }
    }

    public void Attack(Vector3 velocity)
    {
        var obj = Instantiate(granadePrefab, handTransform.position, Quaternion.identity);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = velocity;
    }
}
