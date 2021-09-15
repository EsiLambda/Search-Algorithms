using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SearchProblem
{
    class SourceGoalState
    {
        Random rnd;

        public SourceGoalState()
        {
            rnd = new Random();
        }

        public void SourceGoalColor(List<PictureBox> pbs, Color color1, Color color2)
        {
            int rndNumber = rnd.Next(1, pbs.Count);

            if(pbs[rndNumber].BackColor != Color.Black && pbs[rndNumber].BackColor != color2)
            {
                pbs[rndNumber].BackColor = color1;
            }
            else
            {
                SourceGoalColor(pbs, color1, color2);
            }
        }
    }
}