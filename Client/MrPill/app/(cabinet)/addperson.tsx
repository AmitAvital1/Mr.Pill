import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Alert} from 'react-native';

import { router } from 'expo-router';

import DataHandler from '@/DataHandler'
import RequestHandler from '@/RequestHandler';
import { MrPillLogo } from '@/components/MrPillLogo';
import { AppHomeButton } from '@/components/AppHomeButton';
import { strFC } from '@/components/strFC';

const bgc = "#c9e7ff"

const isValidPhoneNumber = (phoneNumber: string) => {
    if (!phoneNumber) return false;
    if (!/^[0-9]{10}$/.test(phoneNumber)) return false;
    if (phoneNumber.length != 10) return false;
    if (phoneNumber[0] != '0' || phoneNumber[1] != '5') return false;
  
    return true;
}

const AddPersonToCabinetScreen = () => {

  const [phoneNumber, setPhoneNumber] = React.useState<string>('');
  const [isPhoneNumberFormatValid, setIsPhoneNumberFormatValid] = React.useState<boolean>(false);
  const [isAlreadyMember, setIsAlreadyMember] = React.useState<boolean>(false);

  async function handleButtonPress() {

    let response = await sendAddPersonRequest();
    
    if (response) {
      router.dismiss()
      Alert.alert("בקשת הצטרפות לארון נשלחה בהצלחה!");
    } else if (RequestHandler.getResponse().request.status == 400) {
      setIsAlreadyMember(true);
    } else {
      Alert.alert("שגיאה בהוספת משתתף");
    }
    
  }

  const sendAddPersonRequest = async () => {

    DataHandler.setState("targetPhoneNumber", phoneNumber);
    return await RequestHandler.sendRequest('addPersonToCabinet');

  }

  return (    
    <SafeAreaView style={{flex: 1, backgroundColor: bgc}}>
      <View style={styles.pagetop}>
      {MrPillLogo(0.5)}
        <Text style={{textAlign: 'center', fontSize: 40, marginTop: 15}}>
          הוספת משתתף{"\n"}לארון קיים
        </Text>
        <Text style={{textAlign: 'center', fontSize: 30}}>
          הארון שנבחר: {DataHandler.getState("medicineCabinetName")}
        </Text>
      </View>

      <View style={{flex: 1}}>
      <TextInput
        style={styles.input}
        onChangeText={(text)=>{setPhoneNumber(text); setIsAlreadyMember(false); setIsPhoneNumberFormatValid(isValidPhoneNumber(text))}}
        value={phoneNumber}
        placeholder="הכנס מספר טלפון"
        keyboardType="numeric"
        textAlign="right"
      />
      {isAlreadyMember && <Text style={{textAlign: 'center', fontSize: 28, color: "#FF0000"}}>שגיאה בצירוף משתתף, יתכן והמשתמש כבר משתתף בארון.</Text>}
      </View>

      <View style={styles.pagebottom}>
          <AppHomeButton 
            BackgroundColor={isPhoneNumberFormatValid ? "#aaffd3" : bgc} 
            BorderColor={"#000"} 
            ButtonContent={strFC(isPhoneNumberFormatValid ? "הוסף משתתף לארון!" : "הכנס מספר טלפון")}
            ButtonAction={isPhoneNumberFormatValid ? handleButtonPress : ()=>{}} 
          />
      </View>

    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  input: {
    backgroundColor: "#fff9c2",
    height: 60,
    margin: 8,
    borderWidth: 2,
    borderColor: "#dcfff4",
    padding: 10,
    borderRadius: 12,
    fontSize: 25,
    elevation: 5,
  },
  pagetop: {
    flex: 1,
    padding: 10,
    backgroundColor: bgc,
    marginBottom: 10,
    minHeight: 180,
  },
  pagebottom: {
    flex: 2,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 20,
    marginHorizontal: 15,
    padding: 5,
  },
  
});


export default AddPersonToCabinetScreen;