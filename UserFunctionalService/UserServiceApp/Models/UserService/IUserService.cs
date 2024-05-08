using MrPill.DTOs.DTOs;

namespace UserServiceApp.Models.UserService;

public interface IUserService
{
    public bool CreateNewMedication(string medicationName);
    public void UpdateMedication();
    public void DeleteMedication();
    public IEnumerable<MedicationDTO> GetAllMedicationByUserId(int userId);
    public MedicationDTO GetMedicationByName(string medicationName);
    public void SaveMassageToManagerHouseToAddNewUser(LoginComunicationDWrapper loginComunicationDWrapper);
}