using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Pilot.Content.Components
{
    [RequireComponent(typeof(SkillLocator))]
    public class PilotController : MonoBehaviour
    {
        private SkillLocator skillLocator;
        public bool isParachuting;

        public void Awake()
        {
            skillLocator = base.GetComponent<SkillLocator>();
            isParachuting = false;
        }

        public void ConsumeSecondaryStock(int amount)
        {
            if (skillLocator && skillLocator.secondary)
            {
                skillLocator.secondary.DeductStock(amount);
            }
        }
    }
}
