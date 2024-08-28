import React, { useEffect } from "react";
import { MrPillLogo } from "@/components/MrPillLogo";
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
  Alert,
} from "react-native";
import { router } from "expo-router";
import RequestHandler from "@/RequestHandler";


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
  const [isVerifyClicked, setIsVerifyClicked] = React.useState(false);
  const [isNumberInSystem, setIsNumberInSystem] = React.useState(false);

  function isValidData (phoneNumber: string, firstName: string, lastName: string) {

    if (!firstName || !lastName || firstName == "" || lastName == "") return false;
    if (!isValidPhoneNumber(phoneNumber)) return false;
    if (!isValidCodeModelCorrect(validationCode)) return false;

    return true;

  }

  async function handleSubmit() {

    if (!await sendSignupRequest(phoneNumber)) return false;
    
    const statusCode = RequestHandler.getResponse().request.status

    if (statusCode == 200) {
      setIsSignupClicked(true);
    } else if (statusCode == 409) {
      setIsNumberInSystem(true);
    } else {
      console.log(statusCode);
    }
    
  }

  async function handleVerify() {
    
    if (await sendValidationRequest(phoneNumber, firstName, lastName, validationCode)) {

      console.log(RequestHandler.getResponse());
      DataHandler.setToken(JSON.parse(RequestHandler.getResponse().request._response).token);
      router.replace({pathname: '/(home)/home', params: {'userIsLoggedIn': 1}});
    
    } else {

      setIsVerifyClicked(true);

    }
  }

  const sendValidationRequest = async (phoneNumber: string, firstName: string, lastName: string, validationCode: string) => {
    
    DataHandler.setUser(firstName, lastName, phoneNumber, undefined);
    DataHandler.setState('validationCode', validationCode);

    return await RequestHandler.sendRequest('verifySignup');
  }

  const sendSignupRequest = async (phnumber: string) => {
    
    DataHandler.setPhone(phnumber);
    if (await RequestHandler.sendRequest('signup')) {
      return true;
    }
    return false;
  }

  function updateButton() {
    setIsButtonDisabled(!isValidData(phoneNumber, firstName, lastName))
    setIsNumberInSystem(false);
    setIsVerifyClicked(false);
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

      {isNumberInSystem && <Text style={{textAlign: 'center', fontSize: 20, fontWeight: 'bold', color: "#FF0000"}}>המספר כבר קיים במערכת</Text>}

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

        {isVerifyClicked && 
        <Text style={{textAlign: 'center', fontSize: 20, fontWeight: 'bold', color: "#FF0000"}}>קוד SMS שגוי, אנא נסה שנית.</Text>
        }

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
