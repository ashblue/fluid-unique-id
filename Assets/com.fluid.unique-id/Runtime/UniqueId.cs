using System;
using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueId : MonoBehaviour, IUniqueId {
        [HideInInspector]
        [SerializeField]
        protected string _id = null;

        public virtual string Id => _id;

        public void PopulateIdIfEmpty () {
            if (_id != null) return;
            _id = Guid.NewGuid().ToString();
        }
    }
}
