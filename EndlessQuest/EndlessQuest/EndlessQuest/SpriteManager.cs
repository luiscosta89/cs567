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

namespace EndlessQuest
{
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //SpriteBatch for drawing
        SpriteBatch spriteBatch;

        Camera camera;

        //A sprite for the player and a list of automated sprites
        
        Player player;
        List<Sprite> spriteList = new List<Sprite>();

        public float spriteScale = 0.5f;

        public SpriteManager(Game game)
            : base(game)
        {
            // TODO : Construct any child components here
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            //Load the player sprite
            player = new Player(Game.Content.Load<Texture2D>(@"Images/char_spritesheet"),
                                              new Vector2(0, 530), new Point(28, 20), 10, new Point(0, 0), new Point(5, 1), new Vector2(3, 3));

            //Load several different automated sprites into the list
            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(150, 530),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            /*spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(300, 150),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(150, 300),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(600, 150),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            base.LoadContent();*/
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //Update player
            player.Update(gameTime, Game.Window.ClientBounds);

            camera.Update(position.X, clientBounds.Width, clientBounds.Height);

            //Update all sprites
            foreach (Sprite s in spriteList)
            {
                s.Update(gameTime, Game.Window.ClientBounds);

                //Check for collisions and exit game if there is one
                if (s.CollisionRect.Intersects(player.CollisionRect))
                {                       
                    // Play collision sound
                    if (s.collisionCueName != null)
                        ((Game1)Game).PlayCue(s.collisionCueName);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //Draw the player
            player.Draw(gameTime, spriteBatch);

            //Draw all sprites
            foreach (Sprite s in spriteList)
                s.Draw(gameTime, spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
