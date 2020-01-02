using System;
using CaveEngine.ScreenSystem;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.Game
{
    public class MagicMissile : WorldObject, IUpdateable
    {
        private Vector2 directionVector;
        private Vector2 _originalPos;
        private World _world;
        private ICanSpawnMissiles _parentObject;
        private SoundEffect _missileSoundEffect;
        
        public MagicMissile(ScreenManager screenManager, World world, Vector2 pos, Vector2 cursorPos, string propName, Vector2 objectBodySize, Vector2 objectTextureMetersSize, ICanSpawnMissiles parentObject, int columns, int rows) : base(screenManager, propName, objectBodySize, objectTextureMetersSize, columns, rows)
        {
            _world = world;
            _parentObject = parentObject;
            directionVector = cursorPos - pos;
            _originalPos = pos;
            directionVector.Normalize();
            CircleShape shape = new CircleShape(objectBodySize.X, 20f);
            ObjectBody = world.CreateBody();
            ObjectBody.BodyType = BodyType.Dynamic;
            ObjectBody.IsBullet = true;
            ObjectBody.Position = pos + (directionVector / 2);
            ObjectBody.SetCollisionCategories(Category.Cat1);
            ObjectBody.IgnoreGravity = true;

            Fixture fixture = ObjectBody.CreateFixture(shape);
            fixture.Restitution = 0.05f;
            _missileSoundEffect = screenManager.Content.Load<SoundEffect>("Sounds/magic_missile");
            if (GameSettings._Volume)
            {
                _missileSoundEffect.Play();
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
            if (!"DetectionCone".Equals(other.Tag))
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
    }
}