using MoffeinPilot.Modules.Survivors;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.MoffeinPilot
{
    public class PilotMainState : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();

            SetWeaponModel();
        }

        private void SetWeaponModel()
        {
            bool hidePistol = !(base.skillLocator && base.skillLocator.primary && base.skillLocator.primary.baseSkill == PilotSurvivor.SkillDefs.Primaries.Silencer);

            ChildLocator childLocator = base.GetModelChildLocator();
            if (!childLocator) return;

            Transform pistol = childLocator.FindChild("PilotPistol");
            Transform defaultGun  = childLocator.FindChild("PilotWeapon");

            if (hidePistol)
            {
                if (pistol) pistol.gameObject.SetActive(false);
                if (defaultGun) defaultGun.gameObject.SetActive(true);
            }
            else
            {
                if (pistol) pistol.gameObject.SetActive(true);
                if (defaultGun) defaultGun.gameObject.SetActive(false);
            }
        }
    }
}
