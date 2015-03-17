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

        //Menu components
        enum GameState
        {
            MainMenu,
            Playing,
            Intro,
            Selection,
            Options,            
        }
        GameState CurrentGameState = GameState.Playing;

        // Screen adjustments
        int screenWidth = 800, screenHeight = 600;

        // Buttons for the menu

        Button btnPlay, btnSoldier, btnMage;        

        SpriteManager spriteManager;

        private Texture2D clouds;                           // clouds texture
        private Texture2D background;                            // background texture
        private Texture2D mountains;                            // mountains texture
        private Texture2D floor;                                // floor texture
        private Texture2D terrain;                              // terrain texture

        //Audio components
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue playCue;

        HUD hud;

        private List<ParallaxLayer> listParallaxLayer;      // List of parallayx layers

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        // Play Music Cue
        public void PlayCue(string cueName)
        {
            soundBank.PlayCue("Actions");
        }

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
            
            switch(CurrentGameState)
            {
                case GameState.Playing:
                {
                    Components.Add(spriteManager);                                       
                    break;
                }
            }           

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
            
            // Load audio content
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");

            // Plays background music
            Song song = Content.Load<Song>(@"Audio\main_theme");
            MediaPlayer.Play(song);

            /**************************************************************************************/
            
            // Load the Parallax Layers
            
            mountains = this.Content.Load<Texture2D>("Images/l3");
            clouds = this.Content.Load<Texture2D>("Images/l2");
            background = this.Content.Load<Texture2D>("Images/l1");
            terrain = this.Content.Load<Texture2D>("Images/l4");
            floor = this.Content.Load<Texture2D>("Images/l5");

            // First the closest
            listParallaxLayer = new List<ParallaxLayer>();
            listParallaxLayer.Add(new ParallaxLayer(graphics, background, 0.0f));
            listParallaxLayer.Add(new ParallaxLayer(graphics, clouds, 100.0f));
            listParallaxLayer.Add(new ParallaxLayer(graphics, mountains, 100.0f));
            listParallaxLayer.Add(new ParallaxLayer(graphics, terrain, 100.0f));
            listParallaxLayer.Add(new ParallaxLayer(graphics, floor, 100.0f));

            /**************************************************************************************/

            // Starts playing background music
            // soundBank.PlayCue("overworld_theme");

            /**************************************************************************************/
            // Screen stuff for the menu
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            IsMouseVisible = true;

            btnPlay = new Button(Content.Load<Texture2D>(@"Images\button"), graphics.GraphicsDevice);
            btnPlay.setPosition(new Vector2(350, 300));

            btnSoldier = new Button(Content.Load<Texture2D>(@"Images\button"), graphics.GraphicsDevice);
            btnSoldier.setPosition(new Vector2(200, 300));

            btnMage = new Button(Content.Load<Texture2D>(@"Images\button"), graphics.GraphicsDevice);
            btnMage.setPosition(new Vector2(400, 300));

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            /*else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    foreach (ParallaxLayer layer in listParallaxLayer)
                    {
                        layer.MoveLeft(gameTime);
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        foreach (ParallaxLayer layer in listParallaxLayer)
                        {
                            layer.MoveRight(gameTime);
                        }
                    }
                }
            }*/

            // Moves the background automatically
            foreach (ParallaxLayer layer in listParallaxLayer)
            {
                layer.MoveRight(gameTime);                
            }

            //Allows the game to exit via keyboard
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            MouseState mouse = Mouse.GetState();

            switch(CurrentGameState)
            {
                case GameState.MainMenu:
                    {
                        if (btnPlay.isClicked == true)
                        {
                            CurrentGameState = GameState.Playing;
                            btnPlay.Update(mouse);
                        }                            
                        break;
                    }
                case GameState.Selection:
                    {
                        if (btnSoldier.isClicked == true)
                        {
                            CurrentGameState = GameState.Playing;
                            btnSoldier.Update(mouse);
                            btnSoldier.value = 1;
                        }
                        else if(btnMage.isClicked == true)
                        {
                            CurrentGameState = GameState.Playing;
                            btnMage.Update(mouse);
                            btnMage.value = 2;
                        }
                        break;
                    }
                case GameState.Playing: break;

            }

            // TODO: Add your update logic here

            base.Update(gameTime);
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
                        btnPlay.Draw(spriteBatch);
                        break;
                    }
                
               case GameState.Selection:
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"Images\menu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                        btnPlay.Draw(spriteBatch);
                        break;
                    }
                
                case GameState.Playing:
                    foreach (ParallaxLayer layer in listParallaxLayer)
                    {
                        layer.Draw(spriteBatch);
                    }
                    hud.Draw(spriteBatch);
                    break;                    
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
