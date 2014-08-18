namespace simple_filter_mixer.DataModel
{
    public class ConstructorObject
    {
        private string name;
        private object value;
        private object min;
        private object max;

        public object Max
        {
            get { return max; }
            set { max = value; }
        }

        public object Min
        {
            get { return min; }
            set { min = value; }
        }

        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
