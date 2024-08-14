namespace MrPill.DTOs.DTOs;

using System;
using System.ComponentModel.DataAnnotations;

public class MedicationDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Please enter a name.")]
    [StringLength(60, ErrorMessage = "Name should not exceed 60 characters.")]
    public string? EnglishName { get; set; }
    public string? HebrewName { get; set; }

    [StringLength(120, ErrorMessage = "Description should not exceed 120 characters.")]
    public string? EnglishDescription { get; set; }
    public string? HebrewDescription { get; set; }
    public DateTime? Validity { get; set; }
    public int UserId { get; set; }
    public int MedicationRepoId { get; set; }
    public string? ImagePath { get; set; }
    public bool IsPrivate { get; set; }

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

        public MedicationDTOBuilder WithEnglishName(string? name)
        {
            _medicationDTO.EnglishName = name;
            return this;
        }

        public MedicationDTOBuilder WithHebrewName(string? name)
        {
            _medicationDTO.HebrewName = name;
            return this;
        }

        public MedicationDTOBuilder WithIsPrivate(bool privatcy)
        {
            _medicationDTO.IsPrivate = privatcy;
            return this;
        }

        public MedicationDTOBuilder WithEnglishDescription(string? description)
        {
            _medicationDTO.EnglishDescription = description;
            return this;
        }

        public MedicationDTOBuilder WithHebrewDescription(string? description)
        {
            _medicationDTO.HebrewDescription = description;
            return this;
        }

        public MedicationDTOBuilder WithImagePath(string? ImagePath)
        {
            _medicationDTO.ImagePath = ImagePath;
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
