namespace MockAPI.AppData.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? Suite { get; set; }
        public County? County { get; set; }
    }
}