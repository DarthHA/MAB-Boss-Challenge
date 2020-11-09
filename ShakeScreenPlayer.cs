using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge
{

    public class ShakeScreenPlayer : ModPlayer
    {

        public override void ResetEffects()
        {
            this.shake = false;
            this.shakeMega = false;
            this.shakeSubtle = false;
            this.shakeQuake = false;
        }


        public override void UpdateDead()
        {
            this.shake = false;
            this.shakeMega = false;
            this.shakeSubtle = false;
            this.shakeQuake = false;
        }

        public override void ModifyScreenPosition()
        {
            if (MABBossChallenge.mabconfig.BossFightFilters)
            {
                if (this.shake)
                {
                    Main.screenPosition.X += Main.rand.Next(-10, 11);
                    Main.screenPosition.Y += Main.rand.Next(-10, 11);
                }
                if (this.shakeMega)
                {
                    Main.screenPosition.X += Main.rand.Next(-20, 21);
                    Main.screenPosition.Y += Main.rand.Next(-20, 21);
                }
                if (this.shakeSubtle)
                {
                    Main.screenPosition.X += Main.rand.Next(-3, 3);
                    Main.screenPosition.Y += Main.rand.Next(-3, 3);
                }
                if (this.shakeQuake)
                {
                    Main.screenPosition.Y += Main.rand.Next(-5, 5);
                }
            }
        }


        public bool shake;

        public bool shakeMega;

        public bool shakeSubtle;


        public bool shakeQuake;
    }
}