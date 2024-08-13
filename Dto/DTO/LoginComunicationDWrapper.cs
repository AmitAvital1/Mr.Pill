using System.Text;
using System.Text.Json;

namespace MrPill.DTOs.DTOs;

public class LoginComunicationDWrapper
{
    public UserDTO UserDTO { get; set; }
    public bool MergeToNewHouse { get; set; }
    public int ManagerPhone { get; set; }
    public int SenderPhoneNumber { get; set; }
    
    public LoginComunicationDWrapper() { }

    public LoginComunicationDWrapper(UserDTO userDTO, bool mergeToNewHouse, int managerPhone, int senderPhoneNumber)
    {
        UserDTO = userDTO;
        MergeToNewHouse = mergeToNewHouse;
        ManagerPhone = managerPhone;
        SenderPhoneNumber = senderPhoneNumber;
    }

    public byte[] GetBytes()
    {
        string json = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(json);
    }
}
