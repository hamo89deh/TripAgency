namespace TripAgency.Dtos.City

{
    public class AddCityDto:CityDto
    {

    }
    public class EditCityDto : CityDto
    {
        public int Id { get; set; }
    }
    public class ResultCityDto : CityDto
    {
        public int Id { get; set; }
    }
    public class CityDto
    {   
        public string Name { get; set; }
    }
}
