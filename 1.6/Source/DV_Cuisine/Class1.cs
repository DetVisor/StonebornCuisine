using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DV_Cuisine
{
    public class CompProperties_OnDeathEffects : CompProperties
    {
        public EffecterDef effecter;
        public ThingDef spawnThing;
        public int spawnAmount = 10;
        public bool multiplyByBodySize = true;

        public CompProperties_OnDeathEffects()
        {
            compClass = typeof(Comp_OnDeathEffects);
        }
    }
    public class Comp_OnDeathEffects : ThingComp
    {
        public CompProperties_OnDeathEffects Props => (CompProperties_OnDeathEffects)props;

        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            base.Notify_Killed(prevMap, dinfo);
            if (!parent.Spawned && !((Pawn)parent).Corpse.Spawned) return;
            IntVec3 cell = ((Pawn)parent).Corpse.Position;
            Map map = ((Pawn)parent).Corpse.Map;
            float size = ((Pawn)parent).BodySize;
            Props.effecter?.Spawn(cell, map);
            if (Props.spawnThing != null && Props.spawnAmount > 0)
            {
                Thing thing = ThingMaker.MakeThing(Props.spawnThing);
                thing.stackCount = Props.multiplyByBodySize ? (int)(Props.spawnAmount * size) : Props.spawnAmount;
                GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Near);
            }
        }
    }
}
