using UnityEngine;

public class Pistol : BaseGun
{



    public override void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log($"Pressed to shoot");
            if (data.shootType == ShootType.Raycast)
            {
                if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider != null)
                    {
                        #region Debuging
                        Debug.Log($"Collider hit");
                        GameObject gm = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), hit.point, Quaternion.identity);
                        Destroy(gm.GetComponent<Collider>());
                        gm.transform.localScale = new Vector3(.3f, .3f, .3f);
                        Destroy(gm, 3f);
                        #endregion
                    }
                }
            }
        }
    }

    public override void Reload()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(raycastOrigin.position, (raycastOrigin.position + raycastOrigin.forward) * Mathf.Infinity);
    }
}
