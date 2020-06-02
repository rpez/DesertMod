using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace DesertMod.Sounds.Boss
{
    public class DesertBossMusic : ModSound
    {
        public override SoundEffectInstance PlaySound(
          ref SoundEffectInstance soundInstance,
          float volume,
          float pan,
          SoundType type)
        {
            soundInstance = this.sound.CreateInstance();
            soundInstance.Volume = volume * 1f;
            soundInstance.Pan = pan;
            Main.PlaySoundInstance(soundInstance);
            return soundInstance;
        }
    }
}