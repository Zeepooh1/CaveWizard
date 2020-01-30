using System;
using System.Collections.Generic;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Globals;
using CaveWizard.Levels;
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
    public class Player : MagicCreature, IUpdateable {
        public bool Enabled { get; set; }
        public int UpdateOrder { get; set;}
        private readonly float _baseVelocity;
        private bool _inAir;
        public bool InAir => _inAir;
        private float _x, _y;

        private bool _isMoving;
        private Vector2 _playerMovingForce;
        private IInteractable _nearInteractable;

        public Player(ScreenManager screenManager, string propName, Vector2 pos, World world, int columns, int rows, Level sourceLevel) : base(screenManager, propName, new Vector2(0.75f, 0.25f),  new Vector2(0.75f, 0.75f), columns, rows, sourceLevel)
        {
            _x = 0;
            _y = 0;
            _baseVelocity = 0.05f;
            Health = 100;
            
            ObjectBody = world.CreateCapsule(_objectBodySize.X, 0.25f, 10, _objectBodySize.Y , 10, 1f, pos);
            ObjectBody.BodyType = BodyType.Dynamic;
            ObjectBody.SetRestitution(0f);
            ObjectBody.Mass = 2f;
            ObjectBody.FixedRotation = true;
            ObjectBody.SetFriction(1.0f);
            ObjectBody.OnSeparation += Seperation;
            ObjectBody.Tag = "Player";

            AnimationTimeSpans = new Dictionary<CreatureState, TimeSpan>();
            AnimationTimeSpans[CreatureState.Moving] = TimeSpan.FromMilliseconds(200);
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
            CreatureState prevState = CreatureState;
            _playerMovingForce = new Vector2(0, 0);
            CreatureState = CreatureState.Idle;
            if (inputHelper.KeyboardState.IsKeyDown(KeyBinds.PlayerMoveLeft))
            {
                _playerMovingForce += new Vector2(x: -_baseVelocity, 0);
                _creatureEffect = SpriteEffects.None | SpriteEffects.FlipVertically;
                CreatureState = CreatureState.Moving;
            }

            if (inputHelper.KeyboardState.IsKeyDown(KeyBinds.PlayerMoveRight)) {
                _playerMovingForce += new Vector2(x: _baseVelocity, 0);
                _creatureEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                CreatureState = CreatureState.Moving;
            }

            if (inputHelper.IsNewKeyPress(KeyBinds.PlayerJump) && !_inAir)
            {
                ObjectBody.ApplyLinearImpulse(new Vector2(0f, 1f));
                CreatureState = CreatureState.Jump;
                if (GameSettings._Volume)
                {
                    SoundEffects.PlayerjumpSoundEffects.Play();
                }
            }

            if (inputHelper.IsNewKeyPress(KeyBinds.PlayerInteract))
            {
                if (_nearInteractable != null)
                {
                    _nearInteractable.Interact(gameTime);
                }
            }

            if (_inAir)
            {
                CreatureState = CreatureState.Jump;
            }

            if (prevState == CreatureState.Attack)
            {
                CreatureState = CreatureState.Attack;
            }
            
        }
        

        public void Update(GameTime gameTime)
        {
            _currentFrame++;
            if (_gotHit)
            {
                _gotHit = false;
                Health -= 25;
                if (Health <= 0)
                {
                    GlobalDevices._GameState = GameState.MAINMENU;
                }
            }
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

            if (CreatureState == CreatureState.Moving)
            {
                _currRow = 0;
                //9 in 10
                _currColumn = ((int)(gameTime.TotalGameTime.Milliseconds / 150f) % 2 ) + 0; 
            }
            else if (CreatureState == CreatureState.Idle)
            {
                _currRow = 0;
                _currColumn = ((gameTime.TotalGameTime.Milliseconds / 500) % 2) + 0;
            }
            else if (CreatureState == CreatureState.Jump)
            {
                _currRow = 1;
                _currColumn = 5;
            }
            else if (CreatureState == CreatureState.Attack)
            {
                if (AttackTimeStamp == TimeSpan.Zero)
                {
                    AttackTimeStamp = gameTime.TotalGameTime;
                    _currColumn = 0;
                    _currRow = 2;
                }
                else
                {
                    if (gameTime.TotalGameTime.Subtract(AttackTimeStamp).Milliseconds > 200)
                    {
                        CreatureState = CreatureState.Idle;
                        AttackTimeStamp = TimeSpan.Zero;
                    }
                }
            }
            
        }
        
        public void HandleCursor(InputHelper input, World world, Camera2D camera)
        {
            if (input.IsNewMouseButtonPress(KeyBinds.PlayerShoot))
            {
                _magicMissiles.Add(new MagicMissile(_screenManager, world, ObjectBody.Position, camera.ConvertScreenToWorld(input.Cursor), this, 1, 1, (Level)SourceLevel));
                AttackTimeStamp = TimeSpan.Zero;
                CreatureState = CreatureState.Attack;
            }
            if (input.IsNewMouseButtonRelease(KeyBinds.PlayerShoot))
            {
                _currColumn = 0;
                _currRow = 0;
            }
        }

        public void setNewInteractable(IInteractable interactable)
        {
            _nearInteractable = interactable;
        }

        public void GotHit()
        {
            _gotHit = true;
        }
    }
}