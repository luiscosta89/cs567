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

namespace EndlessQuest
{
    class EnemySprite : Sprite
    {//Sprite is automated. Direction is same as speed

        public override Vector2 Direction
        {
            get { return speed; }
        }

        public EnemySprite(Texture2D textureImage, Vector2 position, Point frameSize,
                               int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, string collisionCueName)
                               : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, collisionCueName)
        {
        }

        public EnemySprite(Texture2D textureImage, Vector2 position, Point frameSize,
                                    int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, int millisecondsPerFrame, string collisionCueName)
                                    : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame, collisionCueName)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // First, move the sprite along its direction vector
            position += speed;

            //make them bounce off walls            
            if (position.X <= 0)
                speed.X *= -1;
            else if (position.X >= clientBounds.Width - frameSize.X)
                speed.X *= -1;
            if (position.Y <= 0)
                speed.Y *= -1;
            else if (position.Y >= clientBounds.Height - frameSize.Y)
                speed.Y *= -1;

            base.Update(gameTime, clientBounds);
        }
    }
}
