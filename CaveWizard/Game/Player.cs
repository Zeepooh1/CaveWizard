using System;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CaveWizard.Game {
    public class Player : IDrawable, IUpdateable {
        public int DrawOrder { get; set; }
        public bool Visible { get; set;}
        public bool Enabled { get; set; }
        public int UpdateOrder { get; set;}
        private Vector2 _pos;
        private Texture2D _texture2D;
        private float _baseVelocity;
        private float _currentVelocity;
        private bool Moving;
        private Vector2 TargetVec;
        private float _startingDistance;
        
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public Player(ContentManager content, string propName, Vector2 pos) {
            _texture2D = content.Load<Texture2D>(propName);
            _pos = pos;
            _baseVelocity = 10.0f;
            _currentVelocity = _baseVelocity;
            Moving = false;
            _startingDistance = 0f;
        }

        public void Update(GameTime gameTime) {
            /*if (GlobalDevices._MouseState.LeftButton.Equals(ButtonState.Pressed) && Moving == false) {
                Moving = true;
                TargetVec = GlobalDevices._MouseState.Position.ToVector2();
                _startingDistance = Vector2.Distance(_pos, TargetVec);
            }

            if (!Moving) return;
            if (Vector2.Distance(_pos, TargetVec) < 0.5f) {
                Moving = false;
                _currentVelocity = _baseVelocity;
            }
            else
            {
                Vector2 toMove = TargetVec - _pos;
                toMove.Normalize();
                toMove *= _currentVelocity;
                _pos += toMove;
                _currentVelocity = _baseVelocity *  Vector2.Distance(_pos, TargetVec) / _startingDistance;
            }
*/
            if (Keyboard.GetState().IsKeyDown(KeyBinds.PlayerMoveLeft)) {
                _pos.X -= _currentVelocity;
            }

            if (Keyboard.GetState().IsKeyDown(KeyBinds.PlayerMoveRight)) {
                _pos.X += _currentVelocity;
            }

            if (Keyboard.GetState().IsKeyDown(KeyBinds.PlayerMoveDown)) {
                _pos.Y += _currentVelocity;
            }

            if (Keyboard.GetState().IsKeyDown(KeyBinds.PlayerMoveUp)) {
                _pos.Y -= _currentVelocity;
            }
        }

        public void Draw(GameTime gameTime) {
            GlobalDevices._SpriteBatch.Draw(_texture2D, _pos);
        }
    }
}