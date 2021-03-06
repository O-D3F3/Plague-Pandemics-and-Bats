﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Security.Cryptography.X509Certificates;

namespace PlaguePandemicsBats
{
    public class Button : DrawableGameComponent
    {
        #region Private variables
        private Texture2D _texture;
        private Game1 _game;
        private SpriteBatch _spriteBatch;

        private Vector2 _position;
        private Vector2 _origin;
        private Rectangle _rec;
        private Color _color = new Color(255,255,255,255);
        private MouseState _pastmouse;
        #endregion

        #region Public variables
        public bool isClicked;
        #endregion

        #region Constructor
        public Button(Game1 game, Texture2D texture, Vector2 position) : base(game)
        { 
            _game = game;
            _texture = texture;
            _position = position;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _origin = _texture.Bounds.Size.ToVector2() / 2;
        }
        #endregion

        #region Methods
        /// <summary>
        /// shifts the colors of the buttons depending on the position of the mouse
        /// </summary>
        /// <param name="mouse"></param>
        public void Update(MouseState mouse, int x)
        {       
            isClicked = false;
            Rectangle mouseRec = new Rectangle(mouse.X, mouse.Y, 1, 1);

            mouse = Mouse.GetState();
            _rec = new Rectangle((int)_position.X - (_texture.Width / 2), (int)_position.Y - (_texture.Height / 2), _texture.Width, _texture.Height);
            
            if (x == 0)
            {             
                if (mouseRec.Intersects(_rec))
                {
                    _color = new Color(180, 180, 180, 255);
                    if (mouse.LeftButton == ButtonState.Pressed && _pastmouse.LeftButton == ButtonState.Released)
                    {
                        isClicked = true;
                    }
                }
                else
                    _color = Color.White;
                _pastmouse = mouse;
            }
            if (x == 2)
            {
                if(mouseRec.Intersects(_rec))
                {
                    Texture2D texture = _game.Content.Load<Texture2D>("creditsButtonOnHover");
                    Rectangle rec = new Rectangle(411, 232, texture.Bounds.Width, texture.Bounds.Height);

                    _spriteBatch.Begin();

                    _spriteBatch.Draw(texture, rec, Color.White);

                    _spriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draws the sprites
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, int x)
        {
            Point size = _texture.Bounds.Size;
            Point _size = size;

            if (x == 0) size = _size;

            else if (x == 1) size = new Point(50, 40);

            else if (x == 2) size = new Point(50);

            else if (x == 3) size = new Point(100);

            spriteBatch.Draw(
            _texture,
            new Rectangle(_position.ToPoint(), size),
            null,
            _color,
            rotation: 0,
            _origin,
            SpriteEffects.None,
            0);
        }
        #endregion
    }
}
