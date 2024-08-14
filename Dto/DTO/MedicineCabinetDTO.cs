namespace MrPill.DTOs.DTOs;

public class MedicineCabinetDTO
{
    public int Id { get; set; }
    public string? MedicineCabinetName { get; set; }
    public int? CreatorId { get; set; }

    public class MedicineCabinetDTOBuilder
    {
        private MedicineCabinetDTO _medicineCabinetDTO;

        public MedicineCabinetDTOBuilder()
        {
            _medicineCabinetDTO = new MedicineCabinetDTO();
        }

        public MedicineCabinetDTOBuilder WithId(int id)
        {
            _medicineCabinetDTO.Id = id;
            return this;
        }

        public MedicineCabinetDTOBuilder WithMedicineCabinetName(string MedicineCabinetName)
        {
            _medicineCabinetDTO.MedicineCabinetName = MedicineCabinetName;
            return this;
        }

        public MedicineCabinetDTOBuilder WithCreatorId(int? CreatorId)
        {
            _medicineCabinetDTO.CreatorId = CreatorId;
            return this;
        }

        public MedicineCabinetDTO Build()
        {
            return _medicineCabinetDTO;
        }
    }

    public static MedicineCabinetDTOBuilder Builder()
    {
        return new MedicineCabinetDTOBuilder();
    }
}
