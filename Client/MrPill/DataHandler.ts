import { router } from "expo-router";

let user = {
    FirstName: "",
    LastName: "",
    PhoneNumber: "",
    Token: "",
};

let stateMap = new Map<string, number>();

export default {
  setUser(firstName?: string, lastName?: string, phoneNumber?: string, token?: string) {
    user.FirstName = firstName? firstName : "";
    user.LastName = lastName? lastName : "";;
    user.PhoneNumber = phoneNumber? phoneNumber : "";;
    user.Token = token? token : "";;
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
  setState(key: string, value: number) {
    return stateMap.set(key, value);
  },
  expireSession() {
    this.setState('session', 0);
    router.dismissAll();
    router.navigate('/(login)/welcome');
  }

};