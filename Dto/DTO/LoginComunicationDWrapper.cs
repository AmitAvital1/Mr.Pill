using System.Text;
using System.Text.Json;

namespace MrPill.DTOs.DTOs;

public class LoginComunicationDWrapper
{
    public UserDTO UserDTO { get; set; }
    public int TargetPhoneNumber { get; set; }
    public int SourcePhoneNumber { get; set; }
    public string MedicineCabinetName { get; set; }
    
    public LoginComunicationDWrapper() { }

    public LoginComunicationDWrapper(UserDTO userDTO, int TargetPhoneNumber, int SourcePhoneNumber, string MedicineCabinetName)
    {
        this.UserDTO = userDTO;
        this.TargetPhoneNumber = TargetPhoneNumber;
        this.SourcePhoneNumber = SourcePhoneNumber;
        this.MedicineCabinetName = MedicineCabinetName;
    }

    public byte[] GetBytes()
    {
        string json = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(json);
    }
}

