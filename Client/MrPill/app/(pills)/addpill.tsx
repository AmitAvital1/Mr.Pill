import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';
import axios from 'axios';
import dns from '../dns.json';
import { router } from 'expo-router';
import * as FileSystem from 'expo-file-system';
import { saveTokenToFile } from '@/components/tokenHandlerFunctions';
import DataHandler from '@/DataHandler'

const AddPillScreen = () => {

  const user = DataHandler.getUser()

  const [number, onChangeNumber] = React.useState('');
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () => setDisabled(number == '')
 
  function handleAddPill() {

    let response = sendAddPillRequest();
    
    if (!response) return false;

    response = sendGetPillRequest();

    if (!response) return false;

    return true;

  }

  const sendGetPillRequest = async () => {

    // bug when adding medication and then trying to get all user medications
    try {
      console.log(user.Token)
      const request = {
        method: 'get',
        url: "http://10.0.2.2:5194/user/medications",
        headers: {
            "Authorization": "Bearer " + user.Token, 
            "privacyStatus": "AllMedications",
        },
        data: {
        }
      }

      const response = await axios(request);

      console.log(response.request._response);
      console.log(response.request.status);

    } catch (error) {
      console.error("Error fetching data:", error);
      return false;
    }

  }

  const sendAddPillRequest = async () => {
    try {

      axios.defaults.validateStatus = function () {
        return true;
      };
      
      const request = {
        method: 'post',
        url: "http://10.0.2.2:5194/medications",
        headers: {
            "Authorization": "Bearer " + user.Token,
        },
        data: {
            "medicationBarcode": String(number),
            //"phoneNumber": "0501231234",
            "privatcy": false,
        }
      }
      const response = await axios(request);
      console.log(response.request._response);
      console.log(response.request.status);

      if (response.request.status == 200) {
        return true;
      }
      
    } catch (error) {
      console.error("Error fetching data:", error);
      return false;
    }

  }

  return (
    <SafeAreaView>

      <View style={styles.pagetop}>
        <Text style={{fontSize: 32, flex:1}}>
          הוספת תרופה חדשה
        </Text>
      </View>

      <TextInput
        style={styles.input}
        onChangeText={onChangeNumber}
        value={number}
        placeholder="מספר ברקוד של התרופה"
        keyboardType="numeric"
        textAlign='right'
        onEndEditing={updateButton}
      />

      <Button 
        title="הוסף תרופה" 
        onPress={handleAddPill} 
        //disabled={isDisabled}
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


export default AddPillScreen;