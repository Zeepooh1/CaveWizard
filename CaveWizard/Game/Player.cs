using System;
using CaveEngine.Utils;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.Game {
    public class Player : IDrawable, IUpdateable {
        public int DrawOrder { get; set; }
        public bool Visible { get; set;}
        public bool Enabled { get; set; }
        public int UpdateOrder { get; set;}
        private readonly Texture2D _texture2D;
        private readonly float _baseVelocity;
        private bool _inAir;
        public Body PlayerBody { get; }
        private readonly Vector2 _playerBodySize;
        private readonly Vector2 _playerTextureMetersSize;
        private readonly Vector2 _playerTextureSize;
        private readonly Vector2 _playerTextureOrigin;
        private SpriteEffects _playerEffect = SpriteEffects.FlipVertically;
        
        
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
    
        public Player(ContentManager content, string propName, Vector2 pos, World world) {
            _texture2D = content.Load<Texture2D>(propName);
            _baseVelocity = 10.0f;
            _playerBodySize = new Vector2(1f, 1.5f);
            _playerTextureMetersSize = new Vector2(1.5f, 1.5f);
            _playerTextureSize = new Vector2(_texture2D.Width, _texture2D.Height);
            _playerTextureOrigin = _playerTextureSize / 2f;
            PlayerBody = world.CreateRectangle(_playerBodySize.X, _playerBodySize.Y, 1f, pos);
            PlayerBody.BodyType = BodyType.Dynamic;
            PlayerBody.SetRestitution(0f);
            PlayerBody.Mass = 2f;
            PlayerBody.FixedRotation = true;
            PlayerBody.SetFriction(0.5f);
            PlayerBody.OnSeparation += Seperation;
            PlayerBody.OnCollision += Colided;
            _inAir = true;

        }

        private bool Colided(Fixture sender, Fixture other, Contact contact)
        {
            _inAir = false;
            return true;
            
        }

        private void Seperation(Fixture sender, Fixture other, Contact contact)
        {
            _inAir = true;
        }
        
        

        public void Update(GameTime gameTime) {
            
            if (InputManager.KeyState.IsKeyDown(KeyBinds.PlayerMoveLeft))
            {
                PlayerBody.ApplyForce(new Vector2(x: -_baseVelocity, 0));
                _playerEffect = SpriteEffects.None | SpriteEffects.FlipVertically;
            }

            if (InputManager.KeyState.IsKeyDown(KeyBinds.PlayerMoveRight)) {
                PlayerBody.ApplyForce(new Vector2(x: _baseVelocity, 0));
                _playerEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;

            }

            if (InputManager.KeyState.IsKeyDown(KeyBinds.PlayerMoveDown)) {
                PlayerBody.ApplyForce(new Vector2(x: 0, -_baseVelocity));
            }

            if (InputManager.KeyState.IsKeyDown(KeyBinds.PlayerMoveUp)) {
                PlayerBody.ApplyForce(new Vector2(x: 0, _baseVelocity));
            }

            if (InputManager.KeyState.IsKeyDown(KeyBinds.PlayerJump) && InputManager.PrevKeyState.IsKeyUp(KeyBinds.PlayerJump) && !_inAir)
            {
                PlayerBody.ApplyLinearImpulse(new Vector2(0f, 6f));
            }
            
        }

        public void Draw(GameTime gameTime)
        {
            GlobalDevices._SpriteBatch.Draw(_texture2D, PlayerBody.Position, null, Color.White, PlayerBody.Rotation,
                _playerTextureOrigin, _playerTextureMetersSize / _playerTextureSize, _playerEffect, 0f);
        }
        
        
    }
}