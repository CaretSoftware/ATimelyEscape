using System;
using System.Reflection;
using UnityEngine;

public static class ExtensionMethods {
    public static Vector3 ToVector3(this Vector2 vector2) => new Vector3(vector2.x, 0.0f, vector2.y);

    public static Vector2 ToVector2(this Vector3 vector3) => new Vector2(vector3.x, vector3.z);

    public static Vector3 ProjectOnPlane(this Vector3 vector3) => Vector3.ProjectOnPlane(vector3, Vector3.up);

    // Usage: var copy = myComp.GetCopyOf(someOtherComponent);
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                             BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos) {
            if (pinfo.CanWrite) {
                try {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                } catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }

        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos) {
            finfo.SetValue(comp, finfo.GetValue(other));
        }

        return comp as T;
    }

    // Usage: Health myHealth = gameObject.AddComponent<Health>(enemy.health);
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
}