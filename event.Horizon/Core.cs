using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace @event.Horizon
{

    abstract class Core
    {
        static Game1 s_game;

        public static void Initialize(Game1 game)
        {
            s_game = game;
        }

        public static void EndGame()
        {
            s_game.Exit();
        }

        public static ContentManager Content
        {
            get { return s_game.Content; }
        }

        public static Rectangle Window
        {
            get { return s_game.Window.ClientBounds; }
        }

        public static GraphicsDevice GraphicsDevice
        {
            get { return s_game.GraphicsDevice; }
        }

        public static SpriteBatch spriteBatch
        {
            get { return s_game.spriteBatch; }
        }

        public static Vector2 cameraPos
        {
            get { return s_game.cameraPos; }
        }

        public static SpriteFont spriteFont
        {
            get { return s_game.spriteFont; }
        }

        public static KeyboardState keyState
        {
            get { return s_game.keyState; }
        }

        public static Random random
        {
            get { return s_game.random; }
        }

        public static bool gameReset
        {
            get { return s_game.gameReset; }
        }

    }
}
