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

  const success = await sendAutomaticLoginRequest();

  if (success) {
    DataHandler.setFlag('login', true);
  } else {
    DataHandler.setFlag('login', false);
  }

  router.replace('/(login)/login');
}

async function sendAutomaticLoginRequest() {

  if (DataHandler.isEmpty()) return false;

  if (DataHandler.getFlag('sessionAlive')) {
    router.replace('/(home)/home');
    return true;
  }

  return (await RequestHandler.sendRequest('login'));
}

const WelcomeScreen = () => {

  return (
    <SafeAreaView style={{ backgroundColor: "lavender", flex: 1 }}>

      <View style={styles.pagetop}>
        <Text style={styles.title}>ברוכים הבאים!</Text>
        {MrPillLogo(1)}
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
  },
  buttontext: {
    fontSize: 42,
    color: "white",
  },
  pagetop: {
    height: 180,
    flex: 1,
    padding: 10,
    backgroundColor: "lavender",
    justifyContent: "center",
    alignContent: "center",
    marginVertical: 25
  },
  title: {
    fontSize: 52,
    textAlign: "center",
    fontWeight: "bold",
  },
});

export default WelcomeScreen;
