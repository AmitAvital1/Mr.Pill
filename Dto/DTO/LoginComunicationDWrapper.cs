using System.Text;
using System.Text.Json;

namespace MrPill.DTOs.DTOs;

public class LoginComunicationDWrapper
{
    private readonly UserDTO _userDTO;
    private readonly bool _mergeToNewHouse;
    private readonly int _managerPhone;
    
    public LoginComunicationDWrapper(UserDTO userDTO, bool mergeToNewHouse, int managerPhone)
    {
        _userDTO = userDTO;
        _mergeToNewHouse = mergeToNewHouse;
        _managerPhone = managerPhone;
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
    
    public byte[] GetBytes()
    {
        string json = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(json);
    }
}