namespace eLearning.Models
{
    public class LicenseKey
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int Course_id { get; set; }

        public bool Used { get; set; }
    }
}
