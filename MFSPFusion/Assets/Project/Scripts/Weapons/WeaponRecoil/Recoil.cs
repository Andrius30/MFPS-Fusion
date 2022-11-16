using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Recoil : MonoBehaviour
{
    public static Action<BaseGun> onRecoil;

    public float snappiness;
    public float returnSpeed;

    Vector3 currentRotation;
    Vector3 targetRotation;

    public void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void SetRecoil(BaseGun gun)
    {
        targetRotation += new Vector3(gun.data.recoilX, Random.Range(gun.data.recoilY, -gun.data.recoilY), Random.Range(gun.data.recoilZ, -gun.data.recoilZ));
    }

    void OnEnable()
    {
        onRecoil += SetRecoil;
    }
    void OnDisable()
    {
        onRecoil -= SetRecoil;
        
    }
}
