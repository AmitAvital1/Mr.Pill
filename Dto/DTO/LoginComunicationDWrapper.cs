using System.Text;
using System.Text.Json;

namespace MrPill.DTOs.DTOs;

public class LoginComunicationDWrapper
{
    private readonly UserDTO _userDTO;
    private readonly bool _mergeToNewHouse;
    private readonly int _managerPhone;
    private readonly int _senderPhone;
    
    public LoginComunicationDWrapper(UserDTO userDTO, bool mergeToNewHouse, int managerPhone, int senderPhone)
    {
        _userDTO = userDTO;
        _mergeToNewHouse = mergeToNewHouse;
        _managerPhone = managerPhone;
        _senderPhone = senderPhone;
    }

    public UserDTO UserDTO
    {
        get { return _userDTO; }
    }

    public bool MergeToNewHouse
    {
        get { return _mergeToNewHouse; }
    }

    public int ManagerPhone
    {
        get { return _managerPhone; }
    }

    public int SenderPhoneNumber
    {
        get {return _senderPhone; }
    }
    
    public byte[] GetBytes()
    {
        string json = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(json);
    }
}