namespace MockAPI.AppData.Entities
{
    public class City
    {
        public int Id { get; set; }
        public required string CityName { get; set; }
        public required string StateId { get; set; }        
    }
}