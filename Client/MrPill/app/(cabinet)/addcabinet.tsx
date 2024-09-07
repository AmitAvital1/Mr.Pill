import React from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Alert} from 'react-native';

import { router } from 'expo-router';

import DataHandler from '@/DataHandler'
import RequestHandler from '@/RequestHandler';
import { MrPillLogo } from '@/components/MrPillLogo';
import { AppHomeButton } from '@/components/AppHomeButton';
import { strFC } from '@/components/strFC';

const bgc = "#c9e7ff"

const AddCabinetScreen = () => {

  const [cabinetName, setCabinetName] = React.useState<string>('');
  const [isNameTaken, setIsNameTaken] = React.useState<boolean>(false);

 
  async function handleButtonPress() {

    let response = await sendAddCabinetRequest();
    
    if (response) {
      Alert.alert("נוצר ארון תרופות חדש בהצלחה!");
      router.dismiss()
      router.replace('/(cabinet)/mycabinets');
    } else if (RequestHandler.getResponse().request.status == 400) {
      setIsNameTaken(true);
    }
    
  }

  const sendAddCabinetRequest = async () => {

    DataHandler.setState('medicineCabinetName', cabinetName);
    return await RequestHandler.sendRequest('addCabinet');

  }

  return (    
    <SafeAreaView style={{flex: 1, backgroundColor: bgc }}>
  
      <View style={styles.pagetop}>
        {MrPillLogo(0.5)}
        <Text style={{textAlign: 'center', fontSize: 40, flex:1, marginTop: 50}}>
          הוספת ארון חדש
        </Text>
      </View>

      <View style={{flex: 1, marginTop: 30}}>
      <TextInput
        style={styles.input}
        onChangeText={(text)=>{setCabinetName(text); setIsNameTaken(false)}}
        value={cabinetName}
        placeholder="הארון החדש שלי"
        keyboardType="default"
        textAlign='right'
      />
      {isNameTaken && <Text style={{textAlign: 'center', fontSize: 28, color: "#FF0000"}}>כבר קיים ארון בשם זה, אנא בחר שם אחר.</Text>}
      </View>
      <View style={styles.pagebottom}>
          <AppHomeButton 
            BackgroundColor={cabinetName.length < 4 ? bgc : "#aaffd3"} 
            BorderColor={"#000"} 
            ButtonContent={strFC(cabinetName.length < 4 ? "בחר שם לארון" : "הוסף ארון חדש")} 
            ButtonAction={cabinetName.length < 4 ? ()=>{} : handleButtonPress} />
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
    height: 180,
    padding: 10,
    backgroundColor: bgc,
    marginBottom: 10,
    marginTop: 8
  },
  pagebottom: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 20,
    marginHorizontal: 15,
    marginVertical: 20,
    padding: 5,
    minHeight: 180,
    maxHeight: 180,
  },
  
});


export default AddCabinetScreen;