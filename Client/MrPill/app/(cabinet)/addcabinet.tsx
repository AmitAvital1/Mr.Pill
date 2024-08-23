import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';
import axios from 'axios';
import dns from '../dns.json';
import { router } from 'expo-router';
import * as FileSystem from 'expo-file-system';
import { saveTokenToFile } from '@/components/tokenHandlerFunctions';
import DataHandler from '@/DataHandler'



const AddCabinetScreen = () => {

  const user = DataHandler.getUser()

  const [cabinetName, onChangeCabinetName] = React.useState('');
  const [isDisabled, setDisabled] = React.useState(true);
  const updateButton = () => setDisabled(cabinetName.length < 3 || cabinetName.length > 20)
 
  async function handleAddCabinet() {

    let response = await sendAddCabinetRequest();
    
    if (!response) {
      DataHandler.expireSession();
    }

    response = await sendGetCabinetsRequest();

    if (!response) {
      DataHandler.expireSession();
    }

    return true;
  }

  const sendGetCabinetsRequest = async () => {

    // bug when adding medication and then trying to get all user medications
    try {
      console.log(user.Token)
      const request = {
        method: 'get',
        url: "http://10.0.2.2:5194/user/cabinet",
        headers: {
            "Authorization": "Bearer " + user.Token, 
        },
        data: {
        }
      }

      const response = await axios(request);

      console.log("full: " + response.request._response);
      console.log("status: " + response.request.status);

    } catch (error) {
      console.error("Error fetching data:", error);
      return false;
    }

  }

  const sendAddCabinetRequest = async () => {
    try {

      axios.defaults.validateStatus = function () {
        return true;
      };
      
      const request = {
        method: 'post',
        url: "http://10.0.2.2:5194/medicine-cabinet?Name=" + cabinetName,
        headers: {
            "Authorization": "Bearer " + user.Token,
        },
      }
      const response = await axios(request);
      console.log("full: " + response.request._response);
      console.log("status: " + response.request.status);

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
        onChangeText={onChangeCabinetName}
        value={cabinetName}
        placeholder="שם של ארון חדש"
        keyboardType="default"
        textAlign='right'
        onEndEditing={updateButton}
      />

      <Button 
        title="הוסף ארון" 
        onPress={handleAddCabinet} 
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


export default AddCabinetScreen;