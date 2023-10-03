using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace LowLevelGraphics
{
    public partial class Form1 : Form
    {
        private Random random = new Random();

        // Car
        private Rectangle carBody;
        private Point[] roofPoints;
        private Point[] rWheels;
        private Point[] mWheels;
        private Rectangle[] rimsL;
        private Rectangle[] rimsR;

        // Car action
        private bool carStart;
        private bool carBreak;

        // Speed
        private int speed;
        private int topSpeed;

        // Timer
        private Timer timer;

        // Rotation
        private int rotationAngle;

        // Background
        private Image bg;
        private int image1X, image2X;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.Width = 818;
            this.Height = 497;
            bg = new Bitmap(Image.FromFile("city.jpg"), this.Width, this.Height);
            carStart = false;
            carBreak = false;
            speed = 0;
            topSpeed = 0;
            rotationAngle = 0;
            image1X =  0;
            image2X = this.Width - 20;
            InitializeCar();
            InitializeTimer();
        }

        private void InitializeCar()
        {
            // Starting point of car or origin
            // ROOF
            int[] roofStartP = { 150, 100, 300 };               // x & y locations
            int[] roofWL = { 150, 50 };                         // roof width & length
            roofPoints = new Point[4] {
                new Point(roofStartP[0], roofStartP[2]),
                new Point(roofStartP[1], roofStartP[2] + roofWL[1]),
                new Point(roofStartP[0] + roofWL[0], roofStartP[2] + roofWL[1]),
                new Point(roofStartP[1] + roofWL[0], roofStartP[2])
            };
            // BODY
            int bodyX = 70;
            int bodyY = roofPoints[1].Y;
            carBody = new Rectangle(bodyX, bodyY, 270, 50);

            // RUBBER WHEELS
            int[] wheelStartP = { 100, 270, 375 };
            int[] wheelDist = { 10, 5 };
            rWheels = new Point[4] {
                new Point(wheelStartP[0], wheelStartP[2] - wheelDist[1]),       // Back wheels (left)
                new Point(wheelStartP[1], wheelStartP[2] - wheelDist[1]),       // Back wheels (right)
                new Point(wheelStartP[0] - wheelDist[0], wheelStartP[2]),       // Front wheels (left)
                new Point(wheelStartP[1] - wheelDist[0], wheelStartP[2])        // Front wheels (right)
            };

            // METAL WHEELS (only on front wheels)
            mWheels = new Point[] {
                new Point(100, rWheels[2].Y + 10),
                new Point(270, rWheels[3].Y + 10)
            };
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 50; // Update every 50 milliseconds
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Start button is enabled if speed is 0
            if (!carStart && speed == 0)
            {
                carBreak = false;
                btnStart.Enabled = true;
            }
            // Else start button is disabled
            else
            {
                btnStart.Enabled = false;
            }

            // Speed increases to reaches to top speed (for speeding up)
            if (speed < topSpeed && carStart)
            {
                if (carBreak)
                {
                    int s = speed + 2;
                    if (s > 0 || s == 0)
                    {
                        speed = 0;
                        carStart = false;
                    }
                    else
                    {
                        speed += 2;
                    }
                }
                else
                {
                    speed++;
                    if (speed == 0)
                    {
                        carStart = false;
                    }
                }
            }
            // Speed decreases to reach top speed (for slowing down)
            else if (speed > topSpeed && carStart)
            {
                if (carBreak)
                {
                    int s = speed - 2;
                    if (s < 0 || s == 0)
                    {
                        speed = 0;
                        carStart = false;
                    }
                    else
                    {
                        speed -= 2;
                    }
                }
                else
                {
                    speed--;
                    if (speed == 0)
                    {
                        carStart = false;
                    }
                }
            }
            
            // Background moves forward if speed is positive
            if (speed > 0)
            {
                MoveBGForward(speed);
            }
            // Else background moves backwards
            else
            {
                MoveBGBackward(speed);
            }

            displaySpeed();
            rotationAngle += speed;
            this.DoubleBuffered = true;
            this.Invalidate();

        }

        private void Timer_Start(object sender, EventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(bg, image1X, 0);
            g.DrawImage(bg, image2X, 0);
            DrawCar(g);
            if (rotationAngle == 0)
            {
                DrawRims(g);
            }
            else
            {
                RotateLeftRims(g);
                RotateRightRims(g);
            }
        }

        private void DrawCar(Graphics g)
        {
            Brush brush;
            // Draw car roof (polygon)
            g.FillPolygon(brush = new SolidBrush(Color.Blue), roofPoints);

            // Draw back wheels (ellipses)
            for (int i = 0; i < rWheels.Length; i++)
            {
                g.FillEllipse(brush = new SolidBrush(Color.Black), rWheels[i].X, rWheels[i].Y, 55, 55);
            }

            // Draw car body (rectangle)
            g.FillRectangle(brush = new SolidBrush(Color.Red), carBody);

            // Draw front rubber wheels (ellipses)
            for (int i = rWheels.Length - 1; i > 1; i--)
            {
                g.FillEllipse(brush = new SolidBrush(Color.FromArgb(48, 39, 39)), rWheels[i].X, rWheels[i].Y, 55, 55);
            }

            // Draw front metal wheels (ellipses)
            for (int i = 0; i < mWheels.Length; i++)
            {
                g.FillEllipse(brush = new SolidBrush(Color.DarkGray), mWheels[i].X, mWheels[i].Y, 35, 35);
            }

        }

        private void DrawRims(Graphics g)
        {
            int[] rimsLX = { 114, 100 };
            int[] rimsLY = { 385, 399 };

            rimsL = new Rectangle[]
            {
                new Rectangle(rimsLX[0], rimsLY[0], 8, 35),
                new Rectangle(rimsLX[1], rimsLY[1], 35, 8)
            };

            int[] rimsRX = { 284, 270 };
            int[] rimsRY = { 385, 399 };

            rimsR = new Rectangle[]
            {
                new Rectangle(rimsRX[0], rimsRY[0], 8, 35),
                new Rectangle(rimsRX[1], rimsRY[1], 35, 8)
            };

            Brush brush = new SolidBrush(Color.Black);

            g.FillRectangle(brush, rimsL[0]);
            g.FillRectangle(brush, rimsL[1]);
            g.FillRectangle(brush, rimsR[0]);
            g.FillRectangle(brush, rimsR[1]);
        }

        private void RotateLeftRims(Graphics g)
        {
            foreach (Rectangle rim in rimsL)
            {
                // Save the current state of the graphics object
                Matrix originalTransform = g.Transform;

                // Translate to the center of the current rim
                int rimCenterX = rim.X + rim.Width / 2;
                int rimCenterY = rim.Y + rim.Height / 2;
                g.TranslateTransform(rimCenterX, rimCenterY);

                // Apply rotation transformation
                g.RotateTransform(rotationAngle);

                // Draw rotated rim
                Brush brush = new SolidBrush(Color.Black);
                g.FillRectangle(brush, -rim.Width / 2, -rim.Height / 2, rim.Width, rim.Height);

                // Restore the graphics object to its original state by applying reverse transformations
                g.Transform = originalTransform;
            }
        }

        private void RotateRightRims(Graphics g)
        {
            foreach (Rectangle rim in rimsR)
            {
                // Save the current state of the graphics object
                Matrix originalTransform = g.Transform;

                // Translate to the center of the current rim
                int rimCenterX = rim.X + rim.Width / 2;
                int rimCenterY = rim.Y + rim.Height / 2;
                g.TranslateTransform(rimCenterX, rimCenterY);

                // Apply rotation transformation
                g.RotateTransform(rotationAngle);

                // Draw rotated rim
                Brush brush = new SolidBrush(Color.Black);
                g.FillRectangle(brush, -rim.Width / 2, -rim.Height / 2, rim.Width, rim.Height);

                // Restore the graphics object to its original state by applying reverse transformations
                g.Transform = originalTransform;
            }
        }

        private void MoveBGForward(int pixels)
        {
            image1X -= speed;
            image2X -= speed;

            int width = this.Width;

            if (image1X <= -width)
            {
                image1X = image2X - 25 + width;
            }

            if (image2X <= -width)
            {
                image2X = image1X - 25 + width;
            }
        }

        private void MoveBGBackward(int pixels)
        {
            image1X -= speed;
            image2X -= speed;

            int width = this.Width;

            if (image1X >= width)
            {
                image1X = image2X + 1 - width;
            }

            if (image2X >= width)
            {
                image2X = image1X + 1 - width;
            }
        }

        private void displaySpeed()
        {
            if (speed > 0)
            {
                lblSpeed.Text = speed.ToString();
                lblStatus.Text = "KPH     (Forward)";
            }
            else if (speed < 0)
            {
                lblSpeed.Text = Math.Abs(speed).ToString();
                lblStatus.Text = "KPH     (Backward)";
            }
            else
            {
                lblSpeed.Text = speed.ToString();
                lblStatus.Text = "KPH     (Stop)";
            }
            
        }

        private void Start(object sender, EventArgs e)
        {
            if (!carStart)
            {
                topSpeed = 20;
                carStart = true;
                Timer_Start(sender, e);
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            MessageBox.Show("Thank you for using my output. Tune in for the other cars!");
            timer.Stop();
            Application.Exit();
        }

        private void Accelerate(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                if (topSpeed < 150)
                {
                    if (!carStart)
                    {
                        carStart = true;
                    }
                    topSpeed++;
                }
            }   
            
            else if (e.KeyCode == Keys.A)
            {
                if (topSpeed > -150)
                {
                    if (!carStart)
                    {
                        carStart = true;
                    }
                    topSpeed--;
                }
            }

            else if (e.KeyCode == Keys.S)
            {
                topSpeed = 0;
                carBreak = true;
            }

        }
    }
}
