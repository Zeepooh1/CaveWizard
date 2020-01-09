using System;
using CaveEngine.ScreenSystem;
using CaveWizard.Globals;
using CaveWizard.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.Game
{
    public class MagicMissile : TexturedWorldObject, IUpdateable
    {
        private Vector2 directionVector;
        private Vector2 _originalPos;
        private World _world;
        private ICanSpawnMissiles _parentObject;
        
        public MagicMissile(ScreenManager screenManager, World world, Vector2 pos, Vector2 cursorPos, ICanSpawnMissiles parentObject, int columns, int rows, Level sourceLevel) : base(screenManager, "Textures/magicMissile",
            new Vector2(0.05f), new Vector2(0.25f), columns, rows, sourceLevel)
        {
            _world = world;
            _parentObject = parentObject;
            directionVector = cursorPos - pos;
            _originalPos = pos;
            directionVector.Normalize();
            CircleShape shape = new CircleShape(0.05f, 20f);
            ObjectBody = world.CreateBody();
            ObjectBody.BodyType = BodyType.Dynamic;
            ObjectBody.IsBullet = true;
            ObjectBody.Position = pos + (directionVector / 2);
            ObjectBody.SetCollisionCategories(Category.Cat1);
            ObjectBody.IgnoreGravity = true;
            ObjectBody.Mass = 0.01f;
            
            
            Fixture fixture = ObjectBody.CreateFixture(shape);
            fixture.Restitution = 0.05f;
            if (GameSettings._Volume)
            {
                SoundEffects.MissileOut.Play();
            }

            ObjectBody.LinearVelocity = directionVector * 10f;
            ObjectBody.OnCollision += Collided;

            ObjectBody.Tag = "MagicMissile";
            foreach (var fixture1 in ObjectBody.FixtureList)
            {
                fixture1.Tag = "MagicMissile";
            }
        }

        private bool Collided(Fixture sender, Fixture other, Contact contact)
        {
            if (!other.IsSensor)
            {
                
                _world.RemoveAsync(ObjectBody);
                _parentObject.RemoveMissiles(this);
            }

            return true;
        }


        public void Update(GameTime gameTime)
        {
            if (Vector2.Distance(ObjectBody.Position, _originalPos) > 5f)
            {
                
                _parentObject.AddToRemoveLater(this);
            }
        }

        public bool Enabled { get; }
        public int UpdateOrder { get; }
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public void Draw()
        {
            _screenManager.SpriteBatch.Draw(Texture2D, ObjectBody.Position, null, Color.White, ObjectBody.Rotation,
                _textureHalf, _objectTextureMetersSize / (_objectTextureSize), SpriteEffects.None, 0f);

        }
    }
}