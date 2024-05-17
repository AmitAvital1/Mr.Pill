namespace MrPill.DTOs.DTOs;

public class HouseRequestDTO
{
    public int Id { get; set; }
    public int HouseId { get; set; }
    public string? SenderPhoneNumber { get; set; }
    public bool IsHandled { get; set; }
    public bool IsApproved { get; set; }
    public bool IsSenderSeen { get; set; }
    public bool MergeToNewHouse { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }

    public class HouseRequestDTOBuilder
    {
        private HouseRequestDTO _houseRequestDTO;

        public HouseRequestDTOBuilder()
        {
            _houseRequestDTO = new HouseRequestDTO();
        }

        public HouseRequestDTOBuilder WithId(int id)
        {
            _houseRequestDTO.Id = id;
            return this;
        }

        public HouseRequestDTOBuilder WithHouseId(int houseId)
        {
            _houseRequestDTO.HouseId = houseId;
            return this;
        }

        public HouseRequestDTOBuilder WithSenderPhoneNumber(string senderPhoneNumber)
        {
            _houseRequestDTO.SenderPhoneNumber = senderPhoneNumber;
            return this;
        }

        public HouseRequestDTOBuilder WithIsHandled(bool isHandled)
        {
            _houseRequestDTO.IsHandled = isHandled;
            return this;
        }

        public HouseRequestDTOBuilder WithIsApproved(bool isApproved)
        {
            _houseRequestDTO.IsApproved = isApproved;
            return this;
        }

        public HouseRequestDTOBuilder WithIsSenderSeen(bool isSenderSeen)
        {
            _houseRequestDTO.IsSenderSeen = isSenderSeen;
            return this;
        }

        public HouseRequestDTOBuilder WithMergeToNewHouse(bool mergeToNewHouse)
        {
            _houseRequestDTO.MergeToNewHouse = mergeToNewHouse;
            return this;
        }

        public HouseRequestDTOBuilder WithDateStart(DateTime dateStart)
        {
            _houseRequestDTO.DateStart = dateStart;
            return this;
        }

        public HouseRequestDTOBuilder WithDateEnd(DateTime dateEnd)
        {
            _houseRequestDTO.DateEnd = dateEnd;
            return this;
        }

        public HouseRequestDTO Build()
        {
            return _houseRequestDTO;
        }
    }

    public static HouseRequestDTOBuilder Builder()
    {
        return new HouseRequestDTOBuilder();
    }
}