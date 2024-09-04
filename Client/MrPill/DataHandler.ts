import { router } from "expo-router";
import { Alert } from "react-native";
import AsyncStorage from '@react-native-async-storage/async-storage';

/* 

--- DataHandler ---

'singleton object' for centralized maintenance and persistence of data on device memory and storage.
things like the basic user data, application states, will be saved here during application runtime when data needs to persist
between different application screens.

*/
let user = {
    FirstName: "",
    LastName: "",
    PhoneNumber: "",
    Token: "",
};

// using 3 tables for easy access to different types of data
let map = new Map<string, any>();
let stateMap = new Map<string, string>();
let flagMap = new Map<string, boolean>();
let disposable: any = null;

export default {

  reset() {
    this.resetUser();

    map = new Map<string, any>;
    stateMap = new Map<string, string>;
    flagMap = new Map<string, boolean>();
    flagMap.set('sessionAlive', true);

  },
  resetUser() {
    user = {
      FirstName: "",
      LastName: "",
      PhoneNumber: "",
      Token: "",
    };

    this.saveUserToStorage();
  },
  set(key: string, value: any) {
    map.set(key,value);
  },
  
  get(key: string, dispose?: boolean) {
    const result = map.get(key);

    if (dispose) {
      map.delete(key);
    }

    return result;
  },

  setUser(firstName?: string, lastName?: string, phoneNumber?: string, token?: string) {
    user.FirstName    = firstName   ? firstName   : user.FirstName    ;
    user.LastName     = lastName    ? lastName    : user.LastName     ;
    user.PhoneNumber  = phoneNumber ? phoneNumber : user.PhoneNumber  ;
    user.Token        = token       ? token       : user.Token        ;

    this.saveUserToStorage();
  },
  setPhone(phoneNumber: string) {
    user.PhoneNumber = phoneNumber;
  },
  setToken(token: string) {
    user.Token = token;
  },
  getUser() {
    if (this.isEmpty()) {
      this.loadUserFromStorage()
    }
    return user;
  },
  isEmpty() {
    return (user.FirstName == "" || user.LastName == "" || user.PhoneNumber == "" || user.Token == "");
  },

  getState(key: string, dispose?: boolean) {
    const result = stateMap.get(key);
    
    if (dispose) {
      stateMap.delete(key);
    }

    return result;
  },

  setState(key: string, value: string) {
    return stateMap.set(key, value);
  },
  expireSession() {
    this.setFlag('sessionAlive', false);
    if (this.getFlag('hasOpenedApp')) {
      Alert.alert("עבר זמן מאז שהתחברת בפעם האחרונה, אנא התחבר מחדש.");
    }
    router.dismissAll();
    router.push('/(login)/welcome');
  },

  getFlag(key: string) {
    return flagMap.get(key);
  },
  setFlag(key: string, value: boolean) {
    return flagMap.set(key, value);
  },
  
  async saveDataToStorage(key: string, value: string) {
    try {
      await AsyncStorage.setItem(key, value);
    } catch (e) {
      console.error(e);
    }
  },

  async loadDataFromStorage(key: string) {
    try {

      const value = await AsyncStorage.getItem(key);

      if (value !== null) {
        return value;
      }

    } catch (e) {

      console.error(e);

    }
  },
  
  async loadUserFromStorage() {
    try {

      user = {
        FirstName: await AsyncStorage.getItem('userFirstName') || "",
        LastName: await AsyncStorage.getItem('userLastName') || "",
        PhoneNumber: await AsyncStorage.getItem('userPhoneNumber') || "",
        Token: await AsyncStorage.getItem('userToken') || "",
      } 

    } catch (e) {

      console.error(e);

    }
  },

  async saveUserToStorage() {
    try {

      this.saveDataToStorage('userFirstName', user.FirstName);
      this.saveDataToStorage('userLastName', user.LastName);
      this.saveDataToStorage('userPhoneNumber', user.PhoneNumber);
      this.saveDataToStorage('userToken', user.Token);

    } catch (e) {

      console.error(e);

    }
  },

};