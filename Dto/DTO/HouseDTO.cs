namespace MrPill.DTOs.DTOs;

public class HouseDTO
{
    public int Id { get; set; }
    public string? FamilyName { get; set; }
    public int? ManagerId { get; set; }

    public class HouseDTOBuilder
    {
        private HouseDTO _houseDTO;

        public HouseDTOBuilder()
        {
            _houseDTO = new HouseDTO();
        }

        public HouseDTOBuilder WithId(int id)
        {
            _houseDTO.Id = id;
            return this;
        }

        public HouseDTOBuilder WithFamilyName(string familyName)
        {
            _houseDTO.FamilyName = familyName;
            return this;
        }

        public HouseDTOBuilder WithManagerId(int? managerId)
        {
            _houseDTO.ManagerId = managerId;
            return this;
        }

        public HouseDTO Build()
        {
            return _houseDTO;
        }
    }

    public static HouseDTOBuilder Builder()
    {
        return new HouseDTOBuilder();
    }
}
