using System;
using System.Collections.Generic;
using System.Numerics;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Game;
using CaveWizard.Globals;
using CaveWizard.Helpers;
using CaveWizard.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace CaveWizard.Levels
{
    public class Level : PhysicsGameScreen, IGameLevel, IMenubale
    {
        public Player Player { get; set; }
        private List<Ground> _ground;
        private List<Enemy> _enemies;
        private List<Enemy> _enemiesToDestroy;
        private FinishDoor _finishDoor;
        private Generator _generator;
        private UIScreen _uiScreen;
        public EnemyDiedEventHandler EnemyDiedEvent; 
        private Menu _menu;
        private bool _scheduledNewLevel;
        private int _level;
        private TimeSpan _timeSinceLevelSchedule;

        public Level()
        {
            TransitionOffTime = TimeSpan.Zero;
            TransitionOnTime = TimeSpan.Zero;
            EnemyDiedEvent = EnemyDied;
            _level = 1;

        }

        private void EnemyDied(Enemy enemy)
        {
            _enemiesToDestroy.Add(enemy);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _scheduledNewLevel = false;
            GlobalDevices._GameState = GameState.PLAY;
            World.Gravity = new Vector2(0, -10f);
            _generator = new Generator(_level);
            _ground = new List<Ground>();
            _enemies = new List<Enemy>();
            _enemiesToDestroy = new List<Enemy>();

            for (int i = 0; i < _generator.LevelPresented.Count; i++)
            {
                int holesEncountered = 0;
                for (int j = 0; j < _generator.LevelPresented[i].Count; j++)
                {
                    char currElement = _generator.LevelPresented[i][j];
                    if (j == 0)
                    {
                        _ground.Add(new Ground(ScreenManager, "Textures/wall",
                            new Vector2((float) -(j - 1), (float) -(i + (i / 2))), World, 3, 3, _generator.LevelPresented,
                            i == 0 ? 0 : 1, 2, this));
                        if (i % 2 == 1)
                        {
                            _ground.Add(new Ground(ScreenManager, "Textures/wall",
                                new Vector2((float) -(j - 1), (float) -((i + 1) + ((i) / 2))), World, 3, 3, _generator.LevelPresented,
                                1, 2, this));
                        }
                    }

                    if (j == _generator.LevelPresented[i].Count - 1)
                    {
                        _ground.Add(new Ground(ScreenManager, "Textures/wall",
                            new Vector2((float) -(j + 1), (float) -(i + (i / 2))), World, 3, 3, _generator.LevelPresented,
                            i == 0 ? 0 : 1, 0, this));
                        if (i % 2 == 1)
                        {
                            _ground.Add(new Ground(ScreenManager, "Textures/wall",
                                new Vector2((float) -(j + 1), (float) -((i + 1) + ((i) / 2))), World, 3, 3, _generator.LevelPresented,
                                1, 0, this));
                        }
                        
                    }
                    if (currElement == '.')
                    {
                        holesEncountered++;
                    }

                    if (currElement == 'S')
                    {
                        Player = new Player(ScreenManager, "Textures/blackMageConcept",
                            new Vector2((float) -j, (float) -i), World, 6, 3, this);
                    }
                    else if (currElement == '#')
                    {
                        _ground.Add(new Ground(ScreenManager, "Textures/ground",
                            new Vector2((float) -(j), (float) -(i + (i / 2))), World, 3, 1, _generator.LevelPresented,
                            i, j, this));

                    }
                    else if (currElement == 'E')
                    {
                        _enemies.Add(new Enemy(ScreenManager, Player, "Textures/skeleton",
                            new Vector2((float) -(j), (float) -(i + (i / 2))), World, new Vector2(1f, 0.5f),
                            new Vector2(1f, 1f), 10, 5, this));
                    }
                    else if (currElement == 'F')
                    {
                        _finishDoor = new FinishDoor(ScreenManager, World,
                            new Vector2((float) -(j), (float) -(i + (i / 2))),
                            "Textures/Door", new Vector2(0.75f, 0.75f), Vector2.One, 2, 1, this);
                    }
                }
            }

            _uiScreen = new UIScreen(GlobalDevices._GraphicsDeviceManager, ScreenManager);
            _uiScreen.LoadContent(ScreenManager.Content);
            var elementFactory = new CaveWizardUIElementFactory(Player);

            _uiScreen.AddUIElement(elementFactory.CreateElement("HudHotBar"));
            _uiScreen.AddUIElement(elementFactory.CreateElement("HudHealthBar"));
            Camera.TrackingBody = Player.ObjectBody;
            Camera.Zoom = 4f;
            SetUserAgent(Player.ObjectBody, 2, 0);

            if (GameSettings._Music)
            {
                MediaPlayer.Play(SoundEffects.GameSong);
                MediaPlayer.IsRepeating = true;
            }
        }

        public override void UnloadContent()
        {
            foreach (var enemy in _enemies)
            {
                World.Remove(enemy.ObjectBody);
            }
            foreach (var ground in _ground)
            {
                World.Remove(ground.ObjectBody);
            }
            _enemies.Clear();
            _ground.Clear();
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null,
                RasterizerState.CullNone, ScreenManager.BatchEffect);
            _finishDoor.Draw(gameTime);
            Player.Draw(gameTime);
            foreach (var ground in _ground)
            {
                ground.Draw(gameTime);
            }

            foreach (var enemy in _enemies)
            {
                enemy.Draw(gameTime);
            }
            ScreenManager.SpriteBatch.End();
            
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            _uiScreen.Draw(gameTime);
            ScreenManager.SpriteBatch.End();
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, _finishDoor.PlayerNearDoor.ToString(), Vector2.One, Color.White);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, Player.ObjectBody.Position.ToString(), new Vector2(1, ScreenManager.Fonts.DetailsFont.MeasureString("M").Y), Color.White);
            ScreenManager.SpriteBatch.End();
            
            
            
            base.Draw(gameTime);
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {

            if (input.IsNewKeyPress(KeyBinds.GoToMenu))
            {
                MediaPlayer.Pause();
                ScreenManager.AddScreen(new Menu("Menu", this));
            }

            if (input.IsNewKeyPress(Keys.U))
            {
                LoadContent();
            }
            
            Player.HandleInput(input, gameTime);

            
            
            base.HandleInput(input, gameTime);
        }

        protected override void HandleCursor(InputHelper input)
        {
            base.HandleCursor(input);

            Player.HandleCursor(input, World, Camera);
            if (input.IsNewMouseButtonPress(MouseButtons.RightButton))
            {
                _ground.Add(new Ground(ScreenManager, "Textures/ground", Camera.ConvertScreenToWorld(input.Cursor), World, 3, 1, null, 0, 0, this));

            }

            if (input.IsNewMouseButtonPress(MouseButtons.MiddleButton))
            {
                _enemies.Add(new Enemy(ScreenManager, Player, "Textures/skeleton",Camera.ConvertScreenToWorld(input.Cursor), World, new Vector2(1f, 0.5f),  new Vector2(1f, 1f), 10, 5, this));
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (GlobalDevices._GameState == GameState.PLAY)
            {
                if (GameSettings._Music)
                {
                    MediaPlayer.Resume();
                }
               
                Player.Update(gameTime);

                if (_enemiesToDestroy.Count > 0)
                {
                    foreach (var enemy in _enemiesToDestroy)
                    {
                        World.Remove(enemy.ObjectBody);
                        _enemies.Remove(enemy);
                    }
                    _enemiesToDestroy.Clear();
                }

                foreach (var enemy in _enemies)
                {
                    enemy.Update(gameTime);
                }
                
                _uiScreen.Update(gameTime);
            }
            else if (GlobalDevices._GameState == GameState.MAINMENU)
            {
                MediaPlayer.Pause();
                _level = 1;
                ExitScreen();
            }
            else if (GlobalDevices._GameState == GameState.NEXTLEVEL)
            { 
                
                if (_scheduledNewLevel)
                {
                    if (gameTime.TotalGameTime.Subtract(_timeSinceLevelSchedule).TotalMilliseconds > 500f)
                    {
                        _timeSinceLevelSchedule = gameTime.TotalGameTime;
                        ScreenManager.AddScreen(new LevelPassage(_timeSinceLevelSchedule));
                        _scheduledNewLevel = false;
                    }

                }
                
                if (gameTime.TotalGameTime.Subtract(_timeSinceLevelSchedule).TotalMilliseconds > SoundEffects.PlayerjumpSoundEffects.Duration.TotalMilliseconds + 1000f && !_scheduledNewLevel)
                {
                    _level++;
                    LoadContent();
                }
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public void MenuDestroyed()
        {
            GlobalDevices._GameState = GameState.PLAY;
        }

        public void ToMainMenu()
        {
            GlobalDevices._GameState = GameState.MAINMENU;

        }

        public void NewLevel(GameTime gameTime)
        {
            GlobalDevices._GameState = GameState.NEXTLEVEL;
            _scheduledNewLevel = true;
            _timeSinceLevelSchedule = gameTime.TotalGameTime;

        }
    }
}