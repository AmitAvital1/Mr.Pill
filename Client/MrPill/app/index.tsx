
import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';

const TextInputExample = () => {
  const [text, onChangeText] = React.useState('');
  const [number, onChangeNumber] = React.useState('');
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () => setDisabled(text == '' || number == '')

  return (
    <SafeAreaView>

      <View style={styles.pagetop}>
        <Text style={{fontSize: 32, flex:1}}>
          הרשמה למר. פיל
        </Text>
      </View>

      <TextInput
        style={styles.input}
        onChangeText={onChangeNumber}
        value={number}
        placeholder="מספר טלפון"
        keyboardType="numeric"
        textAlign='right'
        onEndEditing={updateButton}
      />

      <TextInput
        style={styles.input}
        onChangeText={onChangeText}
        placeholder="שם פרטי ומשפחה"
        value={text}
        textAlign='right'
        onEndEditing={updateButton}
      />

      <Button 
        title="הרשמה" 
        onPress={() => {console.log(text, number)}} 
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
    backgroundColor: 'lavender'
  },
});

export default TextInputExample;