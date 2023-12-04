using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace MoffeinPilot.Content.Components
{
    [RequireComponent(typeof(SkillLocator), typeof(CharacterBody))]
    public class PilotController : MonoBehaviour
    {
        private CharacterBody characterBody;
        private SkillLocator skillLocator;
        public bool isParachuting;
        public bool isWavedashing;
        private HurtBox autoAimHurtbox;
        private Queue<GameObject> activeAirStrikes;

        //Used for Secondary
        public static GameObject autoAimIndicatorPrefab = Modules.Assets.EngiMissileIndicatorButRed;
        public static float autoAimDistance = 200f;
        public static float autoAimAngle = 30f;
        private static float autoAimUpdateTimer = 1f / 20f;

        private readonly BullseyeSearch search = new BullseyeSearch();
        private int currentAutoAimStates;    //Keeps track of whether or not AutoAim is in use
        private Indicator enemyIndicator;

        private float autoAimStopwatch;

        private void Awake()
        {
            activeAirStrikes = new Queue<GameObject>();
            characterBody = base.GetComponent<CharacterBody>();
            skillLocator = base.GetComponent<SkillLocator>();
            isParachuting = false;
            isWavedashing = false;
            currentAutoAimStates = 0;
            autoAimStopwatch = 0f;
            enemyIndicator = new Indicator(base.gameObject, autoAimIndicatorPrefab);
        }

        private void FixedUpdate()
        {
            if (currentAutoAimStates > 0)
            {
                autoAimStopwatch += Time.fixedDeltaTime;
                if (autoAimStopwatch >= PilotController.autoAimUpdateTimer)
                {
                    autoAimStopwatch -= PilotController.autoAimUpdateTimer;

                    //These get called during FireTargetAcquire to ensure maximum accuracy.
                    UpdateAutoAim();
                    UpdateIndicator();
                }
            }
            else
            {
                enemyIndicator.active = false;
            }
        }

        private void OnDestroy()
        {
            if (enemyIndicator != null)
            {
                enemyIndicator.active = false;
            }
        }

        public void ConsumeSecondaryStock(int amount)
        {
            if (skillLocator && skillLocator.secondary)
            {
                skillLocator.secondary.DeductStock(amount);
            }
        }

        public HurtBox GetAutoaimHurtbox()
        {
            return autoAimHurtbox;
        }

        public void BeginAutoAim()
        {
            currentAutoAimStates++;
        }

        public void EndAutoAim()
        {
            currentAutoAimStates--;
            if (currentAutoAimStates < 0) currentAutoAimStates = 0;
        }

        public void UpdateAutoAim()
        {
            if (!characterBody.inputBank) return;
            Ray aimRay = characterBody.inputBank.GetAimRay();

            this.search.teamMaskFilter = TeamMask.GetEnemyTeams(characterBody.teamComponent ? characterBody.teamComponent.teamIndex : TeamIndex.None);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = PilotController.autoAimDistance;
            this.search.maxAngleFilter = PilotController.autoAimAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);

            autoAimHurtbox = this.search.GetResults().FirstOrDefault<HurtBox>();
        }

        public void UpdateIndicator()
        {
            if (!autoAimIndicatorPrefab) return;

            Transform targetTransform = (autoAimHurtbox ? autoAimHurtbox.transform : null);
            this.enemyIndicator.targetTransform = targetTransform;
            enemyIndicator.active = targetTransform != null;
        }

        public void RegisterAirstrike(GameObject gameObject)
        {
            if (!NetworkServer.active) return;

            int maxAirStrikes = 2;
            if (skillLocator) maxAirStrikes = Mathf.Max(2, skillLocator.special.maxStock);

            if (activeAirStrikes.Count >= maxAirStrikes)
            {
                Destroy(activeAirStrikes.Dequeue());
            }
            activeAirStrikes.Enqueue(gameObject);
        }
    }
}
