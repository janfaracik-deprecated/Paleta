namespace Paleta.Models
{
    public class ExportItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Tags { get; set; }

        public override string ToString()
        {
            return Name + " " +
                   Value + " " +
                   Tags;
        }
    }
}