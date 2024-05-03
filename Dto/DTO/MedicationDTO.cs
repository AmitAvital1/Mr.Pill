namespace MrPill.DTOs.DTOs;

using System;
using System.ComponentModel.DataAnnotations;

public class MedicationDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Please enter a name.")]
    [StringLength(50, ErrorMessage = "Name should not exceed 50 characters.")]
    public string? Name { get; set; }

    [StringLength(100, ErrorMessage = "Description should not exceed 100 characters.")]
    public string? Description { get; set; }
    
    public DateTime? Validity { get; set; }
    public int UserId { get; set; }
    public int MedicationRepoId { get; set; }

    public class MedicationDTOBuilder
    {
        private MedicationDTO _medicationDTO;

        public MedicationDTOBuilder()
        {
            _medicationDTO = new MedicationDTO();
        }

        public MedicationDTOBuilder WithId(int id)
        {
            _medicationDTO.Id = id;
            return this;
        }

        public MedicationDTOBuilder WithName(string? name)
        {
            _medicationDTO.Name = name;
            return this;
        }

        public MedicationDTOBuilder WithDescription(string? description)
        {
            _medicationDTO.Description = description;
            return this;
        }

        public MedicationDTOBuilder WithValidity(DateTime? validity)
        {
            _medicationDTO.Validity = validity;
            return this;
        }

        public MedicationDTOBuilder WithUserId(int userId)
        {
            _medicationDTO.UserId = userId;
            return this;
        }

        public MedicationDTOBuilder WithMedicationRepoId(int medicationRepoId)
        {
            _medicationDTO.MedicationRepoId = medicationRepoId;
            return this;
        }

        public MedicationDTO Build()
        {
            return _medicationDTO;
        }
    }

    public static MedicationDTOBuilder Builder()
    {
        return new MedicationDTOBuilder();
    }
}
