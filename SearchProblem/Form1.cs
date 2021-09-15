using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Threading;

namespace SearchProblem
{
    public partial class frmMain : Form
    {
        List<PictureBox> pbs = new List<PictureBox>();
        List<PictureBox> clearPB = new List<PictureBox>();
        List<PictureBox> closedList;
        List<PictureBox> openedList;
        List<Label> pathCellNum = new List<Label>();

        Random rnd;
        Queue bfsQueue;
        Stack dfsStack;
        Color backColor = Color.White, sourceColor = Color.Red, goalColor = Color.Lime, prePathColor = Color.LightPink, pathColor = Color.CornflowerBlue;

        int minTLP, maxTLP, sourceState, goalState;
        bool sourceColorSentinel, goalColorSentinel;

        int[] parentID;
        bool[] openFlag;
        bool firstRun = true;
        double[] f;
        int[] g;
        double[] h;


        PictureBox tempPB;


        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PB_To_List();
            MarginModifier();
            btnGeneratePath_Click(null, null);
        }

        

        private void PB_To_List()
        {
            Control[] control;

            for (int i = 1; i <= tlp.RowCount * tlp.ColumnCount; i++)
            {
                control = this.Controls.Find("pictureBox" + i.ToString(), true);

                pbs.Add((PictureBox)control[0]);    // pbs is an array of picture boxes
            }
        }

        private void BlackMargin()
        {
            for (int i = 0; i <= tlp.ColumnCount - 1; i++)
                pbs[i].BackColor = Color.Black;

            for (int i = pbs.Count - tlp.ColumnCount; i <= pbs.Count - 1; i++)
                pbs[i].BackColor = Color.Black;

            for (int i = 0; i <= tlp.RowCount - 1; i++)
                pbs[i * tlp.ColumnCount].BackColor = Color.Black;

            for (int i = 0; i <= tlp.RowCount - 1; i++)
                pbs[i * tlp.ColumnCount + tlp.ColumnCount - 1].BackColor = Color.Black;
        }

