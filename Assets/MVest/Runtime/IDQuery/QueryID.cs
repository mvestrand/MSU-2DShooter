using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace MVest {

    public class QueryID : ScriptableObject {
        [SerializeField] private SerializableType _dataType; // !! FIX !! Replace with a serializable type class
        public Type DataType { get { return _dataType; } }
    }

    public interface IQueryable {
        TValue Get<TValue>(QueryID id, TValue defaultValue=default);
        bool TryGet<TValue>(QueryID id, out TValue value);
        bool Has(QueryID id);
    }

    public abstract class QueryIDBinding_Base {
        protected static HashSet<QueryIDBinding_Base> bindings;
        protected static void RegisterBinding(QueryIDBinding_Base binding) {
            if (bindings == null)
                bindings = new HashSet<QueryIDBinding_Base>();
            bindings.Add(binding);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void LoadStaticBindings() {
            foreach (var binding in bindings) {
                binding.Clear();
                binding.LoadBindingAsset();
            }
        }

        public abstract void LoadBindingAsset();
        protected abstract void Clear();
    }
    public class Binding<TOwner> : Dictionary<string, (Func<TOwner, object>, Type)> {}

    public class QueryIDBinding<TOwner> : QueryIDBinding_Base {

        protected string bindingAssetPath;


        private Binding<TOwner> stringBindings;
        private Dictionary<QueryID, (Func<TOwner, object>, string)> idBindings;

        public QueryIDBinding(string bindingAssetPath, Binding<TOwner> binding) {
            this.bindingAssetPath = bindingAssetPath;
            stringBindings = (binding != null ? binding : new Binding<TOwner>());
            idBindings = new Dictionary<QueryID, (Func<TOwner, object>, string)>();
            RegisterBinding(this);
        }

        public void Bind(string stringBinding, QueryID id, QueryIDBindingAsset context) {
            // Ignore ids bound to nothing and bindings with no id
            if (string.IsNullOrWhiteSpace(stringBinding) || id == null)
                return;

            // Get the callback and check that the string binding exists
            if (!stringBindings.TryGetValue(stringBinding, out var callback)) {
                Debug.LogWarning($"Failed to bind id [{id.FullName()}] in {context.FullName()}: binding string \"{stringBinding}\" not found");
                return;
            }

            // Check for incompatible data types
            if (id.DataType == null || callback.Item2 == null || !id.DataType.IsAssignableFrom(callback.Item2)) {
                string bindingTypename = (callback.Item2 != null ? callback.Item2.FullName : "null");
                string idTypename = (id.DataType != null ? id.DataType.FullName : "null");
                Debug.LogWarning($"Failed to bind id [{id.FullName()}] in {context.FullName()} to \"{stringBinding}\": cannot assign type {bindingTypename} to type {idTypename}");
                return;
            }

            // Check for duplicate id uses
            if (idBindings.ContainsKey(id)) {
                Debug.LogWarning($"Failed to bind id [{id.FullName()}] in {context.FullName()} to \"{stringBinding}\": id is already bound to \"{idBindings[id].Item2}\"");
                return;
            }

            // Assign the id to the corresponding callback
            idBindings[id] = (callback.Item1, stringBinding);
        }

        protected override void Clear() {
            idBindings.Clear();
        }

        public override void LoadBindingAsset() {
            throw new NotImplementedException();
        }

        public TValue Get<TValue>(QueryID id, TOwner owner, TValue defaultValue) {
            if (id != null && owner != null && idBindings.TryGetValue(id, out var callback)) {
                var obj = callback.Item1(owner);
                if (typeof(TValue).IsAssignableFrom(obj.GetType())) {
                    return (TValue)obj;
                } else {
                    TypeConversionError<TValue>(id, owner, obj);
                }
            }
            return defaultValue;
        }

        public bool TryGet<TValue>(QueryID id, TOwner owner, out TValue value) {
            if (id != null && owner != null && idBindings.TryGetValue(id, out var callback)) {
                var obj = callback.Item1(owner);
                if (typeof(TValue).IsAssignableFrom(obj.GetType())) {
                    value = (TValue)obj;
                    return true;
                } else {
                    TypeConversionError<TValue>(id, owner, obj);
                }
            }
            value = default;
            return false;
        }

        void TypeConversionError<TValue>(QueryID id, TOwner owner, object obj) {
            string ownerName;
            if (owner is UnityEngine.Object) {
                ownerName = (owner as UnityEngine.Object).FullName();
            } else {
                ownerName = typeof(TOwner).FullName;
            }
            Debug.LogError($"Error querying id [{id.DebugName()}] from {ownerName}. Cannot convert from <{obj.GetType().FullName}> to <{typeof(TValue).FullName}>");
        }

    }

}

