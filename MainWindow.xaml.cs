using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;  //detta är en timer som är aktiv mellan intervaller och är en prioriterad process
namespace endlessrunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer(); //en variabel som kommer behövas för att spelet ska fungera

        Rect playerHitBox;  //rad 26 till 28 innehåller en så kallad hitbox för alla objekt som kommer finnas i spelet. En hitbox är en ruta som är viktig för att kunna känna av om spelaren träffar hindret eller marken.
        Rect groundHitBox;
        Rect obstacleHitBox;

        bool jumping;  // denna boolen har ett sant eller falskt värde som gör det enklare att hoppa.

        int force = 20; // detta värde och int speed har värden som går att ändra på utan att spelet inte fungerar, dessa är de fysiska krafter som finns i spelet, så man kan ändra på dom utan att själva spelet ändras.
        int speed = 5; // precis som int force, är detta ett värde som går att ändra på utan att behöva ändra annan kod.

        Random rnd = new Random();  // skapar en random.

        bool gameOver; // detta är en boolean som avgör om spelet är igång eller när man dör.

        double spriteIndex = 0;  // skapat en index som gör det möjligt att kunna kalla på denna arrayen senare.

        ImageBrush playerSprite = new ImageBrush();  // denna rad inkuklusive de 2 rader neråt gör så att alla objekt och bakgrunden kan få en bild.
        ImageBrush backgroundSprite = new ImageBrush();
        ImageBrush obstacleSprite = new ImageBrush();

        int[] obstaclePosition = { 320, 310, 300, 305, 315 };  // detta är de olika positioner obstacle får

        int score = 0;  // återställer eller gör så att sitt score alltid börjar på 0.


        public MainWindow()  // detta är de principer spelet följer när det har startats.
        {
            InitializeComponent();

            MyCanvas.Focus();

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            backgroundSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background.gif"));

            background.Fill = backgroundSprite;
            background2.Fill = backgroundSprite;

            StartGame();

        }

        private void GameEngine(object sender, EventArgs e)  // all kod inom gamengine är hur spelet fungerar. 
        {
            Canvas.SetLeft(background, Canvas.GetLeft(background) - 3); // hur bakgrunden rör sig.
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) - 3); // hur bakgrunden rör sig.

            if (Canvas.GetLeft(background) > -1250)  // om bakgrundens längd tar slut, startar den om på nytt
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background2) + background2.Width);
            }
            if (Canvas.GetLeft(background2) > -1250)
            {
                Canvas.SetLeft(background2, Canvas.GetLeft(background) + background.Width);
            }

            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 12);

            scoreText.Content = "Score: " + score;  // detta är innehållet i score, som kommer upp när man dör.

            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 15, player.Height - 10);
            obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);

            if (playerHitBox.IntersectsWith(groundHitBox))  // denna if satsen har och göra med när player nuddar marken. Detta kommer att göra att player stannar på marken och ändrar hastighet.
            {
                speed = 0;

                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height) ;

                jumping = false;

                spriteIndex += .5;

                if (spriteIndex > 8)
                {
                    spriteIndex = 1;
                }

                RunSprite(spriteIndex); 

            }


            if (jumping == true)  // när player hoppar, kommer denna if satsen sättas igång
            {
                speed = -9;
                force -= 1;

            }
            
            else  // om inte kraven i if satsen uppfylls är denna aktiv istället.
            {
                speed = 12;
            }

            if (force < 0)  // Om force blir mindre än 0, kommer jumping automatiskt bli falskt.
            {
                jumping = false;
            }

            if(Canvas.GetLeft(obstacle) < 50)  // när hindret har gått ut från skärmen kommer den åka framför player igen så det återkommer ett nytt hinder.
            {
                Canvas.SetLeft(obstacle, 950);
                Canvas.SetTop(obstacle, obstaclePosition[rnd.Next(0, obstaclePosition.Length)]);

                score += 1; // lägger på ett poäng per obstacle man har lyckats komma över.
            }


            if (playerHitBox.IntersectsWith(obstacleHitBox)) // if satsen som gör att obstacle faktiskt blir ett hinder som man kan dö av.
            {
                gameOver = true;  // Om kraven i if satsen uppfylls kommer gameover bli aktivt vilket betyder att spelet kommer att stannas.
                gameTimer.Stop();  // tiden kommer även att stanna eftersom man är död.
            }

            if(gameOver == true)  // om man förlorar, kommer dessa funktioner sättas igång.
            {
                obstacle.Stroke = Brushes.Black;  // skapar en linje runt obstacle's hitbox som gör det tydligt hur man dog
                obstacle.StrokeThickness = 1;  // tjockleken på linjen.

                player.Stroke = Brushes.Red;  // skapar en linje runt player som gör det tydligt vart player hitbox träffade obstacle hitbox. 
                player.StrokeThickness = 1;  // tjockleken på linjen runt obstacle.

                scoreText.Content = scoreText + "Score: " + score + "Press enter to play again! :D";  // det som skrivs ut när man är död. I detta fall kommer din score och press enter to play again skrivas ut.

            }
            else  // om inte kraven ovanför fylls kommer inte att tjockleken på player och obstacle finnas kvar, som gör att den försvinner sen om man spelar igen.
            {
                player.StrokeThickness = 0;
                obstacle.StrokeThickness = 0;
            }
            
            
        }

        private void KeyIsDown(object sender, KeyEventArgs e)  // beskriver vad som händer när enter trycks ner.
        {
            if(e.Key == Key.Enter && gameOver == true)
            {
                StartGame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)  // beskriver vad som händer om spelaren trycker på space, i detta fall kommer player att hoppa.
        {
            if (e.Key == Key.Space && jumping == false && Canvas.GetTop(player) > 260)
            {
                jumping = true;
                force = 15;
                speed = -12;
                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_02.gif"));
            }
        }
        private void StartGame()  // hela denna voiden är allt som händer innan spelet sätts igång, alltså vart bakgrunden ska vara, att player börjar springa, obstacle sätts i rätt position, att jumping inte är aktivt och att timern för spelet startar.
        {
            Canvas.SetLeft(background, 0);
            Canvas.SetLeft(background2, 1250);

            Canvas.SetLeft(player, 110);
            Canvas.SetTop (player, 140);

            Canvas.SetLeft(obstacle, 950);
            Canvas.SetTop(obstacle, 310);

            RunSprite (1);

            obstacleSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacle.png"));

            obstacle.Fill = obstacleSprite;

            jumping = false;
            gameOver = false;
            score = 0;

            scoreText.Content = "Score: " + score;

            gameTimer.Start();
        }

        private void RunSprite(double i)  // beskriver vad programmet gör när player börjar springa, då kommer första bilden sättas igång, sen kommer case 2, case 3 etc.
        {

            switch (i)
            {
                case 1:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_01.gif"));
                    break;
                case 2:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_02.gif"));
                    break;
                case 3:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_03.gif"));
                    break;
                case 4:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_04.gif"));
                    break;
                case 5:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_05.gif"));
                    break;
                case 6:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_06.gif"));
                    break;
                case 7:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_07.gif"));
                    break;
                case 8:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_08.gif"));
                    break;
            }

            player.Fill = playerSprite;

        }
    }
}
