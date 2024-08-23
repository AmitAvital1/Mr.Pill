import React, { useEffect } from "react";
import { Link, router } from "expo-router";

import {
  Pressable,
  SafeAreaView,
  StyleSheet,
  Image,
  View,
  Text,
} from "react-native";

import { MrPillLogo } from "@/components/MrPillLogo";
import axios from "axios";
import DataHandler from "@/DataHandler";

async function handleLoginPress() {

  const success = await sendAutomaticLoginRequest();

  if (success) {
    DataHandler.setState('login', 1);
  } else {
    DataHandler.setState('login', 0);
  }

  router.dismissAll();
  router.push('/(login)/login');

}

async function sendAutomaticLoginRequest() {

  if (DataHandler.getState('session') == 1) {
    router.navigate('/(home)/home');
  }

  const user = DataHandler.getUser();
  
  if (DataHandler.isEmpty()) return false;
  
  try {

    axios.defaults.validateStatus = function () {
      return true;
    };

    const request = {
      method: 'post',
      url: "http://10.0.2.2:5181/Mr-Pill/Login",
      headers: { "Content-Type": "application/json" }, 
      data: {
        "PhoneNumber": user.PhoneNumber,
      }
    }

    const response = await axios(request);

    if (response.request.status == 200) {
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
