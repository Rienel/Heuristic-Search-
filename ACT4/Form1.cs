using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace ACT4
{
    public partial class Form1 : Form
    {

        //simulated annealing
        int side;
        int n = 6;
        SixState startState;
        SixState currentState;
        int moveCounter;

        double temperature = 1000.0;
        double coolingRate = 0.99;

        int[,] hTable;
        ArrayList bMoves;
        Object chosenMove;

        public Form1()
        {
            InitializeComponent();
            side = pictureBox1.Width / n;
            startState = randomSixState();
            currentState = new SixState(startState);
            updateUI();
            label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
        }

        private void updateUI()
        {
            pictureBox2.Refresh();
            label3.Text = "Attacking pairs: " + getAttackingPairs(currentState);
            label4.Text = "Moves: " + moveCounter;
            hTable = getHeuristicTableForPossibleMoves(currentState);
            bMoves = getBestMoves(hTable);

            listBox1.Items.Clear();
            foreach (Point move in bMoves)
            {
                listBox1.Items.Add(move);
            }

            if (bMoves.Count > 0)
                chosenMove = chooseMove(bMoves);

            label2.Text = "Chosen move: " + chosenMove;
        }

        private SixState randomSixState()
        {
            Random r = new Random();
            return new SixState(r.Next(n), r.Next(n), r.Next(n), r.Next(n), r.Next(n), r.Next(n));
        }

        private int getAttackingPairs(SixState f)
        {
            int attackers = 0;
            for (int rf = 0; rf < n; rf++)
            {
                for (int tar = rf + 1; tar < n; tar++)
                {
                    if (f.Y[rf] == f.Y[tar]) attackers++;
                    if (f.Y[tar] == f.Y[rf] + tar - rf) attackers++;
                    if (f.Y[rf] == f.Y[tar] + tar - rf) attackers++;
                }
            }
            return attackers;
        }

        private int[,] getHeuristicTableForPossibleMoves(SixState thisState)
        {
            int[,] hStates = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    SixState possible = new SixState(thisState);
                    possible.Y[i] = j;
                    hStates[i, j] = getAttackingPairs(possible);
                }
            }
            return hStates;
        }

        private ArrayList getBestMoves(int[,] heuristicTable)
        {
            ArrayList bestMoves = new ArrayList();
            int bestHeuristicValue = heuristicTable[0, 0];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (bestHeuristicValue > heuristicTable[i, j])
                    {
                        bestHeuristicValue = heuristicTable[i, j];
                        bestMoves.Clear();
                        if (currentState.Y[i] != j)
                            bestMoves.Add(new Point(i, j));
                    }
                    else if (bestHeuristicValue == heuristicTable[i, j])
                    {
                        if (currentState.Y[i] != j)
                            bestMoves.Add(new Point(i, j));
                    }
                }
            }
            label5.Text = "Possible Moves (H=" + bestHeuristicValue + ")";
            return bestMoves;
        }

        private Object chooseMove(ArrayList possibleMoves)
        {
            Random r = new Random();
            int currentAttackingPairs = getAttackingPairs(currentState);

            foreach (Point move in possibleMoves)
            {
                SixState nextState = new SixState(currentState);
                nextState.Y[move.X] = move.Y;
                int nextAttackingPairs = getAttackingPairs(nextState);

                int deltaE = nextAttackingPairs - currentAttackingPairs;

                if (deltaE < 0 || r.NextDouble() < Math.Exp(-deltaE / temperature))
                {
                    return move;
                }
            }

            return possibleMoves[r.Next(possibleMoves.Count)];
        }

        private void executeMove(Point move)
        {
            currentState.Y[move.X] = move.Y;
            moveCounter++;
            temperature *= coolingRate;
            chosenMove = null;
            updateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (getAttackingPairs(currentState) > 0)
                executeMove((Point)chosenMove);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while (getAttackingPairs(currentState) > 0)
            {
                executeMove((Point)chosenMove);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            startState = randomSixState();
            currentState = new SixState(startState);
            moveCounter = 0;
            temperature = 1000.0;
            updateUI();
            pictureBox1.Refresh();
            label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
        }
    }
}