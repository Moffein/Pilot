using RoR2;
using RoR2.Skills;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Pilot.Content.Components
{
    [RequireComponent(typeof(SkillLocator), typeof(CharacterBody))]
    public class PilotController : MonoBehaviour
    {
        private CharacterBody characterBody;
        private SkillLocator skillLocator;
        public bool isParachuting;
        private HurtBox autoAimHurtbox;

        //Used for Secondary
        public static GameObject autoAimIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMissileTrackingIndicator.prefab").WaitForCompletion();
        public static float autoAimDistance = 200f;
        public static float autoAimAngle = 30f;

        private readonly BullseyeSearch search = new BullseyeSearch();
        private int currentAutoAimStates;    //Keeps track of whether or not AutoAim is in use
        private int autoAimSearchFrequency = 20;
        private Indicator enemyIndicator;

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            skillLocator = base.GetComponent<SkillLocator>();
            isParachuting = false;
            currentAutoAimStates = 0;
            enemyIndicator = new Indicator(base.gameObject, autoAimIndicatorPrefab);
        }

        private void FixedUpdate()
        {
            if (currentAutoAimStates > 0)
            {
                UpdateAutoAim();
                UpdateIndicator();
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
    }
}
