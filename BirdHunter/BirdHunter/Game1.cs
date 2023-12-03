//Author: Victoria Mak
//File Name: Game1.cs
//Project Name: BirdHunter
//Creation Date: April 7, 2022
//Modified Date: April 22, 2022
//Description: Play the game where you, the bird, have to hunt for insects for food

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;
using Helper;

namespace BirdHunter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Store a random number generator
        Random rng = new Random();

        //Store the constant game state variables in the system
        const byte MENU = 0;
        const byte PLAY = 1;
        const byte INSTRUCTIONS = 2;
        const byte PAUSE = 3;
        const byte PRE_GAME = 4;
        const byte END_GAME = 5;

        //Store the constant button spacing in the menu
        const byte BUTTON_SPACING = 25;

        //Store the constant shadow displacement
        const byte SHADOW_DISPLACEMENT = 4;

        //Store the maximum health and ammo count
        const byte MAX_AMMO = 10;
        const byte MAX_HEALTH = 10;

        //Store the score value for when the the level gets harder
        const int MIN_SCORE_FOR_LEVEL_2 = 700;
        const int MIN_SCORE_FOR_LEVEL_3 = 1400;

        //Store the level for the spider and the butterfly
        const byte SPIDER_LEVEL = 2;
        const byte BUTTERFLY_LEVEL = 3; 

        //Store the health damage, score boost or reductions, and the ammo pickup boost
        const byte HEALTH_DAMAGE = 5;
        const byte UNEATEN_SCORE_REDUCTION = 100;
        const byte MISCLICK_SCORE_REDUCTION = 50;
        const byte EATEN_SCORE_BOOST = 100;
        const byte AMMO_PICKUP_BOOST = 2;

        //Store the translation directions
        const int UP = -1;
        const int DOWN = 1;
        const int RIGHT = 1;
        const int LEFT = -1;
        const int STOPPED = 0;

        //Store the level
        byte level = 1;

        //Store the current and next game state
        byte gameState = MENU;
        byte nextGameState;

        //Store the screen width and height
        int screenWidth;
        int screenHeight;

        //Store the background images
        Texture2D natureBgImg;
        Texture2D leavesBgImg;

        //Store the instructions image
        Texture2D instructionsImg;

        //Store the button images
        Texture2D backBtnImg;
        Texture2D exitBtnImg;
        Texture2D goBtnImg;
        Texture2D instructionsBtnImg;
        Texture2D menuBtnImg;
        Texture2D playBtnImg;

        //Store the pointer image
        Texture2D pointerImg;

        //Store the insect animation spritesheet images
        Texture2D beetleImg;
        Texture2D spiderImg;
        Texture2D butterflyImg;

        //Store the ammo image
        Texture2D ammoImg;

        //Store the health image
        Texture2D healthImg;

        //Store the hit image
        Texture2D hitImg;

        //Store the fonts for the game texts
        SpriteFont titleFont;
        SpriteFont msgFont;
        SpriteFont statsFont;
        SpriteFont newHighScoreFont;
        SpriteFont reasonForGameOverFont;

        //Store the songs
        Song bgMusic;
        Song menuMusic;
        Song gameOverMusic;

        //Store the sound effects
        SoundEffect eatingSnd;
        SoundEffect countdownSnd;
        SoundEffect gameOverSnd;
        SoundEffect ammoPickupSnd;
        SoundEffect pausedSnd;
        SoundEffect resumedSnd;
        SoundEffect newHighScoreSnd;
        SoundEffect missSnd;
        SoundEffect movingAwaySnd;
        SoundEffect buttonClickSnd;
        SoundEffect squawkSnd;
        SoundEffect ammoSpawnSnd;
        SoundEffect ammoRetreatSnd;

        //Store the hit rectangles
        Rectangle hitRec1;
        Rectangle hitRec2;
        Rectangle hitRec3;

        //Store the timer for the hit timed graphic
        Timer hitGraphicTimer1;
        Timer hitGraphicTimer2;
        Timer hitGraphicTimer3;

        //Store the text and location of the title
        string titleText;
        Vector2 titleLoc;

        //Store the texts and location of the paused messages
        string pausedText;
        string pausedMsg;
        string pausedCurScoreText;
        string pausedHighScoreText;
        Vector2 pausedTextLoc;
        Vector2 pausedMsgLoc;
        Vector2 pausedCurScoreLoc;
        Vector2 pausedHighScoreLoc;

        //Store the location for the stats labels
        Vector2 timerLoc;
        Vector2 healthCountLoc;
        Vector2 ammoCountLoc;
        Vector2 highScoreLoc;
        Vector2 curScoreLoc;
        Vector2 menuHighScoreLoc;

        //Store the ammo icon's frame width, height, x, and y coordinate and the ammo source rectangle
        int ammoIconFrameWidth;
        int ammoIconFrameHeight;
        Vector2 ammoIconPosFromImg;
        Rectangle ammoSrcRec;

        //Store the rectangle for the health
        Rectangle healthIconRec;
        Rectangle ammoIconRec;

        //Store the scalers for the images
        float instructionScaler;
        float pointerScaler = 0.4f;
        float healthScaler = 0.1f;
        float ammoIconScaler = 0.14f;
        float beetleScaler = 0.4f;
        float spiderScaler = 0.4f;
        float butterflyScaler = 0.2f;
        float ammoPickupScaler = 0.8f;
        float hitGraphicScaler = 0.08f;

        //Store the rectangle of the leaves background
        Rectangle bgRec;

        //Store the rectangle of the instructions
        Rectangle instructionRec;

        //Store the rectangle of the buttons
        Rectangle backBtnRec;
        Rectangle exitBtnRec;
        Rectangle goBtnRec;
        Rectangle instructionsBtnRec;
        Rectangle menuBtnRec;
        Rectangle playBtnRec;

        //Store the nature background's position, number of frames wide and high, location, and the animation background
        Vector2 natureBgLoc;
        byte natureFramesWide = 5;
        byte natureFramesHigh = 4;
        float natureBgScaler;
        Animation natureBgAnim;

        //Store the locations for the insects and ammo pickups and for the hidden location
        Vector2 spiderLoc;
        Vector2 spiderStoppingLoc;
        Vector2 butterflyLoc;
        Vector2 butterflyStoppingLoc;
        Vector2 beetleLoc;
        Vector2 beetleStoppingLoc;
        Vector2 ammoPickupLoc;
        Vector2 hiddenLoc;

        //Store the animations for the insects and for the ammo pickup
        Animation spiderAnim;
        Animation butterflyAnim;
        Animation beetleAnim;
        Animation ammoAnim;

        //Store the game timer, insect timers, ammo timer
        Timer gameTimer;
        Timer beetleTimer;
        Timer spiderTimer;
        Timer butterflyTimer;
        Timer ammoPickupTimer;

        //Store the x and y distances from the stopping location where the insects woul accelerate of the insects
        Vector2 accelerationDist;

        //Store the minimum speed multiplier of the insects 
        float minInsectSpeedMultiplier = 0.1f;

        //Store the insect speeds and directions for when they enter and exit the screen
        Vector2 beetleDir;
        Vector2 beetleMaxSpeed = new Vector2(STOPPED, 550f);
        Vector2 spiderDir;
        Vector2 spiderMaxSpeed = new Vector2(600f, STOPPED);
        Vector2 butterflyDir;
        Vector2 butterflyMaxSpeed = new Vector2(550f, STOPPED);

        //Store the current and previous keyboard and mouse state and the rectangle for the pointer
        KeyboardState kb;
        KeyboardState prevKb;
        MouseState mouse;
        MouseState prevMouse;
        Rectangle pointerRec;

        //Store whether the user is hovering over the buttons
        bool hoveringOverPlay = false;
        bool hoveringOverInstructions = false;
        bool hoveringOverExit = false;
        bool hoveringOverGo = false;
        bool hoveringOverBack = false;
        bool hoveringOverMenu = false;


        //Store the high score, current score, health count, ammo count
        int highScore = 0;
        int curScore = 0;
        byte healthCount = MAX_HEALTH;
        byte ammoCount = MAX_AMMO;

        //Store the large game over message, its speed across the screen, its timer, and its location
        string reasonForGameOverMsg;
        Vector2 reasonForGameOverLoc;
        float reasonForGameOverSpeedMultiplier;
        float reasonForGameOverMaxSpeed = 1000f;
        float reasonForGameOverMinMultiplier = 0.03f;
        float reasonForGameOverSpeed;

        //Store the new high score, score and high score messages to be displayed in the end game state
        string gameOverMsg;
        string highScoreMsg;
        string curScoreMsg;
        string newHighScoreMsg;

        //Store the location for the score and high score message
        Vector2 gameOverMsgLoc;
        Vector2 highScoreMsgLoc;
        Vector2 curScoreMsgLoc;
        Vector2 newHighScoreMsgLoc;

        //Store the transparency of the new high score message
        float newHsTransparency;

        //Store the transparency of the graphics for the fading in and out effect and the fading in/out speed per update
        float fadeInOutTransparency = 1f;
        float fadingSpeed = 0.02f;

        //Store whether the scenes are fading in or out
        bool isFadingOut = false;
        bool isFadingIn = false;

        //Store whether the countdown has started and the countdown
        bool countdownStarted = false;
        SoundEffectInstance countdown;

        //Store whether the new high score chime was played
        bool newHighScoreSndHasPlayed = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            // TODO: use this.Content to load your game content here
            //Store the screen width and height
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //Load the background images
            natureBgImg = Content.Load<Texture2D>("Images/Backgrounds/Nature");
            leavesBgImg = Content.Load<Texture2D>("Images/Backgrounds/LeavesBackground");

            //Load the instructions image
            instructionsImg = Content.Load<Texture2D>("Images/Instruction/Instructions");

            //Load the button images
            backBtnImg = Content.Load<Texture2D>("Images/Buttons/BackBtn1");
            exitBtnImg = Content.Load<Texture2D>("Images/Buttons/ExitBtn1");
            goBtnImg = Content.Load<Texture2D>("Images/Buttons/GoBtn1");
            instructionsBtnImg = Content.Load<Texture2D>("Images/Buttons/InstructionsBtn1");
            menuBtnImg = Content.Load<Texture2D>("Images/Buttons/menuBtn1");
            playBtnImg = Content.Load<Texture2D>("Images/Buttons/PlayBtn1");

            //Load the pointer image
            pointerImg = Content.Load<Texture2D>("Images/Pointer/Pointer");

            //Load the health and ammo image
            healthImg = Content.Load<Texture2D>("Images/Health/health");
            ammoImg = Content.Load<Texture2D>("Images/Ammo/Energy");

            //Load the insect images
            spiderImg = Content.Load<Texture2D>("Images/Sprites/Spider");
            butterflyImg = Content.Load<Texture2D>("Images/Sprites/Butterfly");
            beetleImg = Content.Load<Texture2D>("Images/Sprites/Beatle");

            //Load the hit image
            hitImg = Content.Load<Texture2D>("Images/Hit/Hit");
            spiderImg = Content.Load<Texture2D>("Images/Sprites/Spider");

            //Load the fonts
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            msgFont = Content.Load<SpriteFont>("Fonts/MsgFont");
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            newHighScoreFont = Content.Load<SpriteFont>("Fonts/NewHSFont");
            reasonForGameOverFont = Content.Load<SpriteFont>("Fonts/LargeGameOverFont");

            //Load the songs
            bgMusic = Content.Load<Song>("Audio/Music/BirdsChirpingMusic");
            menuMusic = Content.Load<Song>("Audio/Music/MenuMusic");
            gameOverMusic = Content.Load<Song>("Audio/Music/GameOverMusic");

            //Setup the media player volume
            MediaPlayer.Volume = 0.5f;

            //Load the sound effects
            eatingSnd = Content.Load<SoundEffect>("Audio/Sounds/EatingSound");
            countdownSnd = Content.Load<SoundEffect>("Audio/Sounds/CountdownSound");
            gameOverSnd = Content.Load<SoundEffect>("Audio/Sounds/GameOverSound");
            ammoPickupSnd = Content.Load<SoundEffect>("Audio/Sounds/PowerUp");
            pausedSnd = Content.Load<SoundEffect>("Audio/Sounds/Paused");
            resumedSnd = Content.Load<SoundEffect>("Audio/Sounds/Resumed");
            newHighScoreSnd = Content.Load<SoundEffect>("Audio/Sounds/NewHighScore");
            missSnd = Content.Load<SoundEffect>("Audio/Sounds/MissSound");
            movingAwaySnd = Content.Load<SoundEffect>("Audio/Sounds/CrawlingAway");
            buttonClickSnd = Content.Load<SoundEffect>("Audio/Sounds/ButtonClick");
            squawkSnd = Content.Load<SoundEffect>("Audio/Sounds/Squawk");
            ammoSpawnSnd = Content.Load<SoundEffect>("Audio/Sounds/AmmoSpawn");
            ammoRetreatSnd = Content.Load<SoundEffect>("Audio/Sounds/AmmoRetreat");

            //Set the volume of all sound effects
            SoundEffect.MasterVolume = 1f;

            //Set the rectangle of the leaves background
            bgRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Set the instruction scaler
            instructionScaler = screenHeight * 1f / instructionsImg.Height;

            //Set the rectangle of the instructions
            instructionRec = new Rectangle(screenWidth / 2 - (int)(instructionsImg.Width * instructionScaler) / 2, 0, (int)(instructionsImg.Width * instructionScaler), (int)(instructionsImg.Height * instructionScaler));

            //Set the rectangle of the buttons
            playBtnRec = new Rectangle(screenWidth / 2 - playBtnImg.Width / 2, screenHeight / 2 - playBtnImg.Height / 2, playBtnImg.Width, playBtnImg.Height);
            instructionsBtnRec = new Rectangle(screenWidth / 2 - instructionsBtnImg.Width / 2, playBtnRec.Y + playBtnImg.Height + BUTTON_SPACING, instructionsBtnImg.Width, instructionsBtnImg.Height);
            exitBtnRec = new Rectangle(screenWidth / 2 - exitBtnImg.Width / 2, instructionsBtnRec.Y + instructionsBtnImg.Height + BUTTON_SPACING, exitBtnImg.Width, exitBtnImg.Height);
            backBtnRec = new Rectangle(BUTTON_SPACING, screenHeight - backBtnImg.Height - BUTTON_SPACING, backBtnImg.Width, backBtnImg.Height);
            goBtnRec = new Rectangle(screenWidth / 2 - goBtnImg.Width / 2, screenHeight * 3 / 4, goBtnImg.Width, goBtnImg.Height);
            menuBtnRec = new Rectangle(screenWidth / 2 - menuBtnImg.Width / 2, screenHeight * 4 / 5, menuBtnImg.Width, menuBtnImg.Height);

            //Set the scaler and the location for the nature background
            natureBgScaler = (float)natureFramesWide * screenWidth / natureBgImg.Width;
            natureBgLoc = new Vector2(0, screenHeight / 2 - natureBgScaler * natureBgImg.Height / natureFramesHigh / 2);

            //Set the nature background animation
            natureBgAnim = new Animation(natureBgImg, natureFramesWide, natureFramesHigh, 17, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 2, natureBgLoc, natureBgScaler, true);

            //Set the location of the hidden location and the insects
            hiddenLoc = new Vector2(1000, 1000);
            spiderLoc = hiddenLoc;
            beetleLoc = hiddenLoc;
            butterflyLoc = hiddenLoc;
            ammoPickupLoc = hiddenLoc;

            //Set the animations for the insects and the ammo
            spiderAnim = new Animation(spiderImg, 5, 2, 10, 0, 1, Animation.ANIMATE_FOREVER, 1, spiderLoc, spiderScaler, false);
            beetleAnim = new Animation(beetleImg, 4, 5, 19, 0, 1, Animation.ANIMATE_FOREVER, 1, beetleLoc, beetleScaler, false);
            butterflyAnim = new Animation(butterflyImg, 7, 1, 7, 0, 1, Animation.ANIMATE_FOREVER, 1, butterflyLoc, butterflyScaler, false);
            ammoAnim = new Animation(ammoImg, 4, 4, 15, 8, 8, Animation.ANIMATE_FOREVER, 5, ammoPickupLoc, ammoPickupScaler, true);
          
            //Set the rectangles for the hit graphic
            hitRec1 = new Rectangle((int)hiddenLoc.X, (int)hiddenLoc.Y, (int)(hitImg.Width * hitGraphicScaler), (int)(hitImg.Height * hitGraphicScaler));
            hitRec2 = new Rectangle((int)hiddenLoc.X, (int)hiddenLoc.Y, (int)(hitImg.Width * hitGraphicScaler), (int)(hitImg.Height * hitGraphicScaler));
            hitRec3 = new Rectangle((int)hiddenLoc.X, (int)hiddenLoc.Y, (int)(hitImg.Width * hitGraphicScaler), (int)(hitImg.Height * hitGraphicScaler));

            //Set the timers for the hit timed graphic 
            hitGraphicTimer1 = new Timer(200, false);
            hitGraphicTimer2 = new Timer(200, false);
            hitGraphicTimer3 = new Timer(200, false);

            //Set the directions for the insects
            beetleDir = new Vector2(STOPPED, STOPPED);
            spiderDir = new Vector2(STOPPED, STOPPED);
            butterflyDir = new Vector2(STOPPED, STOPPED);

            //Set the acceleration distance of the insects
            accelerationDist = new Vector2(screenWidth / 5, screenHeight / 5);

            //Set the title text and the location of the title
            titleText = "Bird Hunter";
            titleLoc = new Vector2(screenWidth / 2 - titleFont.MeasureString(titleText).X / 2, 20);
            menuHighScoreLoc = new Vector2(screenWidth / 2 - statsFont.MeasureString(Convert.ToString(highScore)).X / 2, titleLoc.Y + titleFont.MeasureString(titleText).Y);

            //Set the paused message texts and location
            pausedText = "Your game is paused.";
            pausedTextLoc = new Vector2(screenWidth / 2 - msgFont.MeasureString(pausedText).X / 2, screenHeight * 1 /4);
            pausedMsg = "Press escape to resume.";
            pausedMsgLoc = new Vector2(screenWidth / 2 - msgFont.MeasureString(pausedMsg).X / 2, pausedTextLoc.Y + 50);
            pausedCurScoreText = "Current Score: " + curScore;
            pausedCurScoreLoc = new Vector2(screenWidth / 2 - msgFont.MeasureString(pausedCurScoreText).X / 2, pausedMsgLoc.Y + msgFont.MeasureString(pausedMsg).Y + 50);
            pausedHighScoreText = "High Score: " + highScore;
            pausedHighScoreLoc = new Vector2(screenWidth / 2 - msgFont.MeasureString(pausedHighScoreText).X / 2, pausedCurScoreLoc.Y + msgFont.MeasureString(pausedCurScoreText).Y + 50);

            //Set the ammo icon frame width, height, and x and y coordinate
            ammoIconFrameWidth = ammoImg.Width / 4;
            ammoIconFrameHeight = ammoImg.Height / 4;
            ammoIconPosFromImg = new Vector2(ammoIconFrameWidth * 3, ammoIconFrameHeight);

            //Set the locations of the stats in the gameplay
            timerLoc = new Vector2(screenWidth * 7 / 8, 5);

            //Set the rectangles for the health and ammo image labels in the gameplay
            healthIconRec = new Rectangle(screenWidth * 1 / 16, 5, (int)(healthImg.Width * healthScaler), (int)(healthImg.Height * healthScaler));
            ammoIconRec = new Rectangle(screenWidth * 1 / 4, 5, (int)(healthImg.Width * ammoIconScaler), (int)(healthImg.Height * ammoIconScaler));
            ammoSrcRec = new Rectangle((int)ammoIconPosFromImg.X, (int)ammoIconPosFromImg.Y, ammoIconFrameWidth, ammoIconFrameHeight);

            //Set the locations for the health and ammo counter and the high and current score
            healthCountLoc = new Vector2(healthIconRec.Right + 5, 5);
            ammoCountLoc = new Vector2(ammoIconRec.Right + 5, 5);
            highScoreLoc = new Vector2(screenWidth / 2 - statsFont.MeasureString(Convert.ToString(highScore)).X, 5);
            curScoreLoc = new Vector2(screenWidth / 2 - statsFont.MeasureString(Convert.ToString(curScore)).X, highScoreLoc.Y + statsFont.MeasureString(Convert.ToString(highScore)).Y + 5);
            
            //Set the rectangle for the pointer
            pointerRec = new Rectangle(0, 0, (int)(pointerImg.Width * pointerScaler), (int)(pointerImg.Height * pointerScaler));

            //Set the game timer, insect timers, and the ammo pickup timers
            gameTimer = new Timer(30000, false);
            beetleTimer = new Timer(3000, false);
            spiderTimer = new Timer(3000, false);
            butterflyTimer = new Timer(3000, false);
            ammoPickupTimer = new Timer(1500, false);

            //Set the large game over message and its location
            reasonForGameOverMsg = "";
            reasonForGameOverLoc = new Vector2(screenWidth / 2 - reasonForGameOverFont.MeasureString(reasonForGameOverMsg).X / 2, screenHeight);

            //Set the high score and current score messages
            gameOverMsg = "Game over!";
            highScoreMsg = "High Score: " + highScore;
            curScoreMsg = "Score: " + curScore;
            newHighScoreMsg = "";

            //Set the location for the high score and current score messages
            gameOverMsgLoc = new Vector2(screenWidth / 2 - msgFont.MeasureString(gameOverMsg).X / 2, 10);
            highScoreMsgLoc = new Vector2(screenWidth / 2 - msgFont.MeasureString(highScoreMsg).X / 2, screenHeight * 2 / 5);
            curScoreMsgLoc = new Vector2(screenWidth / 2 - msgFont.MeasureString(curScoreMsg).X / 2, screenHeight * 3 / 5);
            newHighScoreMsgLoc = new Vector2(screenWidth / 2 - newHighScoreFont.MeasureString(newHighScoreMsg).X / 2, screenHeight / 6);           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here
            //Get the current and previous states of the mouse and keyboard
            prevKb = kb;
            prevMouse = mouse;
            mouse = Mouse.GetState();
            kb = Keyboard.GetState();
            
            //Move the pointer according to the mouse
            pointerRec.X = mouse.X;
            pointerRec.Y = mouse.Y;
         
            //Update the current game state 
            switch (gameState)
            {
                case MENU:
                    UpdateMenu(gameTime);
                    break;
                case PLAY:
                    UpdatePlay(gameTime);
                    break;
                case INSTRUCTIONS:
                    UpdateInstructions(gameTime);
                    break;
                case PAUSE:
                    UpdatePause();
                    break;
                case PRE_GAME:
                    UpdatePregame();
                    break;
                case END_GAME:
                    UpdateEndgame(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //Draw the current game state
            switch (gameState)
            {
                case MENU:
                    DrawMenu();
                    break;
                case PLAY:
                    DrawPlay();
                    break;
                case INSTRUCTIONS:
                    DrawInstructions();
                    break;
                case PAUSE:
                    DrawPause();
                    break;
                case PRE_GAME:
                    DrawPregame();
                    break;
                case END_GAME:
                    DrawEndgame();
                    break;              
            }

            //Draw the pointer
            spriteBatch.Draw(pointerImg, pointerRec, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateMenu(GameTime gameTime)
        {
            //Play the menu music if it is not playing, it has ended, and only if the screen is not transitioning
            if ((MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= menuMusic.Duration) && isFadingOut == false)
            {
                //Play the menu music
                MediaPlayer.Play(menuMusic);              
            }

            //Change the trnasparency of the screen for the transition to fade in
            ChangeTransparencyForFadeIn();

            //Update the background animation
            natureBgAnim.Update(gameTime);

            //Check whether the player clicked on the play, instructions, or exit button
            if (playBtnRec.Contains(mouse.Position))
            {
                //Set the mouse as hovering over the play button as true
                hoveringOverPlay = true;

                //Check whether the mouse clicked on the play button
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                {
                    //Set the screen as fading out and the next game state
                    isFadingOut = true;
                    nextGameState = PRE_GAME;

                    //Stop the menu music
                    MediaPlayer.Stop();

                    //Play the button click sound
                    buttonClickSnd.CreateInstance().Play();
                }
            }
            else
            {
                //Set the mouse as not hovering over the play button
                hoveringOverPlay = false;
            }

            //Check whether the mouse is hovering over the instructions button
            if (instructionsBtnRec.Contains(mouse.Position))
            {
                //Set the mouse as hovering over the instructions button as true
                hoveringOverInstructions = true;

                //Check whether the mouse clicked on the instructions button
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                {
                    //Set the screen as fading out for the transition and set the next game state
                    isFadingOut = true;
                    nextGameState = INSTRUCTIONS;

                    //Play the button click sound
                    buttonClickSnd.CreateInstance().Play();
                }
            }
            else
            {
                //Set the mouse as not hovering over the instructions button
                hoveringOverInstructions = false;
            }

            //Check whether the mouse is hovering over the exit button
            if (exitBtnRec.Contains(mouse.Position))
            {
                //Set the mouse as hovering over the exit button
                hoveringOverExit = true;
                
                //Check whether the mouse clicked on the exit button
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                {
                    //Exit the game
                    Exit();
                }
            }
            else
            {  
                //Set the mouse as not hovering over the exit button
                hoveringOverExit = false;
            }

            //Change the transparency for the fade out transition
            ChangeTransparencyForFadeOut();
        }

        private void UpdatePlay(GameTime gameTime)
        {
            //Update the game timer, insect timers, hit message timers, ammo pickup timers, and animations as long as the game is not over
            if (nextGameState != END_GAME)
            {
                gameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                beetleTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                spiderTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                butterflyTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                hitGraphicTimer1.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                hitGraphicTimer2.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                hitGraphicTimer3.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                ammoPickupTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                //Update the insect animations
                spiderAnim.Update(gameTime);
                beetleAnim.Update(gameTime);
                butterflyAnim.Update(gameTime);
                ammoAnim.Update(gameTime);
            }            

            //Loop the background music if it is done
            if (MediaPlayer.PlayPosition >= bgMusic.Duration && isFadingOut == false)
            {
                //Play the background music
                MediaPlayer.Play(bgMusic);
            }

            //Pause the game if the user presses the escape key
            if (kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyUp(Keys.Escape) && nextGameState != END_GAME)
            {
                //Change the gamestate to pause
                gameState = PAUSE;

                //Pause the background music
                MediaPlayer.Pause();

                //Pause the countdown sound if the countdown started
                if (countdownStarted == true)
                {
                    //Stop the countdown
                    countdown.Pause();
                }

                //Play the paused sound effect 
                pausedSnd.CreateInstance().Play();

                //Update the current score and high score messages and locations in pause
                pausedCurScoreText = "Current Score: " + curScore;
                pausedCurScoreLoc.X = screenWidth / 2 - msgFont.MeasureString(pausedCurScoreText).X / 2;
                pausedHighScoreText = "High Score: " + highScore;
                pausedHighScoreLoc.X = screenWidth / 2 - msgFont.MeasureString(pausedHighScoreText).X / 2;
            }

            //Check whether the mouse clicked and where the mouse clicked if the next game state is not end game
            if (nextGameState != END_GAME)
            {
                //Check for a mouse click
                CheckMouseClick();
            }
            
            //Determine whether the spider timer is up, the spider timer sould start or the spider is still moving to its stopping location
            if (nextGameState != END_GAME)
            {
                //Determine whether the beetle timer is up, the beetle timer should start, or the beetle is still moving to its stopping location
                MoveOrStopInsect(beetleTimer, ref beetleAnim, gameTime, ref beetleLoc, ref beetleDir, beetleMaxSpeed, ref beetleStoppingLoc, beetleScaler);

                //Only move or stop the spider when the spider level has been reached
                if (level >= SPIDER_LEVEL)
                {
                    //Determine whether the spider timer is up, the spider timer should start, or the spider is still moving to its stopping location
                    MoveOrStopInsect(spiderTimer, ref spiderAnim, gameTime, ref spiderLoc, ref spiderDir, spiderMaxSpeed, ref spiderStoppingLoc, spiderScaler);
                }

                //Only move or stop the spider when the butterfly level has been reached
                if (level >= BUTTERFLY_LEVEL)
                {
                    //Determine whether the butterfly timer is up, the butterfly timer should start, or the butterfly is still moving to its stopping location
                    MoveOrStopInsect(butterflyTimer, ref butterflyAnim, gameTime, ref butterflyLoc, ref butterflyDir, butterflyMaxSpeed, ref butterflyStoppingLoc, butterflyScaler);
                }
            }

            //Generate a new beetle once the hit graphic timer is finished
            if (hitGraphicTimer1.IsFinished())
            {
                //Generate a new beetle
                GenerateNewBeetle();

                //Reset the graphic timer
                hitGraphicTimer1.ResetTimer(false);
            }

            //Generate a new spider once the hit graphic timer is finished
            if (hitGraphicTimer2.IsFinished())
            {
                //Generate a new spider
                GenerateNewHorizontalInsect(spiderTimer, ref spiderStoppingLoc, ref spiderLoc, spiderAnim, spiderScaler, ref spiderDir);

                //Reset the graphic timer
                hitGraphicTimer2.ResetTimer(false);
            }

            //Generate a new butterfly once the hit graphic timer is fhished
            if (hitGraphicTimer3.IsFinished())
            {
                //Generate a new spider
                GenerateNewHorizontalInsect(butterflyTimer, ref butterflyStoppingLoc, ref butterflyLoc, butterflyAnim, butterflyScaler, ref butterflyDir);

                //Reset the graphic timer
                hitGraphicTimer3.ResetTimer(false);
            }

            //Remove the ammo pickups when they're not clicked
            RemoveActiveAmmoPickup();

            //Play the countdown when there is 10 seconds remaining and if the game is not over yet
            if (gameTimer.GetTimeRemaining() <= countdownSnd.Duration.TotalMilliseconds && countdownStarted == false && nextGameState != END_GAME)
            {
                //Start the countdown and set it as started
                countdownStarted = true;
                countdown = countdownSnd.CreateInstance();
                countdown.Play();
            }

            //Change the game state to end game when the timer runs out
            if (gameTimer.IsFinished() || healthCount == 0 || (ammoPickupTimer.IsActive() == false && ammoCount <= 0))
            {
                //When the reason for game over message is not moved, play the game over sound, stop the countdown, and set the reason for game over message and center it on the screen
                if (reasonForGameOverLoc.Y == screenHeight)
                {
                    //Play the game over sound
                    gameOverSnd.CreateInstance().Play();

                    //Stop the countdown sound if the countdown started
                    if (countdownStarted == true)
                    {
                        //Stop the countdown
                        countdown.Stop();
                    }

                    //Set the reason for game over message
                    if (gameTimer.IsFinished())
                    {
                        //Set the reason as the time is up
                        reasonForGameOverMsg = "Time's Up!";
                    }
                    else if (healthCount == 0)
                    {
                        //Set the reason as no more health
                        reasonForGameOverMsg = "The bird is dead!";
                    }
                    else if (ammoCount == 0)
                    {
                        //Set the reason as out of ammo
                        reasonForGameOverMsg = "No Energy To Hunt!";
                    }

                    //Set the reason for game over message's x location
                    reasonForGameOverLoc.X = screenWidth / 2 - reasonForGameOverFont.MeasureString(reasonForGameOverMsg).X / 2;
                }

                //Move the reason for game over message across the screen
                reasonForGameOverSpeedMultiplier = (reasonForGameOverMinMultiplier - 1) * (float)Math.Sin(reasonForGameOverLoc.Y / (screenHeight - reasonForGameOverFont.MeasureString(reasonForGameOverMsg).Y) * Math.PI) + 1;             
                reasonForGameOverSpeed = reasonForGameOverSpeedMultiplier * reasonForGameOverMaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                reasonForGameOverLoc.Y -= reasonForGameOverSpeed;
               
                //Switch the next game state to end game
                nextGameState = END_GAME;

                //Check for a new high score
                if (curScore > highScore)
                {
                    //Set the new high score, high score message, new high score message, and recenter the messages on the screen
                    highScore = curScore;
                    highScoreMsg = "High score: " + highScore;
                    highScoreMsgLoc.X = screenWidth / 2 - msgFont.MeasureString(highScoreMsg).X / 2;
                    newHighScoreMsg = "NEW HIGH SCORE!!!";
                    newHighScoreMsgLoc = new Vector2(screenWidth / 2 - newHighScoreFont.MeasureString(newHighScoreMsg).X / 2, screenHeight / 6);
                }

                //Set the score message and recenter the score message
                curScoreMsg = "Score: " + curScore;
                curScoreMsgLoc.X = screenWidth / 2 - msgFont.MeasureString(curScoreMsg).X / 2;
            }

            //Only start fading out once the large game over sign is exited
            if (reasonForGameOverLoc.Y  <= -1 * reasonForGameOverFont.MeasureString(reasonForGameOverMsg).Y)
            {
                //Set the display as fading out and change the transparency for the fading out transition
                isFadingOut = true;
                ChangeTransparencyForFadeOut();

                //Stop the background music
                MediaPlayer.Stop();
            }
        }

        private void UpdateInstructions(GameTime gameTime)
        {
            //Change the transparency for the fade in transition
            ChangeTransparencyForFadeIn();
            
            //Update the background animation
            natureBgAnim.Update(gameTime);

            //Check whether the player hovers over the back button
            if (backBtnRec.Contains(mouse.Position))
            {
                //Set the hovering over the back button as true
                hoveringOverBack = true;

                //Check whether the mouse has clicked on the back button
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                {
                    //Set the screen as fading out and the next game state as the menu
                    isFadingOut = true;
                    nextGameState = MENU;

                    //Play the button click sound
                    buttonClickSnd.CreateInstance().Play();
                }
            }
            else
            {
                //Set the screen as not hovering over the back button
                hoveringOverBack = false;
            }
            
            //Change the transparency for the fade out transition
            ChangeTransparencyForFadeOut();
        }

        private void UpdatePause()
        {
            //Resume the game if the user presses the escape key
            if (kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyUp(Keys.Escape))
            {
                //Change the gamestate to play
                gameState = PLAY;

                //Resume the music
                MediaPlayer.Resume();

                //Resume the countdown sound if the countdown started
                if (countdownStarted == true)
                {
                    //Resume the countdown
                    countdown.Resume();
                }

                //Play the resume sound effect
                resumedSnd.CreateInstance().Play();
            }
        }

        private void UpdatePregame()
        {
            //Change the transparency for the fade in transition
            ChangeTransparencyForFadeIn();

            //Play the background music if it is not playing or if it has ended
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= bgMusic.Duration)
            {
                //Play the background music
                MediaPlayer.Play(bgMusic);
            }

            //Check whether the user is hovering over the go button
            if (goBtnRec.Contains(mouse.Position))
            {
                //Set the mouse as hovering over the go button
                hoveringOverGo = true;

                //Change the gamestate if the user pressed the go button
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    //Reset the game stats and timers
                    ResetGame();

                    //Change the gamestate to the gameplay and activate the game timer
                    gameState = PLAY;
                    gameTimer.Activate();

                    //Prepare the timer and set the stopping location for the first beetle
                    beetleTimer.ResetTimer(true);
                    beetleStoppingLoc.X = rng.Next(screenWidth - beetleAnim.frameWidth + 1);
                    beetleStoppingLoc.Y = rng.Next((int)(statsFont.MeasureString(Convert.ToString(curScore)).Y + curScoreLoc.Y), screenHeight - beetleAnim.frameHeight + 1);
                    beetleLoc = beetleStoppingLoc;

                    //Set the beetle animation location as the randomized location
                    beetleAnim.destRec.X = (int)beetleLoc.X;
                    beetleAnim.destRec.Y = (int)beetleLoc.Y;

                    //Ramdomize the beetle's direction for its exit if it doesn't get hit
                    if (rng.Next(1, 101) <= 50)
                    {          
                        //Set the beetle's direction as up
                        beetleDir.Y = UP;
                    }
                    else
                    {
                        //Set the beetle's direction as down
                        beetleDir.Y = DOWN;
                    }

                    //Play the button click sound
                    buttonClickSnd.CreateInstance().Play();
                }
            }
            else
            {
                //Set the hovering over the go button as false
                hoveringOverGo = false;
            }
        }

        private void UpdateEndgame(GameTime gameTime)
        {
            //Play the game over music if it is not playing or is finished
            if ((MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= gameOverMusic.Duration) && isFadingOut == false)
            {
                //Play the game over music
                MediaPlayer.Play(gameOverMusic);
            }

            //Change the transparency for the fading in transition if it is fading in
            ChangeTransparencyForFadeIn();

            //Change the transparency of the new high score message if the new high score message is being displayed
            if (newHighScoreMsg.Length > 0)
            {
                //Change the transparency of the new high score message for a flashing effect
                newHsTransparency = (float)Math.Cos(0.001 * Math.PI * gameTime.TotalGameTime.TotalMilliseconds);

                //Play the new high score sound if it has not been played yet
                if (newHighScoreSndHasPlayed == false)
                {
                    //Set the new high score sound as played and play the new high score sound
                    newHighScoreSndHasPlayed = true;
                    newHighScoreSnd.CreateInstance().Play();
                }
            }

            //Check whether the mouse is hovering over the menu button
            if (menuBtnRec.Contains(mouse.Position))
            {
                //Set the mouse as hovering over the menu button
                hoveringOverMenu = true;

                //Check whether the mouse clicked on the menu button
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                {
                    //Set the display as fading out, the next game state as menu, and update the high score message location on the menu
                    isFadingOut = true;
                    nextGameState = MENU;
                    menuHighScoreLoc.X = screenWidth / 2 - statsFont.MeasureString(Convert.ToString(highScore)).X / 2;

                    //Stop the music
                    MediaPlayer.Stop();

                    //Play the button click sound
                    buttonClickSnd.CreateInstance().Play();
                }
            }
            else
            {
                //set the hovering over the menu as false
                hoveringOverMenu = false;
            }

            //Change the transparency for fading out for the transition
            ChangeTransparencyForFadeOut();
        }

        private void DrawMenu()
        {
            //Draw the background of the menu
            natureBgAnim.Draw(spriteBatch, Color.White * fadeInOutTransparency, Animation.FLIP_NONE);

            //Draw the title and its shadow
            spriteBatch.DrawString(titleFont, titleText, new Vector2(titleLoc.X + SHADOW_DISPLACEMENT, titleLoc.Y + SHADOW_DISPLACEMENT), Color.Black * fadeInOutTransparency);
            spriteBatch.DrawString(titleFont, titleText, titleLoc, Color.Brown * fadeInOutTransparency);

            //Draw the high score and its shadow in the menu
            spriteBatch.DrawString(statsFont, Convert.ToString(highScore), new Vector2(menuHighScoreLoc.X + SHADOW_DISPLACEMENT, menuHighScoreLoc.Y + SHADOW_DISPLACEMENT), Color.Black * fadeInOutTransparency);
            spriteBatch.DrawString(statsFont, Convert.ToString(highScore), menuHighScoreLoc, Color.Yellow * fadeInOutTransparency);

            //Draw the play button on the menu depending on if the user is hovering over it
            if (hoveringOverPlay)
            {
                //Draw the play button translucent
                spriteBatch.Draw(playBtnImg, playBtnRec, Color.White * 0.5f * fadeInOutTransparency);
            }
            else
            {
                //Draw the play button regularly
                spriteBatch.Draw(playBtnImg, playBtnRec, Color.White * fadeInOutTransparency);
            }

            //Draw the instructions button on the menu depending on if the user is hovering over it
            if (hoveringOverInstructions)
            {
                //Draw the instructions button translucent
                spriteBatch.Draw(instructionsBtnImg, instructionsBtnRec, Color.White * 0.5f * fadeInOutTransparency);
            }
            else
            {
                //Draw the instructions button regularly
                spriteBatch.Draw(instructionsBtnImg, instructionsBtnRec, Color.White * fadeInOutTransparency);
            }

            //Draw the exit button on the menu depending on if the user is hovering over it
            if (hoveringOverExit)
            {
                //Draw the exit button translucent
                spriteBatch.Draw(exitBtnImg, exitBtnRec, Color.White * 0.5f * fadeInOutTransparency);
            }
            else
            {
                //Draw the exit button regularly
                spriteBatch.Draw(exitBtnImg, exitBtnRec, Color.White * fadeInOutTransparency);
            }
        }

        private void DrawPlay()
        {
            //Draw the background of the gameplay
            spriteBatch.Draw(leavesBgImg, bgRec, Color.LightGreen * 0.7f);

            //Display the health, ammo, highscore, current score, and time remaining (and its shadow) in the stats bar
            spriteBatch.Draw(healthImg, healthIconRec, Color.White);
            spriteBatch.DrawString(statsFont, "x " + healthCount, healthCountLoc, Color.White);
            spriteBatch.DrawString(statsFont, "x " + ammoCount, ammoCountLoc, Color.White);
            spriteBatch.Draw(ammoImg, ammoIconRec, ammoSrcRec, Color.White);       
            spriteBatch.DrawString(statsFont, gameTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(timerLoc.X + SHADOW_DISPLACEMENT, timerLoc.Y + SHADOW_DISPLACEMENT), Color.DarkGoldenrod);
            spriteBatch.DrawString(statsFont, gameTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), timerLoc, Color.GhostWhite);
            spriteBatch.DrawString(statsFont, Convert.ToString(highScore), highScoreLoc, Color.Yellow);
            spriteBatch.DrawString(statsFont, Convert.ToString(curScore), curScoreLoc, Color.White);

            //Draw the beetle facing up or down depending on its direction
            if (beetleDir.Y == UP)
            {
                //Draw the beetle as facing up
                beetleAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
            else
            {
                //Draw the beetle as facing down
                beetleAnim.Draw(spriteBatch, Color.White, Animation.FLIP_VERTICAL);
            }

            //Draw the spider facing left or right depending on its direction
            if (spiderDir.X == LEFT)
            {
                //Draw the spider as facing to the left
                spiderAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
            else
            {
                //Draw the spider as facing to the right
                spiderAnim.Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
            }

            //Draw the butterfly facing left or right depending on its direction
            if (butterflyDir.X == LEFT)
            {
                //Draw the butterfly as facing left
                butterflyAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
            else
            {
                //Draw the butterfly as facing right
                butterflyAnim.Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
            }

            //Draw the ammo pickups
            ammoAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);

            //Draw the first hit graphic if it is active
            if (hitGraphicTimer1.IsActive())
            {
                //Draw the first hit graphic
                spriteBatch.Draw(hitImg, hitRec1, Color.White);
            }

            //Draw the second hit graphic if it is active
            if (hitGraphicTimer2.IsActive())
            {
                //Draw the second hit graphic
                spriteBatch.Draw(hitImg, hitRec2, Color.White);
            }

            //Draw the third hit graphic if it is active
            if (hitGraphicTimer3.IsActive())
            {
                //Draw the third hit graphic
                spriteBatch.Draw(hitImg, hitRec3, Color.White);
            }

            //Draw the large reason for game over sign
            spriteBatch.DrawString(reasonForGameOverFont, reasonForGameOverMsg, reasonForGameOverLoc, Color.Black);
        }

        private void DrawInstructions()
        {
            //Draw the background of the instructions
            natureBgAnim.Draw(spriteBatch, Color.White * fadeInOutTransparency, Animation.FLIP_NONE);

            //Draw the instructions
            spriteBatch.Draw(instructionsImg, instructionRec, Color.White * fadeInOutTransparency);

            //Draw the back button
            if (hoveringOverBack)
            {
                //Draw the back button as slightly translucent
                spriteBatch.Draw(backBtnImg, backBtnRec, Color.White * 0.5f * fadeInOutTransparency);
            }
            else
            {
                //Draw the back button regularly
                spriteBatch.Draw(backBtnImg, backBtnRec, Color.White * fadeInOutTransparency);
            }
        }

        private void DrawPause()
        {
            //Draw the background of the gameplay
            spriteBatch.Draw(leavesBgImg, bgRec, Color.LightGreen * 0.7f);

            //Display the paused title
            spriteBatch.DrawString(msgFont, pausedText, pausedTextLoc, Color.Black);
            spriteBatch.DrawString(msgFont, pausedMsg, pausedMsgLoc, Color.Black);
            spriteBatch.DrawString(msgFont, pausedCurScoreText, pausedCurScoreLoc, Color.GhostWhite);
            spriteBatch.DrawString(msgFont, pausedHighScoreText, pausedHighScoreLoc, Color.Yellow);
        }

        private void DrawPregame()
        {
            //Draw the background of the gameplay
            spriteBatch.Draw(leavesBgImg, bgRec, Color.LightGreen * 0.7f * fadeInOutTransparency);

            //Draw the go button
            if (hoveringOverGo)
            {
                //Draw the go button as slightly translucent
                spriteBatch.Draw(goBtnImg, goBtnRec, Color.White * 0.5f * fadeInOutTransparency);
            }
            else
            {
                //Draw the go button regularly
                spriteBatch.Draw(goBtnImg, goBtnRec, Color.White * fadeInOutTransparency);
            }
        }

        private void DrawEndgame()
        {
            //Draw the background of the gameplay
            spriteBatch.Draw(leavesBgImg, bgRec, Color.LightGreen * 0.7f * fadeInOutTransparency);

            //Draw the menu button based on whether the mouse is hovering over it
            if (hoveringOverMenu)
            {
                //Draw the button as the mouse hovering over it
                spriteBatch.Draw(menuBtnImg, menuBtnRec, Color.White * 0.5f * fadeInOutTransparency);
            }
            else
            {
                //Draw the button regularly
                spriteBatch.Draw(menuBtnImg, menuBtnRec, Color.White * fadeInOutTransparency);
            }

            //Display the game over message and its shadow
            spriteBatch.DrawString(msgFont, gameOverMsg, new Vector2(gameOverMsgLoc.X + SHADOW_DISPLACEMENT, gameOverMsgLoc.Y + SHADOW_DISPLACEMENT), Color.Black * fadeInOutTransparency);
            spriteBatch.DrawString(msgFont, gameOverMsg, gameOverMsgLoc, Color.Red * fadeInOutTransparency);

            //Display the new high score message and its shadow
            spriteBatch.DrawString(newHighScoreFont, newHighScoreMsg, new Vector2(newHighScoreMsgLoc.X + SHADOW_DISPLACEMENT, newHighScoreMsgLoc.Y + SHADOW_DISPLACEMENT), Color.Black * newHsTransparency * fadeInOutTransparency);
            spriteBatch.DrawString(newHighScoreFont, newHighScoreMsg, newHighScoreMsgLoc, Color.MonoGameOrange * newHsTransparency * fadeInOutTransparency);

            //Display the high score message and its shadow
            spriteBatch.DrawString(msgFont, highScoreMsg, new Vector2(highScoreMsgLoc.X + SHADOW_DISPLACEMENT, highScoreMsgLoc.Y + SHADOW_DISPLACEMENT), Color.Magenta * fadeInOutTransparency);
            spriteBatch.DrawString(msgFont, highScoreMsg, highScoreMsgLoc, Color.Black * fadeInOutTransparency);

            //Display the current score message and its shadow
            spriteBatch.DrawString(msgFont, curScoreMsg, new Vector2(curScoreMsgLoc.X + SHADOW_DISPLACEMENT, curScoreMsgLoc.Y + SHADOW_DISPLACEMENT), Color.White * fadeInOutTransparency);
            spriteBatch.DrawString(msgFont, curScoreMsg, curScoreMsgLoc, Color.Black * fadeInOutTransparency);
        }

        private void TranslateInsect(GameTime gameTime, ref Animation insectAnim, ref Vector2 insectLoc, Vector2 insectDir, Vector2 insectMaxSpeed, Vector2 insectStoppingLoc)
        {
            //Store the insect speed and the insect speed multiplier for acceleration
            Vector2 insectSpeed;
            Vector2 insectSpeedMultiplier = new Vector2(1f, 1f);

            //Calculate the acceleration in the x or y direction if the insect is within its the distance to its stopping location
            if (Math.Abs(insectLoc.X - insectStoppingLoc.X) <= accelerationDist.X && insectDir.X != 0)
            {
                //Calculate the speed multiplier in the x direction
                insectSpeedMultiplier.X = Math.Abs(insectStoppingLoc.X - insectLoc.X) * (1f - minInsectSpeedMultiplier) / accelerationDist.X + minInsectSpeedMultiplier;
            }
            else if (Math.Abs(insectLoc.Y - insectStoppingLoc.Y) <= accelerationDist.Y && insectDir.Y != 0)
            {
                //Calculate the speed multiplier in the y direction
                insectSpeedMultiplier.Y = Math.Abs(insectStoppingLoc.Y - insectLoc.Y) * (1f - minInsectSpeedMultiplier) / accelerationDist.Y + minInsectSpeedMultiplier;
            }

            //Set the insect speed for the current update
            insectSpeed.X = insectDir.X * insectMaxSpeed.X * (float)gameTime.ElapsedGameTime.TotalSeconds * insectSpeedMultiplier.X;
            insectSpeed.Y = insectDir.Y * insectMaxSpeed.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * insectSpeedMultiplier.Y;

            //Move the insect's location by the insect speed
            insectLoc.X += insectSpeed.X;
            insectLoc.Y += insectSpeed.Y;

            //Set the inect animation location's as the approximate insect's location
            insectAnim.destRec.X = (int)insectLoc.X;
            insectAnim.destRec.Y = (int)insectLoc.Y;
        }

        private void CheckMouseClick()
        {
            //Check only if the left button is pressed currently and not in previous loops
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                //Check whether the mouse intersects the beetle, spider, butterfly, ammo, or nothing
                if (beetleAnim.destRec.Contains(mouse.Position) && beetleTimer.IsFinished() == false && ammoCount > 0)
                {
                    //Increase the current score and reduce the ammo by 1
                    curScore += EATEN_SCORE_BOOST;
                    ammoCount--;

                    //Show the hit graphic
                    ShowHitGraphicAndEatingSound(beetleAnim, ref hitGraphicTimer1, ref hitRec1);

                    //Move the beetle to the hidden location and reset its timer
                    beetleLoc = hiddenLoc;
                    beetleAnim.destRec.Location = beetleLoc.ToPoint();
                    beetleTimer.ResetTimer(false);

                    //Check for leveling up
                    CheckLevelingUp();

                    //Generate an ammo for a 50% chance
                    if (rng.Next(1, 101) <= 50 && ammoPickupTimer.IsActive() == false)
                    {
                        //Generate an ammo pickup
                        GenerateAmmoPickup();                       
                    }
                }
                else if (spiderAnim.destRec.Contains(mouse.Position) && spiderTimer.IsFinished() == false && ammoCount > 0)
                {
                    //Increase the current score and reduce the ammo by 1
                    curScore += EATEN_SCORE_BOOST;
                    ammoCount--;

                    //Show the hit graphic
                    ShowHitGraphicAndEatingSound(spiderAnim, ref hitGraphicTimer2, ref hitRec2);

                    //Move the spider to the hidden location
                    spiderLoc = hiddenLoc;
                    spiderAnim.destRec.Location = spiderLoc.ToPoint();
                    spiderTimer.ResetTimer(false);

                    //Check for leveling up
                    CheckLevelingUp();
                   
                    //Generate an ammo for a 50% chance
                    if (rng.Next(1, 101) <= 50 && ammoPickupTimer.IsActive() == false)
                    {
                        //Generate an ammo pickup
                        GenerateAmmoPickup();                      
                    }
                }
                else if (butterflyAnim.destRec.Contains(mouse.Position) && butterflyTimer.IsFinished() == false && ammoCount > 0)
                {
                    //Increase the current score and reduce the ammo by 1
                    curScore += EATEN_SCORE_BOOST;
                    ammoCount--;

                    //Show the hit graphic
                    ShowHitGraphicAndEatingSound(butterflyAnim, ref hitGraphicTimer3, ref hitRec3);

                    //Move the butterfly to the hidden location
                    butterflyLoc = hiddenLoc;
                    butterflyAnim.destRec.Location = butterflyLoc.ToPoint();
                    butterflyTimer.ResetTimer(false);

                    //Check for leveling up
                    CheckLevelingUp();
                    
                    //Generate an ammo for a 50% chance
                    if (rng.Next(1, 101) <= 50 && ammoPickupTimer.IsActive() == false)
                    {
                        //Generate an ammo pickup
                        GenerateAmmoPickup();
                    }
                }
                else if (ammoAnim.destRec.Contains(mouse.Position))
                {
                    //Increase the ammo count by 2
                    ammoCount += AMMO_PICKUP_BOOST;

                    //Set its count to 10 if the ammo count goes over 10
                    if (ammoCount > MAX_AMMO)
                    {
                        //Set the ammo to the max ammo
                        ammoCount = MAX_AMMO;
                    }

                    //Move the ammo to the hidden location
                    ammoPickupLoc = hiddenLoc;
                    ammoAnim.destRec.Location = ammoPickupLoc.ToPoint();

                    //stop the ammo timer
                    ammoPickupTimer.Deactivate();

                    //Play the ammo pickup sound effect
                    ammoPickupSnd.CreateInstance().Play();
                }        
                else
                {
                    //Only reduce the ammo if it is greater than 0
                    if (ammoCount > 0)
                    {
                        //Reduce the ammo by 1
                        ammoCount--;    
                        
                        //Play the miss sound
                        missSnd.CreateInstance().Play();                    
                    }
                    else
                    {
                        //Play the squawk sound for a miss with no ammo
                        squawkSnd.CreateInstance().Play();
                    }

                    //Reduce the current score for misclicking
                    curScore -= MISCLICK_SCORE_REDUCTION;
                }
            }            
        }

        private void GenerateNewBeetle()
        {
            //Reset the insect timer and ramdomize the beetle's stopping location
            beetleTimer.ResetTimer(false);
            beetleStoppingLoc.X = rng.Next(screenWidth - beetleAnim.frameWidth + 1);
            beetleStoppingLoc.Y = rng.Next((int)(statsFont.MeasureString(Convert.ToString(curScore)).Y + curScoreLoc.Y), screenHeight - (int)(beetleAnim.frameHeight * beetleScaler) + 1);

            //Set the beetle's x coordinate as its stopping location
            beetleLoc.X = beetleStoppingLoc.X;

            //Ramdomize the beetle's direction and y coordinate for its next entrance
            if (rng.Next(1, 101) <= 50)
            {
                //Set the beetle's direction and its location below the screen
                beetleDir.Y = UP;
                beetleLoc.Y = screenHeight;
            }
            else
            {
                //Set the beetle as moving down and its location as above the screen
                beetleDir.Y = DOWN;
                beetleLoc.Y = -1 * beetleAnim.frameHeight * beetleScaler;
            }

            //Set the beetle animation location as the randomized location
            beetleAnim.destRec.X = (int)beetleLoc.X;
            beetleAnim.destRec.Y = (int)beetleLoc.Y;

            //Animate the beetle
            beetleAnim.isAnimating = true;
        }

        private void GenerateNewHorizontalInsect(Timer insectTimer, ref Vector2 insectStoppingLoc, ref Vector2 insectLoc, Animation insectAnim, float insectScaler, ref Vector2 insectDir)
        {
            //Reset the insect timer and ramdomize the spider's stopping location
            insectTimer.ResetTimer(false);
            insectStoppingLoc.X = rng.Next(screenWidth - insectAnim.frameWidth + 1);
            insectStoppingLoc.Y = rng.Next((int)(statsFont.MeasureString(Convert.ToString(curScore)).Y + curScoreLoc.Y), screenHeight - (int)(insectAnim.frameHeight * insectScaler) + 1);
      
            //Set the insect's y coordinate as its stopping location
            insectLoc.Y = insectStoppingLoc.Y;

            //Ramdomize the insect's direction and x coordinate for its next entrance
            if (rng.Next(1, 101) <= 50)
            {
                //Set the insect's direction as moving left and set its location to the right of the screen
                insectDir.X = LEFT;
                insectLoc.X = screenWidth;
            }
            else
            {
                //Set the insect's direction as moving right and set its location to the left of the screen
                insectDir.X = RIGHT;
                insectLoc.X = -1 * insectAnim.frameWidth * insectScaler;
            }

            //Set the insect animation location as the randomized location
            insectAnim.destRec.X = (int)insectLoc.X;
            insectAnim.destRec.Y = (int)insectLoc.Y;

            //Animate the insect
            insectAnim.isAnimating = true;
        }

        private void GenerateAmmoPickup()
        {
            //Set the random ammo location
            ammoPickupLoc.X = rng.Next(screenWidth - ammoAnim.destRec.Width + 1);
            ammoPickupLoc.Y = rng.Next((int)(statsFont.MeasureString(Convert.ToString(curScore)).Y + curScoreLoc.Y), screenHeight - ammoAnim.frameHeight + 1);

            //Set the ammo animation location to the randomly generated location
            ammoAnim.destRec.Location = ammoPickupLoc.ToPoint();

            //Reset the ammo timer
            ammoPickupTimer.ResetTimer(true);

            //Play the ammo spawn sound
            ammoSpawnSnd.CreateInstance().Play();
        }

        private void ResetGame()
        {
            //Reset the current score, the health count, ammo count, and level
            curScore = 0;
            healthCount = MAX_HEALTH;
            ammoCount = MAX_AMMO;
            level = 1;

            //Reset the new high score, the location of the reason for game over message, and set the countdown and new high score sound as not played yet
            newHighScoreMsg = "";
            reasonForGameOverLoc.Y = screenHeight;
            countdownStarted = false;
            newHighScoreSndHasPlayed = false;

            //Set the insect locations back to the hidden location other than the beetle
            spiderLoc = hiddenLoc;
            spiderAnim.destRec.Location = spiderLoc.ToPoint();
            butterflyLoc = hiddenLoc;
            butterflyAnim.destRec.Location = butterflyLoc.ToPoint();

            //Set the ammo pickups to the hidden location
            ammoPickupLoc = hiddenLoc;
            ammoAnim.destRec.Location = ammoPickupLoc.ToPoint();

            //Reset the game timer
            gameTimer.ResetTimer(false);

            //Reset the hit graphic timers
            hitGraphicTimer1.ResetTimer(false);
            hitGraphicTimer2.ResetTimer(false);
            hitGraphicTimer3.ResetTimer(false);

            //Reset the insect timers other than the beetle
            spiderTimer.ResetTimer(false);
            butterflyTimer.ResetTimer(false);

            //Reset the ammo pickup timers
            ammoPickupTimer.ResetTimer(false);
        }

        private void RemoveActiveAmmoPickup()
        {
            //Check whether the pickup timer is done
            if (ammoPickupTimer.IsFinished())
            {
                //Move the ammo to the hidden location
                ammoPickupLoc = hiddenLoc;
                ammoAnim.destRec.Location = ammoPickupLoc.ToPoint();

                //Reset the timer
                ammoPickupTimer.ResetTimer(false);

                //Play the ammo retreating sound
                ammoRetreatSnd.CreateInstance().Play();
            }            
        }

        private void ShowHitGraphicAndEatingSound(Animation insectAnim, ref Timer hitGraphicTimer, ref Rectangle hitRec)
        {          
            //Set the location of the first hit graphic at the insect's location
            hitRec.X = insectAnim.destRec.X + insectAnim.destRec.Width / 2 - hitRec.Width / 2;
            hitRec.Y = insectAnim.destRec.Y + insectAnim.destRec.Height / 2 - hitRec.Height / 2;

            //Restart the first hit graphic timer
            hitGraphicTimer.Activate();

            //Play the eating sound
            eatingSnd.CreateInstance().Play();           
        }

        private void CheckLevelingUp()
        {
            //Check for any leveling up based on the score
            if (level == 1 && curScore >= MIN_SCORE_FOR_LEVEL_2)
            {
                //Set the level to level 2 and generate the first spider
                level = SPIDER_LEVEL;
                GenerateNewHorizontalInsect(spiderTimer, ref spiderStoppingLoc, ref spiderLoc, spiderAnim, spiderScaler, ref spiderDir);                
            }
            else if (level == SPIDER_LEVEL && curScore >= MIN_SCORE_FOR_LEVEL_3)
            {
                //Set the level to level 3 and generate the first butterfly
                level = BUTTERFLY_LEVEL;
                GenerateNewHorizontalInsect(butterflyTimer, ref butterflyStoppingLoc, ref butterflyLoc, butterflyAnim, butterflyScaler, ref butterflyDir);
            }
        }

        private void ChangeTransparencyForFadeIn()
        {
            //Transition the fade in only if the screen is supposed to be fading in
            if (isFadingIn)
            {
                //Increase the transparency of the screen by the fading speed
                fadeInOutTransparency += fadingSpeed;

                //Stop the transition once the transparency is completely opaque
                if (fadeInOutTransparency >= 1f)
                {
                    isFadingIn = false;
                }
            }
        }

        private void ChangeTransparencyForFadeOut()
        {
            //Change the screen transparency for the transition only if the screen is supposed to be fading out
            if (isFadingOut)
            {
                //Stop fading in if the screen is still fading in
                if (isFadingIn)
                {
                    //Set the fading in as false
                    isFadingIn = false;
                }
                
                //Decrease the transparency of the screen by the fading speed
                fadeInOutTransparency -= fadingSpeed;

                //Change the game state when the transparency is at 0
                if (fadeInOutTransparency <= 0f)
                {
                    //Set the sceen as fading in and not fading out and the game state as the next game state
                    isFadingOut = false;
                    isFadingIn = true;
                    gameState = nextGameState;
                }
            }
        }

        private void MoveOrStopInsect(Timer insectTimer, ref Animation insectAnim, GameTime gameTime, ref Vector2 insectLoc, ref Vector2 insectDir, Vector2 insectMaxSpeed, ref Vector2 insectStoppingLoc, float insectScaler)
        {
            //Check wither the insect timer is finished, it is at the stopping location, or is moving to the stopping location 
            if (insectTimer.IsFinished())
            {
                //If the beetle is still not animating, start animating it and reduce its health and score and start its animation
                if (insectAnim.isAnimating == false)
                {                    
                    //Start animating the insect
                    insectAnim.isAnimating = true;

                    //Reduce the health by 5 and the score by 100
                    curScore -= UNEATEN_SCORE_REDUCTION;
                    healthCount -= HEALTH_DAMAGE;

                    //Play the moving away sound
                    movingAwaySnd.CreateInstance().Play();
                }

                //If the insect is off the screen horizontally, generate a new spider or butterfly or if it is off vertically, generate a new beetle
                if (insectAnim.destRec.Left >= screenWidth || insectAnim.destRec.Right <= 0)
                {
                    //Generate a new horizontal-moving insect
                    GenerateNewHorizontalInsect(insectTimer, ref insectStoppingLoc, ref insectLoc, insectAnim, insectScaler, ref insectDir);
                }
                else if (insectAnim.destRec.Top > screenHeight || insectAnim.destRec.Bottom < 0)
                {
                    //Generate a new beetle
                    GenerateNewBeetle();
                }

                //Move the insect off the screen
                TranslateInsect(gameTime, ref insectAnim, ref insectLoc, insectDir, insectMaxSpeed, insectStoppingLoc);
            }
            else if ((insectAnim.destRec.X >= insectStoppingLoc.X && insectDir.X == RIGHT) || (insectAnim.destRec.X <= insectStoppingLoc.X && insectDir.X == LEFT) || (insectDir.Y == DOWN && insectAnim.destRec.Y >= insectStoppingLoc.Y) || (insectDir.Y == UP && insectAnim.destRec.Y <= insectStoppingLoc.Y))
            {
                //Stop the insect
                if (insectAnim.isAnimating)
                {
                    //stop the animation when the beetle has reached its stopping position and start its timer                    
                    insectAnim.isAnimating = false;
                    insectTimer.Activate();
                }
            }
            else if (insectLoc != hiddenLoc)
            {                
                //Translate the insect to its stopping location
                TranslateInsect(gameTime, ref insectAnim, ref insectLoc, insectDir, insectMaxSpeed, insectStoppingLoc);              
            }
        }        
    }
}
