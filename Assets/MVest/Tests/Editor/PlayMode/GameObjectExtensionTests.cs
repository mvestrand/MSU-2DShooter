using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using MVest;

namespace MVest.Tests {


    public class GameObjectExtensionTests {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NameHierarchy_RootObject() {
            string obj1Name = "RootObj";
            GameObject obj = new GameObject(obj1Name);
            Assert.AreEqual(obj1Name, obj.HierarchyName());
            yield break;
        }

        [UnityTest]
        public IEnumerator NameHierarchy_SubObjects() {
            string obj1Name = "RootObj";
            string obj2Name = "SubObj1";
            string obj3Name = "SubObj2";
            GameObject obj1 = new GameObject(obj1Name);
            GameObject obj2 = new GameObject(obj2Name);
            GameObject obj3 = new GameObject(obj3Name);
            obj2.transform.parent = obj1.transform;
            obj3.transform.parent = obj2.transform;

            Assert.AreEqual(obj1Name + "/" + obj2Name + "/" + obj3Name, obj3.HierarchyName());
            yield break;
        }

        [UnityTest]
        public IEnumerator NameHierarchy_NullObject() {
            GameObject obj1 = null;
            Assert.AreEqual("null", obj1.HierarchyName());
            yield break;
        }

        [UnityTest]
        public IEnumerator NameHierarchy_DestroyNull() {
            // Create an object hierarchy
            string obj1Name = "RootObj";
            string obj2Name = "SubObj1";
            string obj3Name = "SubObj2";
            GameObject obj1 = new GameObject(obj1Name);
            GameObject obj2 = new GameObject(obj2Name);
            GameObject obj3 = new GameObject(obj3Name);
            obj2.transform.parent = obj1.transform;
            obj3.transform.parent = obj2.transform;

            // Destroy the root
            GameObject.Destroy(obj1);

            // Wait for end of frame
            yield return null;

            // The sub object should have been destroyed with the root
            Assert.AreEqual("null", obj3.HierarchyName());
        }


        [UnityTest]
        public IEnumerator FullName_RootObject() {
            string obj1Name = "RootObj";
            GameObject obj = new GameObject(obj1Name);
            Assert.AreEqual(obj.FullName(), obj.HierarchyName());
            yield break;
        }

        [UnityTest]
        public IEnumerator FullName_SubObjects() {
            string obj1Name = "RootObj";
            string obj2Name = "SubObj1";
            string obj3Name = "SubObj2";
            GameObject obj1 = new GameObject(obj1Name);
            GameObject obj2 = new GameObject(obj2Name);
            GameObject obj3 = new GameObject(obj3Name);
            obj2.transform.parent = obj1.transform;
            obj3.transform.parent = obj2.transform;

            Assert.AreEqual(obj3.FullName(), obj3.HierarchyName());
            yield break;
        }

        [UnityTest]
        public IEnumerator FullName_NullObject() {
            GameObject obj1 = null;
            Assert.AreEqual("null", obj1.FullName());
            yield break;
        }

        [UnityTest]
        public IEnumerator FullName_Component() {
            // Create an object hierarchy
            string obj1Name = "RootObj";
            string obj2Name = "SubObj1";
            string obj3Name = "SubObj2";
            GameObject obj1 = new GameObject(obj1Name);
            GameObject obj2 = new GameObject(obj2Name);
            GameObject obj3 = new GameObject(obj3Name);
            obj2.transform.parent = obj1.transform;
            obj3.transform.parent = obj2.transform;
            Animator comp = obj3.AddComponent<Animator>();

            // The sub object should have been destroyed with the root
            Assert.AreEqual(obj3.HierarchyName() + ":" + nameof(Animator), comp.FullName());

            yield break;
        }


    }
}
