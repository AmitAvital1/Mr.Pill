using MrPill.DTOs.DTOs;

namespace UserServiceApp.Models.UserService;

public interface IUserService
{
    public Task<bool> CreateNewMedication(string medicationBarcode, int phoneNumber, bool privatcy);
    public void UpdateMedication();
    public void DeleteMedication();
    public IEnumerable<MedicationDTO> GetAllMedicationByUserId(int phoneNumber, PrivacyStatus privacyStatus);
    public MedicationDTO GetMedicationByName(string medicationName);
    public void SaveMassageToManagerHouseToAddNewUser(LoginComunicationDWrapper loginComunicationDWrapper);
    public int GetUserPhoneNumber(string token);
     public bool IsUserExistInDb(int PhoneNumber);
    public int GetPhoneNumberFromToken(string? token);
}