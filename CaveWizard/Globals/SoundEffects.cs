using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace CaveWizard.Globals
{
    public static class SoundEffects
    {
        public static SoundEffect MissileOut;
        public static SoundEffect PlayerjumpSoundEffects;
        public static SoundEffect HitSoundEffect;

        public static void LoadSoundEffects(ContentManager manager)
        {
            MissileOut = manager.Load<SoundEffect>("Sounds/magic_missile");
            PlayerjumpSoundEffects = manager.Load<SoundEffect>("Sounds/jump");
            HitSoundEffect = manager.Load<SoundEffect>("Sounds/Hit");
        }
    }
}