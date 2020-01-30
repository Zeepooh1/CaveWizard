using CaveEngine.ScreenSystem;
using CaveEngine.WorldSystem;
using CaveWizard.Globals;
using CaveWizard.Levels;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.Game
{
    public class FinishDoor: TexturedWorldObject, IInteractable
    {
        public bool PlayerNearDoor;
        public FinishDoor(ScreenManager screenManager,World world, Vector2 pos, string propName, Vector2 objectBodySize, Vector2 objectTextureMetersSize, int columns, int rows, Level sourceLevel) : base(screenManager, propName, objectBodySize, objectTextureMetersSize, columns, rows, sourceLevel)
        {
            ObjectBody = world.CreateBody(pos);
            ObjectBody.CreateRectangle(_objectBodySize.X, _objectBodySize.Y, 0f, Vector2.Zero);
            ObjectBody.SetIsSensor(true);
            ObjectBody.OnCollision += SomethingNearDoor;
            ObjectBody.OnSeparation += SomethingAwayFromDoor;
            PlayerNearDoor = false;
        }

        private void SomethingAwayFromDoor(Fixture sender, Fixture other, Contact contact)
        {
            if ("Player".Equals(other.Tag))
            {
                ((Level)SourceLevel).Player.setNewInteractable(null);
            }

        }

        private bool SomethingNearDoor(Fixture sender, Fixture other, Contact contact)
        {
            if ("Player".Equals(other.Tag))
            {

                ((Level)SourceLevel).Player.setNewInteractable(this);
            }
            return true;
        }

        public void Interact(GameTime gameTime)
        {
            _currColumn = 1;
            SoundEffects.OpenDoor.Play();
            ((Level)SourceLevel).NewLevel(gameTime);
        }
    }
}