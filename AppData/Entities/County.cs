namespace MockAPI.AppData.Entities
{
    public class County
    {
        public int Id { get; set; }
        public int DataId { get; set; }
        public required string CountyName { get; set; }
        public required string StateId { get; set; }
        public required string City { get; set; }        
        public required string Lat { get; set; }
        public required string Lng { get; set; }
        public int Population { get; set; }
        public float Density { get; set; }
        public required string Zips { get; set; }
    }
}