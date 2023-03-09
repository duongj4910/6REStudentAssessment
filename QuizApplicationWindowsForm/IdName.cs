
namespace QuizApplicationWindowsForm
{
    class QuizAttribute
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = (value < 0) ? 0 : value; }
        }

        public string attribute { get; set; }
        public string Info = "";

        public QuizAttribute() { return; }

        public QuizAttribute(int id, string name)
        {
            this.Id = id;
            this.attribute = name;
        }
    }
}
