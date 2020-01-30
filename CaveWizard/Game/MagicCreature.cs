using System.Collections.Generic;
using CaveEngine.ScreenSystem;
using CaveEngine.WorldSystem;
using CaveWizard.Levels;
using Microsoft.Xna.Framework;

namespace CaveWizard.Game
{
    public class MagicCreature: Creature, ICanSpawnMissiles
    {
        protected List<MagicMissile> _magicMissiles;
        protected List<MagicMissile> _magicMissilesToRemove;
        public MagicCreature(ScreenManager screenManager, string propName, Vector2 objectBodySize, Vector2 objectTextureMetersSize, int columns, int rows, Level sourceLevel) : base(screenManager, propName, objectBodySize, objectTextureMetersSize, columns, rows, sourceLevel)
        {
            _magicMissiles = new List<MagicMissile>();
            _magicMissilesToRemove = new List<MagicMissile>();

        }
        
        public void RemoveMissiles(WorldObject missile)
        {
            _magicMissiles.Remove((MagicMissile)missile);
        }

        public void AddToRemoveLater(WorldObject missile)
        {
            _magicMissilesToRemove.Add((MagicMissile)missile);   
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var magicMissile in _magicMissiles)
            {
                magicMissile.Draw();
            }
            base.Draw(gameTime);
        }
    }
}