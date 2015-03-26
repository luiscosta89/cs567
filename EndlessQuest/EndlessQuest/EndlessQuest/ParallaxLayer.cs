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
//using EndlessQuest.Sprite;
//using EndlessQuest.Player;

namespace EndlessQuest
{
    class ParallaxLayer
    {
        private Texture2D image;
        private Vector2 position;
        private Vector2 speed;
        

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public ParallaxLayer(float velX, float velY)
        {
            this.position = new Vector2(0, 0);
            this.speed = new Vector2(velX, velY);
        }

        public void LoadContent(ContentManager content, string filename)
        {
            this.image = content.Load<Texture2D>(filename);
        }

        public void Update(GameTime gameTime, Game1 game)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Use speed from player but this update must be after the collision detection

            this.position.X -= this.speed.X * deltaTime;
            this.position.X = this.position.X % this.image.Width;            
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(this.image, this.position, Color.White);
            batch.Draw(this.image, new Vector2(this.position.X + this.image.Width, 0), Color.White);
        }

        public float player { get; set; }
    }
}
