using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;

namespace QuizApplicationWindowsForm
{
    public partial class FrmExam : Form
    {
        List<QuizAttribute> categoryList = new List<QuizAttribute>();
        List<Question> questionList = new List<Question>();
        List<Question> randomizedquestionList = new List<Question>();
        Question currentQuestion;
        int score;
        int difficulty { get; }

        public FrmExam(int difficulty)
        {
            InitializeComponent();
            LoadQuizData();
            InitilizeData();
            this.difficulty = difficulty;
        }

        private void BtnA_Click(object sender, EventArgs e)
        {
            bool boolean;
            boolean = CheckCorrectAnswer(0);
            CheckButton(BtnA, boolean);
            CheckOtherButton(BtnB, 1);
            CheckOtherButton(BtnC, 2);
            CheckOtherButton(BtnD, 3);
        }

        private void BtnB_Click(object sender, EventArgs e)
        {
            bool boolean;
            boolean = CheckCorrectAnswer(1);
            CheckButton(BtnB, boolean);
            CheckOtherButton(BtnA, 0);
            CheckOtherButton(BtnC, 2);
            CheckOtherButton(BtnD, 3);
        }

        private void BtnC_Click(object sender, EventArgs e)
        {
            bool boolean;
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
            categoryList.Add(new QuizAttribute(0, "Multiplication and Division"));
            categoryList.Add(new QuizAttribute(1, "Addition and Subtraction"));

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
                jsonString = System.IO.File.ReadAllText(Application.StartupPath + @"\QuizQuestions.json");
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
            currentQuestion = NextQuestion((int)cbxCategory.SelectedValue, difficulty);
            ShowQuestion();

            if (currentQuestion == null)
            {
                string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
                string FilePath = Path.Combine(executableLocation, "Information.txt");
                string name = "";
                if (File.Exists(FilePath))
                {
                    using (StreamReader reader = new StreamReader(FilePath))
                    {
                        name = reader.ReadToEnd();
                    }
                }
                MessageBox.Show("Hi !  " + name + " Your Total Score: " + score);

                txtQuestion.Text = "";

                BtnNext.Enabled = true;
            }
            else
            {
                this.currentCategory.Text = categoryList[(int)cbxCategory.SelectedValue].attribute;

                EnableButtons(true);
            }
        }
        private Question NextQuestion(int category, int difficulty)
        {
            int i, count = randomizedquestionList.Count;
            for (i = 0; i < count; i++)
            {
                if (randomizedquestionList[i].played == false && randomizedquestionList[i].category == category && randomizedquestionList[i].difficulty == difficulty)
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

        private void EnableButtons(Boolean boolean)
        {
            BtnA.Enabled = boolean;
            BtnB.Enabled = boolean;
            BtnC.Enabled = boolean;
            BtnD.Enabled = boolean;
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
