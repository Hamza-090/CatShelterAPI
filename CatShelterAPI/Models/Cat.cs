namespace CatShelterAPI.Models
{
    public class Cat
    {
        public int CatID { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Color { get; set; }
        public bool IsAdopted { get; set; }
        public DateTime EntryDate { get; set; }
    }
}