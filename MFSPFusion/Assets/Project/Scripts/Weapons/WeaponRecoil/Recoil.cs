using Fusion;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Recoil : NetworkBehaviour
{
    public static Action<BaseGun> onRecoil;

    public float snappiness;
    public float returnSpeed;
    public float evaluateTime = 2f;


    [SerializeField] AnimationCurve animationCurve;
    Vector3 currentRotation;
    Vector3 targetRotation;

    BaseGun gun;

    public override void FixedUpdateNetwork()
    {
        if (gun == null) return;
        if (Object.HasInputAuthority && gun.gunID == Object.InputAuthority.PlayerId)
        {
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
    }
    public void SetRecoil(BaseGun gun)
    {
        if (Object.HasInputAuthority && gun.gunID == Object.InputAuthority.PlayerId)
        {
            this.gun = gun;
            float animCurve = -animationCurve.Evaluate(evaluateTime);
            targetRotation += new Vector3(animCurve, Random.Range(gun.data.recoilY, -gun.data.recoilY), Random.Range(gun.data.recoilZ, -gun.data.recoilZ));
        }
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
