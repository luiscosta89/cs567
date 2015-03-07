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
    abstract class Sprite
    {
        //Information to draw the sprite
        Texture2D textureImage;
        protected Point frameSize;
        Point currentFrame;
        Point sheetSize;

        //Colision data
        int collisionOffset;

        //Framerate data
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;

        //Movement data
        protected Vector2 speed;
        protected Vector2 position;

        //Abstract definition of direction properly
        public abstract Vector2 Direction
        {
            get;
        }

        public string collisionCueName { get; private set; }

        //Constructors
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize, 
                      int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, string collisionCueName)
                      : this(textureImage, position, frameSize, collisionOffset, currentFrame,
                      sheetSize, speed, defaultMillisecondsPerFrame, collisionCueName)
        {
        }

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
                      int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
                      int millisecondsPerFrame, string collisionCueName)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.collisionCueName = collisionCueName;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                             Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
        
        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if(timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if(currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        //Gets the collision rect based on position, framesize and collision offset
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)position.X + collisionOffset, (int)position.Y + collisionOffset,
                                     frameSize.X - (collisionOffset * 2), frameSize.Y - (collisionOffset * 2));
            }
        }
     }
}
