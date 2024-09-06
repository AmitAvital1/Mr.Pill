import React from "react";
import { Link, router } from "expo-router";

import {
  Pressable,
  SafeAreaView,
  StyleSheet,
  View,
  Text,
} from "react-native";

import { MrPillLogo } from "@/components/MrPillLogo";
import DataHandler from "@/DataHandler";
import RequestHandler from "@/RequestHandler";

async function handleLoginPress() {

  if (DataHandler.isEmpty()) {
    
    router.push('/(login)/login');
    return false;

  } else if (DataHandler.getFlag('sessionAlive')) {

    router.push('/(home)/home');
    return true;

  } else {

    DataHandler.setFlag('login', await RequestHandler.sendRequest('login'));
    router.push('/(login)/login');

  }
}

const WelcomeScreen = () => {

  return (
    <SafeAreaView style={{ backgroundColor: "lavender", flex: 1 }}>

      <View style={styles.pagetop}>
        <Text style={styles.title}>ברוכים הבאים!</Text>
        {MrPillLogo(1, true)}
      </View>

      <View style={styles.button}>
        <Link href="/signup">
            <Text style={styles.buttontext}>משתמש חדש 🤗</Text>
        </Link>
      </View>

      <View style={[styles.button, {marginBottom: 20}]}>
        <Pressable onPress={handleLoginPress}>
          <Text style={styles.buttontext}>משתמש קיים 😎</Text>
        </Pressable>
      </View>

    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  button: {
    backgroundColor: "#6666FC",
    flex: 1,
    borderRadius: 100,
    justifyContent: "center",
    alignItems: "center",
    margin: 10,
    borderColor: "black",
    elevation: 5,
    maxHeight: "25%"
  },
  buttontext: {
    fontSize: 42,
    color: "white",
  },
  pagetop: {
    flex: 1,
    padding: 10,
    backgroundColor: "lavender",
    justifyContent: "center",
    alignContent: "center",
    marginVertical: 25,
    maxHeight: "50%",
  },
  title: {
    fontSize: 52,
    textAlign: "center",
    fontWeight: "bold",
  },
});

export default WelcomeScreen;
