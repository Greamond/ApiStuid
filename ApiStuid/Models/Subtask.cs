namespace ApiStuid.Models
{
    public class Subtask
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ResponsibleId { get; set; }
        public User Responsible { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
    }
}
