/* Code writen by Luis Antonio Costa */
/* WIU ID: 914292829 */

/* CS 567U - Midterm Project */

/* ---------------------------------------- ENDLESS QUEST ----------------------------------------- */

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

namespace EndlessQuest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
              
        public Random rnd { get; private set; }

        private TimeSpan intervalBetweenAttack;
        private TimeSpan lastTimeAttack;

        // Music volume.
        //float musicVolume = 1.0f;

        /**************************************************************************************/
        // Global variables for buttons
        enum BState
        {
            HOVER,
            UP,
            JUST_RELEASED,
            DOWN
        }
        const int NUMBER_OF_BUTTONS = 7,
            PLAY_BUTTON_INDEX = 0,
            QUIT_BUTTON_INDEX = 1,
            SOLDIER_BUTTON_INDEX = 2,
            MAGE_BUTTON_INDEX = 3,
            SKILL1_BUTTON_INDEX = 4,
            SKILL2_BUTTON_INDEX = 5,
            SKILL3_BUTTON_INDEX = 6,
            BUTTON_HEIGHT = 100,
            BUTTON_WIDTH = 200;
        Color background_color;
        Color[] button_color = new Color[NUMBER_OF_BUTTONS];
        Rectangle[] button_rectangle = new Rectangle[NUMBER_OF_BUTTONS];
        BState[] button_state = new BState[NUMBER_OF_BUTTONS];
        Texture2D[] button_texture = new Texture2D[NUMBER_OF_BUTTONS];
        double[] button_timer = new double[NUMBER_OF_BUTTONS];
        //mouse pressed and mouse just pressed
        bool mpressed, prev_mpressed = false;
        //mouse location in window
        int mx, my;
        double frame_time;

        /**************************************************************************************/
        
        //Menu components
        public enum GameState
        {
            MainMenu,
            Playing,
            Selection,                     
        }
        GameState CurrentGameState = GameState.MainMenu;

        // Screen adjustments
        int screenWidth = 800, screenHeight = 600;        

        public SpriteManager spriteManager;

        //Audio components
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue currentSong;       

        // List of layers
        private List<ParallaxLayer> layers;
            
        float firstBackgroundSpeed = 100f;
        float secondBackgroundSpeed = 0.3f;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            rnd = new Random();          
        }

        /**************************************************************************************/

        public int RandomNumber(int min, int max)
        {
            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));

            int rnd = rndNum.Next(min, max);

            return rnd;
        }        
        
        // Play Music Cues
        public void PlayCollisionSound(string cueName)
        {
            soundBank.PlayCue("Actions");           
        }      

        public void PlayAttackSound()
        {
            soundBank.PlayCue("EnemyAttack");
        }

        public void PlaySkill1Sounds()
        {
            if (spriteManager.GetCharType == 1)
                soundBank.PlayCue("NormalAttack");
            else
                soundBank.PlayCue("FireAttack");
        }

        public void PlaySkill2Sounds()
        {
            if (spriteManager.GetCharType == 1)
                soundBank.PlayCue("ChargeAttack");
            else
                soundBank.PlayCue("WaterAttack");
        }

        public void PlaySkill3Sounds()
        {
            if (spriteManager.GetCharType == 1)
                soundBank.PlayCue("SacrificeAttack");
            else
                soundBank.PlayCue("NatureAttack");
        }

        /**************************************************************************************/                   
              
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            spriteManager = new SpriteManager(this);                      

            // starting x and y locations to stack buttons 
            // vertically in the middle of the screen
            int x1 = 300;
            int y1 = 400;

            int x2 = 300;
            int y2 = 100;            
            
            int x3 = 600;
            int y3 = 100;
            
            //For menu buttons
            for (int i = 0; i < 2; i++)
            {
                button_state[i] = BState.UP;
                button_color[i] = Color.White;
                button_timer[i] = 0.0;
                button_rectangle[i] = new Rectangle(x1, y1, BUTTON_WIDTH, BUTTON_HEIGHT);
                y1 += BUTTON_HEIGHT;
            }
                        
            //For selection buttons
                                
                button_state[SOLDIER_BUTTON_INDEX] = BState.UP;
                button_color[SOLDIER_BUTTON_INDEX] = Color.White;
                button_timer[SOLDIER_BUTTON_INDEX] = 0.0;
                button_rectangle[SOLDIER_BUTTON_INDEX] = new Rectangle(x2, y2, BUTTON_WIDTH, BUTTON_HEIGHT+30);
                y2 += BUTTON_HEIGHT;

                button_state[MAGE_BUTTON_INDEX] = BState.UP;
                button_color[MAGE_BUTTON_INDEX] = Color.White;
                button_timer[MAGE_BUTTON_INDEX] = 0.0;
                button_rectangle[MAGE_BUTTON_INDEX] = new Rectangle(x2, y2, BUTTON_WIDTH, BUTTON_HEIGHT+30);
                y2 += BUTTON_HEIGHT;
            

            //For skill buttons
            for (int i = 4; i < NUMBER_OF_BUTTONS; i++)
            {
                button_state[i] = BState.UP;
                button_color[i] = Color.White;
                button_timer[i] = 0.0;
                button_rectangle[i] = new Rectangle(x3, y3, BUTTON_WIDTH, BUTTON_HEIGHT);
                y3 += BUTTON_HEIGHT;
            }

            IsMouseVisible = true;
            background_color = Color.CornflowerBlue;
           
            /**************************************************************************************/
            
            this.layers = new List<ParallaxLayer>();           
            
                
            ParallaxLayer firstLayer = new ParallaxLayer(firstBackgroundSpeed * secondBackgroundSpeed, 0);
            ParallaxLayer secondLayer = new ParallaxLayer(20, 0);
            ParallaxLayer thirdLayer = new ParallaxLayer(20, 0);
            ParallaxLayer fourthLayer = new ParallaxLayer(60, 0);
            ParallaxLayer fifthLayer = new ParallaxLayer(10, 0);

            // Add layers
            this.layers.Add(firstLayer);
            this.layers.Add(secondLayer);
            this.layers.Add(thirdLayer);
            this.layers.Add(fourthLayer);
            this.layers.Add(fifthLayer);
                        
            // Initialize a random time between 3 and 9 seconds
            intervalBetweenAttack = TimeSpan.FromMilliseconds(RandomNumber(3000, 9000));
            
            base.Initialize();
        }

        public void Quit()
        {
            this.Exit();
        }        
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteManager = new SpriteManager(this);

            // Load the Font for the HUD
            spriteManager.Font = Content.Load<SpriteFont>(@"Fonts\Arial");

            /**************************************************************************************/
            
            button_texture[PLAY_BUTTON_INDEX] = Content.Load<Texture2D>(@"Images\play_button");
            button_texture[QUIT_BUTTON_INDEX] = Content.Load<Texture2D>(@"Images\quit_button");
            button_texture[SOLDIER_BUTTON_INDEX] = Content.Load<Texture2D>(@"Images\soldier_button");
            button_texture[MAGE_BUTTON_INDEX] = Content.Load<Texture2D>(@"Images\mage_button");
            button_texture[SKILL1_BUTTON_INDEX] = Content.Load<Texture2D>(@"Images\skill1_button");
            button_texture[SKILL2_BUTTON_INDEX] = Content.Load<Texture2D>(@"Images\skill2_button");
            button_texture[SKILL3_BUTTON_INDEX] = Content.Load<Texture2D>(@"Images\skill3_button");
            
           /**************************************************************************************/
            
            // Load audio content
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            
            // Load musics            
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    {                        
                        currentSong = soundBank.GetCue("Menu");
                        Settings.musicVolume = 0.1f;
                        audioEngine.GetCategory("Music").SetVolume(Settings.musicVolume);
                        currentSong.Play();
                        break;
                    }
            }                     

            /**************************************************************************************/

            // Load the layers
            this.layers[0].LoadContent(Content, @"Images\l1");
            this.layers[1].LoadContent(Content, @"Images\l2");
            this.layers[2].LoadContent(Content, @"Images\l3");
            this.layers[3].LoadContent(Content, @"Images\l4");
            this.layers[4].LoadContent(Content, @"Images\l5");

            /**************************************************************************************/

            // Starts playing background music
            // soundBank.PlayCue("overworld_theme");

            /**************************************************************************************/
            // Screen stuff for the menu
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            IsMouseVisible = true;

            /**************************************************************************************/           
            
        }       

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            /**************************************************************************************/
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Allows the game to exit via keyboard
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();           

            /**************************************************************************************/                          
                       
            // Update layers
            if (spriteManager.GetCollision == false)
            {
                foreach (ParallaxLayer layer in this.layers)
                {
                    layer.Update(gameTime, this);
                }
            }
            
            /**************************************************************************************/

            // Does the battle logic with normal enemies
            if (spriteManager.GetCollision == true)
            {
                // Enemy strikes player while the target has health and the enemy is alive
                if (spriteManager.GetPlayer.GetHealthPoints > 0 && 
                   (lastTimeAttack + intervalBetweenAttack < gameTime.TotalGameTime) &&
                    spriteManager.GetEnemyAlive)
                {
                        spriteManager.EnemyHit(spriteManager.GetIndex);
                        PlayAttackSound();
                        lastTimeAttack = gameTime.TotalGameTime;                   
                    }
            }         
                        
                   
            /**************************************************************************************/

            // TODO: Add your update logic here

            // get elapsed frame time in seconds
            frame_time = gameTime.ElapsedGameTime.Milliseconds / 1000.0;

            // update mouse variables
            MouseState mouse_state = Mouse.GetState();
            mx = mouse_state.X;
            my = mouse_state.Y;
            prev_mpressed = mpressed;
            mpressed = mouse_state.LeftButton == ButtonState.Pressed;

            update_buttons();                      

            base.Update(gameTime);
        }

        public void LoadSprites()
        {
            Components.Add(spriteManager);
        }

        // Wrapper for hit_image_alpha taking Rectangle and Texture
        Boolean hit_image_alpha(Rectangle rect, Texture2D tex, int x, int y)
        {
            return hit_image_alpha(0, 0, tex, tex.Width * (x - rect.X) /
                rect.Width, tex.Height * (y - rect.Y) / rect.Height);
        }

        // Wraps hit_image then determines if hit a transparent part of image 
        Boolean hit_image_alpha(float tx, float ty, Texture2D tex, int x, int y)
        {
            if (hit_image(tx, ty, tex, x, y))
            {
                uint[] data = new uint[tex.Width * tex.Height];
                tex.GetData<uint>(data);
                if ((x - (int)tx) + (y - (int)ty) *
                    tex.Width < tex.Width * tex.Height)
                {
                    return ((data[
                        (x - (int)tx) + (y - (int)ty) * tex.Width
                        ] &
                                0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }

        // Determine if x,y is within rectangle formed by texture located at tx,ty
        Boolean hit_image(float tx, float ty, Texture2D tex, int x, int y)
        {
            return (x >= tx &&
                x <= tx + tex.Width &&
                y >= ty &&
                y <= ty + tex.Height);
        }
       
        // Determine state and color of button
        void update_buttons()
        {
            for (int i = 0; i < NUMBER_OF_BUTTONS; i++)
            {

                if (hit_image_alpha(button_rectangle[i], button_texture[i], mx, my))
                {
                    button_timer[i] = 0.0;
                    if (mpressed)
                    {
                        // mouse is currently down
                        button_state[i] = BState.DOWN;
                        button_color[i] = Color.Blue;
                    }
                    else if (!mpressed && prev_mpressed)
                    {
                        // mouse was just released
                        if (button_state[i] == BState.DOWN)
                        {
                            // button i was just down
                            button_state[i] = BState.JUST_RELEASED;
                        }
                    }
                    else
                    {
                        button_state[i] = BState.HOVER;
                        button_color[i] = Color.LightBlue;
                    }
                }
                else
                {
                    button_state[i] = BState.UP;
                    if (button_timer[i] > 0)
                    {
                        button_timer[i] = button_timer[i] - frame_time;
                    }
                    else
                    {
                        button_color[i] = Color.White;
                    }
                }

                if (button_state[i] == BState.JUST_RELEASED)
                {
                    take_action_on_button(i);
                }
            }
        }

        // Logic for each button click goes here
        void take_action_on_button(int i)
        {
            //take action corresponding to which button was clicked
            switch (i)
            {
                case PLAY_BUTTON_INDEX:
                    
                        //Stops current music and plays level music
                        CurrentGameState = GameState.Selection;
                        Settings.musicVolume = 1.0f;                        
                        currentSong.Stop(AudioStopOptions.AsAuthored);
                        currentSong = soundBank.GetCue("Selection");
                        currentSong.Play();
                        Settings.musicVolume = 1.0f;

                        break;                   
                
                case QUIT_BUTTON_INDEX:
                    
                    this.Exit();
                    break;
                case SKILL1_BUTTON_INDEX:
                    if (spriteManager.GetCollision && spriteManager.GetCharType == 1)
                    {
                        spriteManager.NormalAttack(spriteManager.GetIndex);
                        //After each attack, the player earns experience points
                        PlaySkill1Sounds();                        
                    }
                    else if (spriteManager.GetCollision && spriteManager.GetCharType == 2 && spriteManager.GetPlayer.GetMagicPoints > 0)
                    {
                        spriteManager.FireAttack(spriteManager.GetIndex);
                        //After each attack, the player earns experience points
                        PlaySkill1Sounds();                        
                    }
                    spriteManager.GetCollision = false;                       
                    break;
                case SKILL2_BUTTON_INDEX:
                    if (spriteManager.GetCollision && spriteManager.GetCharType == 1)
                    {
                        spriteManager.ChargeAttack(spriteManager.GetIndex);
                        //After each attack, the player earns experience points                        
                        PlaySkill2Sounds();                        
                    }
                    else if (spriteManager.GetCollision && spriteManager.GetCharType == 2 && spriteManager.GetPlayer.GetMagicPoints > 0)
                    {
                        spriteManager.WaterAttack(spriteManager.GetIndex);
                        //After each attack, the player earns experience points                        
                        PlaySkill2Sounds();                        
                    }
                    spriteManager.GetCollision = false;               
                    break;
                case SKILL3_BUTTON_INDEX:
                    if (spriteManager.GetCollision && spriteManager.GetCharType == 1)
                    {
                        spriteManager.SacrificeAttack(spriteManager.GetIndex);
                        //After each attack, the player earns experience points
                        PlaySkill3Sounds();                       
                    }
                    else if (spriteManager.GetCollision && spriteManager.GetCharType == 2 && spriteManager.GetPlayer.GetMagicPoints > 0)
                    {
                        spriteManager.NatureAttack(spriteManager.GetIndex);
                        //After each attack, the player earns experience points
                        PlaySkill3Sounds();                        
                    }
                    spriteManager.GetCollision = false;                    
                    break;
                case SOLDIER_BUTTON_INDEX:
                    {
                        spriteManager.GetCharType = 1; // 1 for selecting Soldier
                        CurrentGameState = GameState.Playing;
                                             
                        currentSong.Stop(AudioStopOptions.AsAuthored);
                        currentSong = soundBank.GetCue("Levels");
                        currentSong.Play();
                        Settings.musicVolume = 1.0f;
                        
                        LoadSprites();                                                              

                        break;
                    }
                case MAGE_BUTTON_INDEX:
                    {
                        spriteManager.GetCharType = 2; // 2 for selecting Mage
                        
                        currentSong.Stop(AudioStopOptions.AsAuthored);
                        currentSong = soundBank.GetCue("Levels");
                        currentSong.Play();
                        Settings.musicVolume = 1.0f;
                        
                        LoadSprites();                                                  

                        break;
                    }
                
                default:
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
               

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"Images\menu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                        spriteBatch.Draw(button_texture[PLAY_BUTTON_INDEX], button_rectangle[PLAY_BUTTON_INDEX], button_color[PLAY_BUTTON_INDEX]);
                        spriteBatch.Draw(button_texture[QUIT_BUTTON_INDEX], button_rectangle[QUIT_BUTTON_INDEX], button_color[QUIT_BUTTON_INDEX]);
                        spriteBatch.DrawString(spriteManager.Font, "Game coded by Luis Antonio Costa", new Vector2(220, 60), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "Artwork by Atilla Gallio", new Vector2(260, 80), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "Version 1.0", new Vector2(650, 550), Color.Black); 
                        break;                        
                    }
                
               case GameState.Selection:
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"Images\background"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                        spriteBatch.Draw(button_texture[SOLDIER_BUTTON_INDEX], button_rectangle[SOLDIER_BUTTON_INDEX], button_color[SOLDIER_BUTTON_INDEX]);
                        spriteBatch.Draw(button_texture[MAGE_BUTTON_INDEX], button_rectangle[MAGE_BUTTON_INDEX], button_color[MAGE_BUTTON_INDEX]);
                        spriteBatch.DrawString(spriteManager.Font, "Choose your character class", new Vector2(250, 50), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "Instructions to play:", new Vector2(280, 360), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "1 - Your character will run on enemies, be prepared to fight!", new Vector2(50, 400), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "2 - Select one skill of your character to attack your enemy fast!", new Vector2(50, 420), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "3 - Some skills from the soldier cost Health Points!", new Vector2(50, 440), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "4 - All the skills from the mage cost Magic Points!", new Vector2(50, 460), Color.Black);
                        spriteBatch.DrawString(spriteManager.Font, "5 - After earning 100 Experience Points you will increase level!", new Vector2(50, 480), Color.Black);
                        
                        break;
                    }

               case GameState.Playing:
                    {                      

                        if (spriteManager.isGameOver == false)
                        {
                            foreach (ParallaxLayer layer in this.layers)
                            {
                                layer.Draw(spriteBatch);
                            }
                            // Draw buttons
                            spriteBatch.Draw(button_texture[SKILL1_BUTTON_INDEX], button_rectangle[SKILL1_BUTTON_INDEX], button_color[SKILL1_BUTTON_INDEX]);
                            spriteBatch.Draw(button_texture[SKILL2_BUTTON_INDEX], button_rectangle[SKILL2_BUTTON_INDEX], button_color[SKILL2_BUTTON_INDEX]);
                            spriteBatch.Draw(button_texture[SKILL3_BUTTON_INDEX], button_rectangle[SKILL3_BUTTON_INDEX], button_color[SKILL3_BUTTON_INDEX]);
                            
                        }
                        else
                        {
                            spriteBatch.Draw(Content.Load<Texture2D>(@"Images\game_over"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                            spriteBatch.DrawString(spriteManager.Font, "GAME OVER!", new Vector2(350, 200), Color.Red);
                            spriteBatch.DrawString(spriteManager.Font, "The night has and your quest is over...", new Vector2(200, 250), Color.White);
                            currentSong.Stop(AudioStopOptions.AsAuthored);                           
                        }
                        break;
                    }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
