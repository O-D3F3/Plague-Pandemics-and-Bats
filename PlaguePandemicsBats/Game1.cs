﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlaguePandemicsBats
{
    #region Enumerators
    /// <summary>
    /// Can be used to know the direction of the player or of the enemies
    /// </summary>
    public enum Direction
    {
        Up, Down, Left, Right
    }

    /// <summary>
    /// Type of Colliders, used when drawing a sprite
    /// </summary>
    public enum ColliderType
    {
        OBB, AABB, Circle
    }

    /// <summary>
    /// Used to know in Which State the game is in
    /// </summary>
    public enum GameState
    {
        MainMenu, Options, Highscores, Playing, Paused, ChooseName, ChooseCharacter
    }
    #endregion

    public class Game1 : Game
    {
        #region  private variables
        private GraphicsDeviceManager _graphics;
        private SoundEffect _playSound;
        private Song _menuSong;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Texture2D _pausedTexture;
        private Rectangle _pausedRect;
        private Camera _camera;
        private SpriteManager _spriteManager;
        private CollisionManager _collisionManager;
        private Player _player;
        private SpawnerZombie _spZ;
        private ShooterZombie _shZ;
        private Cat _cat;
        private Scene _scene;
        private Button _buttonPlay, _buttonQuit, _guyButton, _girlButton, _highScoreButton, _optnButton, _creditsButton, _back2menuButton;
        private UI _ui;
        private List<Projectile> _projectiles;
        private List<Enemy> _enemies;
        private List<Cat> _friendlies;
        private List<Button> _buttons;
        private List<EnemyProjectile> _enemyProjectiles;
        private GameState _gameState = GameState.MainMenu;
        private int _highScore;
        #endregion

        #region Public variables
        public TilingBackground background;
        #endregion

        #region Constructor
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get game's sprite batch.
        /// </summary>
        public SpriteBatch SpriteBatch => _spriteBatch;

        /// <summary>
        /// Get the game's Camera
        /// </summary>
        public Camera Camera => _camera;

        /// <summary>
        /// Get game's sprite manager
        /// </summary>
        public SpriteManager SpriteManager => _spriteManager;

        /// <summary>
        /// Get game's collision manager
        /// </summary>
        public CollisionManager CollisionManager => _collisionManager;

        /// <summary>
        /// Get game's player 
        /// </summary>
        public Player Player => _player;

        /// <summary>
        /// Get game's UI
        /// </summary>
        public UI UI => _ui;

        /// <summary>
        /// Get game's Projectiles
        /// </summary>
        public List<Projectile> Projectiles => _projectiles;

        /// <summary>
        /// Get game's enemies
        /// </summary>
        public List<Enemy> Enemies => _enemies;

        /// <summary>
        /// Get game's player friendlies
        /// </summary>
        public List<Cat> Friendlies => _friendlies;

        /// <summary>
        /// Get Game's Button
        /// </summary>
        public List<Button> Buttons => _buttons;

        /// <summary>
        /// Get Game's Enemy Projectiles
        /// </summary>
        public List<EnemyProjectile> EnemyProjectiles => _enemyProjectiles;
        #endregion

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //string file = File.ReadAllText(Player.filePath);
            //Player.Highscore = Int32.Parse(file);

            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height / 2;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width / 2;
            _graphics.ApplyChanges();

            Components.Add(new KeyboardManager(this));

            _spriteManager = new SpriteManager(this);

            _camera = new Camera(this, worldWidth: 9f);

            /*LISTS*/
            _enemies = new List<Enemy>();
            _friendlies = new List<Cat>();
            _buttons = new List<Button>();
            _projectiles = new List<Projectile>();
            _enemyProjectiles = new List<EnemyProjectile>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteFont = Content.Load<SpriteFont>("minecraft");
            _playSound = Content.Load<SoundEffect>("playsound");
            _menuSong = Content.Load<Song>("menusong");

            //Pause Stuff
            _pausedTexture = Content.Load<Texture2D>("pause");
            _pausedRect = new Rectangle(-1, 0, _pausedTexture.Width / 2, _pausedTexture.Height / 2);

            _collisionManager = new CollisionManager(this);

            SpriteManager.AddSpriteSheet("NonTrimmedSheet");
            SpriteManager.AddSpriteSheet("TrimmedSheet");
            SpriteManager.AddSpriteSheet("Fullgrass");

            _scene = new Scene(this, "MainScene");
            _player = _scene.Player;
            _ui = new UI(this);
            _cat = new Cat(this);
            _spZ = new SpawnerZombie(this, Vector2.One);
            _shZ = new ShooterZombie(this, new Vector2(3, 1));

            background = new TilingBackground(this, "Fullgrass", new Vector2(4));

            //buttons 
            _buttonPlay = new Button(this, Content.Load<Texture2D>("play"), new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 1.8f)); 
            _highScoreButton = new Button(this, Content.Load<Texture2D>("button"), new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 1.46f));
            _buttonQuit = new Button(this, Content.Load<Texture2D>("quit"), new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 1.23f));
            _optnButton = new Button(this, Content.Load<Texture2D>("optnButton"),new Vector2(GraphicsDevice.Viewport.Width / 1.05f, GraphicsDevice.Viewport.Height / 5.7f));
            _creditsButton = new Button(this, Content.Load<Texture2D>("creditsButton"), new Vector2(GraphicsDevice.Viewport.Width / 1.05f, GraphicsDevice.Viewport.Height / 3.3f));
            _back2menuButton = new Button(this, Content.Load<Texture2D>("mmButton"), new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 1.46f));
            _girlButton = new Button(this, Content.Load<Texture2D>("girlbutton"), new Vector2(190, 370));
            _guyButton = new Button(this, Content.Load<Texture2D>("guybutton"), new Vector2(770, 360));
           

            //Temporary List Adding
            _friendlies.Add(_cat);
            _buttons.Add(_buttonPlay);
            _buttons.Add(_buttonQuit);
            _buttons.Add(_highScoreButton);
            _buttons.Add(_optnButton);
            _buttons.Add(_creditsButton);

            if (_gameState == GameState.MainMenu) MediaPlayer.Play(_menuSong);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F1))
                Exit();

            MouseState mouseState = Mouse.GetState();

            switch (_gameState)
            {
                case GameState.MainMenu:
                    MediaPlayer.Play(_menuSong);
                    IsMouseVisible = true;

                    if (_buttonPlay.isClicked || KeyboardManager.IsKeyGoingDown(Keys.Enter))
                    {
                        _gameState = GameState.ChooseCharacter;                   
                    }
                    if (_highScoreButton.isClicked)
                    {
                        _gameState = GameState.Highscores;
                    }

                    _buttonPlay.Update(mouseState);
                    _highScoreButton.Update(mouseState);
                    _buttonQuit.Update(mouseState);
                    _optnButton.Update(mouseState);
                    _creditsButton.Update(mouseState);

                    break;
                case GameState.ChooseCharacter:
                    IsMouseVisible = true;

                    if (_girlButton.isClicked)
                    {
                        _player.Gender = 0;
                        _playSound.Play();
                        _gameState = GameState.Playing;
                    }

                    if (_guyButton.isClicked)
                    {
                        _player.Gender = 1;
                        _playSound.Play();
                        _gameState = GameState.Playing;
                    }

                    _girlButton.Update(mouseState);
                    _guyButton.Update(mouseState);

                    break;
                case GameState.Highscores:
                    IsMouseVisible = true;

                    _back2menuButton = new Button(this, Content.Load<Texture2D>("mmButton"), new Vector2(830, 60));

                    if (_back2menuButton.isClicked) _gameState = GameState.MainMenu;

                    _back2menuButton.Update(mouseState);

                    break;
                case GameState.Options:
                    IsMouseVisible = true;
                    break;
                case GameState.Playing:
                    MediaPlayer.Stop();
                    IsMouseVisible = false;

                    if (KeyboardManager.IsKeyGoingDown(Keys.Escape))
                    {
                        _gameState = GameState.Paused;

                        _buttonPlay.isClicked = false;
                    }

                    _player.Update(gameTime);
                    _collisionManager.Update(gameTime);
                    _player.LateUpdate(gameTime);

                    foreach (Projectile p in Projectiles.ToArray())
                    {
                        p.Update(gameTime);
                    }

                    foreach (EnemyProjectile eP in EnemyProjectiles.ToArray())
                    {
                        eP.Update(gameTime);
                    }

                    foreach (Enemy e in Enemies.ToArray())
                    {
                        e.Update(gameTime);
                        e.LateUpdate(gameTime);
                    }

                    foreach (Cat c in Friendlies.ToArray())
                    {
                        c.Update(gameTime);
                        c.LateUpdate(gameTime);
                    }
                    break;

                case GameState.Paused:
                    IsMouseVisible = true;
                    if (KeyboardManager.IsKeyGoingDown(Keys.Escape))
                        _gameState = GameState.Playing;
                    if (_buttonPlay.isClicked)
                        _gameState = GameState.Playing;
                    if (_buttonQuit.isClicked)
                        Exit();
                    if (_back2menuButton.isClicked)
                        _gameState = GameState.MainMenu;

                    _buttonPlay.Update(mouseState);
                    _buttonQuit.Update(mouseState);
                    _back2menuButton.Update(mouseState);

                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_gameState == GameState.Playing)
            {
                background.Draw(gameTime);
                
                _scene.Draw(gameTime);     
                _player.Draw(_spriteBatch);

                foreach (Projectile p in Projectiles.ToArray())
                {
                    p.Draw(_spriteBatch);
                }

                foreach (EnemyProjectile eP in EnemyProjectiles.ToArray())
                {
                    eP.Draw(_spriteBatch);
                }

                foreach (Enemy e in Enemies.ToArray())
                {
                    e.Draw(_spriteBatch);
                }

                foreach (Cat c in Friendlies.ToArray())
                {
                    c.Draw(_spriteBatch);
                }

                _spriteBatch.DrawString(_spriteFont, $"SCORE {Player.Score}", new Vector2(10, 10), Color.Black);
            }

            if (_gameState == GameState.Paused)
            {
                background.Draw(gameTime);

                _scene.Draw(gameTime);
                _ui.Draw(_spriteBatch, gameTime);
                _player.Draw(_spriteBatch);

                foreach (Projectile p in Projectiles.ToArray())
                {
                    p.Draw(_spriteBatch);
                }

                foreach (EnemyProjectile eP in EnemyProjectiles.ToArray())
                {
                    eP.Draw(_spriteBatch);
                }

                foreach (Enemy e in Enemies.ToArray())
                {
                    e.Draw(_spriteBatch);
                }

                foreach (Cat c in Friendlies.ToArray())
                {
                    c.Draw(_spriteBatch);
                }

                Rectangle rec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                Pixel.DrawRectangle(rec, Color.Black * 0.5f);

                _spriteBatch.Draw(_pausedTexture, _pausedRect, Color.White);

                _buttonPlay.Draw(_spriteBatch, 0);
                _buttonQuit.Draw(_spriteBatch, 0);
                _back2menuButton.Draw(_spriteBatch, 0);
            }

            if (_gameState == GameState.MainMenu)
            {
                Texture2D texture = Content.Load<Texture2D>("mainmenu");
                //fullscreen
                Rectangle rec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                Color color = Color.White;
                
                _spriteBatch.Draw(texture , rec , color);

                _buttonPlay.Draw(_spriteBatch, 0);
                _buttonQuit.Draw(_spriteBatch, 0);
                _highScoreButton.Draw(_spriteBatch, 0);
                _creditsButton.Draw(_spriteBatch, 1);
                _optnButton.Draw(_spriteBatch, 2);
            }

            if(_gameState == GameState.ChooseCharacter)
            {
                Texture2D texture = Content.Load<Texture2D>("characterPickMenu");
                //fullscreen
                Rectangle rec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                Color color = Color.White;

                _spriteBatch.Draw(texture, rec, color);

                _girlButton.Draw(_spriteBatch, 0);
                _guyButton.Draw(_spriteBatch, 0);

                _spriteBatch.DrawString(_spriteFont, "MARIA SOTO", new Vector2(100, 150), Color.LightBlue);
                _spriteBatch.DrawString(_spriteFont, "OLIVER BUCHANAN", new Vector2(630, 150), Color.LightBlue);
            }

            if( _gameState == GameState.Highscores)
            {
                Texture2D texture = Content.Load<Texture2D>("highscoreMenu");
                //fullscreen
                Rectangle rec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                Color color = Color.White;

                _spriteBatch.Draw(texture, rec, color);

                _highScoreButton = new Button(this, Content.Load<Texture2D>("button"), new Vector2(100, 60));
                _back2menuButton = new Button(this, Content.Load<Texture2D>("mmButton"), new Vector2(830, 60));
                _highScoreButton.Draw(_spriteBatch, 0);
                _back2menuButton.Draw(_spriteBatch, 0);
            }

            if (_gameState == GameState.Options)
            {
               
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Read high Scores from the file
        /// If file is not available HighScore is set to 0
        /// </summary>
        public void LoadHighScores()
        {
            try
            {
                string[] file = File.ReadAllLines($@"{Content.RootDirectory}\highscore.txt");

                if (file.Length != 0 && !int.TryParse(file[0], out _highScore))
                {
                    _highScore = 0;
                }
            }
            catch (FileNotFoundException)
            {
                _highScore = 0;
            }
        }

        /// <summary>
        /// Saves the New HighScore
        /// </summary>
        /// <param name="newHighScore">The new High Score to save</param>
        private void SaveHighScore(int newHighScore)
        {
            File.WriteAllText($@"{Content.RootDirectory}\highscore.txt", newHighScore.ToString());
        }
    }
}
