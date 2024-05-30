import React from "react";
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
        onPress={() => {
          console.log(firstname, lastname, number);
        }}
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
