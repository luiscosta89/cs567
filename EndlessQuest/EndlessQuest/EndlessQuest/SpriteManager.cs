using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;

namespace EndlessQuest
{
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public bool collisionHappened = false;
        public Vector2 currentCollisionPosition;
        public int index;       

        public SpriteFont Font { get; set; }
        private Vector2 healthPointsPos = new Vector2(20, 10);
        private Vector2 magicPointsPos = new Vector2(20, 30);
                      
        //SpriteBatch for drawing
        SpriteBatch spriteBatch;
        
        //A sprite for the player and a list of automated sprites
        public Player Player
        {
            get { return player; }
        }
                
        Player player;     
        
        List<EnemySprite> spriteList = new List<EnemySprite>();

        // Returns is collision happened
        public bool GetCollision
        {
            get { return collisionHappened;}
            set { collisionHappened = value; }
        }

        public Vector2 GetCurrentCollisionPosition
        {
            get { return currentCollisionPosition; }
        }

        public int GetIndex
        {
            get { return index; }
            set { index = value; }
        }

        float timer = 10;         //Initialize a 10 second timer
        const float TIMER = 10;
        
        public SpriteManager(Game game)
            : base(game)
        {
            // TODO : Construct any child components here
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);


            
            //Font = Content.Load<SpriteFont>(@"Fonts\Arial");

            //Load the player sprite
            player = new Player(Game.Content.Load<Texture2D>(@"Images/char_spritesheet"),
                                              new Vector2(0, 515), new Point(28, 20), 10, new Point(0, 0), new Point(5, 1), new Vector2(1, 1));

            //Load several different automated sprites into the list
            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(150, 510),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(300, 510),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(450, 510),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(600, 510),
                           new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));
            base.LoadContent();
        }

        /**************************************************************************************/
        //Player basic skills
        public void NormalAttack(int number)
        {
            if (spriteList[number].GetHealthPoints == 0)
                spriteList.RemoveAt(number);
            else
                spriteList[number].GetHealthPoints -= player.GetIntelligence;            
        }

        public void ChargeAttack(int number)
        {
            if (spriteList[number].GetHealthPoints == 0)
                spriteList.RemoveAt(number);
            else
                spriteList[number].GetHealthPoints -= player.GetDextrexity;
                player.GetDefense -= 5; //Each attack decreases by 5 the player defense
        }
    
        public void SacrificeAttack(int number)
        {
            if (spriteList[number].GetHealthPoints == 0)
                spriteList.RemoveAt(number);
            else
                spriteList[number].GetHealthPoints -= player.GetIntelligence;
                player.GetVitality -= 5; // Each attack decreases by 5 the player vitality
        }
        
        
        /**************************************************************************************/
                
        
        public void EnemyHit(int number)
        {
            player.GetHealthPoints -= spriteList[number].GetDamagePoints;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;            
            
            //Update player
            player.Update(gameTime, Game.Window.ClientBounds);

            //Update all sprites
            foreach (EnemySprite s in spriteList)
            {
                s.Update(gameTime, Game.Window.ClientBounds);

                //Check for collisions and exit game if there is one
                if (s.CollisionRect.Intersects(player.CollisionRect))
                {
                    collisionHappened = true;
                    //currentCollisionPosition = player.Direction;
                    index = spriteList.IndexOf(s); // gets the index of the enemy

                    //TODO logic of combat here

                    //if (lastTimeAttack + intervalBetweenAttack1 < gameTime.TotalGameTime)
                        while(player.GetHealthPoints >= 0)
                        {
                            EnemyHit(index);                            
                            //lastTimeAttack = gameTime.TotalGameTime;
                        }                                 
                   

                    if (player.GetHealthPoints <= 0)
                        //Exits game in case player's life reaches zero
                        //Game.Exit();
                    // Play collision sound
                    if (s.collisionCueName != null)
                        ((Game1)Game).PlayCue(s.collisionCueName);
                }
                //collisionHappened = false;

                /*if (timer < 0)
                {
                    EnemyHit(index);
                    timer = TIMER;   //Reset Timer
                }*/
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

            // Draw the Score in the top-left of screen
            spriteBatch.DrawString(
                Font,                          // SpriteFont
                "HP: " + player.GetHealthPoints.ToString(),  // Text
                healthPointsPos,                      // Position
                Color.Black);                  // Tint
            spriteBatch.DrawString(
                Font,
                "MP: " + player.GetMagicPoints.ToString(),
                magicPointsPos,
                Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
