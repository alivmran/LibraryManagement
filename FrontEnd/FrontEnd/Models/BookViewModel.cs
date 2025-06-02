namespace Library.Frontend.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int AuthorId { get; set; }
    }
}
