//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vintagestory.API.Client;
//using Vintagestory.API.Common;
//using Vintagestory.API.MathTools;
//using Vintagestory.Client.NoObf;
//using VintageStoryHarmony;

//namespace WereWolf.assets.Werewolf.Shaders
//{
//    internal class WolfVisionRenderer : IRenderer
//    {
//        private ICoreClientAPI capi;
//        private bool nightVisionActive;

//        public WolfVisionRenderer(ICoreClientAPI capi)
//        {
//            this.capi = capi;
//        }

//        public bool Toggle(KeyCombination comb)
//        {
//            nightVisionActive = !nightVisionActive;
//            capi.Logger.Notification($"WolfVision toggled: {nightVisionActive}");
//            return true;
//        }

//        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
//        {
//            if (!nightVisionActive) return;

//            if (stage == EnumRenderStage.AfterPostProcessing)
//            {
//                // Draw a semi-transparent green rectangle over the whole screen
//                float width = capi.Render.FrameWidth;
//                float height = capi.Render.FrameHeight;

//                // color format: 0xAARRGGBB
//                int greenOverlay = 0x4033FF33; // semi-transparent green
//                capi.Render.RenderRectangle(0, 0, 0, width, height, greenOverlay);
//            }
//        }

//        public double RenderOrder => 0.85;
//        public int RenderRange => 0;

//        public void Dispose() { }
//    }
//}