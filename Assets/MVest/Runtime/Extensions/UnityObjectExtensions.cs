using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVest {


    public static class UnityObjectExtensions {
        public static string FullName(this UnityEngine.Object obj) {
            if (obj is GameObject) {
                return ((GameObject)obj).HierarchyName();
            } else if (obj is Component) {
                return ((Component)obj).gameObject.HierarchyName() + ":" + obj.GetType().Name;
            } else if (obj == null) {
                return "null";
            } else {
                return obj.name;
            }
        }

        public static string DebugName(this UnityEngine.Object obj) {
            if (obj != null) {
                return obj.FullName() + "[" + obj.GetInstanceID() + "]";
            } else {
                return "null";
            }
        }

    }

}
