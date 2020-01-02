using System;
using System.Collections.Generic;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Content;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.Game {
    public class Player : WorldObject, IDrawable, IUpdateable, ICanSpawnMissiles {
        public int DrawOrder { get; set; }
        public bool Visible { get; set;}
        public bool Enabled { get; set; }
        public int UpdateOrder { get; set;}
        private readonly float _baseVelocity;
        private bool _inAir;
        private List<MagicMissile> _magicMissiles;
        private List<MagicMissile> _magicMissilesToRemove;
        public bool InAir => _inAir;
        private float _x, _y;
        private SoundEffect _jumpSoundEffects;

        private SpriteEffects _playerEffect = SpriteEffects.FlipVertically;
        private bool _isMoving;
        private Vector2 _playerMovingForce;

        public Player(ScreenManager screenManager, string propName, Vector2 pos, World world, int columns, int rows) : base(screenManager, propName, new Vector2(0.75f, 0.25f),  new Vector2(0.75f, 0.75f), columns, rows)
        {
            _magicMissiles = new List<MagicMissile>();
            _magicMissilesToRemove = new List<MagicMissile>();
            _x = 0;
            _y = 0;
            _baseVelocity = 0.05f;
            ObjectBody = world.CreateCapsule(_objectBodySize.X, 0.25f, 10, _objectBodySize.Y , 10, 1f, pos);
            ObjectBody.BodyType = BodyType.Dynamic;
            ObjectBody.SetRestitution(0f);
            ObjectBody.Mass = 2f;
            ObjectBody.FixedRotation = true;
            ObjectBody.SetFriction(1.0f);
            ObjectBody.OnSeparation += Seperation;
         //   ObjectBody.OnCollision += Colided;
            ObjectBody.Tag = "Player";
            foreach (var fixture1 in ObjectBody.FixtureList)
            {
                fixture1.Tag = "Player";
            }
            ObjectBody.SetCollisionCategories((Category) (Category.All - Category.Cat1));
            
            //create bottom rectangle for floor detection
            Vertices vertices = PolygonTools.CreateRectangle(0.1f, 0.1f);
            vertices.Translate(new Vector2(0f, -0.4f));
            Shape rectangle = new PolygonShape(vertices, 1f);
            Fixture fixture = ObjectBody.CreateFixture(rectangle);
            fixture.Tag = "AirDetectorBottom";
            fixture.IsSensor = true;
            fixture.OnSeparation = fixtureSeperated;
            fixture.OnCollision += Colided;
            //set sound effect
            _jumpSoundEffects = screenManager.Content.Load<SoundEffect>("Sounds/jump");
            
//            PlayerBody.CreateRectangle(0.2f, 0.2f, 1f, new Vector2(0f, -0.8f)).IsSensor = true;
            _inAir = true;
            _isMoving = false;
            _playerMovingForce = new Vector2(0, 0);
            

        }


        public event EventHandler<EventArgs> DrawOrderChanged;

        public event EventHandler<EventArgs> VisibleChanged;

        public event EventHandler<EventArgs> EnabledChanged;

        public event EventHandler<EventArgs> UpdateOrderChanged;

        private void fixtureSeperated(Fixture sender, Fixture other, Contact contact)
        {
            var contactList = sender.Body.ContactList;
            _inAir = true;
            while (contactList.Next != null)
            {
                if (contactList.Contact.IsTouching)
                {
                    if(!"DetectionCone".Equals(contactList.Contact.FixtureB.Tag))
                        _inAir = false;
                }

                contactList = contactList.Next;
            }

        }

        private bool Colided(Fixture sender, Fixture other, Contact contact)
        {
            _inAir = false;
            FixedArray2<Vector2> points;
            Vector2 normal;
            contact.GetWorldManifold(out normal, out points);
            return true;
            
        }

        private void Seperation(Fixture sender, Fixture other, Contact contact)
        {
//            if (PlayerBody.ContactList.Next == null)
//            {
//                _inAir = true;
//            }
//            
        }

        public void HandleInput(InputHelper inputHelper, GameTime gameTime)
        {
            _playerMovingForce = new Vector2(0, 0);
            if (inputHelper.KeyboardState.IsKeyDown(KeyBinds.PlayerMoveLeft))
            {
                _playerMovingForce += new Vector2(x: -_baseVelocity, 0);
                _playerEffect = SpriteEffects.None | SpriteEffects.FlipVertically;
            }

            if (inputHelper.KeyboardState.IsKeyDown(KeyBinds.PlayerMoveRight)) {
                _playerMovingForce += new Vector2(x: _baseVelocity, 0);
                _playerEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
            }


            if (inputHelper.IsNewKeyPress(KeyBinds.PlayerJump) && !_inAir)
            {
                ObjectBody.ApplyLinearImpulse(new Vector2(0f, 1f));
                if (GameSettings._Volume)
                {
                    _jumpSoundEffects.Play();
                }
            }
            
        }
        

        public void Update(GameTime gameTime)
        {
            _currentFrame++;
            if (!_playerMovingForce.Equals(Vector2.Zero))
            {
                ObjectBody.Position += _playerMovingForce;
            }

            _magicMissilesToRemove.Clear();
            foreach (var magicMissile in _magicMissiles)
            {
                magicMissile.Update(gameTime);
            }

            foreach (var magicMissile in _magicMissilesToRemove)
            {
                magicMissile.ObjectBody.World.Remove(magicMissile.ObjectBody);
                _magicMissiles.Remove(magicMissile);
            }            
        }

        public void Draw(GameTime gameTime)
        {
            int width =  Texture2D.Width / _columns; 
            int height = Texture2D.Height / _rows;
            
            Rectangle sourceRectangle = new Rectangle(width * _currColumn, height * _currRow, width, height);
            
            _screenManager.SpriteBatch.Draw(Texture2D, ObjectBody.Position, sourceRectangle, Color.White, ObjectBody.Rotation,
                  _textureHalf, _objectTextureMetersSize / (_objectTextureSize / new Vector2(_columns, _rows)), _playerEffect, 0f);
        }


        public void HandleCursor(InputHelper input, World world, Camera2D camera)
        {
            if (input.IsNewMouseButtonPress(KeyBinds.PlayerShoot))
            {
                _magicMissiles.Add(new MagicMissile(_screenManager, world, ObjectBody.Position, camera.ConvertScreenToWorld(input.Cursor), null,
                    new Vector2(0.05f), new Vector2(0.05f), this, 1, 1));

                _currColumn = 5;
                _currRow = 1;
            }
            if (input.IsNewMouseButtonRelease(KeyBinds.PlayerShoot))
            {
                _currColumn = 0;
                _currRow = 0;
            }
        }

        public void RemoveMissiles(WorldObject missile)
        {
            _magicMissiles.Remove((MagicMissile)missile);
        }

        public void AddToRemoveLater(WorldObject missile)
        {
          _magicMissilesToRemove.Add((MagicMissile)missile);   
        }
    }
}