let user = {
    FirstName: "",
    LastName: "",
    PhoneNumber: "",
    Token: "",
};

export default {

  setUser(firstName: string, lastName: string, phoneNumber: string, token: string) {
    user.FirstName = firstName;
    user.LastName = lastName;
    user.PhoneNumber = phoneNumber;
    user.Token = token;
  },
  getUser() {
    return user;
  }

};