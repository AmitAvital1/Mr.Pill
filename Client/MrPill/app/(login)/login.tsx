import React, { useEffect } from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text} from 'react-native';

import { router } from 'expo-router';

import DataHandler from '@/DataHandler';
import RequestHandler from '@/RequestHandler';

import { MrPillLogo } from '@/components/MrPillLogo';
import ConfirmButton from '@/components/ConfirmButton';

const bgc = "#98cce2", dark_bgc = "#4c685f", light_bgc = "#dcfff9";

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

const LogInScreen = () => {

  const user = DataHandler.getUser()
  const loginType = DataHandler.getFlag('login');

  const [phoneNumber, onChangePhoneNumber] = React.useState<string>(loginType ? user.PhoneNumber : "");
  const [validationCode, setValidationCode] = React.useState<string>("");
 
  const [isPhoneValid, setIsPhoneValid] = React.useState<boolean>(loginType || false);
  const [isNumberInSystem, setIsNumberInSystem] = React.useState<boolean>(true);

  const [isInitialButtonDisabled, setIsInitialButtonDisabled] = React.useState(true);
  const [isConfirmButtonDisabled, setIsConfirmButtonDisabled] = React.useState(true);


  function isValidData() {

    if (!isValidPhoneNumber(phoneNumber)) return false;
    if (!isValidCodeModelCorrect(validationCode)) return false;

    return true;

  }

  async function handleVerify() {
    let response = await sendVerifyLoginRequest();
    console.log(response);
    if (response) {
      DataHandler.setFlag('sessionAlive', true);
      router.replace({pathname: '/(home)/home', params: {'userIsLoggedIn': 1}});
    }
  }

  async function handleLogin() {
    let response = await sendLoginRequest();
    if (response) {
      setIsPhoneValid(true);
    }
  }

  function updateButton() {
    setIsInitialButtonDisabled(!isValidPhoneNumber(phoneNumber));
    setIsConfirmButtonDisabled(!isValidCodeModelCorrect(validationCode))
  }

  useEffect(() => {
    setIsInitialButtonDisabled(!isValidPhoneNumber(phoneNumber));
  }, [phoneNumber]);

  useEffect(() => {
    setIsConfirmButtonDisabled(!isValidCodeModelCorrect(validationCode));
  }, [validationCode]);

  const sendVerifyLoginRequest = async () => {

      // send phone number to DataHandler
      DataHandler.setPhone(phoneNumber);
      DataHandler.setState('validationCode', validationCode);

      if (await RequestHandler.sendRequest("verifyLogin")) {
        const response = RequestHandler.getParsedResponse();
        DataHandler.setUser(response.firstName, response.lastName, phoneNumber, JSON.parse(RequestHandler.getResponse().request._response).token)
        return true;
      }
      
      return false;
  }

  const sendLoginRequest = async () => {

      // send phone number to DataHandler
      DataHandler.setPhone(phoneNumber);

      if (await RequestHandler.sendRequest("login")) {

        setIsPhoneValid(true);
        return true;

      } else if (RequestHandler.getResponse().request.status == 404) {

        setIsNumberInSystem(false);

      }

  }
  
  return (
    <SafeAreaView style={{backgroundColor: bgc, flex: 1}}>

      <View style={styles.pagetop}>
        <Text style={{alignSelf: "center", fontSize: 38, flex: 0, fontWeight: "bold", marginBottom: -15}}>התחברות למר. פיל</Text>
        {MrPillLogo(0.75)}
      </View>

      
      {!isPhoneValid &&
      <TextInput
        style={styles.input}
        onChangeText={(input: any) => {onChangePhoneNumber(input); setIsNumberInSystem(true); updateButton();}}
        value={phoneNumber}
        placeholder={"מספר טלפון"}
        keyboardType="numeric"
        textAlign="right"
        onEndEditing={()=>{}}
      />}

      {!isNumberInSystem && <Text style={{textAlign: 'center', fontSize: 20, fontWeight: 'bold', color: "#FF0000"}}>המספר לא קיים במערכת</Text>}
      
      {!isPhoneValid && <View>
        <ConfirmButton
          title={isInitialButtonDisabled? "הכנס טלפון": "לחץ להמשך"}
          onPress={handleLogin}
          marginTop={15}
          marginBottom={15}
          borderColor={isInitialButtonDisabled? light_bgc : dark_bgc}
          isDisabled={isInitialButtonDisabled}
        />
      </View>}

      {isPhoneValid && 
        <View>
          
          <TextInput
            style={styles.input}
            onChangeText={(input: any) => {setValidationCode(input); updateButton()}}
            value={validationCode}
            placeholder="קוד אימות מהודעת SMS"
            keyboardType="numeric"
            textAlign="right"
            onEndEditing={()=>{}}
          />
        </View>
      }

      {isPhoneValid && <View>
        <ConfirmButton
          title={isConfirmButtonDisabled? "הכנס קוד": "לחץ להמשך"}
          onPress={handleVerify}
          marginTop={15}
          marginBottom={15}
          borderColor={isConfirmButtonDisabled? light_bgc : dark_bgc}
          isDisabled={isConfirmButtonDisabled}
        />
      </View>}
      
    </SafeAreaView>
  );

};

const styles = StyleSheet.create({
  input: {
    backgroundColor: light_bgc,
    height: 60,
    margin: 8,
    borderWidth: 2,
    borderColor: dark_bgc,
    padding: 10,
    borderRadius: 12,
    fontSize: 25,
    elevation: 5,
  },
  pagetop: {
    height: 180,
    padding: 10,
    backgroundColor: bgc,
    marginBottom: 10,
    marginTop: 8
  },
  
});

export default LogInScreen;