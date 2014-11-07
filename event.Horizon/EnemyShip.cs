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

	class EnemyShip:GameObject
	{
        bool sideMovement = false;

        public EnemyShip(Texture2D enemyShipTex, Vector2 cameraPos)
            : base(enemyShipTex, new Vector2(Core.random.Next(800-enemyShipTex.Width), Core.random.Next(-800,-enemyShipTex.Height-10)))
        {
        }
       

        public void Update(Vector2 cameraPos)
        {
            position.Y += 1f;

            if (-cameraPos.Y + 800 < position.Y)
                position = new Vector2(Core.random.Next(800 - texture.Width), (Core.random.Next(-(int)cameraPos.Y - 800, -(int)cameraPos.Y - 100)));


            int move = Core.random.Next(50);
            switch (move)
            {
                case 1:
                    sideMovement = true;
                    break;
                case 2:
                    sideMovement = false;
                    break;
            }

            if (sideMovement == true)
            {
                if (position.X > 750-texture.Width)
                    sideMovement = false;
                else
                    position.X += 2;
            }

            else
            {
                if (position.X < 50)
                    sideMovement = true;
                else
                    position.X -= 2;
            }
            
        }
	}
}
