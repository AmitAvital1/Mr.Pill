import React, {useEffect, useState} from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';
import axios from 'axios';
import dns from '../dns.json';

const LogInScreen = () => {

  const [number, onChangeNumber] = React.useState('');
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () => setDisabled(number == '')
  /*
  function handleLogin() {
   sendLoginRequest();
  }

  const sendLoginRequest = async () => {
    try {
      const UserDTO = {
        PhoneNumber: number
      }
      
      const headers = {
        "Content-Type": "application/json",
      };

      const response = await axios.post("http://localhost:5181/Login",UserDTO ,{ headers });
      
    } catch (error) {

      console.error("Error fetching data:", error);
    }
  */
    
    const [isLoading, setLoading] = useState(true);
    const [data, setData] = useState<string[]>([]);

    const handleLogin = async () => {
      /*
      try {
        const addr = `${dns.login_service}${'Health'}`;
        
        const response = await fetch(addr, {
          method: 'POST',
          headers: {
            //Accept: 'application/json',
            'Content-Type': "application/x-www-form-urlencoded"
          },
          //body: JSON.stringify({
          //  firstParam: 'yourValue',
          //  secondParam: 'yourOtherValue',
          //}),
        });
        
        console.log(response);
      }
      */
        //const response = await fetch(addr);
        //const json = await response.json();
        //console.log(json.data);

/*
        const response = await fetch('https://10.0.2.2:5001/Mr-Pill/Health', {
          method: 'POST',
          headers: {
            Accept: 'application/json'
          }
        });*/

        //const response = await fetch('https://10.0.2.2:5001/Mr-Pill/Health');
        try {

        const response = await fetch('https://google.com/');
        console.log(response);

        //const axresponse = await axios.post("http://10.0.2.2:5001/Mr-Pill/Health");
        //console.log(axresponse.data);
        
        }

      catch(error) {
        console.log(error);
      }

    };

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