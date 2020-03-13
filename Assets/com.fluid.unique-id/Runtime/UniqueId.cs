using UnityEngine;

namespace CleverCrow.Fluid.UniqueIds {
    public class UniqueId : MonoBehaviour, IUniqueId {
        [HideInInspector]
        [SerializeField]
        private string _id = null;

        public string Id => _id;
    }
}
