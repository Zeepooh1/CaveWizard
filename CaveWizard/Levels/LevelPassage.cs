using System;
using CaveEngine.ScreenSystem;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CaveWizard.Levels
{
    public class LevelPassage : GameScreen
    {
        private TimeSpan _timeSinceEnvoked;
        public LevelPassage(TimeSpan timeSinceEnvoked)
        {
            _timeSinceEnvoked = timeSinceEnvoked;
            
            SoundEffects.NextLevelStomp.Play();
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (gameTime.TotalGameTime.Subtract(_timeSinceEnvoked).TotalMilliseconds >
                SoundEffects.PlayerjumpSoundEffects.Duration.TotalMilliseconds + 1000f)
            {
                ExitScreen();
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}