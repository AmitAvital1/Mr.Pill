using System;
namespace MrPill.DTOs.DTOs;

public class MedicationDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? Validity { get; set; }
    public int UserId { get; set; }
    public int MedicationRepoId { get; set; }
}