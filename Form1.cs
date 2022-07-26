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
    public enum Mode
    {
        AddSource,
        AddDestination,
        AddBorder,
        RemoveBorder,
        Search,
        Reset,
        None
    }

    public enum Type
    {
        Source,
        Destination,
        Border,
        Search,
        Path,
        None
    }

    class CActor
    {
        public int X, Y;
        public int Wd, Ht;
        public Type type = Type.None;    // 1 --> Empty & 2--> yellow && 3 --> Sky
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
        const int N = 30;
        

        Timer tt = new Timer();
        Bitmap off;

        Tuple startNode = new Tuple();
        Tuple endNode = new Tuple();

        List<String> path = new List<String>();
        List<CActor> buttons = new List<CActor>();
        List<Tuple> queue = new List<Tuple>();
        
        int[][] dirs = { new[] { 0, -1 }, new[] { 0, 1 }, new[] { -1, 0 }, new[] { 1, 0 } };
        CActor[,] Matrix = new CActor[N, N];

        String strLastNode = "";
        
        Dictionary<String, int> visited = new Dictionary<String, int>();
        Dictionary<String, String> parents = new Dictionary<String, String>();

        Mode mode = Mode.None;

        bool stop = false;
        bool click = false;
        bool found = false;
        
        int startX, startY, endX, endY;
        int W;
        int H;
        int XB;
        int YB;

        int backtrackCt = 0;
        int ctmsg = 0;
        int searchCt = 0;

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

        void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            startX = startY = endX = endY = -1;
            resize();
            CreateActs();
        }

        void tt_Tick(object sender, EventArgs e)
        {
            if (mode == Mode.Search)
            {
                searchCt++;
                if (queue.Count != 0 && !stop)
                {
                    Tuple currNode = queue[0];
                    queue.RemoveAt(0);
                    strLastNode = parseTuple(currNode);

                    if (!visited.ContainsKey(currNode.x + "-" + currNode.y))
                    {

                        visited.Add(currNode.x + "-" + currNode.y, 1);

                        if (currNode.x + "-" + currNode.y != startNode.x + "-" + startNode.y)
                        {
                            int r = currNode.x;
                            int c = currNode.y;

                            drawSpecific(r, c, Color.DarkGray, this.CreateGraphics());
                            Matrix[currNode.x, currNode.y].type = Type.Search;
                        }

                        foreach (var dir in dirs)
                        {
                            var x = currNode.x + dir[0];
                            var y = currNode.y + dir[1];
                            if (x < 0 || x >= N || y < 0 || y >= N || Matrix[x, y].type == Type.Border) continue;

                            Tuple newNode = new Tuple(x, y);
                            if (parseTuple(newNode) == parseTuple(endNode))
                            {
                                this.Text = "Found Path";
                                stop = true;
                                found = true;
                                backtrack();
                                mode = Mode.None;
                            }

                            if (!visited.ContainsKey(newNode.x + "-" + newNode.y))
                            {
                                parents[parseTuple(newNode)] = parseTuple(currNode);
                                queue.Add(newNode);
                            }
                        }
                    }
                    
                }
                else
                {
                    mode = Mode.None;
                }
            }
            else
            {
                if (mode == Mode.Reset)
                {
                    reset();
                }

                DrawDubb(this.CreateGraphics());

                if(stop)
                {

                    if (backtrackCt < path.Count)
                    {
                        Tuple pnn = unparseTuple(path[backtrackCt]);
                        Matrix[pnn.x, pnn.y].type = Type.Path;
                        drawSpecific(pnn.x, pnn.y, Color.Yellow, this.CreateGraphics());
                        backtrackCt++;
                    }
                    else
                    {
                        stop = false;
                    }       
                }
            }

            if (isNoPath())
            {
                if (ctmsg++ == 0)
                {
                    MessageBox.Show("No path from source to destination!!");
                }
            }
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            click = true;
            for (int i = 0; i < buttons.Count; i++)
            {
                if (e.X >= buttons[i].X && e.X <= buttons[i].X + buttons[i].Wd && e.Y >= buttons[i].Y && e.Y <= buttons[i].Y + buttons[i].Ht)
                {
                    if (i == 4 && (startX == -1 || endX == -1))
                    {
                        MessageBox.Show("Make Sure to Add Source & Destination");
                        click = false;
                        break;
                    }

                    switch (i)
                    {
                        case 0:
                            mode = Mode.AddSource; break;
                        case 1:
                            mode = Mode.AddDestination; break;
                        case 2:
                            mode = Mode.AddBorder; break;
                        case 3:
                            mode = Mode.RemoveBorder; break;
                        case 4:
                            mode = Mode.Search; break;
                        case 5:
                            mode = Mode.Reset; break;
                        default:
                            mode = Mode.None; break;
                    }
                    break;
                }
            }

            if (e.X >= XB && e.Y >= YB)
            {
                int iR = (e.Y - YB) / H;
                int iC = (e.X - XB) / W;

                if (iR < N && iC < N)
                {
                    if (mode == Mode.AddSource)
                    {
                        addSource(iR, iC);
                    }
                    else if (mode == Mode.AddDestination)
                    {
                        addDestination(iR, iC);
                    }
                    else if (mode == Mode.AddBorder)
                    {
                        Matrix[iR, iC].type = Type.Border;
                    }
                    else if (mode == Mode.RemoveBorder)
                    {
                        Matrix[iR, iC].type = Type.None;
                    }

                    if (mode != Mode.AddSource && iR == startX && iC == startY)
                    {
                        startX = startY = -1;
                        startNode.x = -1;
                        startNode.y = -1;
                    }
                    if (mode != Mode.AddDestination && iR == endX && iC == endY)
                    {
                        endX = endY = -1;
                        endNode.x = -1;
                        endNode.y = -1;
                    }
                }
            }

            DrawDubb(this.CreateGraphics());
        }

        void addSource(int iR, int iC)
        {
            if (startX != -1)
            {
                Matrix[startX, startY].type = Type.None;
            }

            Matrix[iR, iC].type = Type.Source;
            startX = iR;
            startY = iC;

            startNode.x = startX;
            startNode.y = startY;
            queue.Add(startNode);
        }

        void addDestination(int iR, int iC)
        {
            if (endX != -1)
            {
                Matrix[endX, endY].type = Type.None;
            }
            Matrix[iR, iC].type = Type.Destination;

            endX = iR;
            endY = iC;
            endNode.x = endX;
            endNode.y = endY;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (click)
            {
                if (mode == Mode.AddBorder)
                {
                    int iR = (e.Y - YB) / H;
                    int iC = (e.X - XB) / W;

                    if (iR < N && iC < N && iC > -1 && iR > -1)
                    {
                        Matrix[iR, iC].type = Type.Border;
                    }
                }
            }
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

        bool isNoPath()
        {
            return queue.Count == 0 && found == false && mode == Mode.Search && searchCt > 0;
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

        void reset()
        {
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    Matrix[r, c].type = Type.None;
                }
            }

            searchCt = 0;
            path = new List<string>();
            startX = startY = endX = endY = -1;
            startNode = new Tuple();
            endNode = new Tuple();
            strLastNode = "";
            queue = new List<Tuple>();
            visited = new Dictionary<string, int>();
            parents = new Dictionary<string, string>();
            mode = Mode.None;
            backtrackCt = 0;
            stop = false;
            found = false;
            click = false;

            DrawDubb(this.CreateGraphics());
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


        
        void resize()
        {
            W = (int)((this.ClientSize.Width * 0.80) / N);
            H = (int)((this.ClientSize.Height * 0.80) / N);

            YB = (int)(((this.ClientSize.Height) - (H * N)) / 2);
            XB = (int)(((this.ClientSize.Width) - (W * N)) / 2);
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
                    if (Matrix[r, c].type == Type.None)
                        g.FillRectangle(Brushes.Transparent, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == Type.Source)
                        g.FillRectangle(Brushes.Red, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == Type.Destination)
                        g.FillRectangle(Brushes.Green, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == Type.Border)
                        g.FillRectangle(Brushes.White, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == Type.Search)
                        g.FillRectangle(Brushes.DarkGray, Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
                    if (Matrix[r, c].type == Type.Path)
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

        void drawSpecific(int r, int c, Color col, Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            g2.DrawRectangle(new Pen(Color.Gray, 3), Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
            g2.FillRectangle(new SolidBrush(col), Matrix[r, c].X, Matrix[r, c].Y, Matrix[r, c].Wd, Matrix[r, c].Ht);
            g.DrawImage(off, 0, 0);
        }

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            click = false;
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }


    }
}

