﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace sheepscore12
{
    public class ShGame
    {
        public enum ShMethod
        {
            Sheep, PeehsDM, PeehsFB, PeehsHybrid, Heep, Heep15, Heep2, Kangaroo, Manual
        }

        public enum ShBonusType
        {
            None, Add, Override
        }

        public enum ShRoundingType
        {
            None, Up, Down, Nearest
        }

        public static string GetCorrectText(ShMethod method, bool correct)
        {
            if (method == ShMethod.Sheep
                || method == ShMethod.Heep || method == ShMethod.Heep15 || method == ShMethod.Heep2
                || method == ShMethod.Manual)
                return (correct ? "Valid" : "Invalid");
            else
                return (correct ? "Correct" : "Incorrect");
        }

        public static bool IsScoreDescending(ShMethod method)
        {
            if (method == ShMethod.PeehsDM || method == ShMethod.PeehsFB || method == ShMethod.PeehsHybrid)
                return false;
            else
                return true;
        }

        public class ShQuestion
        {
            public string Text { get; set; }
            public List<ShGroup> Groups; //actual groups

            public int GameIndex
            {
                get
                {
                    return _Game.Questions.IndexOf(this);
                }
            }

            public ShGame Game { get { return _Game; } }
            private ShGame _Game; //reference to Player in SheepGame.Players

            //constructor

            public ShQuestion(ShGame ref_game, string new_text)
            {
                _Game = ref_game;
                Text = new_text;
                Groups = new List<ShGroup>(ref_game.Players.Count);
            }

            public ShGroup StartNewGroup(string new_text)
            {
                ShGroup newGrp = new ShGroup(this, new_text);
                this.Groups.Add(newGrp);
                return newGrp;
            }

            //delete unused groups
            public void SyncGroups()
            {
                Groups.RemoveAll((ShGroup shg) => (shg.Answers.Count == 0));
            }

            //get all answers in all groups
            public List<ShAnswer> GetAllAnswers()
            {
                List<ShAnswer> all_answers = new List<ShAnswer>();
                foreach (ShGroup grp in Groups)
                {
                    all_answers.AddRange(grp.Answers);
                }
                return all_answers;
            }

            //returns list of scores for this question
            //SCORING METHODS
            //Sheep:    each player gets total answers in his group as his score
            //Peehs1:   incorrects = 1.5 * highest correct score
            //Peehs2:   incorrects = highest correct score + 0.5 * number of distinct
            //          correct answers
            //Heep:     highest score gets 0, 2nd highest get doubled
            //Kangaroo: must be incorrect; correct answers get 0
            public Dictionary<ShPlayer, decimal> Scores(bool include_bonus)
            {
                Dictionary<ShPlayer, decimal> curScore = new Dictionary<ShPlayer, decimal>();

                foreach (ShPlayer plr in _Game.Players)
                { curScore.Add(plr, 0); }

                //int[] curScore = new int[_Game.Players.Count];

                // for (int iPlayer = 0; iPlayer < _Game.Players.Count; iPlayer++)
                // { curScore[iPlayer] = 0; }

                if (_Game.Players.Count == 0 || _Game.Questions.Count == 0) return curScore;

                int highest_score = 0;
                int second_highest_score = 0;
                int num_distinct_correct = 0;
                foreach (ShGroup grp in this.Groups)
                {
                    //each player's score is num answers in that group
                    foreach (ShAnswer ans in grp.Answers)
                    {
                        if (!curScore.ContainsKey(ans.Player))
                            continue;

                        curScore[ans.Player] = grp.Answers.Count;

                    }

                    //for peehs/heep
                    if (grp.Correct)
                    {
                        num_distinct_correct++;

                        if (grp.Answers.Count > highest_score)
                        {
                            second_highest_score = highest_score;
                            highest_score = grp.Answers.Count;
                        }
                        else if (grp.Answers.Count > second_highest_score &&
                                grp.Answers.Count < highest_score)
                        {
                            second_highest_score = grp.Answers.Count;
                        }

                    }

                }

                //apply special scores depending on scoring method
                foreach (ShGroup grp in this.Groups)
                {
                    foreach (ShAnswer ans in grp.Answers)
                    {
                        if (!curScore.ContainsKey(ans.Player))
                        { continue; }

                        switch (_Game.Method)
                        {

                            case ShMethod.Sheep:
                                if (!grp.Correct)
                                {
                                    curScore[ans.Player] = 0m;
                                }
                                //incorrect means invalid for sheep
                                break;
                            case ShMethod.PeehsDM:
                                //incorrect -> 1.5*sheep
                                if (!grp.Correct)
                                {
                                    curScore[ans.Player] = 1.5m * (decimal)highest_score;
                                }
                                break;
                            case ShMethod.PeehsFB:
                                //incorrect -> sheep + 0.5*distinct
                                if (!grp.Correct)
                                {
                                    curScore[ans.Player] = highest_score + 0.5m * (decimal)num_distinct_correct;
                                }
                                break;
                            case ShMethod.PeehsHybrid:
                                // (DM + FB) / 2
                                if (!grp.Correct)
                                {
                                    curScore[ans.Player] = 1.25m * (decimal)highest_score + 0.25m * (decimal)num_distinct_correct;
                                }
                                break;
                            case ShMethod.Heep:
                            case ShMethod.Heep15:
                            case ShMethod.Heep2:
                                //highest -> 0, second highest *= 2
                                if (curScore[ans.Player] == highest_score ||
                                    !grp.Correct)
                                {
                                    curScore[ans.Player] = 0m;
                                    //incorrect means invalid
                                }
                                else if (curScore[ans.Player] == second_highest_score)
                                {
                                    if (_Game.Method == ShMethod.Heep15)
                                        curScore[ans.Player] *= 1.5m;
                                    else if (_Game.Method == ShMethod.Heep2)
                                        curScore[ans.Player] *= 2m;
                                }
                                break;
                            case ShMethod.Kangaroo:
                                //correct -> 0
                                if (grp.Correct)
                                {
                                    curScore[ans.Player] = 0m;
                                }
                                break;
                            case ShMethod.Manual:
                                curScore[ans.Player] = 0m;
                                break;

                        }

                        //apply rounding
                        switch (_Game.Rounding)
                        {
                            case ShRoundingType.Up:
                                curScore[ans.Player] = Math.Ceiling(curScore[ans.Player]);
                                break;
                            case ShRoundingType.Down:
                                curScore[ans.Player] = Math.Floor(curScore[ans.Player]);
                                break;
                            case ShRoundingType.Nearest:
                                curScore[ans.Player] = Math.Round(curScore[ans.Player]);
                                break;
                        }

                        //apply player & group bonuses
                        if (include_bonus)
                        {
                            var tempScore = curScore[ans.Player];

                            if (grp.BonusType == ShBonusType.Override) tempScore = grp.GroupBonus;
                            else if (grp.BonusType == ShBonusType.Add) tempScore += grp.GroupBonus;

                            if (ans.BonusType == ShBonusType.Override) tempScore = ans.AnswerBonus;
                            else if (ans.BonusType == ShBonusType.Add) tempScore += ans.AnswerBonus;

                            curScore[ans.Player] = tempScore;
                        }
                    }
                }

                return curScore;
            }

            //returns list of total scores after this question
            public Dictionary<ShPlayer, decimal> ScoreUpTo(bool include_bonus)
            {
                Dictionary<ShPlayer, decimal> curScore = new Dictionary<ShPlayer, decimal>();
                Dictionary<ShPlayer, decimal> nextScore;

                foreach (ShPlayer plr in _Game.Players)
                    curScore.Add(plr, 0);

                foreach (ShQuestion que in _Game.Questions.Where(
                    q => q.GameIndex <= this.GameIndex))
                {
                    nextScore = que.Scores(include_bonus);

                    foreach (KeyValuePair<ShPlayer, decimal> k in nextScore)
                    {
                        if (curScore.ContainsKey(k.Key))
                        { curScore[k.Key] += k.Value; }
                    }
                }

                return curScore;
            }

        }

        public class ShPlayer
        {
            public string Name { get; set; }

            public int GameIndex
            {
                get
                {
                    return _Game.Players.IndexOf(this);
                }
            }

            public decimal StartScore { get; set; }

            public ShGame Game { get { return _Game; } }
            private ShGame _Game; //reference to Game

            public List<ShAnswer> Answers; //references

            //constructor. answers start blank
            public ShPlayer(ShGame ref_game, string player_name, decimal start_score = 0)
            {
                _Game = ref_game;
                Name = player_name;
                Answers = new List<ShAnswer>(ref_game.Questions.Count);
                StartScore = start_score;
            }
            //destructor. remove all this player's answers from groups
            ~ShPlayer()
            {
                foreach (ShAnswer ans in Answers)
                {
                    ans.Group.Answers.Remove(ans);
                }

            }
        }

        public class ShGroup
        {

            public string Text { get; set; }
            public bool Correct { get; set; }
            public decimal GroupBonus { get; set; }
            public ShBonusType BonusType { get; set; }

            public List<ShAnswer> Answers; //actual answers

            public ShQuestion Question { get { return _Question; } }
            private ShQuestion _Question; //reference to question

            //constructor
            //declares with an empty list for Answers
            public ShGroup(ShQuestion ref_question, string new_text)
            {
                Text = new_text;
                Correct = true;
                GroupBonus = 0;

                _Question = ref_question;
                Answers = new List<ShAnswer>(ref_question.Game.Players.Count);
            }

            //moves all answers to ref_group and deletes itself
            public void MergeToGroup(ShGroup ref_group)
            {
                if (ref_group == this)
                {
                   // throw new Exception("Trying to merge a group to itself");
                     return;
                }

                while (Answers.Count != 0)
                {
                    Answers.First().ChangeGroup(ref_group);
                }

                _Question.Groups.Remove(this);
            }

            //gets score for this group
            public decimal GetScore(bool include_bonus)
            {
                if (this.Answers.Count == 0)
                    return 0;
                else
                {
                    try
                    {
                        //not using bonus from _Question.Scores because we don't want to 
                        //include individual player bonuses
                        var baseScore = _Question.Scores(false)[this.Answers[0].Player];
                        
                        if (include_bonus)
                        {
                            if (this.BonusType == ShBonusType.Override)
                                return this.GroupBonus;
                            else if (this.BonusType == ShBonusType.Add)
                                return this.GroupBonus + baseScore;
                            else
                                return baseScore;
                        }
                        else
                        {
                            return baseScore;
                        }                        
                    }
                    catch
                    {
                        return 0;
                    }
                }

            }

            //remove player references for answers that are getting deleted
            ~ShGroup()
            {
                foreach (ShAnswer ans in Answers)
                {
                    ans.Player.Answers.Remove(ans);
                }
            }

        }

        public class ShAnswer

        {
            //must always have an associated group and player

            public string Text { get; set; }
            public decimal AnswerBonus { get; set; }
            public ShBonusType BonusType { get; set; }

            public ShGroup Group { get { return _Group; } }
            private ShGroup _Group; //reference to Group in SheepQuestion.Groups

            public ShPlayer Player { get { return _Player; } }
            private ShPlayer _Player; //reference to Player in SheepGame.Players

            //constructor
            public ShAnswer(ShGroup ref_group, ShPlayer ref_player, string new_text)
            {
                this.Text = new_text;
                this.AnswerBonus = 0;
                this._Group = ref_group;
                this._Player = ref_player;
            }

            //change group of answer
            //use GroupSync() after to remove empty groups
            public void ChangeGroup(ShGroup ref_group)
            {
                if (ref_group == this._Group) return;
                if (this._Group.Question != ref_group.Question)
                    throw new Exception("Moving an answer to a group in a different question.");

                ShGroup oldGroup = this._Group;
                //add to new group
                this._Group = ref_group;
                ref_group.Answers.Add(this);

                //remove it from old group
                oldGroup.Answers.Remove(this);
                if (oldGroup.Answers.Count == 0)
                {
                    //delete old group if it's empty
                    oldGroup.Question.Groups.Remove(oldGroup);
                }
            }

            //creates new group and moves answer to it
            public void StartNewGroup()
            {
                ShGroup oldGroup = this._Group;
                ShGroup newGroup = new ShGroup(_Group.Question, Text);
                oldGroup.Question.Groups.Add(newGroup);
                this._Group = newGroup;
                newGroup.Answers.Add(this);
                oldGroup.Answers.Remove(this);
                if (oldGroup.Answers.Count == 0)
                {
                    //delete old group if it's empty
                    oldGroup.Question.Groups.Remove(oldGroup);
                }
            }

        }

        public List<ShQuestion> Questions;
        public List<ShPlayer> Players;

        public ShMethod Method;
        public ShRoundingType Rounding;

        //constructort 1
        public ShGame()
        {
            Questions = new List<ShQuestion>();
            Players = new List<ShPlayer>();
            Method = ShMethod.Sheep;
            Rounding = ShRoundingType.None;
        }

#if false
        //Constructor2
        //initialize with an array of questions, players, answers
        public ShGame(string[] new_questions, string[] new_players, string[,] new_answers)
        {
            if (new_answers.GetLength(0) != new_questions.Length
                 || new_answers.GetLength(1) != new_players.Length)
                throw new Exception("Answer list must be size [num questions, num players]");

            Questions = new List<ShQuestion>(new_questions.Length);
            Players = new List<ShPlayer>(new_players.Length);

            Questions.AddRange(new_questions.Select(txt => new ShQuestion(this, txt)));
            Players.AddRange(new_players.Select(txt => new ShPlayer(this, txt)));

            for (int iques = 0; iques < new_questions.Length; iques++)
            {
                for (int iplayer = 0; iplayer < new_players.Length; iplayer++)
                {
                    ShGroup new_group = new ShGroup(Questions[iques], new_answers[iques, iplayer]);
                    Questions[iques].Groups.Add(new_group);

                    ShAnswer new_answer = new ShAnswer(new_group, Players[iplayer], new_answers[iques, iplayer]);
                    new_group.Answers.Add(new_answer);
                    Players[iplayer].Answers.Add(new_answer);
                }
            }

        }
#endif

        //destructor
        ~ShGame()
        {
            Players.Clear();
            Questions.Clear();
        }

        //nicely delete a question
        public void NiceDeleteQuestion(ShQuestion que)
        {
            foreach (ShGroup grp in que.Groups)
            {
                foreach (ShAnswer ans in grp.Answers)
                {
                    ans.Player.Answers.Remove(ans);
                }
                grp.Answers.Clear();
            }
            que.Groups.Clear();
            Questions.Remove(que);
        }

        //nicely delete a player
        public void NiceDeletePlayer(ShPlayer ply)
        {
            foreach (ShAnswer ans in ply.Answers)
            {
                //remove answer from group
                ans.Group.Answers.Remove(ans);
                //delete group if empty
                if (ans.Group.Answers.Count == 0)
                {
                    ans.Group.Question.Groups.Remove(ans.Group);
                }

            }
            ply.Answers.Clear();
            Players.Remove(ply);
        }

        //nicely add a question, giving each player a blank answer
        //and making one group
        public ShQuestion NiceAddQuestion(string qtxt)
        {
            ShQuestion newQ = new ShQuestion(this, qtxt);
            Questions.Add(newQ);
            if (Players.Count > 0)
            {
                ShGroup newG = new ShGroup(newQ, "(blank)");
                newQ.Groups.Add(newG);
                foreach (ShPlayer ply in Players)
                {
                    ShAnswer newA = new ShAnswer(newG, ply, "(blank)");
                    ply.Answers.Add(newA);
                    newG.Answers.Add(newA);
                }
            }
            return newQ;
        }

        //nicely add a player, using new_answers
        public ShPlayer NiceAddPlayer(string new_name, string[] new_answers, decimal start_score = 0)
        {
            ShPlayer newP = new ShPlayer(this, new_name, start_score);
            Players.Add(newP);
            for (int iques = 0; iques < Questions.Count; iques++)
            {
                //get text for new answers
                string newAnsTxt = "(blank)";
                if (iques < new_answers.Length)
                    if (new_answers[iques] != "")
                        newAnsTxt = new_answers[iques];

                ShGroup newG = new ShGroup(Questions[iques], newAnsTxt);
                Questions[iques].Groups.Add(newG);
                ShAnswer newA = new ShAnswer(newG, newP, newAnsTxt);
                newG.Answers.Add(newA);
                newP.Answers.Add(newA);
            }
            return newP;
        }

        //nicely add a player, setting answers blank
        public ShPlayer NiceAddPlayer(string new_name)
        {
            ShPlayer newP = new ShPlayer(this, new_name);
            Players.Add(newP);
            for (int iques = 0; iques < Questions.Count; iques++)
            {
                ShGroup newG = new ShGroup(Questions[iques], "(blank)");
                Questions[iques].Groups.Add(newG);
                ShAnswer newA = new ShAnswer(newG, newP, "(blank)");
                newG.Answers.Add(newA);
                newP.Answers.Add(newA);
            }
            return newP;
        }

        public string PrintStuff()
        {
            string s = "Players: " + string.Join(", ", Players.
                Select(x => x.Name).ToArray()) + Environment.NewLine;

            s += "Answers by player:" + Environment.NewLine;
            for (int p = 0; p < Players.Count; p++)
                s += Players[p].Name + " :: " + string.Join(", ",
                    Players[p].Answers.Select(x => x.Text).ToArray()) + Environment.NewLine;

            for (int q = 0; q < Questions.Count; q++)
            {
                s += "--- " + Questions[q].Text + " ---" + Environment.NewLine;
                for (int g = 0; g < Questions[q].Groups.Count; g++)
                {
                    s += Questions[q].Groups[g].Text + " (" +
                        string.Join(" ", Questions[q].Groups[g].Answers.
                        Select(x => x.Player.Name).ToArray()) + ")" + Environment.NewLine;
                }

            }
            return s;
        }

        //attempts to guess groupings for ans
        public void GuessGroup(ShAnswer ans)
        {
            string anstxt = System.Text.RegularExpressions.Regex.Replace(ans.Text, "\\W", "").ToLower();

            foreach (ShGroup grp in ans.Group.Question.Groups)
            {
                if (grp == ans.Group)
                    continue;

                if (System.Text.RegularExpressions.Regex.Replace(grp.Text, "\\W", "").ToLower() == anstxt)
                {
                    ans.ChangeGroup(grp);
                    return;
                }
            }
            //didn't find any but let's try individual answers
            foreach (ShGroup grp in ans.Group.Question.Groups)
            {
                if (grp == ans.Group)
                    continue;

                foreach (ShAnswer ans2 in grp.Answers)
                {
                    if (System.Text.RegularExpressions.Regex.Replace(ans2.Text, "\\W", "").ToLower() == anstxt)
                    {
                        ans.ChangeGroup(grp);
                        return;
                    }
                }
            }

        }

        //saves all data to XmlWriter.
        public void WriteToXML(XmlWriter xw)
        {
            xw.WriteStartDocument();
            xw.WriteStartElement("SheepScore2012Game");
            xw.WriteElementString("ScoringMethod", Method.ToString());
            xw.WriteElementString("Rounding", Rounding.ToString());

            foreach (ShQuestion que in Questions)
            {
                xw.WriteStartElement("Question");
                xw.WriteAttributeString("GameIndex", que.GameIndex.ToString());
                xw.WriteString(que.Text);
                xw.WriteEndElement();
            }

            foreach (ShPlayer plr in Players)
            {
                xw.WriteStartElement("Player");
                xw.WriteAttributeString("GameIndex", plr.GameIndex.ToString());
                xw.WriteAttributeString("StartScore", plr.StartScore.ToString());
                xw.WriteString(plr.Name);
                xw.WriteEndElement();
            }

            foreach (ShQuestion que in Questions)
                foreach (ShGroup grp in que.Groups)
                {
                    xw.WriteStartElement("Group");
                    xw.WriteAttributeString("QuestionIndex", que.GameIndex.ToString());
                    xw.WriteAttributeString("GroupBonus", grp.GroupBonus.ToString());
                    xw.WriteAttributeString("BonusType", grp.BonusType.ToString());
                    xw.WriteAttributeString("Correct", grp.Correct.ToString());
                    xw.WriteElementString("Text", grp.Text);

                    foreach (ShAnswer ans in grp.Answers)
                    {
                        xw.WriteStartElement("Answer");
                        xw.WriteAttributeString("AnswerBonus", ans.AnswerBonus.ToString());
                        xw.WriteAttributeString("BonusType", ans.BonusType.ToString());
                        xw.WriteAttributeString("PlayerIndex", ans.Player.GameIndex.ToString());
                        xw.WriteString(ans.Text);
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();

                }


            xw.WriteEndElement();
            xw.WriteEndDocument();

        }

        //read data from xmlreader
        public void ReadFromXML(XmlReader xr)
        {
            Questions.Clear();
            Players.Clear();
            Method = ShMethod.Sheep;
            Rounding = ShRoundingType.None;

#region xrread

            while (xr.Read())
            {
                if (xr.IsStartElement())
                {
                    switch (xr.Name)
                    {
                        case "ScoringMethod":
                            Method = (ShMethod)Enum.Parse(typeof(ShMethod),
                                xr.ReadElementString());
                            break;
                        case "Rounding":
                            Rounding = (ShRoundingType)Enum.Parse(typeof(ShRoundingType),
                                xr.ReadElementString());
                            break;
                        case "Question":
                            int qindex = Convert.ToInt32(xr["GameIndex"]);
                            while (Questions.Count < qindex + 1)
                            { Questions.Add(new ShQuestion(this, "(blank)")); }
                            Questions[qindex].Text = xr.ReadElementString();
                            break;
                        case "Player":
                            int pindex = Convert.ToInt32(xr["GameIndex"]);
                            decimal start_score = Convert.ToDecimal(xr["StartScore"]);
                            while (Players.Count < pindex + 1)
                            { Players.Add(new ShPlayer(this, "(blank)", start_score)); }
                            Players[pindex].Name = xr.ReadElementString();
                            break;
                        case "Group":
                            //assuming that question and player have already been
                            //completely read in as they should be at the start
                            //of the xml file
                            int group_q_index = Convert.ToInt32(xr["QuestionIndex"]);
                            bool tempcorrect = Convert.ToBoolean(xr["Correct"]);
                            decimal tempgroupbonus = Convert.ToDecimal(xr["GroupBonus"]);
                            var tempgroupbonustype = (ShBonusType)Enum.Parse(typeof(ShBonusType), xr["BonusType"]);
                            ShGroup newGroup = new ShGroup(this.Questions[group_q_index], "");
                            newGroup.Correct = tempcorrect;
                            newGroup.GroupBonus = tempgroupbonus;
                            newGroup.BonusType = tempgroupbonustype;
                            Questions[group_q_index].Groups.Add(newGroup);

                            XmlReader subxr = xr.ReadSubtree();

                            while (subxr.Read())
                            {
                                if (subxr.IsStartElement())
                                {
                                    switch (subxr.Name)
                                    {
                                        case "Text":
                                            newGroup.Text = subxr.ReadElementString();
                                            break;
                                        case "Answer":
                                            int ans_p_index = Convert.ToInt32(subxr["PlayerIndex"]);
                                            decimal tempansbonus = Convert.ToDecimal(subxr["AnswerBonus"]);
                                            var tempansbonustype = (ShBonusType)Enum.Parse(typeof(ShBonusType), xr["BonusType"]);
                                            string anstext = xr.ReadElementString();
                                            ShAnswer newAns = new ShAnswer(newGroup, Players[ans_p_index], anstext);
                                            newAns.AnswerBonus = tempansbonus;
                                            newAns.BonusType = tempansbonustype;
                                            newGroup.Answers.Add(newAns);
                                            Players[ans_p_index].Answers.Add(newAns);
                                            break;
                                    }
                                }

                            }
                            break;
                    }

                }
            }
#endregion


        }



    }
}
