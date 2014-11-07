using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace @event.Horizon
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        
        public SpriteBatch spriteBatch;
        public SpriteFont spriteFont;
        public KeyboardState keyState;
        public Random random;
        public Vector2 cameraPos;
        public bool gameReset;
        private bool playerAlive = true;
        private Texture2D myTargetTexture, gameStartScreen, gameEndScreen, explosion;
        private Matrix camera;
        private RenderTarget2D renderTarget;
        private PresentationParameters pp; 
        private PlayerShip playerShip;
        private Texture2D playerShipTex, enemyShipTex;

        private float currentScore, scoreTimer;

        private List<Texture2D> Map = new List<Texture2D>();
        private List<float> MapYPos = new List<float>();
        private List<Texture2D> Background = new List<Texture2D>();
        private List<float> BackgroundYPos = new List<float>();
        private List<EnemyShip> enemyShip = new List<EnemyShip>();

        enum GameState { GameMenu, InGame, GameOver }
        GameState currentGameState = GameState.GameMenu;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
        }


        protected override void Initialize()
        {
            camera = Matrix.CreateTranslation(0, cameraPos.Y, 0);
            base.Initialize();
        }


        protected void BackgroundLoad()
        {
            Background.Add(Content.Load<Texture2D>("Bakgrund"));
            BackgroundYPos.Add(0);
            Background.Add(Content.Load<Texture2D>("Bakgrund"));
            BackgroundYPos.Add(-800);
        }


        protected void MapLoad()
        {
            for (int i = 0; i < 3; i++)
            {
                MapYPos.Add(-(i + 1) * 800);
                Map.Add(Content.Load<Texture2D>("map" + (i + 1)));
            }
        }


        protected void EnemyLoad()
        {
            for (int i = 0; i < 5; i++)
            {
                enemyShip.Add(new EnemyShip(enemyShipTex, cameraPos));
            }
        }


        protected override void LoadContent()
        {
            Core.Initialize(this);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("SpriteFont");
            playerShipTex = Content.Load<Texture2D>("playerShip");
            enemyShipTex = Content.Load<Texture2D>("enemyShip");
            gameStartScreen = Content.Load<Texture2D>("GameStartScreen");
            gameEndScreen = Content.Load<Texture2D>("GameEndScreen");
            explosion = Content.Load<Texture2D>("Explosion");
            pp = GraphicsDevice.PresentationParameters;
            random = new Random();
      
            renderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Color);  
            playerShip = new PlayerShip(playerShipTex);

            BackgroundLoad();
            MapLoad();
            EnemyLoad();
            
        }


        protected override void UnloadContent()
        {   }


        protected void DrawToRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(0, renderTarget);
            GraphicsDevice.Clear(Color.TransparentBlack);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, camera);

            for (int i = 0; i < Map.Count; i++)
                spriteBatch.Draw(Map[i], new Vector2(0, MapYPos[i]), Color.White);

            foreach (EnemyShip ship in enemyShip)
            {
                ship.Draw();
            }

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(0, null);
            myTargetTexture = renderTarget.GetTexture();
        }


        protected void Score()
        {
            scoreTimer += 1;
            if (scoreTimer == 50)
            {
                scoreTimer = 0;
                currentScore += 1;
            }
        }


        protected void BackgroundUpdate()
        {
            for (int i = 0; i < BackgroundYPos.Count; i++)
                BackgroundYPos[i] += 1;

            if (BackgroundYPos[0] >= 800)
            {
                BackgroundYPos.RemoveAt(0);
                Background.Add(Background[0]);
                BackgroundYPos.Add(BackgroundYPos[BackgroundYPos.Count - 1] - 800);
                Background.RemoveAt(0);
            }
        }


        protected void MapUpdate()
        {
            if (MapYPos[0] >= -cameraPos.Y + 800)
            {
                MapYPos.RemoveAt(0);

                Map.Add(Content.Load<Texture2D>("map" + Core.random.Next(1, 3)));
                MapYPos.Add(MapYPos[MapYPos.Count - 1] - 800);
                Map.RemoveAt(0);
            }
        }

        //Handles Update for the Gamestates
        #region Update Gamestates

        //Handles the Update in the menu
        protected void UpdateGameMenu()
        {
            if (Core.keyState.IsKeyDown(Keys.Enter))
            { 
                currentGameState = GameState.InGame;
            }
        }

        //Handles the Update ingame
        protected void UpdateInGame()
        {
            camera = Matrix.CreateTranslation(0, cameraPos.Y, 0);
            if (playerAlive == true)
            {
                cameraPos.Y += 2;
                
                Score();
                MapUpdate();
                playerShip.Update();

                foreach (EnemyShip ship in enemyShip)
                { ship.Update(cameraPos); }

                if (playerShip.PixelCollision(myTargetTexture) != null)
                {
                    playerAlive = false;    
                }
            }
            else
            {
                cameraPos.Y += 0;
                playerShip.SetTexture(explosion);
                gameReset = true;
                currentGameState = GameState.GameOver;
            }
        }

        //Handles the Update at gameover
        protected void UpdateGameOver()
        {
            if (gameReset == true)
            {
                cameraPos = new Vector2(0, 0);
                playerShip.SetTexture(playerShipTex);
                playerAlive = true;
                MapUpdate();
                MapLoad();
                enemyShip.Clear();
                EnemyLoad();
                playerShip.Update();
                gameReset = false;
            }

            if (Core.keyState.IsKeyDown(Keys.Enter))
            {
                currentScore = 0;
                currentGameState = GameState.InGame;
            }

            if (Core.keyState.IsKeyDown(Keys.Escape))
            {
                Core.EndGame();
            }
        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            keyState = Keyboard.GetState();

            BackgroundUpdate();

            switch (currentGameState)
            {
                case GameState.GameMenu:
                    UpdateGameMenu();
                    break;

                case GameState.InGame:
                    UpdateInGame();
                    break;

                case GameState.GameOver:
                    UpdateGameOver();  
                    break;
            }

            base.Update(gameTime);
        }

        //Handles Draw in the Gamestates
        #region Draw Gamestates

        //Handles the Draw in the menu
        protected void DrawGameMenu()
        {
            for (int i = 0; i < Background.Count; i++)
                spriteBatch.Draw(Background[i],
                    new Vector2(0, BackgroundYPos[i]),
                    Color.White);

            spriteBatch.Draw(gameStartScreen,
                Vector2.Zero,
                Color.White);

            playerShip.Draw();
        }
        
        //Handles the Draw ingame
        protected void DrawInGame()
        {
            spriteBatch.Draw(myTargetTexture,
                        Vector2.Zero,
                        Color.White);

            spriteBatch.DrawString(spriteFont,
                "Score: " + currentScore,
                new Vector2(5, 2),
                Color.White);

            playerShip.Draw();
        }

        //Handles the Draw at gameover
        protected void DrawGameOver()
        {
            spriteBatch.DrawString(spriteFont,
                        "Final Score: " + currentScore.ToString(),
                        new Vector2(Window.ClientBounds.Width / 2 - spriteFont.MeasureString("Final Score: XXX").X / 2, Window.ClientBounds.Height / 2 - 50),
                        Color.White);

            spriteBatch.Draw(gameEndScreen,
                Vector2.Zero,
                Color.White);
        }
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            DrawToRenderTarget();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            for (int i = 0; i < Background.Count; i++)
                spriteBatch.Draw(Background[i],
                    new Vector2(0, BackgroundYPos[i]),
                    Color.White);

            switch (currentGameState)
            {
                case GameState.GameMenu:
                    DrawGameMenu();
                    break;

                case GameState.InGame:
                    DrawInGame();
                    break;

                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    
    }
}
