namespace MrPill.DTOs.DTOs
{
    public class CabinetRequestDTO
    {
        public int Id { get; set; }
        public string? TargetPhoneNumber { get; set; }
        public string? SenderPhoneNumber { get; set; }
        public string? CabinetName { get; set; }
        public string? SenderName { get; set; }
        public bool IsHandle { get; set; }
        public bool IsApprove { get; set; }
        public bool IsSenderSeen { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public class CabinetRequestDTOBuilder
        {
            private readonly CabinetRequestDTO _cabinetRequestDTO;

            public CabinetRequestDTOBuilder()
            {
                _cabinetRequestDTO = new CabinetRequestDTO();
            }

            public CabinetRequestDTOBuilder WithId(int id)
            {
                _cabinetRequestDTO.Id = id;
                return this;
            }

            public CabinetRequestDTOBuilder WithSenderName(string Name)
            {
                _cabinetRequestDTO.SenderName = Name;
                return this;
            }

            public CabinetRequestDTOBuilder WithCabinetName(string cabinetName)
            {
                _cabinetRequestDTO.CabinetName = cabinetName;
                return this;
            }

            public CabinetRequestDTOBuilder WithTargetPhoneNumber(string? targetPhoneNumber)
            {
                _cabinetRequestDTO.TargetPhoneNumber = targetPhoneNumber;
                return this;
            }

            public CabinetRequestDTOBuilder WithSenderPhoneNumber(string senderPhoneNumber)
            {
                _cabinetRequestDTO.SenderPhoneNumber = senderPhoneNumber;
                return this;
            }

            public CabinetRequestDTOBuilder WithIsHandle(bool isHandle)
            {
                _cabinetRequestDTO.IsHandle = isHandle;
                return this;
            }

            public CabinetRequestDTOBuilder WithIsApprove(bool isApprove)
            {
                _cabinetRequestDTO.IsApprove = isApprove;
                return this;
            }

            public CabinetRequestDTOBuilder WithIsSenderSeen(bool isSenderSeen)
            {
                _cabinetRequestDTO.IsSenderSeen = isSenderSeen;
                return this;
            }

            public CabinetRequestDTOBuilder WithDateStart(DateTime dateStart)
            {
                _cabinetRequestDTO.DateStart = dateStart;
                return this;
            }

            public CabinetRequestDTOBuilder WithDateEnd(DateTime dateEnd)
            {
                _cabinetRequestDTO.DateEnd = dateEnd;
                return this;
            }

            public CabinetRequestDTO Build()
            {
                return _cabinetRequestDTO;
            }
        }

        public static CabinetRequestDTOBuilder Builder()
        {
            return new CabinetRequestDTOBuilder();
        }
    }
}
