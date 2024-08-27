import { router } from "expo-router";

let user = {
    FirstName: "",
    LastName: "",
    PhoneNumber: "",
    Token: "",
};

let stateMap = new Map<string, number | string>();
stateMap.set('session', 1);

export default {
  setUser(firstName?: string, lastName?: string, phoneNumber?: string, token?: string) {
    user.FirstName    = firstName   ? firstName   : user.FirstName    ;
    user.LastName     = lastName    ? lastName    : user.LastName     ;
    user.PhoneNumber  = phoneNumber ? phoneNumber : user.PhoneNumber  ;
    user.Token        = token       ? token       : user.Token        ;
    user.Token        = token       ? token       : user.Token        ;
  },
  setPhone(phoneNumber: string) {
    user.PhoneNumber = phoneNumber;
  },
  setToken(token: string) {
    user.Token = token;
  },
  getUser() {
    return user;
  },
  isEmpty() {
    return (user.FirstName == "" || user.LastName == "" || user.PhoneNumber == "" || user.Token == "");
  },
  getState(key: string) {
    return stateMap.get(key);
  },
  setState(key: string, value: number | string) {
    return stateMap.set(key, value);
  },
  expireSession() {
    this.setState('session', 0);
    router.dismissAll();
    router.navigate('/(login)/welcome');
  }

};