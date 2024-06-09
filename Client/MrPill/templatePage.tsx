import React from 'react';
import { View, Text, StyleSheet, Button, Pressable } from 'react-native';
import { AppTextButton } from "@/components/AppTextButton";
import { AppHomeButton } from "@/components/AppHomeButton";

const SimplePage: React.FC = () => {

  const buttonContent = () => {
    return (



      <Text>Hellosssss</Text>



    );
  }

  const buttonAction = () => {


    
  }

  return (
    <View style={styles.container}>
      <AppTextButton ButtonContent='hello' ButtonAction={()=>{}}/>
      <View style={styles.container}>
        <Pressable onPress={printHello}>
          <Text style={styles.text}>Hello, React Native!</Text>
        </Pressable>
      </View>
    </View>
  );
};

const printHello = () => console.log("Hello")

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f8f8f8',
  },
  text: {
    fontSize: 20,
    color: '#333',
  },
});

export default SimplePage;
