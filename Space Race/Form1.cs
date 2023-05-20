using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Security.Cryptography.X509Certificates;

namespace Space_Race
{
    public partial class Form1 : Form
    {
        //players
        Rectangle player1 = new Rectangle();
        int player1Speed = 4;
        Rectangle player2 = new Rectangle();
        int player2Speed = 4;


        //asteroids
        List<Rectangle> asteroidList = new List<Rectangle>();
        List<int> asteroidSpeeds = new List<int>();

        //paint
        SolidBrush lightGreenBrush = new SolidBrush(Color.LightGreen);
        SolidBrush lightBlueBrush = new SolidBrush(Color.LightBlue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        Pen whitePen = new Pen(Color.White);

        Rectangle line = new Rectangle();


        //player 1
        bool wDown = false;
        bool sDown = false;

        //player 2
        bool upArrowDown = false;
        bool downArrowDown = false;

        //random value generators
        Random randGen = new Random();
        int randValue = 0;

        //state
        string state = "waiting";

        //variables
        int playerScore1 = 0;
        int playerScore2 = 0;

        //sounds
        SoundPlayer pointSound = new SoundPlayer(Properties.Resources.beep);
        SoundPlayer deathSound = new SoundPlayer(Properties.Resources.death);


        public void InitializeGame()
        {
            //reset everything
            playerScore1 = 0;
            playerScore2 = 0;

            asteroidList.Clear();
            asteroidSpeeds.Clear();

            player1 = new Rectangle(150, 400, 15, 10);
            player2 = new Rectangle(400, 400, 15, 10);

            line = new Rectangle(300, 0, 8, 600);

            titleLabel.Text = "";
            subtitleLabel.Text = "";
            player1Score.Text = "";
            player2Score.Text = "";
            
            Rectangle asteroid = new Rectangle(randValue, 0, 8, 8);
            asteroidList.Add(asteroid);
            asteroidSpeeds.Add(randGen.Next(2, 20));

            state = "playing";
            gameTimer.Enabled = true;
        }
        public Form1()
        {
            InitializeComponent();

            Rectangle asteroid = new Rectangle(20, 0, 10, 10);
            asteroidList.Add(asteroid);
            asteroidSpeeds.Add(randGen.Next(2, 20));
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
            }

            //player 2
            switch (e.KeyCode)
            {
                case Keys.Down:
                    downArrowDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //player 1
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
            }

            //player 2
            switch (e.KeyCode)
            {
                case Keys.Down:
                    downArrowDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
            }

            //initialize game
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (state == "waiting" || state == "over")
                    {
                        InitializeGame();
                    }
                    break;
                case Keys.Escape:
                    if (state == "waiting" || state == "over")
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //player2 Controls
            if (upArrowDown == true && player2.Y > 0)
            {
                player2.Y -= player2Speed;
            }
            if (downArrowDown == true && player2.Y < this.Height - player2.Height)
            {
                player2.Y += player2Speed;
            }
            //player1 Controls
            if (wDown == true && player1.Y > 0)
            {
                player1.Y -= player1Speed;
            }
            if (sDown == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y += player1Speed;
            }

            //asteroid controls
            for (int i = 0; i < asteroidList.Count; i++)
            {
                int x = asteroidList[i].X + asteroidSpeeds[i];
                asteroidList[i] = new Rectangle(x, asteroidList[i].Y, 10, 10);
            }

            randValue = randGen.Next(0, 100);
            if (randValue <= 20)
            {
                randValue = randGen.Next(10, this.Height - 10 - 10);

                Rectangle asteroid = new Rectangle(0, randValue, 10, 10);
                asteroidSpeeds.Add(randGen.Next(2, 20));
                asteroidList.Add(asteroid);
            }

            //asteroid interactions
            for (int i = 0; i < asteroidList.Count; i++)
            {
                if (asteroidList[i].IntersectsWith(player1))
                {
                    deathSound.Play();
                    player1.Y = 400;
                    player1.X = 150;
                    asteroidList.RemoveAt(i);
                    asteroidSpeeds.RemoveAt(i);
                }
            }
            for (int i = 0; i < asteroidList.Count; i++)
            {
                if (asteroidList[i].IntersectsWith(player2))
                {
                    deathSound.Play();
                    player2.Y = 400;
                    player2.X = 400;
                    asteroidList.RemoveAt(i);
                    asteroidSpeeds.RemoveAt(i);
                }
            }
            for (int i = 0; i < asteroidList.Count; i++)
            {
                if (asteroidList[i].X >= this.Width)
                {
                    asteroidList.RemoveAt(i);
                    asteroidSpeeds.RemoveAt(i);
                }
            }
            //player interactions
            if (player1.Y <= 0)
            {
                pointSound.Play();
                player1.Y = 400;
                player1.X = 150;

                playerScore1 = playerScore1 + 1;
                player1Score.Text = $"{playerScore1}";
            }
            else if (player2.Y <= 0)
            {
                pointSound.Play();
                player2.Y = 400;
                player2.X = 400;

                playerScore2 = playerScore2 + 1;
                player2Score.Text = $"{playerScore2}";
            }
            //determine winner
            if (playerScore1 == 3)
            {

                asteroidList.Clear();
                asteroidSpeeds.Clear(); 
                gameTimer.Stop();

                winnerLabel.Text = "Player 1 Wins !!";
                Thread.Sleep(800);
                state = "over";
            }
            if (playerScore2 == 3)
            {
                
                asteroidList.Clear();
                asteroidSpeeds.Clear();

                winnerLabel.Text = "Player 2 Wins !!";
                Thread.Sleep(800);
                state = "over";
            }

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (state == "waiting")
            {
                player1Score.Text = "";
                player2Score.Text = "";

                titleLabel.Text = "SPACE RACERS";
                subtitleLabel.Text = "Press Space to Play or Esc. to Exit";
            }
            if (state == "playing")
            {
                e.Graphics.DrawRectangle(whitePen, line);

                e.Graphics.FillRectangle(lightGreenBrush, player1);
                e.Graphics.FillRectangle(lightBlueBrush, player2);

                for (int i = 0; i < asteroidList.Count; i++)
                {
                    e.Graphics.DrawEllipse(whitePen, asteroidList[i]);
                }
            }
            if (state == "over")
            {
                winnerLabel.Text = "";
                player1Score.Text = "";
                player2Score.Text = "";


                titleLabel.Text = "GAME OVER";
                subtitleLabel.Text = "\n Press Space to Play or Esc. to Exit";
            }
         
        }

    }
}
