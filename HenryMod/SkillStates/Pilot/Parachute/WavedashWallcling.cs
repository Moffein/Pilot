using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class WavedashWallcling : Wavedash
    {
        public static float delayBeforeRecling = 0.5f;
        public static float minReclingAngle = 45f;

        public Vector3 clingStartDirection;
        private bool canRecling = false;
        private float reclingStopwatch = 0f;
    }
}
