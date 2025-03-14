namespace VignaTeste.MinimalAPI.Models
{
    public class GroqResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }

        public Choice[] Choices { get; set; }
    }
}
