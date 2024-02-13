using UnityEngine;

namespace Pilot.Content.Components.ProjectileGhost
{
    public class RotationVisuals : MonoBehaviour
    {
        Transform cross, arrow1, arrow2;

        private static float spinSpeed = 180f;
        private Vector3 rotVector;

        private void Awake()
        {
            rotVector = Vector3.up * spinSpeed;
            ChildLocator cl = base.GetComponent<ChildLocator>();
            if (cl)
            {
                cross = cl.FindChild("Crosshair");
                arrow1 = cl.FindChild("Arrow1");
                arrow2 = cl.FindChild("Arrow2");
            }

            if (!cross && !arrow1 && !arrow2)
            {
                Destroy(this);
            }
        }


        private void Update()
        {
            if (cross)
            {
                cross.transform.Rotate(rotVector * Time.deltaTime, Space.Self);
            }
            if (arrow1)
            {
                arrow1.transform.Rotate(-rotVector * Time.deltaTime, Space.Self);
            }
            if (arrow2)
            {
                arrow2.transform.Rotate(rotVector * Time.deltaTime, Space.Self);
            }
        }
    }
}
