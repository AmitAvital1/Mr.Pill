import React from 'react';
import { View, Text, StyleSheet, Button, Pressable } from 'react-native';

const SimplePage: React.FC = () => {
  return (
    <View style={styles.container}>
      <View style={styles.container}>
        <Pressable onPress={() => {}}>
          <Text style={styles.text}>Hello, React Native!</Text>
        </Pressable>
      </View>
    </View>
  );
};

const printHello = () => console.log("Hello");

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
