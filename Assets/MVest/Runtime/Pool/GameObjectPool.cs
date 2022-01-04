using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

using Sirenix.OdinInspector;

namespace MVest {

    public class GameObjectPool : MonoBehaviour{

        private int _requestedCount = 0;

        private int _inactiveCount = 0;
        private int _activeCount = 0;
        private PooledMonoBehaviour _original;
        private PooledMonoBehaviour _nextInactive = null;

        public int CountInactive { get { return _inactiveCount; } }
        public int CountActive { get { return _activeCount; } }
        public int CountAll { get { return _activeCount + _inactiveCount; } }
        public PooledMonoBehaviour Original { get { return _original; } }
        internal void SetOriginal(PooledMonoBehaviour original) {
            _original = original;
        }
        internal void SetOptions(bool collectionChecks) {
            _collectionChecks = collectionChecks;
        }

        private bool _collectionChecks;
        public bool CollectionChecks { get { return _collectionChecks; } }

        public PooledMonoBehaviour Get() {
            var obj = GetPooled();

            // Reset the transform depending on what we were passed
            obj.transform.parent = this.transform;
            obj.transform.SetPositionAndRotation(_original.transform.position, _original.transform.rotation);
            obj.transform.localScale = _original.transform.localScale;

            ResetAndActivateObject(obj);
            return obj;
        }

        public void Release(PooledMonoBehaviour obj) {
            AddToPool(obj);
        }

        public void Clear() {
            PooledMonoBehaviour next = _nextInactive;
            while (next != null) {
                PooledMonoBehaviour curr = next;
                next = curr.nextPooledObject;
                DestroyItem(curr);
            }
            _inactiveCount = 0;
            _nextInactive = null;
        }

        public void RequestPreallocate(int count) {
            if (count <= 0)
                return;
            _requestedCount += count;
        }

        public void CancelPreallocate(int count) {
            if (count <= 0)
                return;
            _requestedCount -= count;
        }

        public PooledMonoBehaviour Get(Transform parent, bool instantiateInWorldSpace=false) {
            var obj = GetPooled();

            // Reset the transform depending on what we were passed
            obj.transform.parent = parent;
            if (instantiateInWorldSpace) {
                obj.transform.SetPositionAndRotation(_original.transform.position, _original.transform.rotation);
            } else {
                obj.transform.localPosition = _original.transform.position;
                obj.transform.localRotation = _original.transform.rotation;
            }
            obj.transform.localScale = _original.transform.localScale;

            ResetAndActivateObject(obj);
            return obj;
        }

        public PooledMonoBehaviour Get(Vector3 position, Quaternion rotation) {
            var obj = GetPooled();

            // Reset the transform depending on what we were passed
            obj.transform.parent = this.transform;
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.transform.localScale = _original.transform.localScale;

            ResetAndActivateObject(obj);
            return obj;
        }

        public PooledMonoBehaviour Get(Vector3 position, Quaternion rotation, Transform parent) {
            var obj = GetPooled();

            // Reset the transform depending on what we were passed
            obj.transform.parent = parent;
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.transform.localScale = _original.transform.localScale;

            ResetAndActivateObject(obj);
            return obj;
        }


        public void RemoveFromPool(PooledMonoBehaviour item) {

            // Check if the object is in the inactive object pool
            if (item.prevPooledObject != null || item.nextPooledObject != null || item == _nextInactive) {
                
                // Update neighbor references
                if (item.prevPooledObject != null)
                    item.prevPooledObject.nextPooledObject = item.nextPooledObject;
                if (item.nextPooledObject != null)
                    item.nextPooledObject.prevPooledObject = item.prevPooledObject;
                if (item == _nextInactive)
                    _nextInactive = item.nextPooledObject;

                _inactiveCount--;
                if (_collectionChecks)
                    CheckOutOfPool(item);
            } else if (item._pool == this) { // This is an active pool object
                _activeCount--;
                item._pool = null;
            }
        }

        private PooledMonoBehaviour CreateNewInactiveObject() {
            // Instantiate a new inactive object
            var obj = Instantiate<PooledMonoBehaviour>(_original, this.inactiveObjectTransform);
            obj.name = System.String.Format("{0} #{1:D4}", _original.name, GetNextId());
            obj._pool = this;
            obj._SetOriginal(_original);

            // Add the object to the inactive list
            AddToPool(obj);
            return obj;
        }

        private void DestroyItem(PooledMonoBehaviour item) {
            if (item == null)
                return;
            RemoveFromPool(item);

            Destroy(item);
        }


        private void AddToPool(PooledMonoBehaviour obj) {
            obj.gameObject.SetActive(false);
            obj.transform.parent = inactiveObjectTransform;
            obj.nextPooledObject = _nextInactive;
            _nextInactive = obj;
            _inactiveCount++;
        }

        private PooledMonoBehaviour TakeFromPool() {
            var obj = _nextInactive;
            _nextInactive = obj.nextPooledObject;
            obj.nextPooledObject = null;
            _inactiveCount--;
            if (_collectionChecks) 
                CheckOutOfPool(obj);
            return obj;
        }

        private PooledMonoBehaviour GetPooled() {
            if (_nextInactive == null)
                CreateNewInactiveObject();
            return TakeFromPool();
        }

        private void ResetAndActivateObject(PooledMonoBehaviour obj) {
            _activeCount++;
            obj.gameObject.SetActive(true);
            obj.Restart();
        }

        private int _nextId = 0;
        private int GetNextId() {
            return _nextId++;
        }

        
        internal Transform inactiveObjectTransform;
        internal void CreateInactiveObjectHolder() {
            GameObject inactiveObject = new GameObject("inactive");
            inactiveObject.SetActive(false);
            inactiveObjectTransform = inactiveObject.transform;
            inactiveObjectTransform.parent = this.transform;
        }


        #region Collection Checking
        private Dictionary<PooledMonoBehaviour, string> releaseObjectCalls = new Dictionary<PooledMonoBehaviour, string>();

        private void CheckOutOfPool(PooledMonoBehaviour obj) {
            releaseObjectCalls.Remove(obj);
        }
        internal void CheckIntoPool(PooledMonoBehaviour obj, string stackTrace) {
            if (releaseObjectCalls.TryGetValue(obj, out var oldStackTrace)) {
                Debug.LogErrorFormat("Duplicate Release() calls! \nOld call: {0}, \nNew call: {1}", oldStackTrace, stackTrace);
            } else {
                releaseObjectCalls.Add(obj, stackTrace);
            }
        }
        #endregion

        protected void Update() {
            if (Math.Min(_original.MaxPoolSize, Math.Max(_requestedCount, _original.MinPoolSize)) > CountAll) {
                //Debug.LogFormat("Creating: {0}", _original.name);
                CreateNewInactiveObject();
            }
        }

        //internal ObjectPool<GameObject> pool;

    }

}







