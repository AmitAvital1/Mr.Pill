import React from "react";
import axios from 'axios'; 

import {
  SafeAreaView,
  StyleSheet,
  TextInput,
  View,
  Text,
  Button,
} from "react-native";


const SignUpScreen = () => {
  const [firstname, onChangeFirstName] = React.useState("");
  const [lastname, onChangeLastName] = React.useState("");
  const [number, onChangeNumber] = React.useState("");
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () =>
    setDisabled(firstname == "" || lastname == "" || number == "");

  function handleSubmit() {
    sendSignupRequest(firstname, lastname, number);
  }

  const sendSignupRequest = async (firstname: string, lastname: string, number: string) => {
    try {
      
      // for debugging
      axios.defaults.validateStatus = function () {
        return true;
      }; //


      const request = {
        method: 'post',
        url: "http://10.0.2.2:5181/Mr-Pill/Register",
        headers: { "Content-Type": "application/json" }, 
        data: {
          FirstName: firstname,
          LastName: lastname,
          PhoneNumber: number,
        }
      }

      const response = await axios(request)
      console.log(response.data);
      
    } catch (error) {

      console.error("Error fetching data:", error);
    }
  }

  return (
    <SafeAreaView>
      <View style={styles.pagetop}>
        <Text style={{ fontSize: 32, flex: 1 }}>הרשמה למר. פיל</Text>
      </View>

      <TextInput
        style={styles.input}
        onChangeText={onChangeNumber}
        value={number}
        placeholder="מספר טלפון"
        keyboardType="numeric"
        textAlign="right"
        onEndEditing={updateButton}
      />

      <TextInput
        style={styles.input}
        onChangeText={onChangeFirstName}
        placeholder="שם פרטי"
        value={firstname}
        textAlign="right"
        onEndEditing={updateButton}
      />

      <TextInput
        style={styles.input}
        onChangeText={onChangeLastName}
        placeholder="שם משפחה"
        value={lastname}
        textAlign="right"
        onEndEditing={updateButton}
      />

      <Button
        title="הרשמה"
        onPress={handleSubmit}
        disabled={isDisabled}
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
    backgroundColor: "lavender",
  },
});


export default SignUpScreen;
