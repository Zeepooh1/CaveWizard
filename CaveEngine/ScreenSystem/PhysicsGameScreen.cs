﻿﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

 using System;
 using CaveEngine.Utils;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Input;
 using tainicom.Aether.Physics2D.Diagnostics;
 using tainicom.Aether.Physics2D.Dynamics;
 using tainicom.Aether.Physics2D.Dynamics.Joints;

 namespace CaveEngine.ScreenSystem
{
    public class PhysicsGameScreen : GameScreen
    {
        public Camera2D Camera;
        protected DebugView DebugView;
        protected World World;
        protected Body HiddenBody;
        protected FixedMouseJoint _fixedMouseJoint;

        private float _agentForce;
        private float _agentTorque;
        private Body _userAgent;

        protected PhysicsGameScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.75);
            TransitionOffTime = TimeSpan.FromSeconds(0.75);
            HasCursor = true;
            EnableCameraControl = true;
            _userAgent = null;
            World = null;
            Camera = null;
            DebugView = null;
        }

        public bool EnableCameraControl { get; set; }

        protected void SetUserAgent(Body agent, float force, float torque)
        {
            _userAgent = agent;
            _agentForce = force;
            _agentTorque = torque;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (World == null)
            {
                World = new World(Vector2.Zero);
                World.JointRemoved += JointRemoved;
            }
            else
            {
                World.Clear();
            }

            // enable multithreading
            World.ContactManager.VelocityConstraintsMultithreadThreshold = 256;
            World.ContactManager.PositionConstraintsMultithreadThreshold = 256;
            World.ContactManager.CollideMultithreadThreshold = 256;

            if (DebugView == null)
            {
                DebugView = new DebugView(World);
                DebugView.RemoveFlags(DebugViewFlags.Shape);
                DebugView.RemoveFlags(DebugViewFlags.Joint);
                DebugView.DefaultShapeColor = Color.White;
                DebugView.SleepingShapeColor = Color.LightGray;
                DebugView.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.Content);
            }

            if (Camera == null)
                Camera = new Camera2D(ScreenManager.GraphicsDevice);
            else
                Camera.ResetCamera();

            HiddenBody = World.CreateBody(Vector2.Zero);

            // Loading may take a while... so prevent the game from "catching up" once we finished loading
    //        ScreenManager.Game.ResetElapsedTime();
        }
        
        protected virtual void JointRemoved(World sender, Joint joint)
        {
            if (_fixedMouseJoint == joint)
                _fixedMouseJoint = null;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!coveredByOtherScreen && !otherScreenHasFocus)
            {
                // variable time step but never less then 30 Hz
                World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            }

            Camera.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            DebugView.UpdatePerformanceGraph(World.UpdateTime);
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            // Control debug view
            if (input.IsNewButtonPress(Buttons.Start))
            {
                EnableOrDisableFlag(DebugViewFlags.Shape);
                EnableOrDisableFlag(DebugViewFlags.DebugPanel);
                EnableOrDisableFlag(DebugViewFlags.PerformanceGraph);
                EnableOrDisableFlag(DebugViewFlags.Joint);
                EnableOrDisableFlag(DebugViewFlags.ContactPoints);
                EnableOrDisableFlag(DebugViewFlags.ContactNormals);
                EnableOrDisableFlag(DebugViewFlags.Controllers);
            }

            if (input.IsNewKeyPress(Keys.F1))
                EnableOrDisableFlag(DebugViewFlags.Shape);
            if (input.IsNewKeyPress(Keys.F2))
            {
                EnableOrDisableFlag(DebugViewFlags.DebugPanel);
                EnableOrDisableFlag(DebugViewFlags.PerformanceGraph);
            }
            if (input.IsNewKeyPress(Keys.F3))
                EnableOrDisableFlag(DebugViewFlags.Joint);
            if (input.IsNewKeyPress(Keys.F4))
            {
                EnableOrDisableFlag(DebugViewFlags.ContactPoints);
                EnableOrDisableFlag(DebugViewFlags.ContactNormals);
            }
            if (input.IsNewKeyPress(Keys.F5))
                EnableOrDisableFlag(DebugViewFlags.PolygonPoints);
            if (input.IsNewKeyPress(Keys.F6))
                EnableOrDisableFlag(DebugViewFlags.Controllers);
            if (input.IsNewKeyPress(Keys.F7))
                EnableOrDisableFlag(DebugViewFlags.CenterOfMass);
            if (input.IsNewKeyPress(Keys.F8))
                EnableOrDisableFlag(DebugViewFlags.AABB);

            if (HasCursor)
                HandleCursor(input);

            if (_userAgent != null)
                HandleUserAgent(input);

            if (EnableCameraControl)
                HandleCamera(input, gameTime);

            base.HandleInput(input, gameTime);
        }

        protected virtual void HandleCursor(InputHelper input)
        {
        }

        private void HandleCamera(InputHelper input, GameTime gameTime)
        {
            Vector2 camMove = Vector2.Zero;

            if (input.KeyboardState.IsKeyDown(Keys.Up))
                camMove.Y -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (input.KeyboardState.IsKeyDown(Keys.Down))
                camMove.Y += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (input.KeyboardState.IsKeyDown(Keys.Left))
                camMove.X -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (input.KeyboardState.IsKeyDown(Keys.Right))
                camMove.X += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (input.KeyboardState.IsKeyDown(Keys.PageUp))
                Camera.Zoom += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            if (input.KeyboardState.IsKeyDown(Keys.PageDown))
                Camera.Zoom -= 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            if (camMove != Vector2.Zero)
                Camera.MoveCamera(camMove);
            if (input.IsNewKeyPress(Keys.Home))
                Camera.ResetCamera();
        }

        protected virtual void HandleUserAgent(InputHelper input)
        {
            Vector2 force = _agentForce * new Vector2(input.GamePadState.ThumbSticks.Right.X, -input.GamePadState.ThumbSticks.Right.Y);
            float torque = _agentTorque * (input.GamePadState.Triggers.Right - input.GamePadState.Triggers.Left);

            _userAgent.ApplyForce(force);
            _userAgent.ApplyTorque(torque);

            float forceAmount = _agentForce * 0.6f;

            force = Vector2.Zero;
            torque = 0;

            if (input.KeyboardState.IsKeyDown(Keys.A))
                force += new Vector2(-forceAmount, 0);
            if (input.KeyboardState.IsKeyDown(Keys.S))
                force += new Vector2(0, -forceAmount);
            if (input.KeyboardState.IsKeyDown(Keys.D))
                force += new Vector2(forceAmount, 0);
            if (input.KeyboardState.IsKeyDown(Keys.W))
                force += new Vector2(0, forceAmount);
            if (input.KeyboardState.IsKeyDown(Keys.Q))
                torque -= _agentTorque;
            if (input.KeyboardState.IsKeyDown(Keys.E))
                torque += _agentTorque;

            _userAgent.ApplyForce(force);
            _userAgent.ApplyTorque(torque);
        }

        private void EnableOrDisableFlag(DebugViewFlags flag)
        {
            if ((DebugView.Flags & flag) == flag)
                DebugView.RemoveFlags(flag);
            else
                DebugView.AppendFlags(flag);
        }

        public override void Draw(GameTime gameTime)
        {
            DebugView.RenderDebugData(Camera.Projection, Camera.View);
            base.Draw(gameTime);
        }
    }
}