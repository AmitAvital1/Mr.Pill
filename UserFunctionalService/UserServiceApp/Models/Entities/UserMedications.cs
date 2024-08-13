namespace UserServiceApp.Models;
public class UserMedications
{
    public int Id { get; set; }
    public required string Barcode { get; set; }
    public int? PillSize { get; set; } 
    public DateTime Validity { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
     public int MedicationRepoId { get; set; }
     public PrivacyStatus IsPrivate  { get; set; }
    public  required MedicationRepo MedicationRepo { get; set; }
}


// need to add validity and duration
// boolean that check if the user takes it daily 
// the number of pills in the pill
// how many times a day the pill been taken