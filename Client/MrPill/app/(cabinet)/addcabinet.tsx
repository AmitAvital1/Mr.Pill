import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';

import { router } from 'expo-router';

import DataHandler from '@/DataHandler'
import RequestHandler from '@/RequestHandler';

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
    router.dismiss()
    router.replace('/(cabinet)/mycabinets');
  }

  const sendAddCabinetRequest = async () => {

    DataHandler.setState('medicineCabinetName', cabinetName);
    return await RequestHandler.sendRequest('addCabinet');

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
        onChange={updateButton}
      />

      <Button 
        title="הוסף ארון" 
        onPress={handleAddCabinet} 
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


export default AddCabinetScreen;