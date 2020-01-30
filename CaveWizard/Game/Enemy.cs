using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using CaveEngine.DrawingSystem;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Globals;
using CaveWizard.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.Game
{
    public enum Orientation
    {
        Left,
        Right
    }
    public class Enemy : MagicCreature, IUpdateable
    {
        private bool _playerSeen;
        private Player _player;
        private TimeSpan _timeSinceLastMissile;
        private bool _canShoot;
        private Orientation _orientation;
        private Fixture _leftCone;
        private Fixture _rightCone;
        private Texture2D _healthTexture;
        private Rectangle _healthSourceRectangle;
        private Color _healthBarColor;

        public Enemy(ScreenManager screenManager, Player player, string propName,Vector2 pos, World world, Vector2 objectBodySize, Vector2 objectTextureMetersSize, int columns, int rows, Level sourceLevel) : base(screenManager, propName, objectBodySize, objectTextureMetersSize, columns, rows, sourceLevel)
        {
            ObjectBody = world.CreateCapsule(_objectBodySize.X, _objectBodySize.Y/3f, 10, _objectBodySize.Y/3f , 10, 1f, pos);
            ObjectBody.BodyType = BodyType.Dynamic;
            ObjectBody.SetRestitution(0f);
            ObjectBody.Mass = 2f;
            ObjectBody.FixedRotation = true;
            ObjectBody.SetFriction(0.5f);
            ObjectBody.Tag = "Enemy";


            Health = 100;
            _playerSeen = false;
            _gotHit = false;
            _canShoot = true;
            _player = player;
            _timeSinceLastMissile = TimeSpan.Zero;
            _orientation = (Orientation)RandomNumberGenerator.GetInt32(2);
            if(_orientation == Orientation.Left)
            {
                _creatureEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;

            }
            _leftCone = createVision(Orientation.Left);
            _rightCone = createVision(Orientation.Right);
            foreach (var fixture in ObjectBody.FixtureList)
            {
                if (!"DetectionCone".Equals(fixture.Tag))
                {
                    fixture.Tag = "Enemy";
                }
            }

            _healthBarColor = Color.Green;
            makeHealthBar();
        
            ObjectBody.OnCollision += OnCollision;
        }

        private Fixture createVision(Orientation orientation)
        {
            Vertices vertices = new Vertices();
            if (orientation == Orientation.Right)
            {
                vertices.Add(new Vector2(0f, 0.75f));
                vertices.Add(new Vector2(0f, -0.75f));
                vertices.Add(new Vector2(5f, 0.75f));
                vertices.Add(new Vector2(5f, -0.75f));
            }
            else
            {
                vertices.Add(new Vector2(0f, 0.75f));
                vertices.Add(new Vector2(0f, -0.75f));
                vertices.Add(new Vector2(-5f, 0.75f));
                vertices.Add(new Vector2(-5f, -0.75f));
            }

            Shape shape = new PolygonShape(vertices, 0f);
            Fixture detectionCone = ObjectBody.CreateFixture(shape);
            detectionCone.Tag = "DetectionCone";
            detectionCone.IsSensor = true;
            detectionCone.OnCollision += PlayerSpoted;
            detectionCone.OnSeparation += PlayerOutOfSight;
            return detectionCone;

        }

        private void PlayerOutOfSight(Fixture sender, Fixture other, Contact contact)
        {
            if ("DetectionCone".Equals(sender.Tag) && "Player".Equals(other.Tag))
            {
                _playerSeen = false;
            }
        }

        private void makeHealthBar()
        {
            if (Health > 0)
            {
                
                if (_healthTexture == null)
                {
                    Vertices verts = new Vertices();
                    verts.Add(new Vector2(0.5f * (Health / 100f), 0.125f));
                    verts.Add(new Vector2(0.5f * (Health / 100f), 0f));
                    verts.Add(new Vector2(0f, 0.125f));
                    verts.Add(new Vector2(0f, 0f));

                    Shape shape = new PolygonShape(verts, 0f);
                    _healthTexture =
                        _screenManager.Assets.TextureFromShape(shape, MaterialType.Squares, Color.White, 0.001f);
                   
                    _healthSourceRectangle = new Rectangle(0, 0, _healthTexture.Width * (Health / 100), _healthTexture.Height);
                }
                else
                {
                    _healthSourceRectangle = new Rectangle(0, 0, (int)(_healthTexture.Width * (Health / 100f)), _healthTexture.Height);
                }
            }
        }

        private bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            if ("Enemy".Equals(sender.Tag) && "MagicMissile".Equals(other.Tag))
            {
                _gotHit = true;
                if (GameSettings._Volume)
                {
                    SoundEffects.HitSoundEffect.Play();
                }

                Vector2 fromWhere = _player.ObjectBody.Position -  ObjectBody.Position;
                fromWhere = Vector2.Normalize(fromWhere);
                if (fromWhere.X > 0 && _orientation == Orientation.Left)
                {
                    _orientation = Orientation.Right;
                    _creatureEffect = SpriteEffects.FlipVertically;
                    PlayerSpoted(_rightCone, _player.ObjectBody.FixtureList.Find(p => "Player".Equals(p.Tag)), contact);
                }
                else if (fromWhere.X < 0 && _orientation == Orientation.Right)
                {
                    _orientation = Orientation.Left;
                    _creatureEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                    PlayerSpoted(_leftCone, _player.ObjectBody.FixtureList.Find(p => "Player".Equals(p.Tag)), contact);

                }

            }
            return true;
            
        }

        private bool PlayerSpoted(Fixture sender, Fixture other, Contact contact)
        {
            Vector2 fromWhere = _player.ObjectBody.Position -  ObjectBody.Position;
            fromWhere = Vector2.Normalize(fromWhere);

            if ("DetectionCone".Equals(sender.Tag) && "Player".Equals(other.Tag))
            {
                if (fromWhere.X < 0 && _orientation == Orientation.Left)
                {
                    _playerSeen = true;
                }
                else if (fromWhere.X > 0 && _orientation == Orientation.Right)
                {
                    _playerSeen = true;
                }
            }

            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (_gotHit)
            {
                Health -= 25;
                makeHealthBar();
                _gotHit = false;
                if (Health <= 50)
                {
                    _healthBarColor = Color.Yellow;
                }

                if (Health <= 25)
                {
                    _healthBarColor = Color.Red;
                }
            }
            if (Health <= 0)
            {
                ((Level)SourceLevel).EnemyDiedEvent.Invoke(this);
            }
            if (_playerSeen)
            {
                if (_timeSinceLastMissile.Equals(TimeSpan.Zero))
                {
                    CreatureState = CreatureState.Moving;
                }

                if (_canShoot)
                {
                    _timeSinceLastMissile = gameTime.TotalGameTime;
                    CreatureState = CreatureState.Attack;
                    _canShoot = false;
                }
                else
                {
                    
                    if (gameTime.TotalGameTime.Subtract(_timeSinceLastMissile).CompareTo(new TimeSpan(0, 0, 3)) > 0)
                    {
                        CreatureState = CreatureState.Moving;
                        _canShoot = true;
                        _timeSinceLastMissile = TimeSpan.Zero;
                    }
                }


            }
            else
            {
                CreatureState = CreatureState.Idle;
            }

            foreach (var magicMissile in _magicMissiles)
            {
                magicMissile.Update(gameTime);
            }

            if (CreatureState == CreatureState.Moving)
            {
                _currRow = 2;
                _currColumn = (int) (gameTime.TotalGameTime.TotalMilliseconds / 300f) % 10;       
                if (_currColumn != 0 && _currColumn != 5)
                {
                    Vector2 playerDir = _player.ObjectBody.Position - ObjectBody.Position;
                    playerDir.Normalize();
                    ObjectBody.Position += new Vector2(playerDir.X, 0) * 0.01f;
                }
            }
            else if (CreatureState == CreatureState.Idle)
            {
                _currRow = 0;
                _currColumn = (int) (gameTime.TotalGameTime.TotalMilliseconds / 200f) % 10;
            }
            else if (CreatureState == CreatureState.Attack)
            {
                _currRow = 3;
                int prevColumn = _currColumn;
                _currColumn = (int) (gameTime.TotalGameTime.Subtract(_timeSinceLastMissile).Milliseconds / 100f) % 10;
                if (_currColumn == 9 && prevColumn != 9)
                {
                    _magicMissiles.Add(new MagicMissile(_screenManager, ObjectBody.World, ObjectBody.Position, _player.ObjectBody.Position,  this, 1, 1, (Level)SourceLevel));
                    CreatureState = CreatureState.Moving;
                }
            }
            
            
        }

        public override void Draw(GameTime gameTime)
        {

            if (Health > 0)
            {
                

                _screenManager.SpriteBatch.Draw(_healthTexture, ObjectBody.Position + new Vector2(-0.65f, 0.75f), _healthSourceRectangle,
                    _healthBarColor, 0f, new Vector2(0.0f, 0.0f), 0.1f, SpriteEffects.None, 0f);
            }

            base.Draw(gameTime);
        }


        public bool Enabled { get; }
        public int UpdateOrder { get; }
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
       
        
        public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}