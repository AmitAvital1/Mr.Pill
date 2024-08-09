import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';
import axios from 'axios';
import dns from '../dns.json';
import { router } from 'expo-router';
import * as FileSystem from 'expo-file-system';
import { saveTokenToFile } from '@/components/tokenHandlerFunctions';

const LogInScreen = () => {

  const [number, onChangeNumber] = React.useState('');
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () => setDisabled(number == '')
 
  function handleLogin() {
    let response = sendLoginRequest();
    router.navigate({pathname: '(home)/home', params: {'userIsLoggedIn': 1}});
  }

  const sendLoginRequest = async () => {
    try {

      axios.defaults.validateStatus = function () {
        return true;
      };

      const UserDTO = {
        "FirstName": "",
        "LastName": "",
        "PhoneNumber": number
      }

      const request = {
        method: 'post',
        url: "http://10.0.2.2:5181/Mr-Pill/Login",
        headers: { "Content-Type": "application/json" }, 
        data: {
          "FirstName": "tt",
          "LastName": "gg",
          "PhoneNumber": number,
          "UserToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiR2VuZXJhbC1Vc2VyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsIlBob25lTnVtYmVyIjoiMDUwMDExMTIyMiIsImV4cCI6MTcyMjk3NDAxNCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjIxL01yX1BpbGxfQXBwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjIxL01yX1BpbGxfQXBwIn0._c4HO-CKGuXaS-h2EzV89X9eYYGqXaZiyqbqyPyGWYw",
        }
      }

      console.log("send");
      //const response = await axios(request);
      const response = await axios(request);
      console.log(response.request._response);
      saveTokenToFile(response.request._response.token)
      
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
        value={number}
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