        private void MarginModifier()
        {
            foreach (PictureBox pb in pbs)
            {
                pb.Dock = DockStyle.Fill;
                pb.Margin = new Padding(0, 0, 0, 0);
                pb.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void DetermineSourceGoalState()
        {
            foreach (PictureBox pb in pbs)
            {
                if (pb.BackColor == sourceColor)
                    sourceState = pbs.IndexOf(pb);
                if (pb.BackColor == goalColor)
                    goalState = pbs.IndexOf(pb);
            }
        }

        private void DrawFinalPattern(PictureBox pb)
        {
            if (pb != pbs[sourceState])
            {
                if (pb != pbs[goalState])
                {
                    pb.BackColor = pathColor;
                    clearPB.Add(pb);
                }

                DrawFinalPattern(pbs[parentID[pbs.IndexOf(pb)]]);
            }
        }

        private void AddNumToCells()
        {
            Label lblNum;

            for (int i = clearPB.Count - 1; i >= 0; i--)
            {
                lblNum = new Label();
                lblNum.Font = new Font("Arial", 5, FontStyle.Regular);

                clearPB[clearPB.Count - i - 1].Controls.Add(lblNum);
                lblNum.Text = (i + 1).ToString();
                lblNum.BringToFront();

                pathCellNum.Add(lblNum);
            }
        }



        private void btnGeneratePath_Click(object sender, EventArgs e)
        {
            minTLP = 1;
            maxTLP = tlp.RowCount * tlp.ColumnCount;
            int[] randomArray = new int[200];   // 200 cells for being black

            rnd = new Random();

            foreach (PictureBox pb in pbs)
            {
                pb.BackColor = backColor;
            }

            //  fill the randomArray with 200 random numbers
            for (int i = 0; i < randomArray.Length; i++)
            {
                randomArray[i] = rnd.Next(minTLP, maxTLP);
            }

            //  make the back color of random cells black
            foreach (int value in randomArray)
            {
                pbs[value].BackColor = Color.Black;
            }

            BlackMargin();

            SourceGoalState sourceGoal = new SourceGoalState();
            sourceGoal.SourceGoalColor(pbs, sourceColor, goalColor);  // for determining the source state
            sourceGoal.SourceGoalColor(pbs, goalColor, sourceColor);  // for determining the goal state

            DetermineSourceGoalState();
        }

        private void btnHandy_Click(object sender, EventArgs e)
        {
            sourceState = -1;
            goalState = -1;

            sourceColorSentinel = false;
            goalColorSentinel = false;

            if (firstRun == true)
            {
                foreach (PictureBox pb in pbs)
                {
                    pb.BackColor = backColor;
                    
                    pb.Click += (s, ea) =>
                    {
                        switch(cmbColor.Text)
                        {
                            case "Black":
                                if (pb.BackColor == Color.Red)
                                    sourceColorSentinel = false;
                                else if (pb.BackColor == Color.Lime)
                                    goalColorSentinel = false;
                                pb.BackColor = Color.Black;
                                break;

                            case "White":
                                if (pb.BackColor == Color.Red)
                                    sourceColorSentinel = false;
                                else if (pb.BackColor == Color.Lime)
                                    goalColorSentinel = false;
                                pb.BackColor = Color.White;
                                break;

                            case "Red":
                                if (pb.BackColor == Color.Lime)
                                    goalColorSentinel = false;
                                if (sourceColorSentinel == false)
                                    pb.BackColor = Color.Red;
                                sourceColorSentinel = true;
                                sourceState = pbs.IndexOf(pb);
                                break;

                            case "Lime":
                                if (pb.BackColor == Color.Red)
                                    sourceColorSentinel = false;
                                if (goalColorSentinel == false)
                                    pb.BackColor = Color.Lime;
                                goalColorSentinel = true;
                                goalState = pbs.IndexOf(pb);
                                break;
                        }
                    };
                }

                BlackMargin();

                firstRun = false;
            }

            else
            {
                foreach (PictureBox pb in pbs)
                {
                    pb.BackColor = backColor;
                }

                BlackMargin();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BlackMargin();

            try
            {
                switch (cmbAlgorithm.Text)
                {
                    case "BFS":
                        BFS();
                        return;

                    case "DFS":
                        DFS();
                        return;

                    case "UCS":
                        BFS();
                        return;

                    case "A*":
                        A_Star();
                        return;
                }
            }
            catch
            {
                BlackMargin();

                System.Windows.Forms.MessageBox.Show("Couldn't find a path!");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Label lbl in pathCellNum)
                {
                    lbl.Dispose();
                }

                foreach (PictureBox pb in pbs)
                {
                    if (openFlag[pbs.IndexOf(pb)] == true && pb.BackColor != sourceColor && pb.BackColor != goalColor)
                        pb.BackColor = backColor;
                }

                lblTime.Text = "";
                lblOpenedNodes.Text = "";

                clearPB.Clear();
                pathCellNum.Clear();
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("No things to clear!");
            }
        }



        private void btnGeneratePath_MouseMove(object sender, MouseEventArgs e)
        {
            btnGeneratePath.BackColor = Color.Teal;
        }

        private void btnHandy_MouseMove(object sender, MouseEventArgs e)
        {
            btnHandy.BackColor = Color.Teal;
        }

        private void btnSearch_MouseMove(object sender, MouseEventArgs e)
        {
            btnSearch.BackColor = Color.Teal;
        }

        private void btnClear_MouseMove(object sender, MouseEventArgs e)
        {
            btnClear.BackColor = Color.Teal;
        }

        private void btnGeneratePath_MouseLeave(object sender, EventArgs e)
        {
            btnGeneratePath.BackColor = Color.FromArgb(60, 60, 60);
        }

        private void btnHandy_MouseLeave(object sender, EventArgs e)
        {
            btnHandy.BackColor = Color.FromArgb(60, 60, 60);
        }

        private void btnSearch_MouseLeave(object sender, EventArgs e)
        {
            btnSearch.BackColor = Color.FromArgb(60, 60, 60);
        }

        private void btnClear_MouseLeave(object sender, EventArgs e)
        {
            btnClear.BackColor = Color.FromArgb(60, 60, 60);
        }



        private void BFS()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            bfsQueue = new Queue();

            parentID = new int[tlp.RowCount * tlp.ColumnCount];

            bfsQueue.Enqueue(pbs[sourceState]);

            openFlag = new bool[tlp.RowCount * tlp.ColumnCount];

            PictureBox tempPB = pbs[sourceState];

            int numberOfOpenedNodes = 0;

            while (tempPB != pbs[goalState])
            {
                tempPB = (PictureBox)bfsQueue.Dequeue();

                if (tempPB.BackColor != sourceColor && tempPB.BackColor != goalColor)
                    tempPB.BackColor = prePathColor;

                openFlag[pbs.IndexOf(tempPB)] = true;
                InputNeighbours_BFS(tempPB);
            }

            DrawFinalPattern(pbs[goalState]);
            AddNumToCells();

            watch.Stop();

            var executionTime = watch.ElapsedMilliseconds;

            lblTime.Text = (executionTime + " ms").ToString();

            foreach (bool val in openFlag)
            {
                if (val == true)
                    numberOfOpenedNodes++;
            }

            lblOpenedNodes.Text = numberOfOpenedNodes.ToString();
        }
        
        private void InputNeighbours_BFS(PictureBox pb)
        {
            if (pbs[pbs.IndexOf(pb) - tlp.ColumnCount].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) - tlp.ColumnCount] == false)
            {
                bfsQueue.Enqueue(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]);
                parentID[pbs.IndexOf(pb) - tlp.ColumnCount] = pbs.IndexOf(pb);
            }

            if (pbs[pbs.IndexOf(pb) + 1].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) + 1] == false)
            {
                bfsQueue.Enqueue(pbs[pbs.IndexOf(pb) + 1]);
                parentID[pbs.IndexOf(pb) + 1] = pbs.IndexOf(pb);
            }

            if (pbs[pbs.IndexOf(pb) + tlp.ColumnCount].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) + tlp.ColumnCount] == false)
            {
                bfsQueue.Enqueue(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]);
                parentID[pbs.IndexOf(pb) + tlp.ColumnCount] = pbs.IndexOf(pb);
            }

            if (pbs[pbs.IndexOf(pb) - 1].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) - 1] == false)
            {
                bfsQueue.Enqueue(pbs[pbs.IndexOf(pb) - 1]);
                parentID[pbs.IndexOf(pb) - 1] = pbs.IndexOf(pb);
            }
        }



        private void DFS()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            dfsStack = new Stack();

            parentID = new int[tlp.RowCount * tlp.ColumnCount];

            dfsStack.Push(pbs[sourceState]);

            openFlag = new bool[tlp.RowCount * tlp.ColumnCount];

            tempPB = pbs[sourceState];

            int numberOfOpenedNodes = 0;

            while (tempPB != pbs[goalState])
            {
                tempPB = (PictureBox)dfsStack.Pop();
                
                if (tempPB.BackColor != sourceColor && tempPB.BackColor != goalColor)
                    tempPB.BackColor = prePathColor;
                
                openFlag[pbs.IndexOf(tempPB)] = true;
                InputNeighbours_DFS(tempPB);
            }

            DrawFinalPattern(pbs[goalState]);
            AddNumToCells();

            watch.Stop();

            var executionTime = watch.ElapsedMilliseconds;

            lblTime.Text = (executionTime + " ms").ToString();

            foreach (bool val in openFlag)
            {
                if (val == true)
                    numberOfOpenedNodes++;
            }

            lblOpenedNodes.Text = numberOfOpenedNodes.ToString();
        }
        
        private void InputNeighbours_DFS(PictureBox pb)
        {
            if (pbs[pbs.IndexOf(pb) - 1].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) - 1] == false)
            {
                dfsStack.Push(pbs[pbs.IndexOf(pb) - 1]);
                parentID[pbs.IndexOf(pb) - 1] = pbs.IndexOf(pb);
            }

            if (pbs[pbs.IndexOf(pb) + tlp.ColumnCount].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) + tlp.ColumnCount] == false)
            {
                dfsStack.Push(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]);
                parentID[pbs.IndexOf(pb) + tlp.ColumnCount] = pbs.IndexOf(pb);
            }

            if (pbs[pbs.IndexOf(pb) + 1].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) + 1] == false)
            {
                dfsStack.Push(pbs[pbs.IndexOf(pb) + 1]);
                parentID[pbs.IndexOf(pb) + 1] = pbs.IndexOf(pb);
            }

            if (pbs[pbs.IndexOf(pb) - tlp.ColumnCount].BackColor != Color.Black && openFlag[pbs.IndexOf(pb) - tlp.ColumnCount] == false)
            {
                dfsStack.Push(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]);
                parentID[pbs.IndexOf(pb) - tlp.ColumnCount] = pbs.IndexOf(pb);
            }
        }



        private void A_Star()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            closedList = new List<PictureBox>();
            openedList = new List<PictureBox>();

            int numberOfOpenedNodes = 0;

            parentID = new int[tlp.RowCount * tlp.ColumnCount];

            openFlag = new bool[tlp.RowCount * tlp.ColumnCount];

            PictureBox tempPB = pbs[sourceState];

            f = new double[tlp.RowCount * tlp.ColumnCount];
            g = new int[tlp.RowCount * tlp.ColumnCount];
            h = new double[tlp.RowCount * tlp.ColumnCount];

            openedList.Add(tempPB);

            g[sourceState] = 0;

            CalculateF(tempPB, g[sourceState], h[sourceState]);

            while (tempPB != pbs[goalState])
            {
                tempPB = OpenListMin(openedList);

                if (tempPB.BackColor != sourceColor && tempPB.BackColor != goalColor)
                    tempPB.BackColor = prePathColor;

                closedList.Add(tempPB);

                openFlag[pbs.IndexOf(tempPB)] = true;

                InputNeighbours_A_Star(tempPB, g[pbs.IndexOf(tempPB)]);

                openedList.Remove(tempPB);
            }

            DrawFinalPattern(pbs[goalState]);
            AddNumToCells();

            watch.Stop();

            var executionTime = watch.ElapsedMilliseconds;

            lblTime.Text = (executionTime + " ms").ToString();

            foreach (bool val in openFlag)
            {
                if (val == true)
                    numberOfOpenedNodes++;
            }

            lblOpenedNodes.Text = numberOfOpenedNodes.ToString();
        }
        
        private void InputNeighbours_A_Star(PictureBox pb, int preG)
        {
            if (closedList.IndexOf(pbs[pbs.IndexOf(pb) - 1]) == -1)
            {
                if (pbs[pbs.IndexOf(pb) - 1].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) - 1]) == -1)
                {
                    openedList.Add(pbs[pbs.IndexOf(pb) - 1]);
                    g[pbs.IndexOf(pb) - 1] = preG + 1;
                    Heuristic(pbs[pbs.IndexOf(pb) - 1]);
                    CalculateF(pbs[pbs.IndexOf(pb) - 1], g[pbs.IndexOf(pb) - 1], Heuristic(pbs[pbs.IndexOf(pb) - 1]));
                    parentID[pbs.IndexOf(pb) - 1] = pbs.IndexOf(pb);
                }
                else if (pbs[pbs.IndexOf(pb) - 1].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) - 1]) != -1)
                {
                    double tempF = f[pbs.IndexOf(pb) - 1];

                    if (CalculateF(pbs[pbs.IndexOf(pb) - 1], preG + 1, h[pbs.IndexOf(pb) - 1]) < tempF)
                        parentID[pbs.IndexOf(pb) - 1] = pbs.IndexOf(pb);
                    else
                        f[pbs.IndexOf(pb) - 1] = tempF;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (closedList.IndexOf(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]) == -1)
            {
                if (pbs[pbs.IndexOf(pb) + tlp.ColumnCount].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]) == -1)
                {
                    openedList.Add(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]);
                    g[pbs.IndexOf(pb) + tlp.ColumnCount] = preG + 1;
                    Heuristic(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]);
                    CalculateF(pbs[pbs.IndexOf(pb) + tlp.ColumnCount], g[pbs.IndexOf(pb) + tlp.ColumnCount], Heuristic(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]));
                    parentID[pbs.IndexOf(pb) + tlp.ColumnCount] = pbs.IndexOf(pb);
                }
                else if (pbs[pbs.IndexOf(pb) + tlp.ColumnCount].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) + tlp.ColumnCount]) != -1)
                {
                    double tempF = f[pbs.IndexOf(pb) + tlp.ColumnCount];

                    if (CalculateF(pbs[pbs.IndexOf(pb) + tlp.ColumnCount], preG + 1, h[pbs.IndexOf(pb) + tlp.ColumnCount]) < tempF)
                        parentID[pbs.IndexOf(pb) + tlp.ColumnCount] = pbs.IndexOf(pb);
                    else
                        f[pbs.IndexOf(pb) + tlp.ColumnCount] = tempF;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (closedList.IndexOf(pbs[pbs.IndexOf(pb) + 1]) == -1)
            {
                if (pbs[pbs.IndexOf(pb) + 1].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) + 1]) == -1)
                {
                    openedList.Add(pbs[pbs.IndexOf(pb) + 1]);
                    g[pbs.IndexOf(pb) + 1] = preG + 1;
                    Heuristic(pbs[pbs.IndexOf(pb) + 1]);
                    CalculateF(pbs[pbs.IndexOf(pb) + 1], g[pbs.IndexOf(pb) + 1], Heuristic(pbs[pbs.IndexOf(pb) + 1]));
                    parentID[pbs.IndexOf(pb) + 1] = pbs.IndexOf(pb);
                }
                else if (pbs[pbs.IndexOf(pb) + 1].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) + 1]) != -1)
                {
                    double tempF = f[pbs.IndexOf(pb) + 1];

                    if (CalculateF(pbs[pbs.IndexOf(pb) + 1], preG + 1, h[pbs.IndexOf(pb) + 1]) < tempF)
                        parentID[pbs.IndexOf(pb) + 1] = pbs.IndexOf(pb);
                    else
                        f[pbs.IndexOf(pb) + 1] = tempF;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (closedList.IndexOf(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]) == -1)
            {
                if (pbs[pbs.IndexOf(pb) - tlp.ColumnCount].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]) == -1)
                {
                    openedList.Add(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]);
                    g[pbs.IndexOf(pb) - tlp.ColumnCount] = preG + 1;
                    Heuristic(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]);
                    CalculateF(pbs[pbs.IndexOf(pb) - tlp.ColumnCount], g[pbs.IndexOf(pb) - tlp.ColumnCount], Heuristic(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]));
                    parentID[pbs.IndexOf(pb) - tlp.ColumnCount] = pbs.IndexOf(pb);
                }
                else if (pbs[pbs.IndexOf(pb) - tlp.ColumnCount].BackColor != Color.Black && openedList.IndexOf(pbs[pbs.IndexOf(pb) - tlp.ColumnCount]) != -1)
                {
                    double tempF = f[pbs.IndexOf(pb) - tlp.ColumnCount];

                    if (CalculateF(pbs[pbs.IndexOf(pb) - tlp.ColumnCount], preG + 1, h[pbs.IndexOf(pb) - tlp.ColumnCount]) < tempF)
                        parentID[pbs.IndexOf(pb) - tlp.ColumnCount] = pbs.IndexOf(pb);
                    else
                        f[pbs.IndexOf(pb) - tlp.ColumnCount] = tempF;
                }
            }
        }

        private PictureBox OpenListMin(List<PictureBox> ol)
        {
            PictureBox minPB = ol[0];

            for(int i = 1; i <= ol.Count - 1; i++)
            {
                if (f[pbs.IndexOf(ol[i])] < f[pbs.IndexOf(minPB)])
                    minPB = ol[i];
            }

            return minPB;
        }
        
        private double Heuristic(PictureBox pb)
        {
            return h[pbs.IndexOf(pb)] = Math.Sqrt(Math.Pow(pb.Location.X - pbs[goalState].Location.X, 2) + Math.Pow(pb.Location.Y - pbs[goalState].Location.Y, 2));
        }

        private double CalculateF(PictureBox pb, int g, double h)
        {
            return f[pbs.IndexOf(pb)] = g + h;
        }
    }
}