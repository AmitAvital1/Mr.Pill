import React from 'react';
import {Link} from 'expo-router'

import {SafeAreaView, StyleSheet, TextInput, View, Text, Button, Pressable} from 'react-native';

const WelcomeScreen = () => {
  const [text, onChangeText] = React.useState('');
  const [number, onChangeNumber] = React.useState('');
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () => setDisabled(text == '' || number == '')

  return (
    <SafeAreaView>

      <View style={styles.pagetop}>
        <Text style={styles.title}>
          ברוכים הבאים למר. פיל
        </Text>
      </View>

      <View>
        <Link href="/signup">
          <Text style={styles.textbutton}>
            משתמש חדש
          </Text>
        </Link>
      </View>

      <View>
        <Link href="/login">
          <Text style={styles.textbutton}>
            משתמש קיים
          </Text>
        </Link>
      </View>

      
    </SafeAreaView>
  );
};


const styles = StyleSheet.create({
  textbutton: {
    height: 40,
    margin: 12,
    borderWidth: 1,
    padding: 10,
    fontSize: 32,
    flex: 1,
    textAlign: 'center',
    backgroundColor: 'beige'
  },
  pagetop: {
    height: 160, 
    padding: 10,
    backgroundColor: 'lavender'
  },
  title: {
    fontSize: 48,
    flex: 1,
    margin: 10,
    textAlign: 'center'
  }
});

export default WelcomeScreen;