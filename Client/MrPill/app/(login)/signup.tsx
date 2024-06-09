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
  const [firstname, onChangeLastName] = React.useState("");
  const [lastname, onChangeFirstName] = React.useState("");
  const [number, onChangeNumber] = React.useState("");
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () =>
    setDisabled(firstname == "" || lastname == "" || number == "");

  function handleSubmit() {
    sendSignupRequest();
  }

  const sendSignupRequest = async () => {
    try {
      // const UserDTO = {
      //   PhoneNumber: number
      // }
      
      const headers = {
        "Content-Type": "application/json",
      };

      const response = await axios.post("http://10.0.2.2:5181/Mr-Pill/Signup", { headers });
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
