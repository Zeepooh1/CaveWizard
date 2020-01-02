﻿using System;
using System.Collections.Generic;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.Game
{
    public class Enemy : WorldObject, IUpdateable, IDrawable, ICanSpawnMissiles
    {
        private bool _playerSeen;
        private Player _player;
        private TimeSpan _timeSinceLastMissile;
        private bool _canShoot;
        private SoundEffect _hitSoundEffect;
        private List<MagicMissile> _magicMissiles;
        private List<MagicMissile> _magicMissilesToRemove;
        public Enemy(ScreenManager screenManager, Player player, string propName,Vector2 pos, World world, Vector2 objectBodySize, Vector2 objectTextureMetersSize, int columns, int rows) : base(screenManager, propName, objectBodySize, objectTextureMetersSize, columns, rows)
        {
            ObjectBody = world.CreateCapsule(_objectBodySize.X, _objectBodySize.Y/2f, 10, _objectBodySize.Y/2f , 10, 1f, pos);
            ObjectBody.BodyType = BodyType.Dynamic;
            ObjectBody.SetRestitution(0f);
            ObjectBody.Mass = 2f;
            ObjectBody.FixedRotation = true;
            ObjectBody.SetFriction(0.5f);
            _playerSeen = false;
            _canShoot = true;
            _player = player;
            _timeSinceLastMissile = TimeSpan.Zero;
            _magicMissiles = new List<MagicMissile>();
            _magicMissilesToRemove = new List<MagicMissile>();
            Vertices vertices = new Vertices();
            vertices.Add(new Vector2(0f, 0.25f));
            vertices.Add(new Vector2(5f, 2.25f));
            vertices.Add(new Vector2(5f, -1.75f));
            Shape shape = new PolygonShape(vertices, 0f);
            Fixture detectionCone = ObjectBody.CreateFixture(shape);
            detectionCone.Tag = "DetectionCone";
            detectionCone.IsSensor = true;
            detectionCone.OnCollision += PlayerSpoted;
            detectionCone.OnSeparation += PlayerOutOfSight;
            _hitSoundEffect = screenManager.Content.Load<SoundEffect>("Sounds/Hit");
            ObjectBody.Tag = "Enemy";
            foreach (var fixture in ObjectBody.FixtureList)
            {
                if (!"DetectionCone".Equals(fixture.Tag))
                {
                    fixture.Tag = "Enemy";
                }
            }

            ObjectBody.OnCollision += OnCollision;
        }

        private void PlayerOutOfSight(Fixture sender, Fixture other, Contact contact)
        {
            if ("DetectionCone".Equals(sender.Tag) && "Player".Equals(other.Tag))
            {
                _playerSeen = false;
            }
        }

        private bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            if ("Enemy".Equals(sender.Tag) && "MagicMissile".Equals(other.Tag))
            {
                if (GameSettings._Volume)
                {
                    _hitSoundEffect.Play();
                }
            }
            return true;
            
        }

        private bool PlayerSpoted(Fixture sender, Fixture other, Contact contact)
        {
            if ("DetectionCone".Equals(sender.Tag) && "Player".Equals(other.Tag))
            {
                _playerSeen = true;
            }
         /*   else
            {
                _playerSeen = false;
            }*/
            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (_playerSeen)
            {
                if (_canShoot)
                {
                    _magicMissiles.Add(new MagicMissile(_screenManager, ObjectBody.World, ObjectBody.Position, _player.ObjectBody.Position, null,
                        new Vector2(0.05f), new Vector2(0.05f), this, 1, 1));
                    _timeSinceLastMissile = gameTime.TotalGameTime;
                    _canShoot = false;
                }
                else
                {
                    if (gameTime.TotalGameTime.Subtract(_timeSinceLastMissile).CompareTo(new TimeSpan(0, 0, 3)) > 0)
                    {
                        _canShoot = true;
                    }
                }

                Vector2 playerDir = _player.ObjectBody.Position - ObjectBody.Position;
                playerDir.Normalize();
                ObjectBody.Position += new Vector2(playerDir.X, 0) * 0.01f;

            }

            foreach (var magicMissile in _magicMissiles)
            {
                magicMissile.Update(gameTime);
            }
            
        }

           
    
        public bool Enabled { get; }
        public int UpdateOrder { get; }
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public void RemoveMissiles(WorldObject missile)
        {
            _magicMissiles.Remove((MagicMissile)missile);
        }

        public void AddToRemoveLater(WorldObject missile)
        {
            _magicMissilesToRemove.Add((MagicMissile)missile);   
        }

        public void Draw(GameTime gameTime)
        {
            int width =  Texture2D.Width / _columns; 
            int height = Texture2D.Height / _rows;
            
            Rectangle sourceRectangle = new Rectangle(width * _currColumn, height * _currRow, width, height);
            
            _screenManager.SpriteBatch.Draw(Texture2D, ObjectBody.Position, sourceRectangle, Color.White, ObjectBody.Rotation,
                _textureHalf, _objectTextureMetersSize / (_objectTextureSize / new Vector2(_columns, _rows)), SpriteEffects.FlipVertically, 0f);
        }

        public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}