using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shortest_Path_BFS_Visualized
{
    class CActor
    {
        public int X, Y;
        public int Wd, Ht;
        public int type = -1;    // 1 --> Empty & 2--> yellow && 3 --> Sky
    }
    
    class Tuple
    {
        public int x = -1, y = -1;
        public Tuple()
        {

        }

        public Tuple(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public partial class Form1 : Form
    {
        Timer t = new Timer();
        Bitmap off;

        Tuple startNode = new Tuple();
        Tuple endNode = new Tuple();

        String strLastNode = "";
        List<String> path = new List<String>();

        List<CActor> buttons = new List<CActor>();
        Dictionary<String, int> visited = new Dictionary<String, int>();
        Dictionary<String, String> parents = new Dictionary<String, String>();
        List<Tuple> queue = new List<Tuple>();
        
        bool stop = false;
        int mode = -1;
        int startX, startY, endX, endY;
        //List<CActor> LActs = new List<CActor>();
        const int N = 30;
        CActor[,] Matrix = new CActor[N, N];
        int W;
        int H;
        bool click = false;
        int XB;
        int YB;
        int backtrackCt = 0;
        bool found = false;
        int ctmsg = 0;
        bool res = false;
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Load += new EventHandler(Form1_Load);
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            MouseUp += Form1_MouseUp;
            MouseMove += Form1_MouseMove;
            tt.Tick += new EventHandler(tt_Tick);
            tt.Interval = 1;
            tt.Start();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (click)
            {
                if (mode == 2)
                {
                    int iR = (e.Y - YB) / H;
                    int iC = (e.X - XB) / W;

                    if (iR < N && iC < N && iC > -1 && iR > -1)
                    {
                        Matrix[iR, iC].type = 2;
                    }
                }
            }
        }

        void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            startX = startY = endX = endY = -1;
            resize();
            CreateActs();
        }

        void resize()
        {
            W = (int)((this.ClientSize.Width * 0.80) / N);
            H = (int)((this.ClientSize.Height * 0.80) / N);

            YB = (int)(((this.ClientSize.Height) - (H * N)) / 2);
            XB = (int)(((this.ClientSize.Width) - (W * N)) / 2);
            //MessageBox.Show(this.ClientSize.Width)
        }

        Timer tt = new Timer();

        void tt_Tick(object sender, EventArgs e)
        {
            if (mode == 4)
            {
                if (queue.Count != 0)
                {
                    Tuple currNode = queue[0];
                    queue.RemoveAt(0);
                    if (!stop)
                    {
                        strLastNode = parseTuple(currNode);
                        if (!visited.ContainsKey(currNode.x + "-" + currNode.y))
                        {

                            visited.Add(currNode.x + "-" + currNode.y, 1);

                            if (currNode.x + "-" + currNode.y != startNode.x + "-" + startNode.y)
                            {
                                int r = currNode.x;
                                int c = currNode.y;
                                this.CreateGraphics().DrawRectangle(new Pen(Color.Gray, 3), Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                                this.CreateGraphics().FillRectangle(Brushes.DarkGray, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                                Matrix[currNode.x, currNode.y].type = 3;
                            }


                            if (currNode.x + 1 >= 0 && currNode.x + 1 < N && currNode.y >= 0 && currNode.y < N && Matrix[currNode.x + 1, currNode.y].type != 2)
                            {
                                Tuple newNode = new Tuple(currNode.x + 1, currNode.y);
                                if (parseTuple(newNode) == parseTuple(endNode))
                                {
                                    this.Text = "Found Path";
                                    stop = true;
                                    found = true;
                                    backtrack();
                                    mode = -1;
                                }

                                if (!visited.ContainsKey(newNode.x + "-" + newNode.y))
                                {
                                    parents[parseTuple(newNode)] = parseTuple(currNode);
                                    queue.Add(newNode);
                                }
                            }
                            if (currNode.x - 1 >= 0 && currNode.x - 1 < N && currNode.y >= 0 && currNode.y < N && Matrix[currNode.x - 1, currNode.y].type != 2)
                            {
                                Tuple newNode = new Tuple(currNode.x - 1, currNode.y);
                              
                                if (parseTuple(newNode) == parseTuple(endNode))
                                {
                                    this.Text = "Found Path";
                                    stop = true;
                                    found = true;
                                    backtrack();
                                    mode = -1;
                                }

                                if (!visited.ContainsKey(newNode.x + "-" + newNode.y))
                                {
                                    parents[parseTuple(newNode)] = parseTuple(currNode);
                                    queue.Add(newNode);
                                }

                            }
                            if (currNode.x >= 0 && currNode.x < N && currNode.y + 1 >= 0 && currNode.y + 1 < N && Matrix[currNode.x, currNode.y + 1].type != 2)
                            {
                                Tuple newNode = new Tuple(currNode.x, currNode.y + 1);
                                if (parseTuple(newNode) == parseTuple(endNode))
                                {
                                    this.Text = "Found Path";
                                    stop = true;
                                    found = true;
                                    backtrack();
                                    mode = -1;
                                }

                                if (!visited.ContainsKey(newNode.x + "-" + newNode.y))
                                {
                                    parents[parseTuple(newNode)] = parseTuple(currNode);
                                    queue.Add(newNode);
                                }
                            }
                            if (currNode.x >= 0 && currNode.x < N && currNode.y - 1 >= 0 && currNode.y - 1 < N && Matrix[currNode.x, currNode.y - 1].type != 2)
                            {
                                Tuple newNode = new Tuple(currNode.x, currNode.y - 1);
                                if (parseTuple(newNode) == parseTuple(endNode))
                                {
                                    this.Text = "Found Path";
                                    stop = true;
                                    found = true;
                                    backtrack();
                                    mode = -1;
                                }

                                if (!visited.ContainsKey(newNode.x + "-" + newNode.y))
                                {
                                    parents[parseTuple(newNode)] = parseTuple(currNode);
                                    queue.Add(newNode);
                                }
                            }

                        }
                    }
                }
            }

            if(queue.Count == 0 && found == false && mode == 4)
            {
                if (ctmsg++ == 0)
                {
                    MessageBox.Show("No path from source to destination!!");
                }
            }
            if (mode != 4)
            {

                if (mode == 5)
                {
                    reset();
                }

                DrawDubb(this.CreateGraphics());
                if(stop == true)
                {
                    if (backtrackCt < path.Count)
                    {
                        Tuple pnn = unparseTuple(path[backtrackCt]);
                        Matrix[pnn.x, pnn.y].type = 4;
                        backtrackCt++;
                    }
                    else
                    {
                        stop = false;
                    }       
                }
            }
        }

        void backtrack()
        {
            path.Add(strLastNode);
            String strStartNode = parseTuple(startNode);

            while (path[path.Count - 1] != strStartNode)
            {
                path.Add(parents[path[path.Count - 1]]);
            }

            path.RemoveAt(path.Count - 1);
            path.Reverse();
        }

        String parseTuple(Tuple t)
        {
            return t.x + "-" + t.y;
        }
        
        Tuple unparseTuple(String s)
        {
            Tuple t = new Tuple();

            t.x = int.Parse(s.Split('-')[0]);
            t.y = int.Parse(s.Split('-')[1]);
            return t;
        }

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            click = false;
        }
       
        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            click = true;
            for(int i = 0; i < buttons.Count; i++)
            {
                if(e.X >= buttons[i].X && e.X <= buttons[i].X + buttons[i].Wd && e.Y >= buttons[i].Y && e.Y <= buttons[i].Y + buttons[i].Ht)
                {
                    if(i == 4 && (startX == -1 || endX == -1))
                    {
                        click = false;
                        break;
                    }
                    mode = i;
                    break;
                }
            }

            if (e.X >= XB && e.Y >= YB)
            {
                int iR = (e.Y - YB) / H;
                int iC = (e.X - XB) / W;

                if (iR < N && iC < N)
                {
                    if (mode == 0)
                    {
                        if (startX != -1) {
                            Matrix[startX, startY].type = -1;
                        }

                        Matrix[iR, iC].type = 0;
                        startX = iR;
                        startY = iC;

                        startNode.x = startX;
                        startNode.y = startY;
                        queue.Add(startNode);
                    }
                    else if (mode == 1)
                    {
                        if (endX != -1)
                        {
                            Matrix[endX, endY].type = -1;
                        }
                        Matrix[iR, iC].type = 1;

                        endX = iR;
                        endY = iC;
                        endNode.x = endX;
                        endNode.y = endY;
                    }
                    else if (mode == 2)
                    {
                       Matrix[iR, iC].type = 2;
                    }
                    else if (mode == 3)
                    {
                        Matrix[iR, iC].type = -1;
                    }

                    if (mode != 0 && iR == startX && iC == startY)
                    {
                        startX = startY = -1;
                        startNode.x = -1;
                        startNode.y = -1;
                    }
                    if (mode != 1 && mode != 4 && mode != 5 && iR == endX && iC == endY)
                    {
                        endX = endY = -1;
                        endNode.x = -1;
                        endNode.y = -1;
                    }
                }
            }

            DrawDubb(this.CreateGraphics());
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }


        void CreateActs()
        {
            int tx = XB;
            int ty = YB;
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    CActor pnn = new CActor();
                    pnn.X = tx;
                    pnn.Y = ty;
                    pnn.Wd = W;
                    pnn.Ht = H;

                    Matrix[r, c] = pnn;

                    tx += W;
                }
                ty += H;
                tx = XB;
            }

            for(int i = 0; i < 6; i++)
            {
                CActor btn = new CActor();
                btn.Wd = (int)(XB * 0.8);
                btn.Ht = 50;
                btn.X = 5;
                btn.Y = (i + 1) * btn.Ht + (i * 10);
                buttons.Add(btn);
            }
        }

        void reset()
        {
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    Matrix[r, c].type = -1;
                }
            }

            path = new List<string>();
            startX = startY = endX = endY = -1;
            startNode = new Tuple();
            endNode = new Tuple();
            strLastNode = "";
            queue = new List<Tuple>();
            visited = new Dictionary<string, int>();
            parents = new Dictionary<string, string>();
            mode = -1;
            backtrackCt = 0;
            stop = false;
            found = false;
            click = false;

            DrawDubb(this.CreateGraphics());
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.Black);

            Pen Pn = new Pen(Color.Gray, 3);
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    g.DrawRectangle(Pn, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == -1)
                        g.FillRectangle(Brushes.Transparent, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == 0)
                        g.FillRectangle(Brushes.Red, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == 1)
                        g.FillRectangle(Brushes.Green, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == 2)
                        g.FillRectangle(Brushes.White, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == 3)
                        g.FillRectangle(Brushes.DarkGray, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == 4)
                        g.FillRectangle(Brushes.Yellow, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                }
            }

            SolidBrush b = new SolidBrush(Color.Yellow);

            for (int i = 0; i < buttons.Count; i++)
            {
                g.FillRectangle(b, buttons[i].X, buttons[i].Y + 5, buttons[i].Wd, buttons[i].Ht);
                g.DrawRectangle(new Pen(Color.DarkGray), buttons[i].X, buttons[i].Y + 5, buttons[i].Wd, buttons[i].Ht);
                Font drawFont = new Font("Arial", 12);
                String str = "";
                if(i == 0)
                {
                    str = "Add Source";
                }
                else if (i == 1)
                {
                    str = "Add Destination";
                }
                if (i == 2)
                {
                    str = "Add Border";
                }
                if (i == 3)
                {
                    str = "Remove Border";
                }
                if (i == 4)
                {
                    str = "Search!";
                }
                if (i == 5)
                {
                    str = "Reset";
                }

                g.DrawString(str, drawFont, new SolidBrush(Color.Gray), buttons[i].X + 5, buttons[i].Y + 20);
            }
        }

        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }

        private void T_Tick(object sender, EventArgs e)
        {
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

    }
}

