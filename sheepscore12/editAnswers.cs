using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sheepscore12
{
    public partial class editAnswers : Form
    {
        public editAnswers()
        {
            InitializeComponent();
        }

        int add_index = 0;

        //temporary data type for this editor
        public class EdPlayer
        {
            public string Name;
            public string Answers; //each answer on a new line
            //keep track of original position in players[] so we
            //can maintain groupings. -1 if a new player
            public int OriginalPosition;
            public decimal StartScore;
            public const int NewPlayerOriginalPosition = -1;
            public int EdIndex;
            public bool NeedsRegrouping;

            public EdPlayer()
            {
                Name = ""; Answers = ""; OriginalPosition = NewPlayerOriginalPosition; StartScore = 0; NeedsRegrouping = false;
            }
            public EdPlayer(string newName, string newAnswers, decimal start_score = 0, int origPos = NewPlayerOriginalPosition)
            {
                Name = newName;
                Answers = newAnswers;
                OriginalPosition = origPos;
                StartScore = start_score;
                NeedsRegrouping = false;
            }
        }


        int curPlayer, numQuestions;
        public List<EdPlayer> ed_players;
        public string HelpString;

        InputText FormNewPlayer = new InputText();

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ed_players[curPlayer].Answers != textBox1.Text)
            {
                ed_players[curPlayer].NeedsRegrouping = true;
                ed_players[curPlayer].Answers = textBox1.Text;
            }

            ed_players[curPlayer].StartScore = numStartScore.Value;            

            curPlayer = comboBox1.SelectedIndex;

            textBox1.Text = ed_players[curPlayer].Answers;
            numStartScore.Value = ed_players[curPlayer].StartScore;
            textBox1.Select(0, 0);

            //label1.Text = curPlayer + ", " + comboBox1.SelectedIndex + ", " + ed_players[curPlayer].Name;
        }

        //Load data
        private void button1_Click(object sender, EventArgs e)
        {

            openFileDialog1.Title = "Load Entries from File";
            openFileDialog1.Filter = "Text File|*.txt";

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            string[] filecontents;
            List<EdPlayer> new_ed_list = new List<EdPlayer>();

            try 
            {
                filecontents = System.IO.File.ReadAllLines(openFileDialog1.FileName);
            }
            catch
            {
                MessageBox.Show("Couldn't load " + openFileDialog1.FileName);
                return;
            }

            try
            {
                
                //go through file and look for each PM start
                //which is a long string of ======================================
                List<int> PMlist = new List<int>();

                for (int i = 0; i < filecontents.Length;i++)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(filecontents[i], "={20,}"))
                    {
                        PMlist.Add(i);
                    }
                }

                //for each found PM...
                for (int i = 0; i < PMlist.Count; i++)
                {
                    //name is found next line down
                    string newPlayerName = string.Join(" ", System.Text.RegularExpressions.Regex.Split(
                        filecontents[PMlist[i] + 1], "\\s+").Where((txt,indx)=>(indx>=2)).ToArray() );

                    EdPlayer new_ep;

                    if (ed_players.Any(ep => string.Compare(ep.Name, newPlayerName) == 0))
                    {
                        //if this player already exists just overwrite his answers
                        new_ep = ed_players.First(ep => string.Compare(ep.Name, newPlayerName) == 0);
                    }
                    else
                    {
                        //it is a new player
                        new_ep = new EdPlayer(newPlayerName, "", 0, EdPlayer.NewPlayerOriginalPosition);
                        new_ed_list.Add(new_ep);
                    }

                    //answers go from 6 lines down to before the next ===============
                    //or end of file if it's the last one
                    if (i == PMlist.Count-1)
                    {
                        new_ep.Answers =string.Join(Environment.NewLine,
                            filecontents.Where((txt, line) => (line >= (PMlist[i] + 6))).ToArray());
                    }
                    else
                    {
                        new_ep.Answers = string.Join(Environment.NewLine,
                            filecontents.Where((txt, line) => (line >= (PMlist[i] + 6))
                                && (line < PMlist[i + 1])).ToArray());
                    }

                }



            }
            catch
            {
                MessageBox.Show("Couldn't read " + openFileDialog1.FileName);
            }

            ed_players.AddRange(new_ed_list);
            updateComboBox();

            if (new_ed_list.Count == 0)
            {
                MessageBox.Show("Didn't find any entries in " + openFileDialog1.FileName);
            }
            else
            {
                MessageBox.Show("Loaded " + new_ed_list.Count.ToString()
                    + (new_ed_list.Count == 1 ? " entry" : " entries") +
                    " from " + openFileDialog1.FileName);
            }
            
        }

        //New Player
        private void button2_Click(object sender, EventArgs e)
        {
            FormNewPlayer.StartPosition = FormStartPosition.CenterParent;
            FormNewPlayer.label1.Text = "Enter new player name:";
            FormNewPlayer.Text = "New Player";
            FormNewPlayer.textBox1.Clear();

            FormNewPlayer.ShowDialog();

            int cur_add_index = add_index++;

            if (FormNewPlayer.DialogResult == DialogResult.OK)
            {
                //save old player values
                if (curPlayer >= 0 && curPlayer < ed_players.Count)
                {
                    ed_players[curPlayer].Answers = textBox1.Text;
                    ed_players[curPlayer].StartScore = numStartScore.Value;
                }

                var new_player = new EdPlayer(FormNewPlayer.textBox1.Text, "", 0, EdPlayer.NewPlayerOriginalPosition);
                new_player.EdIndex = cur_add_index;

                ed_players.Add(new_player);

                ed_players = (from ep in ed_players
                              orderby ep.Name
                              select ep).ToList();

                //to make sure new player gets selected
                curPlayer = ed_players.FindIndex(p => p.EdIndex == cur_add_index);
            } 
            
            updateComboBox();

        }

        //Delete Player
        private void button3_Click(object sender, EventArgs e)
        {
            ed_players = ed_players.Where((ep, ind) => (ind != curPlayer)).ToList();
            if (curPlayer >= ed_players.Count)
                curPlayer = ed_players.Count;

            updateComboBox();

        }

        //save changes
        private void button5_Click(object sender, EventArgs e)
        {
            //make sure current thing gets saved
            if (curPlayer < ed_players.Count)
            {
                if (ed_players[curPlayer].Answers != textBox1.Text)
                {
                    ed_players[curPlayer].NeedsRegrouping = true;
                    ed_players[curPlayer].Answers = textBox1.Text;
                }
                ed_players[curPlayer].StartScore = numStartScore.Value;
            }
            this.DialogResult = DialogResult.OK;
        }

        //cancel
        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        //update combo box; try to keep the same selection
        private void updateComboBox()
        {
            if (ed_players.Count == 0)
            {
                textBox1.Text = HelpString;
                numStartScore.Value = 0;
                comboBox1.Items.Clear();
                return;
                
            }

            comboBox1.Items.Clear();

            ed_players = (from ep in ed_players
                         orderby ep.Name
                         select ep).ToList();

            comboBox1.Items.AddRange(ed_players.Select(x=>x.Name).ToArray());

            //make sure selection is valid
            if (curPlayer < 0)
            {
                curPlayer = 0;
            }
            else if (curPlayer >= comboBox1.Items.Count)
            {
                curPlayer = comboBox1.Items.Count - 1;
            }

            //update textbox before changing combobox selectedindex
            //otherwise selectionchanged event will fire and overwrite stuff
            textBox1.Text = ed_players[curPlayer].Answers;
            numStartScore.Value = ed_players[curPlayer].StartScore;
            comboBox1.SelectedIndex = curPlayer;
        }

        //change name
        private void button7_Click(object sender, EventArgs e)
        {
            if (curPlayer >= ed_players.Count)
            {
                MessageBox.Show("No player selected.");
                return;
            }

            FormNewPlayer.StartPosition = FormStartPosition.CenterParent;
            FormNewPlayer.Text = "Change Name";
            FormNewPlayer.textBox1.Text = ed_players[comboBox1.SelectedIndex].Name;
            FormNewPlayer.label1.Text = "Enter new player name:";

            FormNewPlayer.ShowDialog();

            if (FormNewPlayer.DialogResult == DialogResult.OK)
            {
                ed_players[comboBox1.SelectedIndex].Name = FormNewPlayer.textBox1.Text;
            }

            updateComboBox();
        }

        //showing form
        private void editAnswers_Shown(object sender, EventArgs e)
        {
            HelpString = "Click " + buttonLoad.Text + " to load players and answers " + Environment.NewLine +
                         "from a PM text file, or click " + buttonNewPlayer.Text + Environment.NewLine +
                         "to add players manually.";


            ed_players = Form1.sg.Players.Select(x => new EdPlayer(
                x.Name, string.Join(Environment.NewLine, x.Answers.Select(ans => ans.Text).ToArray())
                , x.StartScore, x.GameIndex)).ToList();

            numQuestions = Form1.sg.Questions.Count;
            curPlayer = 0;
            updateComboBox();
        }

        private void editAnswers_Load_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
