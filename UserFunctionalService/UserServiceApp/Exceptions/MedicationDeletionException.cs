namespace UserServiceApp.Models.Exceptions;
public class MedicationDeletionException : Exception
{
    public MedicationDeletionException(string message) : base(message) { }
}