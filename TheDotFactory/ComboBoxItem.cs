namespace TheDotFactory
{

    // to allow mapping string/value
    struct ComboBoxItem
    {
        public string name;
        public string value;

        // ctor
        public ComboBoxItem(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        // override ToString() function
        public override string ToString()
        {
            // use name
            return this.name;
        }
    }
}
