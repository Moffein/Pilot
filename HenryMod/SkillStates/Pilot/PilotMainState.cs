using EntityStates.MoffeinPilot.FireSelect;
using MoffeinPilot.Modules.Survivors;
using RoR2.Skills;
using RoR2.UI;
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
            if (base.skillLocator == null)
                return;
            if (base.skillLocator.primary == null)
                return;

            bool showPistol = base.skillLocator.primary.baseSkill == PilotSurvivor.SkillDefs.Primaries.Silencer;

            ChildLocator childLocator = base.GetModelChildLocator();
            if (!childLocator) return;

            Transform pistol = childLocator.FindChild("PilotPistol");
            Transform defaultGun  = childLocator.FindChild("PilotWeapon");

            if (pistol) pistol.gameObject.SetActive(showPistol);
            if (defaultGun) defaultGun.gameObject.SetActive(!showPistol);
        }
    }
}
