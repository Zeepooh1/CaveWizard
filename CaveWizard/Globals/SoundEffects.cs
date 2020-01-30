using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace CaveWizard.Globals
{
    public static class SoundEffects
    {
        public static SoundEffect MissileOut;
        public static SoundEffect PlayerjumpSoundEffects;
        public static SoundEffect HitSoundEffect;
        public static Song GameSong;
        public static SoundEffect NextLevelStomp;
        public static SoundEffect OpenDoor;

        public static void LoadSoundEffects(ContentManager manager)
        {
            MissileOut = manager.Load<SoundEffect>("Sounds/magic_missile");
            PlayerjumpSoundEffects = manager.Load<SoundEffect>("Sounds/jump");
            HitSoundEffect = manager.Load<SoundEffect>("Sounds/Hit");
            GameSong = manager.Load<Song>("Sounds/chamber_of_secrets");
            NextLevelStomp = manager.Load<SoundEffect>("Sounds/stomp");
            OpenDoor = manager.Load<SoundEffect>("Sounds/door_open");
        }
    }
}