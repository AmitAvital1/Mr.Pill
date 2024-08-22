import React, { useEffect } from "react";
import axios from 'axios'; 
import { MrPillLogo } from "@/components/MrPillLogo";
import { saveTokenToFile, readTokenFromFile } from "@/components/tokenHandlerFunctions";
import { AppHomeButton } from "@/components/AppHomeButton";
import { ConfirmButton } from "@/components/ConfirmButton";
import DataHandler from "@/DataHandler";

declare global {
  var userToken: string
  var userData: {
    FirstName: string,
    LastName: string,
    PhoneNumber: string
  };
}

import {
  SafeAreaView,
  StyleSheet,
  TextInput,
  View,
  Text,
  Button,
} from "react-native";
import { router } from "expo-router";


const isValidPhoneNumber = (phoneNumber: string) => {
  if (!phoneNumber) return false;
  if (!/^[0-9]{10}$/.test(phoneNumber)) return false;
  if (phoneNumber.length != 10) return false;
  if (phoneNumber[0] != '0' || phoneNumber[1] != '5') return false;

  return true;
}

const isValidCodeModelCorrect = (validCode: string) => {
  if (!validCode) return false;
  if (!/^[0-9]{6}$/.test(validCode)) return false;
  if (validCode.length != 6) return false;

  return true;
}

const bgc = "#97ffde"

const SignUpScreen = () => {

  const [firstName, onChangeFirstName] = React.useState("");
  const [lastName, onChangeLastName] = React.useState("");
  const [phoneNumber, onChangePhoneNumber] = React.useState("");
  const [validationCode, onChangeValidationCode] = React.useState("");
  const [isButtonDisabled, setIsButtonDisabled] = React.useState(true);
  const [isSignupClicked, setIsSignupClicked] = React.useState(false);
  const [isSignupSuccessful, setIsSignupSuccessful] = React.useState(false);


  function isValidData (phoneNumber: string, firstName: string, lastName: string) {

    if (!firstName || !lastName || firstName == "" || lastName == "") return false;
    if (!isValidPhoneNumber(phoneNumber)) return false;
    if (!isValidCodeModelCorrect(validationCode)) return false;

    return true;

  }

  function handleSubmit() {

    setIsSignupClicked(true);

    return sendSignupRequest(phoneNumber);
  }

  async function handleVerify() {
    
    const response = await sendValidationRequest(phoneNumber, firstName, lastName, validationCode);
    
    if (!response) return null;
    if (!response.request) return null;

    if (response.request.status == 200) {
      setIsSignupSuccessful(true);
      DataHandler.setUser(firstName, lastName, phoneNumber, JSON.parse(response.request._response).token)
      router.replace({pathname: '/(home)/home', params: {'userIsLoggedIn': 1}});
    }
    
    return response;

  }

  const sendValidationRequest = async (phoneNumber: string, firstName: string, lastName: string, validationCode: string) => {
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
          FirstName: firstName,
          LastName: lastName,
          PhoneNumber: phoneNumber,
          Code: validationCode
        }
      }
      const response = await axios(request)
      return response;
    } catch (error) {
      console.error("Error fetching data:", error);
      return null;
    }
  }

  const sendSignupRequest = async (phnumber: string) => {

    try {
      // for debugging
      axios.defaults.validateStatus = function () {
        return true;
      }; //

      const request = {
        method: 'post',
        url: "http://10.0.2.2:5181/Mr-Pill/GenerateRegistrationCode",
        headers: { }, 
        data: {
          PhoneNumber: phnumber,
        }
      }

      const response = await axios(request);
      return response;
      
    } catch (error) {
      
      console.error("Error fetching data:", error);
      return null;

    }
  }

  function updateButton() {
    setIsButtonDisabled(!isValidData(phoneNumber, firstName, lastName))
    console.log(isSignupSuccessful)
  }

  useEffect(() => {
    setIsButtonDisabled(!isValidPhoneNumber(phoneNumber));
  }, [phoneNumber]);

  return (
    <SafeAreaView style={{backgroundColor: bgc, flex: 1}}>

      <View style={styles.pagetop}>
        <Text style={{alignSelf: "center", fontSize: 38, flex: 0, fontWeight: "bold", marginBottom: -15}}>הרשמה למר. פיל</Text>
        {MrPillLogo(0.75)}
      </View>

      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangePhoneNumber(input); updateButton()}}
        value={phoneNumber}
        placeholder="מספר טלפון"
        keyboardType="numeric"
        textAlign="right"
        onEndEditing={updateButton}
      />

      {!isSignupClicked && <View>
        <ConfirmButton
          title={isButtonDisabled? "הכנס טלפון": "לחץ להמשך"}
          onPress={handleSubmit}
          isDisabled={isButtonDisabled}
          marginTop={15}
          marginBottom={15}
          borderColor={"#4c685f"}
        />
      </View>}


      {isSignupClicked && <View>
      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangeFirstName(input); updateButton()}}
        placeholder="שם פרטי"
        value={firstName}
        textAlign="right"
        onEndEditing={updateButton}
      />

      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangeLastName(input); updateButton()}}
        placeholder="שם משפחה"
        value={lastName}
        textAlign="right"
        onEndEditing={updateButton}
      />

      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangeValidationCode(input); updateButton()}}
        value={validationCode}
        placeholder="קוד"
        keyboardType="numeric"
        textAlign="right"
        onEndEditing={updateButton}
      />
      </View>}

      {isSignupClicked && <View>
        <ConfirmButton
          title={isButtonDisabled? "הכנס פרטים": "הירשם!"}
          onPress={handleVerify}
          isDisabled={isButtonDisabled}
          marginTop={15}
          marginBottom={15}
          borderColor={"#4c685f"}
        />
      </View>}

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
