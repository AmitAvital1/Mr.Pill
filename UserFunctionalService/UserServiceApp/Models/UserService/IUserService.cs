using MrPill.DTOs.DTOs;

namespace UserServiceApp.Models.UserService;

public interface IUserService
{
    public bool CreateNewMedication(string medicationName, int phoneNumber);
    public void UpdateMedication();
    public void DeleteMedication();
    public IEnumerable<MedicationDTO> GetAllMedicationByUserId(int userId);
    public MedicationDTO GetMedicationByName(string medicationName);
    public void SaveMassageToManagerHouseToAddNewUser(LoginComunicationDWrapper loginComunicationDWrapper);
    public int GetUserPhoneNumber(string token);
     public bool isUserExistInDb(int PhoneNumber);
}