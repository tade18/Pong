using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private Texture2D _ballTexture;
        private Vector2 _ballPosition;
        private Vector2 _ballVelocity;

        private Texture2D _paddleTexture;
        private Vector2 _paddle1Position;
        private Vector2 _paddle2Position;

        private int _paddleWidth = 20;
        private int _paddleHeight = 100;
        private int _ballSize = 20;
        
        private int _windowWidth = 1000;
        private int _windowHeight = 625;
        
        private float _paddleSpeed = 10f;
        private float _ballSpeed = 7f;
        
        private int _scorePlayer1 = 0;
        private int _scorePlayer2 = 0;
        private SpriteFont _font;
        
        private float elapsedTime = 0f;
        private float speedMultiplier = 1f;
        private const float speedIncreaseInterval = 1f;
        private const float speedFactor = 1.25f;
        
        private float countdownTime = 3f;
        private bool isCountingDown = true;
        
        private int winningPlayer = 0;
        private bool isGameOver = false;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = _windowWidth,
                PreferredBackBufferHeight = _windowHeight
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _paddle1Position = new Vector2(20, (_windowHeight - _paddleHeight) / 2);
            _paddle2Position = new Vector2(_windowWidth - 20 - _paddleWidth, (_windowHeight - _paddleHeight) / 2);
            _ballPosition = new Vector2(_windowWidth / 2-10, _windowHeight / 2-10);
            _ballVelocity = new Vector2(_ballSpeed, _ballSpeed);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _paddleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _paddleTexture.SetData(new[] { Color.White });

            _ballTexture = new Texture2D(GraphicsDevice, 1, 1);
            _ballTexture.SetData(new[] { Color.White });

            _font = Content.Load<SpriteFont>("ScoreFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();
            
            if (isGameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    ResetGame();
                }
                return;
            }
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += deltaTime;
            
            if (isCountingDown)
            {
                countdownTime -= deltaTime;
                if (countdownTime <= 0)
                {
                    isCountingDown = false;
                    countdownTime = 3f;
                }
                return;
            }

            
            if (elapsedTime >= speedIncreaseInterval)
            {
                speedMultiplier *= speedFactor;
                elapsedTime = 0f;
            }
            
            _ballPosition += _ballVelocity * speedMultiplier * deltaTime;
            
            if (keyboardState.IsKeyDown(Keys.W) && _paddle1Position.Y > 0)
                _paddle1Position.Y -= _paddleSpeed;
            if (keyboardState.IsKeyDown(Keys.S) && _paddle1Position.Y < _windowHeight - _paddleHeight)
                _paddle1Position.Y += _paddleSpeed;
            
            if (keyboardState.IsKeyDown(Keys.Up) && _paddle2Position.Y > 0)
                _paddle2Position.Y -= _paddleSpeed;
            if (keyboardState.IsKeyDown(Keys.Down) && _paddle2Position.Y < _windowHeight - _paddleHeight)
                _paddle2Position.Y += _paddleSpeed;
            
            _ballPosition += _ballVelocity;
            
            if (_ballPosition.Y <= 0 || _ballPosition.Y >= _windowHeight - _ballSize)
                _ballVelocity.Y *= -1;
            
            Rectangle ballRect = new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, _ballSize, _ballSize);
            Rectangle paddle1Rect = new Rectangle((int)_paddle1Position.X, (int)_paddle1Position.Y, _paddleWidth, _paddleHeight);
            Rectangle paddle2Rect = new Rectangle((int)_paddle2Position.X, (int)_paddle2Position.Y, _paddleWidth, _paddleHeight);

            if (ballRect.Intersects(paddle1Rect) && _ballVelocity.X < 0)
            {
                _ballPosition.X = _paddle1Position.X + _paddleWidth;
                _ballVelocity.X *= -1;
            }

            if (ballRect.Intersects(paddle2Rect) && _ballVelocity.X > 0)
            {
                _ballPosition.X = _paddle2Position.X - _ballSize;
                _ballVelocity.X *= -1;
            }
            
            if (_ballPosition.X <= 0)
            {
                _scorePlayer2++;
                CheckForWin(2);
                ResetBall(-1);
            }
            else if (_ballPosition.X >= _windowWidth - _ballSize)
            {
                _scorePlayer1++;
                CheckForWin(1);
                ResetBall(1);
            }

            base.Update(gameTime);
        }
        
        private void CheckForWin(int player)
        {
            if (_scorePlayer1 == 5)
            {
                winningPlayer = 1;
                isGameOver = true;
            }
            else if (_scorePlayer2 == 5)
            {
                winningPlayer = 2;
                isGameOver = true;
            }
        }
        
        private void ResetBall(int direction)
        {
            _ballPosition = new Vector2(_windowWidth / 2-10, _windowHeight / 2-10);
            _ballVelocity = new Vector2(_ballSpeed * direction, _ballSpeed);
            speedMultiplier = 1f;
            isCountingDown = true;
        }
        
        private void ResetGame()
        {
            _scorePlayer1 = 0;
            _scorePlayer2 = 0;
            isGameOver = false;
            winningPlayer = 0;
            ResetBall(1);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_paddleTexture, new Rectangle((int)_paddle1Position.X, (int)_paddle1Position.Y, _paddleWidth, _paddleHeight), Color.White);

            _spriteBatch.Draw(_paddleTexture, new Rectangle((int)_paddle2Position.X, (int)_paddle2Position.Y, _paddleWidth, _paddleHeight), Color.White);
            if (!isGameOver)
            {
                _spriteBatch.Draw(_ballTexture, new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, _ballSize, _ballSize), Color.White);
            }

            _spriteBatch.DrawString(_font, $"{_scorePlayer1} - {_scorePlayer2}", new Vector2(_windowWidth /2 -35, 20), Color.White);

            if (isCountingDown)
            {
                string countdownText = Math.Ceiling(countdownTime).ToString();
                Vector2 textSize = _font.MeasureString(countdownText);
                Vector2 textPosition = new Vector2(_windowWidth / 2 - textSize.X / 2, _windowHeight-20);
                _spriteBatch.DrawString(_font, countdownText, textPosition, Color.Red);
            }
            
            if (isGameOver)
            {
                string winnerText = $"Player {winningPlayer} won!";
                string restartText = "Press Space for restart";
                Vector2 winnerSize = _font.MeasureString(winnerText);
                Vector2 restartSize = _font.MeasureString(restartText);

                _spriteBatch.DrawString(_font, winnerText, new Vector2(_windowWidth / 2 - winnerSize.X / 2, _windowHeight / 2 - winnerSize.Y / 2 - 20), Color.Yellow);
                _spriteBatch.DrawString(_font, restartText, new Vector2(_windowWidth / 2 - restartSize.X / 2, _windowHeight / 2 - restartSize.Y / 2 + 20), Color.White);
            }
            

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
