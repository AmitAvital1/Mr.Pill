import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';
import axios from 'axios';
import dns from '../dns.json';
import { Link, router } from 'expo-router';
import * as FileSystem from 'expo-file-system';
import { saveTokenToFile } from '@/components/tokenHandlerFunctions';


async function sendAutomaticLoginRequest() {
  if (!globalThis.userData) return false;
  if (!globalThis.userData.FirstName) return false;
  if (!globalThis.userData.LastName) return false;
  if (!globalThis.userData.PhoneNumber) return false;
  if (!globalThis.userToken) return false;

  try {

    axios.defaults.validateStatus = function () {
      return true;
    };

    const request = {
      method: 'post',
      url: "http://10.0.2.2:5181/Mr-Pill/Login",
      headers: { "Content-Type": "application/json" }, 
      data: {
        "PhoneNumber": globalThis.userData.PhoneNumber,
        "UserToken": globalThis.userToken,
      }
    }

    const response = await axios(request);

    if (response.request.status == 200) {
      globalThis.userToken = JSON.parse(response.request._response).token
      return true;
    }
    else {
      console.log(response.request.status)
      return false;
    }
    
  } catch (error) {
    console.error("Error fetching data:", error);
    return false;
  }

}

const LogInScreen = () => {
  
  async function handleAutoLogin() {
    const response = await sendAutomaticLoginRequest();
    if (response == true)
      return (<Link href='/(home)/home'></Link>)
  }
    
  const [phoneNumber, onChangeNumber] = React.useState('');
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () => setDisabled(phoneNumber == '')
 
  async function handleLogin() {
    let response = await sendLoginRequest();
    if (response)
      router.navigate({pathname: '/(home)/home', params: {'userIsLoggedIn': 1}});
  }

  const sendLoginRequest = async () => {
    try {

      axios.defaults.validateStatus = function () {
        return true;
      };

      const request = {
        method: 'post',
        url: "http://10.0.2.2:5181/Mr-Pill/Login",
        headers: { "Content-Type": "application/json" }, 
        data: {
          "PhoneNumber": phoneNumber,
          "UserToken": globalThis.userToken,
        }
      }

      const response = await axios(request);

      if (response.request.status == 200) {
        globalThis.userToken = JSON.parse(response.request._response).token
        return true;
      }
      else {
        console.log(response.request.status)
        return false;
      }
      
    } catch (error) {
      console.error("Error fetching data:", error);
      return false;
    }

  }

  return (
    <SafeAreaView>
      
      <View style={styles.pagetop}>
        <Text style={{fontSize: 32, flex:1}}>
          התחברות למר. פיל
        </Text>
      </View>

      <TextInput
        style={styles.input}
        onChangeText={onChangeNumber}
        value={phoneNumber}
        placeholder="מספר טלפון"
        keyboardType="numeric"
        textAlign='right'
        onEndEditing={updateButton}
      />

      <Button 
        title="התחברות" 
        onPress={handleLogin} 
        //disabled={isDisabled}
      />
      
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  input: {
    height: 40,
    margin: 12,
    borderWidth: 1,
    padding: 10,
  },
  pagetop: {
    height: 100, 
    padding: 10,
    backgroundColor: 'lavender'
  },
});


export default LogInScreen;