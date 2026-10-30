using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

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

    [StaticConstructorOnStartup]
    public class FlippableBuilding : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            //this.flippedGraphicData = new GraphicData();
            //this.flippedGraphicData.CopyFrom(this.def.graphicData);
            //this.flippedGraphicData.drawSize.Rotated();
            ////this.flippedGraphicData.drawSize = new Vector2(-0.5f, 1);

            //Material mat = this.MatAt(thing.Rotation, thing);
        }

        public override void Print(SectionLayer layer)
        {
            if (isFlipped)
            {
                //this.flippedGraphicData.Graphic.Print(layer, this, 0f);
                Material mat = Graphic.MatAt(this.Rotation, this);
                Vector2[] uvs;
                Color32 color;
                Graphic.TryGetTextureAtlasReplacementInfo(mat, this.def.category.ToAtlasGroup(), true, true, out mat, out uvs, out color);

                Printer_Plane.PrintPlane(layer, this.TrueCenter(), Graphic.drawSize, mat, 0, true, uvs, new Color32[]{color,color,color,color}, 0.01f, 0f);
                Graphic_Shadow shadowGraphic = Graphic.ShadowGraphic;
                if (shadowGraphic == null)
                {
                    return;
                }
                shadowGraphic.Print(layer, this, 0f);
                return;
            }
            this.Graphic.Print(layer, this, 0f);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
                yield return gizmo;

            yield return new Command_Action
            {
                defaultLabel = "Flip Graphic",
                defaultDesc = "Flip Graphic of this building",
                icon = ContentFinder<Texture2D>.Get("UI/Commands/SC_FlipGraphic"),
                action = delegate ()
                {
                    this.isFlipped = !isFlipped;


                    this.DirtyMapMesh(this.Map);
                }
            };
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.isFlipped, "isFlipped", false, false);
        }
        private bool isFlipped;
        //private GraphicData flippedGraphicData;
    }
}
