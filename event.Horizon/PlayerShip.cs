using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

	class PlayerShip:GameObject
	{
        private Vector2 startPos;
        
        
        public PlayerShip(Texture2D playerShipTex)
            : base(playerShipTex, new Vector2(400-(playerShipTex.Width/2), 700))
        {
            startPos = new Vector2(400 - (texture.Width / 2), 700);
        }


        public void Movement()
        {
            if (Core.keyState.IsKeyDown(Keys.Left) && position.X > 0)
            { position.X = position.X - 5; }

            if (Core.keyState.IsKeyDown(Keys.Right) && position.X < 800 - texture.Width)
            { position.X = position.X + 5; }

            if (Core.keyState.IsKeyDown(Keys.Up) && position.Y > 0)
            { position.Y = position.Y - 5; }

            if (Core.keyState.IsKeyDown(Keys.Down) && position.Y < 800 - texture.Height)
            { position.Y = position.Y + 5; }
        }


        public override void Update()
        {
            if (Core.gameReset == false)
            {
                Movement();
            }

            if (Core.gameReset == true)
            {
                position = startPos;
            }
            
        }


        public Texture2D GetTexture()
        {
            return texture;
        }


        public void SetTexture(Texture2D texture)
        {
            this.texture = texture;
        }


        public Vector2? PixelCollision(Texture2D collisionTexture)
        { 
            Color[] targetColorMap = new Color[collisionTexture.Width * collisionTexture.Height];
            Color[] playerColorMap = new Color[texture.Width * texture.Height];

            collisionTexture.GetData<Color>(targetColorMap);
            texture.GetData<Color>(playerColorMap);

            for (int y = (int)position.Y; y < position.Y + texture.Height; y++)
            {
                for (int x = (int)position.X; x < position.X + texture.Width; x++)
                {
                    int bgPos = y * collisionTexture.Width + x;
                    int playerPos = (int)((y - position.Y) * texture.Width + (x - position.X));

                    if (targetColorMap[bgPos].A > 0 && playerColorMap[playerPos].A > 0)
                    { 
                        return new Vector2(x, y);
                    }
                }
            }

            return null;
        }
	}
}
