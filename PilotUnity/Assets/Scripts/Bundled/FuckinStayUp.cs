using UnityEngine;

namespace MoffeinPilot.Content.Components {
    [ExecuteAlways]
    public class FuckinStayUp : MonoBehaviour {

        [SerializeField]
        private Transform tran;

        [SerializeField]
        private Transform baseTran;

        [SerializeField]
        private float funny;

        void LateUpdate() {
            tran.position = baseTran.position + Vector3.up * funny;
            Vector3 forward = baseTran.forward;
            forward.y = 0;
            tran.rotation = Quaternion.LookRotation(forward);
        }
    }
}
