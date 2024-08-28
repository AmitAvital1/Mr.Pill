import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button} from 'react-native';

import { router } from 'expo-router';

import DataHandler from '@/DataHandler'
import RequestHandler from '@/RequestHandler';

const AddReminderScreen = () => {

  const user = DataHandler.getUser()

  const [cabinetName, onChangeCabinetName] = React.useState<string>('');
  const [isDisabled, setDisabled] = React.useState<boolean>(true);
  const [reminder, setReminder] = React.useState<any>(undefined);

  const updateButton = () => setDisabled(cabinetName.length < 3 || cabinetName.length > 20)
 
  async function handleAddReminder() {

    let response = await sendAddReminderRequest();
    
    if (!response) {
      DataHandler.expireSession();
    }
    router.dismiss()
    router.replace('/(reminders)/myreminders');
  }

  const sendAddReminderRequest = async () => {

    DataHandler.setReminder(reminder);
    return await RequestHandler.sendRequest('addReminder');

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
        onPress={handleAddReminder} 
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


export default AddReminderScreen;