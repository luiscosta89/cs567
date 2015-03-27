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
        public bool isGameOver = false;

        private bool usedNormalAttack = false;
        private bool usedChargeAttack = false;
        private bool usedSacrificeAttack = false;
        
        private bool usedFireAttack = false;
        private bool usedWaterAttack = false;
        private bool usedNatureAttack = false;

        private bool isLevelUp = false;

        public Vector2 currentCollisionPosition;
        public Vector2 currentSpeed;

        public int index;
        public int charType;
        public int time = 0;

        public SpriteFont Font { get; set; }
        private Vector2 healthPointsPos = new Vector2(20, 10);
        private Vector2 magicPointsPos = new Vector2(20, 30);
        private Vector2 expPointsPos = new Vector2(20, 50);
        private Vector2 levelPos = new Vector2(20, 70);
                      
        //SpriteBatch for drawing        
        SpriteBatch spriteBatch;        
                
        List<EnemySprite> spriteList = new List<EnemySprite>();

        //EnemySprite boss;

        Sprite normal;
        Sprite charge;
        Sprite sacrifice;        
        Sprite fire;
        Sprite water;
        Sprite nature;

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

        public bool GetGameOver
        {
            get { return isGameOver; }
            set { isGameOver = value; }
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


        /**************************************************************************************/
            
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
            for (int i = 0; i < RandomNumber(3, 9); i++)
            {
                randomValue = RandomNumber(200, 700);
                spriteList.Insert(i, new EnemySprite(Game.Content.Load<Texture2D>(@"Images/goomba_sheet"), new Vector2(randomValue, 475),
                new Point(50, 28), 10, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));                
            }            

            // Load sprites for the attacks
            normal = new Attacks(Game.Content.Load<Texture2D>(@"Images/normal"),
                new Vector2(10, 450), new Point(30, 55), 0, new Point(0, 0), new Point(4, 1), new Vector2(0f, 0f));
            
            charge = new Attacks(Game.Content.Load<Texture2D>(@"Images/charge"),
                new Vector2(10, 450), new Point(30, 60), 0, new Point(0, 0), new Point(4, 1), new Vector2(0f, 0f));
            
            sacrifice = new Attacks(Game.Content.Load<Texture2D>(@"Images/blood"),
                new Vector2(10, 450), new Point(30, 50), 0, new Point(0, 0), new Point(8, 1), new Vector2(0f, 0f));
            
            fire = new Attacks(Game.Content.Load<Texture2D>(@"Images/fire"),            
                new Vector2(10, 450), new Point(30, 40), 0, new Point(0, 0), new Point(5, 1), new Vector2(0f, 0f));

            water = new Attacks(Game.Content.Load<Texture2D>(@"Images/water"),
                new Vector2(10, 450), new Point(30, 40), 0, new Point(0, 0), new Point(4, 1), new Vector2(0f, 0f));

            nature = new Attacks(Game.Content.Load<Texture2D>(@"Images/fire"),
                new Vector2(10, 450), new Point(50, 60), 0, new Point(0, 0), new Point(4, 1), new Vector2(0f, 0f));

                base.LoadContent();

            foreach(EnemySprite s in spriteList)
            {
                currentSpeed = s.GetSpeed;
                s.GetHealthPoints = 100;
                s.GetDamagePoints = 10;               
            }

            spriteList.Add(new EnemySprite(Game.Content.Load<Texture2D>(@"Images/bowser_sheet"), new Vector2(3000, 460),
                new Point(48, 32), 0, new Point(0, 0), new Point(4, 1), new Vector2(1, 1), "enemy_collision"));

            // Add boss sprite            
            for (int i = 0; i <= spriteList.Count - 1; i++)
            {
                if(i == spriteList.Count - 1)
                {
                    spriteList[i].GetHealthPoints = 200;
                    spriteList[i].GetDamagePoints = 35;
                }
            }           
        }

        /**************************************************************************************/
        //Soldier basic skills
        public void NormalAttack(int number)
        {
            usedNormalAttack = true;
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else
            {
                spriteList[number].GetHealthPoints -= player.GetIntelligence;
                player.GetExperience += 5;
            }
            isEnemyAlive = true;
        }

        public void ChargeAttack(int number)
        {
            usedChargeAttack = true;
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else
            {
                spriteList[number].GetHealthPoints -= player.GetDextrexity;
                player.GetDefense -= 5; //Each attack decreases by 5 the player defense
                player.GetExperience += 5;
            }
            isEnemyAlive = true;
        
        }
    
        public void SacrificeAttack(int number)
        {
            usedSacrificeAttack = true;
            if (spriteList[number].GetHealthPoints <= 0)
            {
                spriteList.RemoveAt(number);
                isEnemyAlive = false;
                collisionHappened = false;
            }
            else
            {
                spriteList[number].GetHealthPoints -= player.GetIntelligence;
                player.GetVitality -= 5; // Each attack decreases by 5 the player vitality
                player.GetExperience += 5;
            }
            isEnemyAlive = true;
        }

        //Mage special skills
        public void FireAttack(int number)
        {
            usedFireAttack = true;            
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
                player.GetExperience += 5;
            }
            isEnemyAlive = true;
        }

        public void WaterAttack(int number)
        {
            usedWaterAttack = true;
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
                player.GetExperience += 5;
            }
            isEnemyAlive = true;
                
        }
        
        public void NatureAttack(int number)
        {
            usedNatureAttack = true;
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
                player.GetExperience += 5;
            }
            isEnemyAlive = true;
        }
        
        /**************************************************************************************/
                
        public void EnemyHit(int number)
        {
            player.GetHealthPoints -= spriteList[number].GetDamagePoints;
        }
       
        /**************************************************************************************/

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;


            if (isGameOver == false)
            {
                //Update player
                player.Update(gameTime, Game.Window.ClientBounds);

                //Update attacks
                fire.Update(gameTime, Game.Window.ClientBounds);

                if (player.GetExperience >= 100)
                {
                    player.LevelUp();
                    player.GetExperience = 0;
                    isLevelUp = true;
                }

                //Update all common enemy sprites
                foreach (EnemySprite s in spriteList)
                {
                    s.Update(gameTime, Game.Window.ClientBounds);

                    //Check for collisions and exit game if there is one
                    if (s.CollisionRect.Intersects(player.CollisionRect) && s.GetHealthPoints >= 0)
                    {
                        collisionHappened = true;
                        currentCollisionPosition = s.GetPosition;
                        index = spriteList.IndexOf(s); // gets the index of the enemy                    

                        // Makes enemies stop in front of player
                        s.GetSpeed = new Vector2(0, 0);

                        // Makes the rest of the enemis stop in their current positions
                        foreach (EnemySprite t in spriteList)
                        {
                            t.GetSpeed = new Vector2(0, 0);
                        }

                        if (player.GetHealthPoints <= 0)
                        {
                            //Exits game in case player's life reaches zero                        
                            isGameOver = true;
                            //Game.Exit();
                        }
                        else
                            isGameOver = false;

                        // Play collision sound
                        //if (s.collisionCueName != null)
                        //((Game1)Game).PlayCollisionSound(s.collisionCueName);

                    }
                    else
                    {
                        s.GetSpeed = currentSpeed;
                    }
                }
            }

            base.Update(gameTime);
        }

        /**************************************************************************************/

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);       


            /**************************************************************************************/
            // Draw attacks
            if (usedNormalAttack == true)
            {
                normal.Draw(gameTime, spriteBatch);
                usedNormalAttack = false;
            }
            if (usedChargeAttack == true)
            {
                charge.Draw(gameTime, spriteBatch);
                usedChargeAttack = false;
            }
            if (usedSacrificeAttack == true)
            {
                sacrifice.Draw(gameTime, spriteBatch);
                usedSacrificeAttack = false;
            }
            if (usedFireAttack == true)
            {
                fire.Draw(gameTime, spriteBatch);
                usedFireAttack = false;
            }
            if (usedWaterAttack == true)
            {
                water.Draw(gameTime, spriteBatch);
                usedWaterAttack = false;
            }
            if (usedNatureAttack == true)
            {
                nature.Draw(gameTime, spriteBatch);
                usedNatureAttack = false;
            }
            /**************************************************************************************/

            if(isLevelUp == true)                
            {
                spriteBatch.DrawString(Font, "You reached level  " + player.GetLevel, new Vector2(200, 300), Color.Black);  
                isLevelUp = false;
            }

            /**************************************************************************************/

            if (isGameOver == false)
            {
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
                spriteBatch.DrawString(
                    Font,
                    "EXP: " + player.GetExperience.ToString(),
                    expPointsPos,
                    Color.Black);
                spriteBatch.DrawString(
                       Font,
                       "LEVEL: " + player.GetLevel.ToString(),
                       levelPos,
                       Color.Black);
            }          
            
                spriteBatch.End();   
                base.Draw(gameTime);

           // }
        }
    }
}
