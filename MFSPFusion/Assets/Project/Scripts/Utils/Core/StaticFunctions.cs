using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrius.Core.Utils
{
    /// <summary>
    /// This class holds global accessable helper methods
    /// </summary>
    public static class StaticFunctions
    {
        public static Transform FindChild(Transform parent, string childName, bool includeInactive = true)
        {
            foreach (var ch in parent.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (parent.name != ch.name)
                {
                    if (ch.childCount > 0)
                    {
                        foreach (var c in ch.GetComponentsInChildren<Transform>(includeInactive))
                        {
                            if (c.name == childName)
                            {
                                return c;
                            }
                        }
                    }
                    if (ch.name == childName)
                    {
                        return ch;
                    }
                }
            }
            return null;
        }
        public static T FindTypeInChildrens<T>(Transform parent, string name = "", bool includeInactive = true) where T : Component
        {
            if (!string.IsNullOrEmpty(name))
            {
                var obj = FindChild(parent, name);
                if(obj == null)
                {
                    return null;
                }
                T t = obj.GetComponent<T>();
                if (t != null)
                {
                    return t;
                }
            }
            foreach (var ch in parent.GetComponentsInChildren<T>(includeInactive))
            {
                if (parent.name != ch.transform.name)
                {
                    if (ch is T)
                    {
                        return ch;
                    }
                }
            }
            return null;
        }
        public static List<T> GetAllComponentsInChildren<T>(Transform parent, bool includeInactive = true)
        {
            List<T> components = new List<T>();
            foreach (var item in parent.GetComponentsInChildren<T>(includeInactive))
            {
                if (item != null)
                {
                    components.Add(item);
                }
            }
            return components;
        }
        
        
        // ============= MEMORY MANAGEMENT =========================

        public static T LoadFromResourcesAndCreateObject<T>(string pathToModel) where T : Component
        {
            var t = Resources.Load<GameObject>(pathToModel);
            GameObject gm = MonoBehaviour.Instantiate(t);
            T comp = gm.GetComponent<T>();
            return comp;
        }
        
     public static IEnumerator PredictedMovement2D(float velocity, float angle)
    {
        float t = 0;
        while (t < 100)
        {
            float x = velocity * t * Mathf.Cos(angle);
            float y = velocity * t * Mathf.Cos(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
            //transform.position = new Vector3(x, y, 0);
            t += Time.deltaTime;
            yield return null;
        }
    }
    public static Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        Vector3 distance = target - origin; // direction
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float sy = distance.y;
        float sxz = distanceXZ.magnitude; // distance
        float vxz = sxz / time;
        float vy = sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= vxz;
        result.y = vy;

        return result;

    }
    }
}
