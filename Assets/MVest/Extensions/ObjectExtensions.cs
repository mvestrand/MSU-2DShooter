using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {


    public static class ObjectExtensions {
        public static string GetExtendedName(this UnityEngine.Object obj) {
            if (obj is GameObject) {
                return ((GameObject)obj).HierarchyName();
            } else if (obj is MonoBehaviour) {
                return ((MonoBehaviour)obj).gameObject.HierarchyName() + obj.GetType().Name;
            } else if (obj == null) {
                return "null";
            } else {
                return obj.name;
            }
        }

    }

}
