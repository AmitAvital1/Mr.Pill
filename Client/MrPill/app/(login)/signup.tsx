import React, { useEffect } from "react";
import axios from 'axios'; 
import { MrPillLogo } from "@/components/MrPillLogo";
import { saveTokenToFile, readTokenFromFile } from "@/components/tokenHandlerFunctions";
import { AppHomeButton } from "@/components/AppHomeButton";
import { ConfirmButton } from "@/components/ConfirmButton";

import {
  SafeAreaView,
  StyleSheet,
  TextInput,
  View,
  Text,
  Button,
} from "react-native";

declare global {
  var userData: {
    FirstName: string,
    LastName: string,
    PhoneNumber: string
  };
}

const bgc = "#97ffde"

const SignUpScreen = () => {

  const [firstname, onChangeFirstName] = React.useState("");
  const [lastname, onChangeLastName] = React.useState("");
  const [phnumber, onChangePhoneNumber] = React.useState("");
  const [isButtonDisabled, setIsButtonDisabled] = React.useState(true);

  const isValidPhoneNumber = (phoneNumber: string) => {
    if (!phoneNumber) return false;
    if (!/^[0-9]{10}$/.test(phoneNumber)) return false;
    if (phoneNumber.length != 10) return false;
    if (phoneNumber[0] != '0' || phoneNumber[1] != '5') return false;

    return true;
  }

  function isValidData (phoneNumber: string, firstName: string, lastName: string) {

    if (!firstname || !lastname || firstname == "" || lastname == "") return false;
    if (!isValidPhoneNumber(phoneNumber)) return false;

    return true;

  }

  function handleSubmit() {
    sendSignupRequest(firstname, lastname, phnumber);
  }

  function updateButton() {
    setIsButtonDisabled(!isValidData(phnumber, firstname, lastname))
  }

  const sendSignupRequest = async (firstname: string, lastname: string, phnumber: string) => {
    try {
      
      // for debugging
      axios.defaults.validateStatus = function () {
        return true;
      }; //

      const request = {
        method: 'post',
        url: "http://10.0.2.2:5181/Mr-Pill/Register",
        headers: { }, 
        data: {
          FirstName: firstname,
          LastName: lastname,
          PhoneNumber: phnumber,
        }
      }

      globalThis.userData = {
        FirstName: firstname,
        LastName: lastname,
        PhoneNumber: phnumber
      }

      const response = await axios(request)
      console.log(response.data);
      
    } catch (error) {

      console.error("Error fetching data:", error);
    }
  }

  useEffect(() => {
    setIsButtonDisabled(!isValidPhoneNumber(phnumber));
  }, [phnumber]);

  return (
    <SafeAreaView style={{backgroundColor: bgc, flex: 1}}>
      <View style={styles.pagetop}>

        <Text style={{alignSelf: "center", fontSize: 38, flex: 0, fontWeight: "bold", marginBottom: -15}}>הרשמה למר. פיל</Text>
        {MrPillLogo(0.75)}
      
      </View>

      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangePhoneNumber(input); updateButton()}}
        value={phnumber}
        placeholder="מספר טלפון"
        keyboardType="numeric"
        textAlign="right"
        onEndEditing={updateButton}
      />

      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangeFirstName(input); updateButton()}}
        placeholder="שם פרטי"
        value={firstname}
        textAlign="right"
        onEndEditing={updateButton}
      />

      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangeLastName(input); updateButton()}}
        placeholder="שם משפחה"
        value={lastname}
        textAlign="right"
        onEndEditing={updateButton}
      />
      
      <View>
        <ConfirmButton
          title={isButtonDisabled? "הכנס פרטים": "הירשם!"}
          onPress={handleSubmit}
          isDisabled={isButtonDisabled}
          marginTop={15}
          borderColor={"#4c685f"}
        />
      </View>

    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  input: {
    backgroundColor: "#c0fff5",
    height: 60,
    margin: 8,
    borderWidth: 2,
    borderColor: "#4c685f",
    padding: 10,
    borderRadius: 12,
    fontSize: 25,
  },
  pagetop: {
    height: 180,
    padding: 10,
    backgroundColor: bgc,
    marginBottom: 10,
    marginTop: 8
  },
  
});


export default SignUpScreen;
