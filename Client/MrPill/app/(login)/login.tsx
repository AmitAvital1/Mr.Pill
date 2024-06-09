import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';
import axios from 'axios';
import dns from '../dns.json';
import { router } from 'expo-router';

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
      
      const UserDTO = {
        "PhoneNumber": number
      }

      const headers = {
        "Content-Type": "application/json",
      };

      console.log("send");
      const response = await axios.get("http://10.0.2.2:5181/Mr-Pill/Health" ,{ headers });
      console.log(response.data);
      
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