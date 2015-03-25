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
        public bool isEnemyAlive = true;
        public Vector2 currentCollisionPosition;
        public Vector2 currentSpeed;
        public int index;
        public int charType;
        public int time = 0;

        public SpriteFont Font { get; set; }
        private Vector2 healthPointsPos = new Vector2(20, 10);
        private Vector2 magicPointsPos = new Vector2(20, 30);
                      
        //SpriteBatch for drawing        
        SpriteBatch spriteBatch;        
                
        List<EnemySprite> spriteList = new List<EnemySprite>();

        EnemySprite boss;

        //A sprite for the player and a list of automated sprites
        public Player GetPlayer
        {
            get { return player; }
        }

        Player player; 

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

        public int GetCharType
        {
            get { return charType; }
            set { charType = value; }
        }

        public bool GetEnemyAlive
        {
            get { return isEnemyAlive; }
            set { isEnemyAlive = value; }
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
            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));

            int rnd = rndNum.Next(min, max);

            return rnd;
        }
            
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            int randomValue;           
                        
            // Load Soldier Sprite
            if(charType == 1)
            {
                player = new Player(Game.Content.Load<Texture2D>(@"Images/soldier_sheet"),
                                              new Vector2(0, 480), new Point(28, 20), 10, new Point(0, 0), new Point(5, 1), new Vector2(0f, 0f));
            }

            // Load Mage Sprite
            else if(charType == 2)
            {
                player = new Player(Game.Content.Load<Texture2D>(@"Images/mage_sheet"),
                                             new Vector2(0, 480), new Point(28, 20), 10, new Point(0, 0), new Point(5, 1), new Vector2(0f, 0f));
            }            
            
            //Load a random number of different enemy sprites in random positions
            for (int i = 0; i < RandomNumber(3, 10); i++)
            {
                randomValue = RandomNumber(200, 750);
                spriteList.Insert(i, new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(randomValue, 475),
                new Point(50, 30), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));                
            }

            boss = new EnemySprite(Game.Content.Load<Texture2D>(@"Images/bowser_sheet"), new Vector2(3000, 360),
                new Point(58, 60), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision");

                base.LoadContent();
        }

        /**************************************************************************************/
        //Player basic skills
        public void NormalAttack(int number)
        {
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;              
            }
            else
                spriteList[number].GetHealthPoints -= player.GetIntelligence;            
        }

        public void ChargeAttack(int number)
        {
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else
                spriteList[number].GetHealthPoints -= player.GetDextrexity;
                player.GetDefense -= 5; //Each attack decreases by 5 the player defense
        }
    
        public void SacrificeAttack(int number)
        {
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else
                spriteList[number].GetHealthPoints -= player.GetIntelligence;
                player.GetVitality -= 5; // Each attack decreases by 5 the player vitality
        }

        //Player special skills
        public void FireAttack(int number)
        {
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else if (player.GetMagicPoints > 0)
            {
                spriteList[number].GetHealthPoints -= 15;
                player.GetMagicPoints -= 5;                
            }
        }

        public void WaterAttack(int number)
        {
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else if(player.GetMagicPoints > 0)
            {
                spriteList[number].GetHealthPoints -= 10;
                player.GetMagicPoints -= 5;
            }
                
        }
        
        public void NatureAttack(int number)
        {
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else if (player.GetMagicPoints > 0)
            {
                spriteList[number].GetHealthPoints -= 5;
                player.GetMagicPoints -= 5;
            }
        }
        
        /**************************************************************************************/
                
        
        public void EnemyHit(int number)
        {
            player.GetHealthPoints -= spriteList[number].GetDamagePoints;
        }

        public void BossHit()
        {
            player.GetHealthPoints -= boss.GetDamagePoints;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;            
            
            //Update player
            player.Update(gameTime, Game.Window.ClientBounds);

            //Update boss
            boss.Update(gameTime, Game.Window.ClientBounds); 
           
            // Saves current speed of enemy sprites
            //foreach(EnemySprite w in spriteList)
            //{
                //currentSpeed = w.GetSpeed;
                //System.Console.WriteLine("\n X: " + currentSpeed.X);
                //System.Console.WriteLine("\n Y: " + currentSpeed.Y);
           // }

            //Update all common enemy sprites
            foreach (EnemySprite s in spriteList)
            {
                //s.GetSpeed = currentSpeed;
                s.Update(gameTime, Game.Window.ClientBounds);

                currentSpeed = s.GetSpeed;
                //System.Console.WriteLine("\n X: " + currentSpeed.X);
                //System.Console.WriteLine("\n Y: " + currentSpeed.Y);

                //Check for collisions and exit game if there is one
                if (s.CollisionRect.Intersects(player.CollisionRect) && s.GetHealthPoints >= 0)
                {
                    collisionHappened = true;
                    currentCollisionPosition = s.GetPosition;
                    index = spriteList.IndexOf(s); // gets the index of the enemy
                    //System.Console.WriteLine("I'm inside the collision!");
                    
                    // Makes enemies stop in front of player
                    s.GetSpeed = new Vector2(0, 0);
                    
                    // As well as the stage boss
                    boss.GetSpeed = new Vector2(0, 0);                   
                    
                    // Makes the rest of the enemis stop in their current positions
                    foreach(EnemySprite t in spriteList)
                    {
                        t.GetSpeed = new Vector2(0, 0);                         
                    }               

                    if (player.GetHealthPoints <= 0)
                        //Exits game in case player's life reaches zero
                        Game.Exit();
                    // Play collision sound
                    //if (s.collisionCueName != null)
                        //((Game1)Game).PlayCollisionSound(s.collisionCueName);

                }

                //s.GetSpeed = currentSpeed;

                    //Check collision with boss
                    if (boss.CollisionRect.Intersects(player.CollisionRect))
                    {
                        collisionHappened = true;
                    }                    
            }

            time = time + 1;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //Draw the player
            player.Draw(gameTime, spriteBatch);

            boss.Draw(gameTime, spriteBatch);

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
