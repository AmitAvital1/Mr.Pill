import React from "react";
import { Link } from "expo-router";

import {
  Pressable,
  SafeAreaView,
  StyleSheet,
  Image,
  View,
  Text,
} from "react-native";
import { PopButton } from "@/components/PopButton";
import { MrPillLogo } from "@/components/MrPillLogo";
``
const WelcomeScreen = () => {
  //const [text, onChangeText] = React.useState('');

  return (
    <SafeAreaView style={{ backgroundColor: "lavender", flex: 1 }}>
      <PopButton ButtonContent={MrPillLogo} BackgroundColor="white"/>

      <View style={styles.pagetop}>
        <Text style={styles.title}>ברוכים הבאים!</Text>
      </View>

      <View style={styles.button}>
        <Link href="/signup">
          <Text style={styles.buttontext}>משתמש חדש 🤗</Text>
        </Link>
      </View>

      <View style={styles.button}>
        <Link href="/login">
          <Text style={styles.buttontext}>משתמש קיים 😎</Text>
        </Link>
      </View>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  button: {
    backgroundColor: "#6666FC",
    flex: 1,
    borderRadius: 40,
    justifyContent: "center",
    alignItems: "center",
    margin: 10,
    borderColor: "black",
  },
  buttontext: {
    fontSize: 32,
    color: "white",
  },
  pagetop: {
    height: 180,
    flex: 1,
    padding: 10,
    backgroundColor: "lavender",
    justifyContent: "center",
    alignContent: "center",
  },
  title: {
    fontSize: 48,
    flex: 1,
    margin: 10,
    textAlign: "center",
  },
});

export default WelcomeScreen;
