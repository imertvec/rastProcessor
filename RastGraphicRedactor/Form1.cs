using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RastGraphicRedactor
{
    public partial class Form1 : Form
    {
        string mode = "Карандаш";
        PictureBox picObject = new PictureBox();

        //Graphics
        Bitmap bit;
        Bitmap temp;
        Bitmap pattern;
        Graphics bitG;
        Graphics tempG;

        //Pen&brushes
        Pen pen = new Pen(Color.Black);
        Brush brush = Brushes.Black;
        TextureBrush tbrush;
        LinearGradientBrush lbrush;

        //Colors
        Color color = Color.Black;
        Color colorback = Color.White;
        
        //Font for text
        Font font;
        
        //Staly for cube&ellipse
        HatchStyle hs;
        HatchBrush hb;

        //Bool for check events
        bool clear = false;
        bool status = false;

        //Rectangle for cube&triangle
        Rectangle rectangle;
        Rectangle fillrectangle;
        Rectangle cube;

        //Points for cube&triangle
        Point[] tri;
        Point[] fillrec;
        Point[] rec;
        Point[] triPrime;
        Point[] serpPoints;

        //Difficult varriables for position 
        int size = 1;
        int posx = 0;
        int posy = 0;
        int x = 0;
        int y = 0;

        //Serpinsky Triangle
        bool triangleChk = true;
        Random rnd = new Random();
        int rX = 0;
        int rY = 0;

        public Form1()
        {
            InitializeComponent();
            DrawingButtonsNPicture();
            StartSettings();
        }

        void DrawingButtonsNPicture()
        {
            //Buttons
            string[] rbName = { "Карандаш", "Линия", "Рамка", "Окружность", "Треугольник", "Равносторонний треугольник", "Прямоугольник", "Круг", "Ластик", "Текст", "Треугольники Серпинского"};
            int countMode = rbName.Length;
            RadioButton[] rbMas = new RadioButton[countMode];

            for (int i = 0; i < rbMas.Length; i++)
            {
                rbMas[i] = new RadioButton();
                rbMas[i].Text = rbName[i];
                rbMas[i].Appearance = Appearance.Button;
                rbMas[i].Checked = false;

                if(rbName[i].Contains(" "))
                {
                    rbMas[i].Size = new Size(104, 40);
                }

                rbMas[i].Left = 20;
                rbMas[i].Top = 20 + i * rbMas[i].Height;


                rbMas[i].BackColor = Color.Linen;
                rbMas[i].AutoCheck = true;
                rbMas[i].Click += ButtonClick;
                this.flowLayoutPanel1.Controls.AddRange(rbMas);
                rbMas[0].Checked = true;
            }

            //Picture
            picObject.Width = 100;
            picObject.Height = 100;
            picObject.Left = 500;
            picObject.BackColor = Color.Transparent;
            picObject.SizeMode = PictureBoxSizeMode.StretchImage;
            picObject.BorderStyle = BorderStyle.Fixed3D;
            picObject.BackColor = Color.AliceBlue;
            picObject.MouseDown += pic_mouseDown;
            this.flowLayoutPanel1.Controls.Add(picObject);
        } //Ready

        void StartSettings()
        {
            bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bitG = Graphics.FromImage(bit);
            
            this.toolStripStatusLabel1.Text = mode;
            this.toolStripStatusLabel2.Text = "(0:0)";
            this.toolStripStatusLabel3.Text = DateTime.Now.ToLongDateString();
            this.toolStripStatusLabel4.Text = DateTime.Now.ToLongTimeString();
            toolStripButton1.Text = "Сохранить";
            toolStripButton2.Text = "Открыть";
            toolStripButton3.Text = "Очистить";
            toolStripButton4.Text = "Цвет1";
            toolStripButton5.Text = "Цвет2";
            textButton.Enabled = false;
            
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox2.SelectedIndex = 0;

            foreach(var i in Enum.GetNames(typeof(HatchStyle)))
            {
                toolStripComboBox3.Items.Add(i);
            }

            toolStripComboBox3.SelectedIndex = 0;

            pictureBox1.MouseMove += PicMouseMove;
            pictureBox1.MouseDown += PicMouseDown;
            pictureBox1.MouseUp += PicMouseUp;
            //this.timer1.Enabled = true; ADD CHANGE
        } 
        
        void ButtonClick(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            mode = rb.Text;
            this.toolStripStatusLabel1.Text = mode;

            //Settings for Text
            if (mode.Equals("Текст"))
            {
                textButton.Enabled = true;
                if (font != null)
                    text.Enabled = true;
                else
                    text.Enabled = false;
            }
            else
            {
                textButton.Enabled = false;
                text.Enabled = false;
            }

            //Settings for Change style
            if (toolStripComboBox2.SelectedIndex == 1 && mode.Equals("Прямоугольник") ||
                toolStripComboBox2.SelectedIndex == 1 && mode.Equals("Круг"))
            {
                toolStripComboBox3.Enabled = true;
            }
            else
            {
                toolStripComboBox3.Enabled = false;
                toolStripComboBox2.SelectedIndex = 0;
            }
        } //Ready

        void PicMouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                status = true;
            }

            posx = e.X;
            posy = e.Y;
        }

        void PicMouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                status = false;
                bitG.DrawImage(temp, 0, 0);
                pictureBox1.Image = bit;
                
                if(mode.Equals("Треугольники Серпинского"))
                {
                    triangleChk = false;
                    timer1.Enabled = true;
                }
                else
                {
                    triangleChk = true;
                    timer1.Enabled = false;
                }
            }
        }

        void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            size = Convert.ToInt32(toolStripComboBox1.SelectedItem);
        } //Size for pen

        void textButton_Click(object sender, EventArgs e) 
        {
            if(fontDialog1.ShowDialog() == DialogResult.OK)
            {
                font = fontDialog1.Font;
                text.Enabled = true;
            }
        } //Text

        void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bit.Save(saveFileDialog1.FileName);
            }
        } //Save

        void toolStripButton2_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bit = new Bitmap(openFileDialog1.FileName);

                if(bit.Height > this.Height || bit.Width > this.Width)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                }
                
                pictureBox1.Image = bit;
            }
            
            pictureBox1.Invalidate();
        } //Open //Not Ready

        void toolStripButton7_Click(object sender, EventArgs e)
        {
            bitG.Dispose();
            tempG.Dispose();
            Environment.Exit(0);
        } //Exit

        void toolStripButton3_Click(object sender, EventArgs e)
        {
            bitG.Clear(Color.White);
            tempG.Clear(Color.White);
            pictureBox1.Image = bit;
            clear = true;

        } //Clear

        void toolStripButton4_Click(object sender, EventArgs e) 
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                color = colorDialog1.Color;
                toolStripButton4.BackColor = color;
            }
        }//color1

        void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colorback = colorDialog1.Color;
                toolStripButton5.BackColor = colorback;
            }
        } //color2

        void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox2.SelectedIndex == 1 && mode.Equals("Прямоугольник") ||
                toolStripComboBox2.SelectedIndex == 1 && mode.Equals("Круг"))
            {
                toolStripComboBox3.Enabled = true;
            }
            //else 
            {
                //toolStripComboBox3.Enabled = false;
                //toolStripComboBox2.SelectedIndex = 0;
            }

            if (toolStripComboBox2.SelectedIndex == 2 && mode.Equals("Прямоугольник") ||
                        toolStripComboBox2.SelectedIndex == 2 && mode.Equals("Круг"))
            {
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    pattern = new Bitmap(openFileDialog2.FileName);
                }
            }

        } //Change style

        void PicMouseMove(object sender, MouseEventArgs e)
        {
            this.toolStripStatusLabel2.Text = "(" + e.X + ":" + e.Y + ")";

            //Graphics&Bitmap
            temp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            tempG = Graphics.FromImage(temp);

            //Brushes&pen
            hs = (HatchStyle)Enum.Parse(typeof(HatchStyle), toolStripComboBox3.SelectedItem.ToString(), true);
            hb = new HatchBrush(hs, color, colorback);
            brush = new SolidBrush(color);
            pen = new Pen(color, size);
            pen.EndCap = LineCap.Round;
            pen.StartCap = LineCap.Round;

            //Settings for rectangle
            if (pattern != null)
            {
                cube = new Rectangle(posx, posy, e.X, e.Y);
                tbrush = new TextureBrush(pattern, WrapMode.Tile);
            }

            //Functions
            if (status)
            {
                switch (mode)
                {
                    case "Карандаш":
                        bitG.DrawLine(pen, posx, posy, e.X, e.Y);
                        posx = e.X;
                        posy = e.Y;
                        break;
                    case "Линия":
                        tempG.DrawLine(pen, posx, posy, e.X, e.Y);
                        break;
                    case "Рамка":
                        rec = new Point[] { new Point(posx, posy), new Point(e.X, posy), new Point(e.X, e.Y), new Point(posx, e.Y) };
                        tempG.DrawPolygon(pen, rec);
                        break;
                    case "Окружность":
                        rectangle = new Rectangle(posx, posy, Math.Abs(e.X - posx), Math.Abs(e.Y - posy));
                        tempG.DrawEllipse(pen, rectangle);
                        break;
                    case "Треугольник":
                        tri = new Point[] { new Point(x, y), new Point(posx, y), new Point(posx + (x - posx) / 2, posy) };
                        tempG.DrawPolygon(pen, triPrime);
                        break;
                    case "Равносторонний треугольник":
                        int h = e.Y - posy;
                        int bx = (posx + e.X) / 2;
                        int by = posy;
                        triPrime = new Point[] { new Point((int)(bx - (h * Math.Sqrt(3))), e.Y), new Point((int)(bx + (h * Math.Sqrt(3))), e.Y), new Point(bx, by)};
                        tempG.DrawPolygon(pen, triPrime);
                        break;
                    case "Прямоугольник":
                        fillrec = new Point[] { new Point(posx, posy), new Point(x, posy), new Point(x, y), new Point(posx, y) };
                        if (toolStripComboBox2.SelectedIndex == 1)
                            tempG.FillPolygon(hb, fillrec);
                        else if (toolStripComboBox2.SelectedIndex == 0)
                            tempG.FillPolygon(brush, fillrec);
                        else if (toolStripComboBox2.SelectedIndex == 2)
                            tempG.FillPolygon(tbrush, fillrec);
                        else if (toolStripComboBox2.SelectedIndex == 3)
                        {
                            lbrush = new LinearGradientBrush(new Point(posx, posy), new Point(e.X, e.Y), color, colorback);
                            tempG.FillPolygon(lbrush, fillrec);
                        }
                            
                        break;
                    case "Круг":
                        fillrectangle = new Rectangle(posx, posy, Math.Abs(x - posx), Math.Abs(y - posy));
                        if (toolStripComboBox2.SelectedIndex == 1)
                            tempG.FillEllipse(hb, fillrectangle);
                        else if (toolStripComboBox2.SelectedIndex == 0)
                            tempG.FillEllipse(brush, fillrectangle);
                        else if(toolStripComboBox2.SelectedIndex == 2)
                            tempG.FillEllipse(tbrush, fillrectangle);
                        else if (toolStripComboBox2.SelectedIndex == 3)
                        {
                            lbrush = new LinearGradientBrush(new Point(posx, posy), new Point(e.X, e.Y), color, colorback);
                            tempG.FillEllipse(lbrush, fillrectangle);
                        }
                            
                        break;
                    case "Ластик":
                        pen.Color = Color.White;
                        bitG.DrawLine(pen, posx, posy, e.X, e.Y);
                        posx = e.X;
                        posy = e.Y;
                        break;
                    case "Треугольники Серпинского":
                        if(triangleChk)
                        {
                            serpPoints = new Point[] { new Point(x, y), new Point(posx, y), new Point(posx + (x - posx) / 2, posy) };
                            rX = rnd.Next(1, pictureBox1.Height - 1);
                            rY = rnd.Next(1, pictureBox1.Width - 1);
                            tempG.FillEllipse(brush, serpPoints[0].X, serpPoints[0].Y, size, size);
                            tempG.FillEllipse(brush, serpPoints[1].X, serpPoints[1].Y, size, size);
                            tempG.FillEllipse(brush, serpPoints[2].X, serpPoints[2].Y, size, size);
                            tempG.FillEllipse(brush, rX, rY, size, size);
                        }
                        
                        break;
                }

                if (!triangleChk) return;

                if(mode.Equals("Текст"))
                {
                    tempG.DrawString(text.Text, font, brush, e.X, e.Y);
                }

                x = e.X;
                y = e.Y;
                pictureBox1.Invalidate();
            }
        }

        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            switch (mode)
            {
                case "Линия":
                    e.Graphics.DrawLine(pen, posx, posy, x, y);
                    break;
                case "Рамка":
                    rec = new Point[] { new Point(posx, posy), new Point(x, posy), new Point(x, y), new Point(posx, y) };
                    e.Graphics.DrawPolygon(pen, rec);
                    break;
                case "Окружность":
                    e.Graphics.DrawEllipse(pen, rectangle);
                    break;
                case "Треугольник":
                    e.Graphics.DrawPolygon(pen, tri);
                    break;
                case "Равносторонний треугольник":
                    e.Graphics.DrawPolygon(pen, triPrime);
                    break;
                case "Прямоугольник":
                    if (toolStripComboBox2.SelectedIndex == 1)
                        e.Graphics.FillPolygon(hb, fillrec);
                    else if(toolStripComboBox2.SelectedIndex == 0)
                        e.Graphics.FillPolygon(brush, fillrec);
                    else if (toolStripComboBox2.SelectedIndex == 2)
                        e.Graphics.FillPolygon(tbrush, fillrec);
                    else if (toolStripComboBox2.SelectedIndex == 2)
                        e.Graphics.FillPolygon(tbrush, fillrec);
                    else if (toolStripComboBox2.SelectedIndex == 3)
                        e.Graphics.FillPolygon(lbrush, fillrec);
                    break;
                case "Круг":
                    fillrectangle = new Rectangle(posx, posy, Math.Abs(x - posx), Math.Abs(y - posy));
                    if (toolStripComboBox2.SelectedIndex == 1)
                        e.Graphics.FillEllipse(hb, fillrectangle);
                    else if (toolStripComboBox2.SelectedIndex == 0)
                        e.Graphics.FillEllipse(brush, fillrectangle);
                    else if (toolStripComboBox2.SelectedIndex == 2)
                        e.Graphics.FillEllipse(tbrush, fillrectangle);
                    else if (toolStripComboBox2.SelectedIndex == 3)
                        e.Graphics.FillEllipse(lbrush, fillrectangle);
                    break;
                case "Треугольники Серпинского":
                    if(triangleChk)
                    {
                        bitG.Clear(colorback);
                        e.Graphics.FillEllipse(brush, serpPoints[0].X, serpPoints[0].Y, size, size);
                        e.Graphics.FillEllipse(brush, serpPoints[1].X, serpPoints[1].Y, size, size);
                        e.Graphics.FillEllipse(brush, serpPoints[2].X, serpPoints[2].Y, size, size);
                        e.Graphics.FillEllipse(brush, rX, rY, size, size);
                    }
                    
                    break;
            }

            if (!triangleChk)
            {
                bitG.FillEllipse(brush, rX, rY, size, size);
            }

            if (mode.Equals("Текст"))
            {
                e.Graphics.DrawString(text.Text, font, brush, x, y);
            }

            if (clear)
            {
                e.Graphics.Clear(Color.White);
                clear = false;
            }
        }

        #region DragAndDrop
        void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(typeof(Bitmap)))
            {
                temp = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
                bitG.DrawImage(temp, e.X - temp.Height * 0.5f, e.Y - temp.Width * 0.5f, 
                    temp.Width * 0.5f, temp.Height * 0.5f);
                pictureBox1.Invalidate();
            }
        }

        void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                e.Effect = DragDropEffects.Copy;
            }
        } //Copy effect
        
        void pic_mouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (picObject.Image != null)
                {
                    picObject.DoDragDrop(picObject.Image, DragDropEffects.All);
                }
            }

            if(e.Button == MouseButtons.Right)
            {
                if (openFileDialog3.ShowDialog() == DialogResult.OK)
                {
                    picObject.SizeMode = PictureBoxSizeMode.Zoom;
                    picObject.Image = Image.FromFile(openFileDialog3.FileName);
                }
            }
            
        } //Call event of drugAndDrop && change image

        void Form1_Load(object sender, EventArgs e)
        {
            this.pictureBox1.AllowDrop = true;
        } //Allow drop objects on picbox

        #endregion 

        void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                color = bit.GetPixel(e.X, e.Y);
                toolStripButton4.BackColor = color;
            }
               

            if (e.Button == MouseButtons.Right)
            {
                colorback = bit.GetPixel(e.X, e.Y);
                toolStripButton5.BackColor = colorback;
            }
        } //Pipette
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (rnd.Next(0, 3))
            {
                case 0:
                    rX = (int)(rX + serpPoints[0].X) / 2;
                    rY = (int)(rY + serpPoints[0].Y) / 2;
                    break;
                case 1:
                    rX = (int)(rX + serpPoints[1].X) / 2;
                    rY = (int)(rY + serpPoints[1].Y) / 2;
                    break;
                case 2:
                    rX = (int)(rX + serpPoints[2].X) / 2;
                    rY = (int)(rY + serpPoints[2].Y) / 2;
                    break;
            }

            pictureBox1.Invalidate();
        }
    }
}