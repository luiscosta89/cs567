/* Code writen by Luis Antonio Costa */
/* WIU ID: 914292829 */

/* CS 567U - Midterm Project */

/* ---------------------------------------- ENDLESS QUEST ----------------------------------------- */

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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public Random rnd { get; private set; }


        /**************************************************************************************/
        // Global variables for buttons
        enum BState
        {
            HOVER,
            UP,
            JUST_RELEASED,
            DOWN
        }
        const int NUMBER_OF_BUTTONS = 5,
            PLAY_BUTTON_INDEX = 0,
            QUIT_BUTTON_INDEX = 1,
            HAB1_BUTTON_INDEX = 2,
            HAB2_BUTTON_INDEX = 3,
            HAB3_BUTTON_INDEX = 4,
            BUTTON_HEIGHT = 40,
            BUTTON_WIDTH = 88;
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
        enum GameState
        {
            MainMenu,
            Playing,
            Intro,
            Selection,
            Options,            
        }
        GameState CurrentGameState = GameState.MainMenu;

        // Screen adjustments
        int screenWidth = 800, screenHeight = 600;

        SpriteManager spriteManager;

        //Audio components
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue currentSong;

        public HUD hud;

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

        // Play Music Cue
        public void PlayCue(string cueName)
        {
            soundBank.PlayCue("Actions");
        }

        /**************************************************************************************/
        
       
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

            /**************************************************************************************/

            // starting x and y locations to stack buttons 
            // vertically in the middle of the screen
            int x1 = 350;
            int y1 = 300;

            int x2 = 700;
            int y2 = 200;
            
            //For menu buttons
            for (int i = 0; i < 2; i++)
            {
                button_state[i] = BState.UP;
                button_color[i] = Color.White;
                button_timer[i] = 0.0;
                button_rectangle[i] = new Rectangle(x1, y1, BUTTON_WIDTH, BUTTON_HEIGHT);
                y1 += BUTTON_HEIGHT;
            }
            IsMouseVisible = true;
            background_color = Color.CornflowerBlue;
            
            //For abilities buttons
            for (int i = 2; i < NUMBER_OF_BUTTONS; i++)
            {
                button_state[i] = BState.UP;
                button_color[i] = Color.White;
                button_timer[i] = 0.0;
                button_rectangle[i] = new Rectangle(x2, y2, BUTTON_WIDTH, BUTTON_HEIGHT);
                y2 += BUTTON_HEIGHT;
            }
            IsMouseVisible = true;
            background_color = Color.CornflowerBlue;
           
            /**************************************************************************************/
            
            this.layers = new List<ParallaxLayer>();

            ParallaxLayer firstLayer = new ParallaxLayer(firstBackgroundSpeed * secondBackgroundSpeed, 0);
            ParallaxLayer secondLayer = new ParallaxLayer(firstBackgroundSpeed, 0);
            ParallaxLayer thirdLayer = new ParallaxLayer(firstBackgroundSpeed, 0);
            ParallaxLayer fourthLayer = new ParallaxLayer(firstBackgroundSpeed, 0);
            ParallaxLayer fifthLayer = new ParallaxLayer(firstBackgroundSpeed, 0);

            // Add layers
            this.layers.Add(firstLayer);
            this.layers.Add(secondLayer);
            this.layers.Add(thirdLayer);
            this.layers.Add(fourthLayer);
            this.layers.Add(fifthLayer);
            
            base.Initialize();
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

            hud = new HUD();
            hud.Font = Content.Load<SpriteFont>(@"Fonts\Arial");

            /**************************************************************************************/
            button_texture[PLAY_BUTTON_INDEX] =
                Content.Load<Texture2D>(@"Images/play_button");
            button_texture[QUIT_BUTTON_INDEX] =
                Content.Load<Texture2D>(@"Images/quit_button");
            button_texture[HAB1_BUTTON_INDEX] =
                Content.Load<Texture2D>(@"Images/hab1_button");
            button_texture[HAB2_BUTTON_INDEX] =
                Content.Load<Texture2D>(@"Images/hab2_button");
            button_texture[HAB3_BUTTON_INDEX] =
                Content.Load<Texture2D>(@"Images/hab3_button");
            
           /**************************************************************************************/
            
            // Load audio content
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            
            switch(CurrentGameState)
            {
                case GameState.MainMenu:
                    {
                        currentSong = soundBank.GetCue("Menu");
                        currentSong.Play();
                        Settings.musicVolume = 1.0f;
                        break;
                    }                
            }              

            /**************************************************************************************/

            //carrega as imagens das camadas
            this.layers[0].LoadContent(Content, @"Images/l1");
            this.layers[1].LoadContent(Content, @"Images/l2");
            this.layers[2].LoadContent(Content, @"Images/l3");
            this.layers[3].LoadContent(Content, @"Images/l4");
            this.layers[4].LoadContent(Content, @"Images/l5");

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
                    
                       
            /**************************************************************************************/
            
            // Update layers
            foreach (ParallaxLayer layer in this.layers)
            {
                layer.Update(gameTime);
            }          
            
            /**************************************************************************************/
            
            
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

        // wrapper for hit_image_alpha taking Rectangle and Texture
        Boolean hit_image_alpha(Rectangle rect, Texture2D tex, int x, int y)
        {
            return hit_image_alpha(0, 0, tex, tex.Width * (x - rect.X) /
                rect.Width, tex.Height * (y - rect.Y) / rect.Height);
        }

        // wraps hit_image then determines if hit a transparent part of image 
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

        // determine if x,y is within rectangle formed by texture located at tx,ty
        Boolean hit_image(float tx, float ty, Texture2D tex, int x, int y)
        {
            return (x >= tx &&
                x <= tx + tex.Width &&
                y >= ty &&
                y <= ty + tex.Height);
        }
       
        // determine state and color of button
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
                    {
                        //Stops current music and plays level music
                        CurrentGameState = GameState.Playing;
                        currentSong.Stop(AudioStopOptions.AsAuthored);
                        currentSong = soundBank.GetCue("Levels");
                        currentSong.Play();
                        Settings.musicVolume = 1.0f;

                        //Load sprites
                        Components.Add(spriteManager);

                        break;
                    }
                case QUIT_BUTTON_INDEX:
                    this.Exit();
                    background_color = Color.Yellow;
                    break;
                case HAB1_BUTTON_INDEX:
                    background_color = Color.Red;
                    break;
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
                        spriteBatch.Draw(button_texture[0], button_rectangle[0], button_color[0]);
                        spriteBatch.Draw(button_texture[1], button_rectangle[1], button_color[1]);
                        break;                        
                    }
                
               case GameState.Selection:
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"Images\menu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                        break;
                    }
                
                case GameState.Playing:
                    foreach (ParallaxLayer layer in this.layers)
                    {
                        layer.Draw(spriteBatch);
                    }
                    hud.Draw(spriteBatch);
                    // Draw button
                    spriteBatch.Draw(button_texture[2], button_rectangle[2], button_color[2]);
                    spriteBatch.Draw(button_texture[3], button_rectangle[3], button_color[3]);
                    spriteBatch.Draw(button_texture[4], button_rectangle[4], button_color[4]);
                    break;                    
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
