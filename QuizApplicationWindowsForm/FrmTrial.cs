using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuizApplicationWindowsForm
{
    public partial class FrmTrial : Form
    {
        List<QuizAttribute> categoryList = new List<QuizAttribute>();
        List<QuizAttribute> difficultyList = new List<QuizAttribute>();
        List<Question> questionList = new List<Question>();
        List<Question> randomizedquestionList = new List<Question>();
        Question currentQuestion;
        int score;

        public FrmTrial()
        {
            InitializeComponent();
            LoadQuizData();
            InitilizeData();
        }

        private void BtnA_Click_1(object sender, EventArgs e)
        {
            bool boolean;
            // timer.Enabled = false;
            boolean = CheckCorrectAnswer(0);
            CheckButton(BtnA, boolean);
            CheckOtherButton(BtnB, 1);
            CheckOtherButton(BtnC, 2);
            CheckOtherButton(BtnD, 3);
        }
        private void BtnB_Click_1(object sender, EventArgs e)
        {
            bool boolean;
            //timer.Enabled = false;
            boolean = CheckCorrectAnswer(1);
            CheckButton(BtnB, boolean);
            CheckOtherButton(BtnA, 0);
            CheckOtherButton(BtnC, 2);
            CheckOtherButton(BtnD, 3);
        }
        private void BtnC_Click(object sender, EventArgs e)
        {
            bool boolean;
            //timer.Enabled = false;
            boolean = CheckCorrectAnswer(2);
            CheckButton(BtnC, boolean);
            CheckOtherButton(BtnA, 0);
            CheckOtherButton(BtnB, 1);
            CheckOtherButton(BtnD, 3);
        }
        private void BtnD_Click(object sender, EventArgs e)
        {
            bool boolean;
            boolean = CheckCorrectAnswer(3);
            CheckButton(BtnD, boolean);
            CheckOtherButton(BtnA, 0);
            CheckOtherButton(BtnB, 1);
            CheckOtherButton(BtnC, 2);
        }

        private void categoryListLoad()
        {
            categoryList.Clear();
            categoryList.Add(new QuizAttribute(0, "Addition and Subtraction"));
            categoryList.Add(new QuizAttribute(1, "Multiplication and Division"));

            cbxCategory.DataSource = categoryList;
            cbxCategory.DisplayMember = "attribute";
            cbxCategory.ValueMember = "Id";
            cbxCategory.SelectedIndex = 0;
        }

        private void questionListLoad()
        {
            questionList.Clear();
            QuestionsDeserealization();
            randomizedquestionList = RandomizeList(questionList);
        }
        private void QuestionsDeserealization()
        {
            string jsonString = "";

            try
            {
                jsonString = System.IO.File.ReadAllText(Application.StartupPath + @"\TrialQuestions.json");
                questionList = JsonConvert.DeserializeObject<List<Question>>(jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadQuizData()
        {
            categoryListLoad();
            questionListLoad();
        }

        private void InitilizeData()
        {
            score = 0;
            EnableButtons(false);
            currentQuestion = null;
        }
        private void BtnNext_Click(object sender, EventArgs e)
        {
            BtnNext.Enabled = false;
            currentQuestion = NextQuestion((int)cbxCategory.SelectedValue);
            ShowQuestion();

            if (currentQuestion == null)
            {
                MessageBox.Show("Your Total Score: " + score);
                if (score >= 60 && score <= 70)
                {
                    FrmExam fx = new FrmExam(0);
                    fx.Show();
                }
                else if (score >= 70 && score <= 80)
                {
                    FrmExam fx = new FrmExam(1);
                    fx.Show();
                }
                else if (score >= 80)
                {
                    FrmExam fx = new FrmExam(2);
                    fx.Show();
                }

                txtQuestion.Text = "";

                BtnNext.Enabled = true;

                questionListLoad();
            }
            else
            {
                this.currentCategory.Text = categoryList[(int)cbxCategory.SelectedValue].attribute;

                EnableButtons(true);
            }
        }
        private void EnableButtons(Boolean enable)
        {
            BtnA.Enabled = enable;
            BtnB.Enabled = enable;
            BtnC.Enabled = enable;
            BtnD.Enabled = enable;
        }

        private Question NextQuestion(int category)
        {
            int i, count = randomizedquestionList.Count;
            for (i = 0; i < count; i++)
            {
                if (randomizedquestionList[i].played == false && 
                    randomizedquestionList[i].category == category)
                {
                    randomizedquestionList[i].played = true;
                    break;
                }
            }
            ResetButtonsColor();
            return (i < count) ? randomizedquestionList[i] : null;
        }
        public void ResetButtonsColor()
        {
            BtnA.BackColor = Color.White;
            BtnB.BackColor = Color.White;
            BtnC.BackColor = Color.White;
            BtnD.BackColor = Color.White;
        }

        private bool CheckCorrectAnswer(int answer)
        {
            bool boolean;
            if (answer == currentQuestion.correctAnswer)
            {
                score += (currentQuestion.difficulty + 1) * 10;

                boolean = true;
                BtnNext.Enabled = true;
                if (score == 1200)
                {
                    //PlayerWon();
                }
            }
            else
            {
                boolean = false;
                BtnNext.Enabled = true;
            }
            EnableButtons(false);
            //ShowScore();
            return boolean;
        }

        private void ShowQuestion()
        {
            if (currentQuestion == null)
            {
                txtQuestion.Text = "";
                BtnA.Text = "A";
                BtnB.Text = "B";
                BtnC.Text = "C";
                BtnD.Text = "D";
            }
            else
            {
                RandomizeArray(currentQuestion.answers);
                txtQuestion.Text = currentQuestion.question;
                BtnA.Text = "A. " + currentQuestion.answers[0];
                BtnB.Text = "B. " + currentQuestion.answers[1];
                BtnC.Text = "C. " + currentQuestion.answers[2];
                BtnD.Text = "D. " + currentQuestion.answers[3];
            }
        }

        public void CheckButton(Button button, bool boolean)
        {
            if (boolean == true)
            {
                button.BackColor = Color.Green;
            }
            else
            {
                button.BackColor = Color.Red;
            }
        }

        public void CheckOtherButton(Button button, int answer)
        {
            if (answer == currentQuestion.correctAnswer)
            {
                button.BackColor = Color.Green;
            }
        }

        public void ShowScore()
        {
           // lblScore.Text = "Score: " + score.ToString();
        }

        private void FrmTrial_Load(object sender, EventArgs e)
        {
            //RunSection(0);
        }

        public void RandomizeArray(string[] array)
        {
            Random rnd = new Random();
            string corAnswer = currentQuestion.answers[currentQuestion.correctAnswer];
            for (int i = 0; i < array.Length - 1; i++)
            {
                int j = i + rnd.Next(array.Length - i);
                var tmp = array[j];
                array[j] = array[i];
                array[i] = tmp;
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (corAnswer == array[i])
                {
                    currentQuestion.correctAnswer = i;
                }
            }
        }
        public List<T> RandomizeList<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            Random rnd = new Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count);
                randomizedList.Add(list[index]);
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        private void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cbxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
