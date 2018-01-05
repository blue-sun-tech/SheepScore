using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace sheepscore12
{

    using ShMethod = ShGame.ShMethod;
    using ShPlayer = ShGame.ShPlayer;
    using ShAnswer = ShGame.ShAnswer;
    using ShGroup = ShGame.ShGroup;
    using ShQuestion = ShGame.ShQuestion;

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        public class MiscConsts
        {
            public const string ListViewItem = "System.Windows.Forms.ListViewItem";
            public const string ListViewGroup = "System.Windows.Forms.ListViewGroup";
            public const int NewGroupTag = -1;
        }

        public static ShGame sg;

        public static int cur_q_index;
        public static ShMethod curScoreMethod;

        editQuestions FormQuestions = new editQuestions();
        editAnswers FormAnswers = new editAnswers();
        bool sheep_modified;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = Application.ProductName + " *** BETA 1 ***";
            SetScoringMethod(ShMethod.Sheep);
            sg = new ShGame();

            redrawTreeView();
            SetTextForAllTreenodes();
            sheep_modified = false;
        }

        //Redraw all current groups/answers on the listview
        private void redrawTreeView()
        {


            treeView1.Nodes.Clear();

            //give instructions if no questions loaded
            if (sg == null || sg.Questions.Count == 0)
            {
                label_question.Text = "Click " + sheepToolStripMenuItem.Text + " > " +
                    editQuestionsToolStripMenuItem.Text + " to begin.";
                return;
            }

            //make sure we're on a valid question
            if (cur_q_index > sg.Questions.Count - 1)
                cur_q_index = sg.Questions.Count - 1;

            if (cur_q_index < 0)
                cur_q_index = 0;

            ShQuestion curQuestion = sg.Questions[cur_q_index];

            //make sure the updown and label is right
            numericUpDown_question.Minimum = 1;
            numericUpDown_question.Maximum = sg.Questions.Count;
            label_question.Text = curQuestion.Text;

            //give instructions if no players loaded
            if (sg.Players.Count == 0)
            {

                treeView1.Nodes.Add("Click " + sheepToolStripMenuItem.Text + " > " +
                    addRemovePlayersToolStripMenuItem.Text + " to add entries.");
                return;
            }

            TreeNode curGroup;
            TreeNode curItem;

            //loop through each group
            //text will be added later so don't bother with it in this function
            foreach (ShGroup grp in curQuestion.Groups)
            {
                //add group to listview.
                //use tag to keep track of group
                curGroup = new TreeNode("");
                curGroup.Tag = grp;
                treeView1.Nodes.Add(curGroup);

                //add each player's answer to listview.
                //use tag to keep track of answer 
                foreach (ShAnswer ans in grp.Answers)
                {
                    curItem = new TreeNode("");
                    curItem.Tag = ans;
                    curGroup.Nodes.Add(curItem);
                }
                //alternate colors each group
                //curGroup.Expand();

            }

            treeView1.TreeViewNodeSorter = new TreeNodeSorter();

            SetTextForAllTreenodes();

        }

        //change current question
        private void numericUpDown_question_ValueChanged(object sender, EventArgs e)
        {

            if (numericUpDown_question.Value > sg.Questions.Count)
            {
                numericUpDown_question.Value = numericUpDown_question.Maximum =
                    sg.Questions.Count;
            }
            cur_q_index = (int)numericUpDown_question.Value - 1;

            treeView1.BeginUpdate();
            redrawTreeView();
            treeView1.EndUpdate();

        }

        //exit program
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        //load edit questions window
        private void editQuestionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormQuestions.StartPosition = FormStartPosition.CenterParent;
            FormQuestions.ShowDialog();

            if (FormQuestions.DialogResult != DialogResult.OK)
                return;


            //modify questions and
            //resize answers, groupNames, and updown here
            string tempQstr = FormQuestions.textBox1.Text;
            tempQstr.Trim();
            string[] newQuestions = tempQstr.Split(new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            bool tempProceed = true;
            if (newQuestions.Length < sg.Questions.Count)
            {
                if (MessageBox.Show("You are reducing the number of questions."
                    + " This will delete some answers. Continue?",
                    "", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    tempProceed = false;
                }
            }
            if (tempProceed)
            {
                //if reducing questions, delete questions from the back
                if (newQuestions.Length < sg.Questions.Count)
                {
                    List<ShQuestion> questionsToDelete =
                        sg.Questions.GetRange(newQuestions.Length,
                        sg.Questions.Count - newQuestions.Length);
                    //remove answer references from player object
                    foreach (ShQuestion que in questionsToDelete)
                    {
                        sg.NiceDeleteQuestion(que);
                    }

                }
                //overwrite existing questions

                for (int iques = 0; iques < sg.Questions.Count; iques++)
                {
                    sg.Questions[iques].Text = newQuestions[iques];
                }

                //add new questions to the back if necessary
                if (newQuestions.Length > sg.Questions.Count)
                {
                    foreach (string newQtxt in newQuestions.Where(
                        (txt, i) => i >= sg.Questions.Count))
                    {
                        sg.NiceAddQuestion(newQtxt);
                    }

                }

            }

            sheep_modified = true;
            redrawTreeView();
        }

        //load edit players/answers window
        private void addRemovePlayersToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormAnswers.StartPosition = FormStartPosition.CenterParent;
            FormAnswers.ShowDialog();

            if (FormAnswers.DialogResult != DialogResult.OK)
                return;

            //get list of players that will be deleted later
            //by finding players whose positions are not in any of the ed_players original positions
            List<ShPlayer> playersToDelete = new List<ShPlayer>(
                    sg.Players.Where((ply, i) => FormAnswers.ed_players.All(
                        ep => ep.OriginalPosition != i)));

            //loop through each player from the editor
            foreach (editAnswers.EdPlayer ep in FormAnswers.ed_players)
            {
                //get list of answers as strings
                List<string> ansTxt = System.Text.RegularExpressions.Regex.Split(ep.Answers, Environment.NewLine).ToList();

                //if this player is new, add it
                if (ep.OriginalPosition == editAnswers.EdPlayer.NewPlayerOriginalPosition)
                {
                    ShPlayer newPlayer = sg.NiceAddPlayer(ep.Name, ansTxt.ToArray(), ep.StartScore);
                    //guess groupings
                    foreach (ShAnswer ans in newPlayer.Answers)
                    {
                        sg.GuessGroup(ans);
                    }
                }
                else //this player is not new, all we have to do is update answers/names
                {
                    for (int iques = 0; iques < sg.Questions.Count; iques++)
                    {
                        //set some default text
                        string tempAnsTxt = "(blank)";
                        //check if we have text for this answer
                        if (iques < ansTxt.Count)
                            if (ansTxt[iques].Trim() != "")
                                tempAnsTxt = ansTxt[iques].Trim();
                        sg.Players[ep.OriginalPosition].Answers[iques].Text = tempAnsTxt;
                        sg.Players[ep.OriginalPosition].Name = ep.Name;
                        sg.Players[ep.OriginalPosition].StartScore = ep.StartScore;
                    }
                }
            }

            //now delete all the players that no longer exist
            foreach (ShPlayer ply in playersToDelete)
            {
                sg.NiceDeletePlayer(ply);
            }

            sheep_modified = true;
            redrawTreeView();
        }

        //main drag/drop function
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            //stop sorting while dragging
            treeView1.TreeViewNodeSorter = null;

            Point cp = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode destNode = treeView1.GetNodeAt(cp);

            //don't continue if not a valid node
            if (!e.Data.GetDataPresent(typeof(TreeNode)))
                return;

            //   treeView1.BeginUpdate();

            TreeNode movingNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            TreeNode prevParent, newParent;

            ShQuestion curQuestion = sg.Questions[cur_q_index];

            //different code depending on what type of thing we're dragging/dragging to

            if (movingNode.Tag.GetType() == typeof(ShAnswer)
                && destNode.Tag.GetType() == typeof(ShAnswer))
            {
                //moving an answer to another answer
                ShAnswer ansToMove = (ShAnswer)movingNode.Tag;
                ShAnswer destAnswer = (ShAnswer)destNode.Tag;
                ansToMove.ChangeGroup(destAnswer.Group);

                prevParent = movingNode.Parent;
                newParent = destNode.Parent;
                prevParent.Nodes.Remove(movingNode);
                newParent.Nodes.Add(movingNode);

            }
            else if (movingNode.Tag.GetType() == typeof(ShAnswer)
                && destNode.Tag.GetType() == typeof(ShGroup))
            {
                //moving an answer to another group
                ShAnswer ansToMove = (ShAnswer)movingNode.Tag;
                ShGroup destGroup = (ShGroup)destNode.Tag;
                ansToMove.ChangeGroup(destGroup);

                prevParent = movingNode.Parent;
                newParent = destNode;
                prevParent.Nodes.Remove(movingNode);
                newParent.Nodes.Add(movingNode);
            }
            else if (movingNode.Tag.GetType() == typeof(ShGroup)
                && destNode.Tag.GetType() == typeof(ShAnswer))
            {
                //moving a group to an answer
                ShGroup grpToMove = (ShGroup)movingNode.Tag;
                ShAnswer destAnswer = (ShAnswer)destNode.Tag;
                grpToMove.MergeToGroup(destAnswer.Group);

                prevParent = movingNode;
                newParent = destNode.Parent;

                List<TreeNode> ansNodes = new List<TreeNode>(prevParent.Nodes.Cast<TreeNode>());
                foreach (TreeNode nod in ansNodes)
                {
                    prevParent.Nodes.Remove(nod);
                    newParent.Nodes.Add(nod);
                }

            }
            else if (movingNode.Tag.GetType() == typeof(ShGroup)
                && destNode.Tag.GetType() == typeof(ShGroup))
            {
                //moving a group to a group
                ShGroup grpToMove = (ShGroup)movingNode.Tag;
                ShGroup destGroup = (ShGroup)destNode.Tag;
                grpToMove.MergeToGroup(destGroup);

                prevParent = movingNode;
                newParent = destNode;

                List<TreeNode> ansNodes = new List<TreeNode>(prevParent.Nodes.Cast<TreeNode>());
                foreach (TreeNode nod in ansNodes)
                {
                    prevParent.Nodes.Remove(nod);
                    newParent.Nodes.Add(nod);
                }
            }
            else
            { treeView1.EndUpdate(); return; }

            //if prevParent is empty, delete it
            if (prevParent.Nodes.Count == 0) prevParent.Remove();

            SetTextForAllTreenodes();
            sheep_modified = true;

            treeView1.EndUpdate();

        }

        //show appropriate cursor
        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            Point cp = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode destNode = treeView1.GetNodeAt(cp);


            //show Move cursor if this is a valid Drop
            //otherwise show None cursor
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {

                TreeNode movingNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

                //answers can be moved to another answer, group, or newgroup
                if (movingNode.Tag.GetType() == typeof(ShAnswer)
                    && (destNode.Tag.GetType() == typeof(ShAnswer) ||
                    destNode.Tag.GetType() == typeof(ShGroup)))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else if (movingNode.Tag.GetType() == typeof(ShGroup)
                    && (destNode.Tag.GetType() == typeof(ShGroup)
                    || destNode.Tag.GetType() == typeof(ShAnswer)))
                {
                    //groups can be merged to another group 
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }

            }
            else
            {
                e.Effect = DragDropEffects.None;
            }


        }

        //starting a dragdrop
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                treeView1.DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        //returns text that should be displayed on this treenode
        private string TextForTreeNode(ShGroup grp)
        {
            if (grp.Answers.Count == 0)
                return grp.Text;

            string scoreString = "";

            try
            {
                if (curScoreMethod == ShMethod.Manual)
                {
                    scoreString = "[" + grp.GroupBonus + "]";
                }
                else
                {
                    string bonus_text = "";
                    if (grp.BonusType == ShGame.ShBonusType.Override)
                    {
                        bonus_text = " (=" + grp.GroupBonus.ToString("0.#####") + ")";
                    }
                    else if (grp.BonusType == ShGame.ShBonusType.Add)
                    {
                        if (grp.GroupBonus > 0) bonus_text = " + " + grp.GroupBonus.ToString("0.#####");
                        if (grp.GroupBonus < 0) bonus_text = " - " + (-grp.GroupBonus).ToString("0.#####");
                    }
                    scoreString = "[" + grp.Question.Scores(false)
                        [grp.Answers[0].Player].ToString("0.#####")
                        + bonus_text + "]";
                }
            }
            catch
            {
                scoreString = "ERROR";
            }
            return grp.Text + " - " + (grp.Correct ? "" :
                ShGame.GetCorrectText(curScoreMethod, grp.Correct).ToUpper() + " - ") + scoreString;
        }

        private string TextForTreeNode(ShAnswer ans)
        {
            string bonus_text = "";
            if (ans.BonusType == ShGame.ShBonusType.Override)
            {
                bonus_text = " (=" + ans.AnswerBonus + ")";
            }
            else if (ans.BonusType == ShGame.ShBonusType.Add)
            {
                if (ans.AnswerBonus != 0)
                    bonus_text = " (" + (ans.AnswerBonus > 0 ? "+" : "") + ans.AnswerBonus + ")";
            }

            return ans.Text + " - " + ans.Player.Name + bonus_text;

        }

        //update text on all treenode items
        private void SetTextForAllTreenodes()
        {
            if (treeView1.Nodes.Count == 0) return;
            int i = 0;

            foreach (TreeNode grpNode in treeView1.Nodes)
            {
                if (grpNode.Tag == null) continue;

                if (grpNode.Tag.GetType() == typeof(ShGroup))
                {
                    ShGroup grp = (ShGroup)grpNode.Tag;
                    grpNode.Text = TextForTreeNode(grp);

                    foreach (TreeNode ansNode in grpNode.Nodes)
                    {
                        if (ansNode.Tag.GetType() == typeof(ShAnswer))
                        {
                            ansNode.Text = TextForTreeNode((ShAnswer)ansNode.Tag);
                            ansNode.ForeColor = treeView1.ForeColor;
                        }

                    }

                    if (i % 2 == 0)
                        grpNode.BackColor = Color.FromArgb(245, 245, 245);
                    else
                        grpNode.BackColor = Color.FromArgb(230, 230, 230);


                    if (grp.Correct)
                        grpNode.ForeColor = Color.Blue;
                    else
                        grpNode.ForeColor = Color.DarkRed;

                }

                i++;
            }

        }

        public class TreeNodeSorter : System.Collections.IComparer //testey
        {
            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;

                if (tx.Tag == null || ty.Tag == null)
                {
                    return 0;
                }

                if (tx.Tag.GetType() == typeof(ShGroup)
                    && ty.Tag.GetType() == typeof(ShGroup))
                {
                    ShGroup gx = (ShGroup)tx.Tag;
                    ShGroup gy = (ShGroup)ty.Tag;

                    return string.Compare(gx.Text, gy.Text);

                }
                else if (tx.Tag.GetType() == typeof(ShAnswer)
                    && ty.Tag.GetType() == typeof(ShAnswer))
                {
                    int temp = string.Compare(
                        ((ShAnswer)tx.Tag).Text,
                        ((ShAnswer)ty.Tag).Text,
                        true);

                    if (temp != 0)
                        return temp;
                    else
                        return string.Compare(
                        ((ShAnswer)tx.Tag).Player.Name,
                        ((ShAnswer)ty.Tag).Player.Name,
                        true);
                }
                else
                    return 0;
            }
        }

        //show right-click menu
        //set Tag of RCM_group or RCM_answer to the clicked node
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right
                && treeView1.GetNodeAt(e.Location) != null
                && treeView1.GetNodeAt(e.Location).Tag != null)

            {
                TreeNode clicked_node = treeView1.GetNodeAt(e.Location);

                treeView1.SelectedNode = clicked_node;

                if (clicked_node.Tag.GetType() == typeof(ShGroup))
                {

                    ShGroup grp = (ShGroup)clicked_node.Tag;
                    RCM_group.Tag = clicked_node;
                    RCM_group_correct.Text = "Mark " +
                        ShGame.GetCorrectText(curScoreMethod, !grp.Correct);

                    RCM_group.Show(treeView1, e.Location);

                }
                else if (clicked_node.Tag.GetType() == typeof(ShAnswer))
                {
                    RCM_answer.Tag = clicked_node;
                    RCM_answer.Show(treeView1, e.Location);
                }

            }
        }

        private void RCM_group_set_name_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_group.Tag as TreeNode;
            if (clicked_node == null) return;
            ShGroup grp = clicked_node.Tag as ShGroup;
            if (grp == null) return;

            InputText IP = new InputText();
            IP.Text = "Group Name";
            IP.label1.Text = "Enter new group name:";
            IP.textBox1.Text = grp.Text;
            IP.StartPosition = FormStartPosition.CenterParent;

            IP.ShowDialog();

            if (IP.DialogResult == DialogResult.OK)
            {
                grp.Text = IP.textBox1.Text;

                sheep_modified = true;
                SetTextForAllTreenodes();
            }
        }

        //right click menu - mark group as (in)correct/(in)valid
        private void RCM_group_correct_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_group.Tag as TreeNode;
            if (clicked_node == null) return;
            ShGroup grp = clicked_node.Tag as ShGroup;
            if (grp == null) return;

            if (grp.Correct) grp.Correct = false;
            else grp.Correct = true;

            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        //right click menu - set bonus score for a group
        private void RCM_group_bonus_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_group.Tag as TreeNode;
            if (clicked_node == null) return;
            ShGroup grp = clicked_node.Tag as ShGroup;
            if (grp == null) return;

            var ES = new editBonus(grp.BonusType == ShGame.ShBonusType.Override, grp.GroupBonus);
            ES.Text = "Edit Group Score";
            ES.StartPosition = FormStartPosition.CenterParent;
            ES.ShowDialog();
            if (ES.DialogResult == DialogResult.OK)
            {
                try
                {
                    grp.BonusType = ES.BonusType;
                    grp.GroupBonus = ES.BonusValue;
                }
                catch { }
                sheep_modified = true;
                SetTextForAllTreenodes();
            }

        }

        //right click menu - set group name to this answer
        private void RCM_answer_use_as_group_name_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_answer.Tag as TreeNode;
            if (clicked_node == null) return;
            ShAnswer ans = clicked_node.Tag as ShAnswer;
            if (ans == null) return;

            ans.Group.Text = ans.Text;

            sheep_modified = true;
            SetTextForAllTreenodes();

        }

        //right click menu - set bonus score for this answer
        private void RCM_answer_bonus_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_answer.Tag as TreeNode;
            if (clicked_node == null) return;
            ShAnswer ans = clicked_node.Tag as ShAnswer;
            if (ans == null) return;

            var ES = new editBonus(ans.BonusType == ShGame.ShBonusType.Override, ans.AnswerBonus);
            ES.Text = "Edit Answer Score";
            ES.StartPosition = FormStartPosition.CenterParent;
            ES.ShowDialog();
            if (ES.DialogResult == DialogResult.OK)
            {
                try
                {
                    ans.BonusType = ES.BonusType;
                    ans.AnswerBonus = ES.BonusValue;
                }
                catch { }
                sheep_modified = true;
                SetTextForAllTreenodes();
            }

        }

        //right click menu - create a new group with this answer
        private void RCM_move_to_new_group_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_answer.Tag as TreeNode;
            if (clicked_node == null) return;
            ShAnswer ans = clicked_node.Tag as ShAnswer;
            if (ans == null) return;

            ShGroup newGroup = ans.Group.Question.StartNewGroup(ans.Text);
            ans.ChangeGroup(newGroup);

            TreeNode prevParent = clicked_node.Parent;
            TreeNode newParent = treeView1.Nodes.Add("b");
            newParent.Tag = newGroup;
            prevParent.Nodes.Remove(clicked_node);
            newParent.Nodes.Add(clicked_node);
            newParent.Expand();

            //if prevParent is empty, delete it
            if (prevParent.Nodes.Count == 0) prevParent.Remove();

            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        //change scoring method and set checkmarks in menu
        private void SetScoringMethod(ShMethod method)
        {
            curScoreMethod = method;
            if (sg != null) sg.Method = method; 

            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        //generate post for answers
        private void copyAnswersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sg == null || sg.Players.Count == 0 || sg.Questions.Count == 0)
            {
                Clipboard.SetText("Either no questions or no players loaded.");
                return;
            }

            bool FormattedText = OutputFormattedTextToolStripMenuItem.Checked;
            bool UnformattedText = OutputUnformattedTextToolStripMenuItem.Checked;
            bool TableText = !(FormattedText || UnformattedText);

            string OpenBold = UnformattedText ? "" : "[B]";
            string CloseBold = UnformattedText ? "" : "[/B]";

            string txt = OpenBold + "Question " + (cur_q_index + 1).ToString() + ": " +
                sg.Questions[cur_q_index].Text + CloseBold + Environment.NewLine + Environment.NewLine;

            #region sheepscoreoutput
            List<ShGroup> validGroups = new List<ShGroup>(); //valid/correct groups
            List<ShGroup> validAces = new List<ShGroup>();   //valid/correct aces
            List<ShGroup> invalidGroups = new List<ShGroup>(); //invalid/incorrect groups

            if (curScoreMethod == ShMethod.Sheep
                || curScoreMethod == ShMethod.Heep || curScoreMethod == ShMethod.Heep15 || curScoreMethod == ShMethod.Heep2
                || curScoreMethod == ShMethod.PeehsDM || curScoreMethod == ShMethod.PeehsFB || curScoreMethod == ShMethod.PeehsHybrid)
            {
                validGroups = (from g in sg.Questions[cur_q_index].Groups
                               where (g.Answers.Count > 1) && (g.Correct)
                               orderby -g.Answers.Count
                               select g).ToList();

                validAces = (from g in sg.Questions[cur_q_index].Groups
                             where (g.Answers.Count == 1) && (g.Correct)
                             select g).ToList();

                invalidGroups = (from g in sg.Questions[cur_q_index].Groups
                                 where !g.Correct
                                 select g).ToList();
            }
            else if (curScoreMethod == ShMethod.Kangaroo)
            {
                validGroups = (from g in sg.Questions[cur_q_index].Groups
                               where (g.Answers.Count > 1) && (!g.Correct)
                               orderby -g.Answers.Count
                               select g).ToList();

                validAces = (from g in sg.Questions[cur_q_index].Groups
                             where (g.Answers.Count == 1) && (!g.Correct)
                             select g).ToList();

                invalidGroups = (from g in sg.Questions[cur_q_index].Groups
                                 where g.Correct
                                 select g).ToList();
            }
            else if (curScoreMethod == ShMethod.Manual)
            {
                validGroups = (from g in sg.Questions[cur_q_index].Groups
                               where g.Correct
                               orderby -g.Answers.Count
                               select g).ToList();

                invalidGroups = (from g in sg.Questions[cur_q_index].Groups
                                 where !g.Correct
                                 select g).ToList();
            }

            foreach (ShGroup grp in validGroups)
            {
                txt += OpenBold + grp.Text + " - " +
                    GetScoreOutputText(grp.GetScore(false), grp.BonusType, grp.GroupBonus, curScoreMethod) +
                    CloseBold + Environment.NewLine;

                foreach (ShAnswer ans in grp.Answers.OrderBy(a => a.Player.Name))
                {
                    txt += ans.Player.Name + " " + 
                        OpenBold + GetBonusOutputText(ans.BonusType, ans.AnswerBonus, curScoreMethod) + CloseBold +
                        Environment.NewLine;
                }
                txt += Environment.NewLine;
            }

            if (validAces.Count > 0)
            {
                txt += OpenBold + "ACES - " + validAces[0].GetScore(false).ToString("0.#####") + ":" + CloseBold +
                    Environment.NewLine + Environment.NewLine;
            }

            foreach (ShGroup grp in validAces)
            {
                var ans = grp.Answers[0];
                string bonus_text;
                if (ans.BonusType == ShGame.ShBonusType.Override)
                {
                    bonus_text = GetBonusOutputText(ans.BonusType, ans.AnswerBonus, curScoreMethod);
                }
                else
                {
                    bonus_text = GetBonusOutputText(grp.BonusType, grp.GroupBonus, curScoreMethod) + " " +
                        GetBonusOutputText(ans.BonusType, ans.AnswerBonus, curScoreMethod);
                } 
                txt += OpenBold + grp.Text + CloseBold + " - " + grp.Answers[0].Player.Name + " " +
                        OpenBold + bonus_text + CloseBold
                        + Environment.NewLine;
            }

            if (invalidGroups.Count > 0)
            {
                txt += Environment.NewLine + OpenBold + ShGame.GetCorrectText(curScoreMethod,
                    (curScoreMethod == ShMethod.Kangaroo ? true : false)).ToUpper() + "S" + " - "
                    + invalidGroups[0].GetScore(false).ToString("0.#####") + ":" + CloseBold + Environment.NewLine + Environment.NewLine;
            }
            foreach (ShGroup grp in invalidGroups.OrderBy(g => g.Text))
            {
                foreach (ShAnswer ans in grp.Answers.OrderBy(a => a.Player.Name))
                {
                    string bonus_text;
                    if (ans.BonusType == ShGame.ShBonusType.Override)
                    {
                        bonus_text = GetBonusOutputText(ans.BonusType, ans.AnswerBonus, curScoreMethod);
                    }
                    else
                    {
                        bonus_text = GetBonusOutputText(grp.BonusType, grp.GroupBonus, curScoreMethod) + " " +
                            GetBonusOutputText(ans.BonusType, ans.AnswerBonus, curScoreMethod);
                    }

                    txt += OpenBold + ans.Text + CloseBold + " - " + ans.Player.Name + " " +
                            OpenBold + bonus_text + CloseBold
                            + Environment.NewLine;
                }
            }

            #endregion

            Clipboard.SetText(txt);

        }

        //methods for getting score
        private string GetScoreOutputText(decimal score, ShGame.ShBonusType bonus_type, decimal bonus, ShMethod method)
        {
            if (method == ShMethod.Manual)
            {
                if (bonus_type == ShGame.ShBonusType.None) return "0";
                else  return bonus.ToString("0.#####");
            }
            else
                return score.ToString("0.#####") + (bonus == 0 ? "" : " " + GetBonusOutputText(bonus_type, bonus, method));
        }

        private string GetBonusOutputText(ShGame.ShBonusType bonus_type, decimal bonus, ShMethod method)
        {
            if (bonus_type == ShGame.ShBonusType.None)
                return "";

            bool FormattedText = OutputFormattedTextToolStripMenuItem.Checked;
            bool UnformattedText = OutputUnformattedTextToolStripMenuItem.Checked;
            bool TableText = !(FormattedText || UnformattedText);

            string bonuscolor = "Blue";

            if (bonus_type == ShGame.ShBonusType.Override) bonuscolor = "Purple";
             
            if (bonus_type == ShGame.ShBonusType.Add && 
                ((bonus < 0) ^ (method == ShMethod.PeehsFB || method == ShMethod.PeehsDM || method == ShMethod.PeehsHybrid)))
            { bonuscolor = "Red"; }

            string bonusOpen = UnformattedText ? "" : "[COLOR=\"" + bonuscolor + "\"]";
            string bonusClose = UnformattedText ? "" : "[/COLOR]";

            string bonusPfx = "";
            if (bonus_type == ShGame.ShBonusType.Override) bonusPfx = "=";
            else if (bonus > 0) bonusPfx = "+";

            return bonusOpen + "(" + bonusPfx + bonus.ToString("0.#####") + ")" + bonusClose;
        }

        //cgenerate post for score totals up to and including this question
        private void copyScoresUpToThisQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sg == null || sg.Players.Count == 0 || sg.Questions.Count == 0)
            {
                Clipboard.SetText("Either no questions or no players loaded.");
                return;
            }

            Dictionary<ShPlayer, decimal> curScores =
                sg.Questions[cur_q_index].ScoreUpTo(true);

            foreach (ShPlayer plr in sg.Players)
            {
                if (!curScores.ContainsKey(plr))
                {
                    Clipboard.SetText("ERROR");
                    return;
                }
                curScores[plr] += plr.StartScore;
            }

            decimal order_mult = ShGame.IsScoreDescending(curScoreMethod) ? 1 : -1;
            string txt = "";

            bool FormattedText = OutputFormattedTextToolStripMenuItem.Checked;
            bool UnformattedText = OutputUnformattedTextToolStripMenuItem.Checked;
            bool TableText = !(FormattedText || UnformattedText);

            if (FormattedText || TableText) txt += "[b]";

            txt += "Scores after question " + (cur_q_index + 1).ToString() + ":";

            if (FormattedText || TableText) txt += "[/b]";

            txt += Environment.NewLine + Environment.NewLine;

            var sList = from p in sg.Players orderby (curScores[p] * order_mult) descending select p;

            if (TableText)
            {
                txt += "[table=head]Player\tScore" + Environment.NewLine +
                    string.Join(Environment.NewLine, sList.Select(p => p.Name + "\t" + curScores[p].ToString("0.#####")).ToArray())
                    + "[/table]" + Environment.NewLine;
            }
            else
            {
                txt += string.Join(Environment.NewLine, sList.Select
                    (p => curScores[p].ToString("0.#####") + " - " + p.Name).ToArray())
                    + Environment.NewLine;
            }

            Clipboard.SetText(txt);

        }

        private void copyAllScoresUpToThisQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sg == null || sg.Players.Count == 0 || sg.Questions.Count == 0)
            {
                Clipboard.SetText("Either no questions or no players loaded.");
                return;
            }

            //only for tables
            if (!outputTableToolStripMenuItem.Checked)
            {
                Clipboard.SetText("List all scores only available for Table Formatting");
                return;
            }

            decimal order_mult = ShGame.IsScoreDescending(curScoreMethod) ? -1 : 1;

            var curScores = new Dictionary<ShPlayer, List<decimal>>();
            foreach (var plr in sg.Players)
            {
                curScores.Add(plr, new List<decimal>());
            }
            for (int iQues = 0; iQues <= cur_q_index; iQues++)
            {
                var q_scores = sg.Questions[iQues].Scores(true);
                foreach (var plr_scores in curScores)
                {
                    if (!q_scores.ContainsKey(plr_scores.Key))
                    {
                        Clipboard.SetText("ERROR: players list out of sync q=" + iQues.ToString() + " p=" + plr_scores.Key.GameIndex.ToString());
                        return;
                    }
                    plr_scores.Value.Add(q_scores[plr_scores.Key]);
                }
            }
            if (curScores.Any(ps => ps.Value.Count != (1 + cur_q_index)))
            {
                Clipboard.SetText("ERROR: answers list out of sync");
                return;
            }

            var txt = "[b]Scores after question " + (cur_q_index + 1).ToString() + ":[/b]" + Environment.NewLine + Environment.NewLine +
              "[table=head]Player\tTotal";

            var any_starting_scores = sg.Players.Any(p => p.StartScore != 0);

            if (any_starting_scores) txt += "\tStart";

            for (int iQues = 0; iQues <= cur_q_index; iQues++)
            {
                txt += "\tQ" + (iQues + 1).ToString();
            }

            foreach (var plr_scores in curScores.OrderBy(ps => ps.Value.Sum() * order_mult))
            {
                var plr = plr_scores.Key;
                var scrs = plr_scores.Value;
                string start_score_string = any_starting_scores ? "\t" + plr.StartScore.ToString("0.#####") : "";
                txt += Environment.NewLine + plr.Name + "\t[b]" + (plr.StartScore + scrs.Sum()).ToString("0.#####") + "[/b]" +
                    start_score_string + "\t" +
                    string.Join("\t", scrs.Select(s => s.ToString("0.#####")).ToArray());                
            }
            txt += "[/table]" + Environment.NewLine;
            Clipboard.SetText(txt);
        }


        //generate post for player list
        private void copyPlayerListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sg == null || sg.Players.Count == 0 || sg.Questions.Count == 0)
            {
                Clipboard.SetText("Either no questions or no players loaded.");
                return;
            }

            bool FormattedText = OutputFormattedTextToolStripMenuItem.Checked;
            bool UnformattedText = OutputUnformattedTextToolStripMenuItem.Checked;
            bool TableText = !(FormattedText || UnformattedText);

            bool any_starting_scores = sg.Players.Any(p => p.StartScore != 0);

            string ss_pre = (FormattedText || TableText) ? "[b][color=green]" : "";
            string ss_post = (FormattedText || TableText) ? "[/color][/b]" : "";

            string start_score_string = any_starting_scores ? (ss_pre + " (starting score)" + ss_post) : ""; 

            string txt = (UnformattedText ? "" : "[b]")
                + sg.Players.Count.ToString() + " players" + start_score_string + ":" + 
                (UnformattedText ? "" : "[/b]")
                + Environment.NewLine + Environment.NewLine +
                string.Join(Environment.NewLine, sg.Players.OrderBy(p=>p.Name)
                .Select(p=>p.Name + (p.StartScore != 0 ? " " + ss_pre + "(" + p.StartScore + ")" + ss_post : "") ));

            Clipboard.SetText(txt);
        }

        //scoring type option menu items
        private void kangarooToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.Kangaroo);
        }

        private void sheepToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.Sheep);
        }

        private void scoringMethod1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.PeehsDM);
        }

        private void scoringMethod2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.PeehsFB);
        }

        private void PeehsHybridMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.PeehsHybrid);
        }

        private void HeepBonus2MenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.Heep2);
        }

        private void HeepBonus15MenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.Heep15);
        }

        private void HeepNoBonusMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.Heep);
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShMethod.Manual);
        }

        //save reveal file
        private void saveSheepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveSheepDialog();
        }

        private void ShowSaveSheepDialog()
        {
            saveFileDialog1.Title = "Save Sheep Scoring File";
            saveFileDialog1.Filter = "Sheep Score 2017 File|*.sheep17";
            saveFileDialog1.OverwritePrompt = true;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                using (XmlWriter xw = XmlWriter.Create(saveFileDialog1.FileName, xws))
                {
                    sg.WriteToXML(xw);
                }
            }
            catch
            {
                MessageBox.Show("Error writing to " + saveFileDialog1.FileName);
                return;
            }

            sheep_modified = false;
            MessageBox.Show("Successfully wrote " + saveFileDialog1.FileName);
        }

        private void ShowLoadSheepDialog()
        {
            openFileDialog1.Title = "Load Sheep Scoring File";
            openFileDialog1.Filter = "Sheep Score 2017 File|*.sheep17";
            openFileDialog1.CheckFileExists = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            //temp game object - don't overwrite game in memory until we know load worked
            ShGame sg2 = new ShGame();

            try
            {
                XmlReaderSettings xrs = new XmlReaderSettings();
                using (XmlReader xr = XmlReader.Create(openFileDialog1.FileName))
                {
                    sg2.ReadFromXML(xr);
                }
            }
            catch
            {
                MessageBox.Show("Error reading from " + openFileDialog1.FileName);
                return;
            }

            sg = sg2;
            curScoreMethod = sg.Method;
            sheep_modified = false;

        }

        //load reveal file
        private void loadSheepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckToSave();
            ShowLoadSheepDialog();
            redrawTreeView();
        }

        //about window
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MessageBox.Show(Application.ProductName + " v" + Application.ProductVersion
                + Environment.NewLine
                + "by DarkMagus" + Environment.NewLine
                + Environment.NewLine
                + "Visit twofive.ca/sheep for more info");
        }

        //show help
        private void hepToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            try
            {
                System.Diagnostics.Process.Start("help.html");
                //   System.Diagnostics.Process.Start("http://twofive.ca/sheep");
            }
            catch
            {
                MessageBox.Show("Please visit twofive.ca/sheep for help");
            }
        }

        //closing the program - check if we want to save
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult d = CheckToSave();

            if (d == DialogResult.Cancel)
                e.Cancel = true;
        }

        //check if sheep is modified, ask user if he wants to save
        public DialogResult CheckToSave()
        {
            DialogResult d;
            if (sheep_modified)
            {
                d = MessageBox.Show("Do you want to save the current reveal?",
                    Application.ProductName, MessageBoxButtons.YesNoCancel);

                if (d == DialogResult.Yes)
                    ShowSaveSheepDialog();

            }
            else
            { d = DialogResult.Ignore; }

            return d;
        }

        //start new reveal
        private void newSheepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult d = CheckToSave();

            if (d == DialogResult.Cancel)
                return;

            sg = new ShGame();
            redrawTreeView();
            sheep_modified = false;
        }

        private void outputTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputTableToolStripMenuItem.Checked = true;
            OutputFormattedTextToolStripMenuItem.Checked = false;
            OutputUnformattedTextToolStripMenuItem.Checked = false;
        }

        private void OutputFormattedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputTableToolStripMenuItem.Checked = false;
            OutputFormattedTextToolStripMenuItem.Checked = true;
            OutputUnformattedTextToolStripMenuItem.Checked = false;
        }

        private void OutputUnformattedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputTableToolStripMenuItem.Checked = false;
            OutputFormattedTextToolStripMenuItem.Checked = false;
            OutputUnformattedTextToolStripMenuItem.Checked = true;
        }

        private void roundingToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            noRoundingToolStripMenuItem.Checked = sg.Rounding == ShGame.ShRoundingType.None;
            roundUpToolStripMenuItem.Checked = sg.Rounding == ShGame.ShRoundingType.Up;
            roundDownToolStripMenuItem.Checked = sg.Rounding == ShGame.ShRoundingType.Down;
            roundNearestToolStripMenuItem.Checked = sg.Rounding == ShGame.ShRoundingType.Nearest;
        }

        private void noRoundingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sg.Rounding = ShGame.ShRoundingType.None;
            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        private void roundUpToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            sg.Rounding = ShGame.ShRoundingType.Up;
            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        private void roundDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sg.Rounding = ShGame.ShRoundingType.Down;
            sheep_modified = true;
            SetTextForAllTreenodes();

        }

        private void roundNearestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sg.Rounding = ShGame.ShRoundingType.Nearest;
            sheep_modified = true;
            SetTextForAllTreenodes();

        }

        private void scoringToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            sheepToolStripMenuItem1.Checked = sg.Method == ShMethod.Sheep;
            PeehsDMMenuItem.Checked = sg.Method == ShMethod.PeehsDM;
            PeehsFBMenuItem.Checked = sg.Method == ShMethod.PeehsFB;
            PeehsHybridMenuItem.Checked = sg.Method == ShMethod.PeehsHybrid;
            manualToolStripMenuItem.Checked = sg.Method == ShMethod.Manual;
            kangarooToolStripMenuItem.Checked = sg.Method == ShMethod.Kangaroo;
            HeepBonus2MenuItem.Checked = sg.Method == ShMethod.Heep2;
            HeepBonus15MenuItem.Checked = sg.Method == ShMethod.Heep15;
            HeepNoBonusMenuItem.Checked = sg.Method == ShMethod.Heep;
        }
    }
}